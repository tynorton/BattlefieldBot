using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace NeebsBot
{
    class Program
    {
        public static Api Bot = new Api(ConfigurationManager.AppSettings["TelegramApiToken"]);

        static void Main(string[] args)
        {
            var timer = new System.Threading.Timer(
                e => RunYoutubeSearch().Wait(),
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMinutes(15));

            Run().Wait();
        }

        static async Task Run()
        {
            var me = await Bot.GetMe();

            Console.WriteLine("Hello my name is {0}", me.Username);

            var offset = 0;

            List<int> chatIds = new List<int>();
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt")))
            {
                chatIds = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt")).Split(',').Select(int.Parse).ToList();
            }

            while (true)
            {
                try
                {
                    var updates = await Bot.GetUpdates(offset);
                    bool changed = false;
                    foreach (var update in updates)
                    {
                        // Group Chats
                        if (update.Message.Chat.GetType() == typeof (GroupChat) &&
                            !chatIds.Contains(update.Message.Chat.Id))
                        {
                            chatIds.Add(update.Message.Chat.Id);
                            changed = true;
                        }

                        // User Chats
                        if (update.Message.Chat.GetType() == typeof (User))
                        {
                            var user = update.Message.Chat as User;
                            if (update.Message.Text.Trim().Equals("subscribe", StringComparison.OrdinalIgnoreCase) &&
                                !chatIds.Contains(update.Message.Chat.Id))
                            {
                                Console.WriteLine(string.Format("New Subscriber: {0} ({1}, ID: {2}",
                                    user.FirstName + " " + user.LastName, user.Username, update.Message.Chat.Id));
                                chatIds.Add(update.Message.Chat.Id);
                                changed = true;

                                await
                                    Bot.SendTextMessage(update.Message.Chat.Id,
                                        "You have been subscribed to Neebs Gaming notifications!");
                            }

                            if (update.Message.Text.Trim().Equals("unsubscribe", StringComparison.OrdinalIgnoreCase) &&
                                chatIds.Contains(update.Message.Chat.Id))
                            {
                                Console.WriteLine(string.Format("Removed Subscriber: {0} ({1}, ID: {2}",
                                    user.FirstName + " " + user.LastName, user.Username, update.Message.Chat.Id));
                                chatIds.Remove(update.Message.Chat.Id);
                                changed = true;

                                await
                                    Bot.SendTextMessage(update.Message.Chat.Id,
                                        "You have been unsubscribed from Neebs Gaming notifications.");
                            }
                        }

                        offset = update.Id + 1;
                    }

                    if (changed)
                    {
                        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt"),
                            string.Join(",", chatIds));
                    }

                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    // No nothing, just don't crash the app if service is down, or some unknown error.
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static async Task RunYoutubeSearch()
        {
            try
            {
                Console.Write("Searching YouTube for new videos... ");
                var youtube = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApplicationName = ConfigurationManager.AppSettings["YoutubeApiAppName"],
                    ApiKey = ConfigurationManager.AppSettings["YoutubeApiToken"]
                });

                SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
                listRequest.ChannelId = ConfigurationManager.AppSettings["YoutubeChannelId"];
                listRequest.MaxResults = 5;
                listRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
                listRequest.Type = "video";
                SearchListResponse resp = listRequest.Execute();

                DateTime mostRecent = DateTime.MinValue.ToUniversalTime();

                var youtubeRootDir = Path.Combine(Environment.CurrentDirectory, string.Format("data/youtube/"));
                var channelSubscribersPath = Path.Combine(youtubeRootDir,
                    string.Format("{0}.txt", listRequest.ChannelId));

                CreateIfMissing(Path.Combine(Environment.CurrentDirectory, string.Format("data/youtube/")));

                if (File.Exists(channelSubscribersPath))
                {
                    var dateStr = File.ReadAllText(channelSubscribersPath);
                    long ticks = long.Parse(dateStr);
                    mostRecent = DateTime.SpecifyKind(new DateTime(ticks), DateTimeKind.Utc);
                }

                Console.WriteLine("Done.");

                var newItems =
                    resp.Items.Where(
                        obj => obj.Snippet.PublishedAt.HasValue && obj.Snippet.PublishedAt.Value > mostRecent)
                        .ToList();

                if (newItems.Any())
                {
                    Console.WriteLine("Found New Videos! Notifying clients.");
                    var latestEp = newItems.OrderByDescending(obj => obj.Snippet.PublishedAt.Value)
                        .FirstOrDefault();
                    File.WriteAllText(channelSubscribersPath, latestEp.Snippet.PublishedAt.Value.Ticks.ToString());
                    await NotifyChats(newItems);
                }
            }
            catch (Exception ex)
            {
                // No nothing, just don't crash the app if service is down, or some unknown error.
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task NotifyChats(List<SearchResult> newItems)
        {
            try
            {
                var chatIds = new List<int>();
                if (File.Exists(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt")))
                {
                    chatIds = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt"))
                        .Split(',')
                        .Select(int.Parse)
                        .ToList();
                }

                foreach (var chatId in chatIds)
                {
                    Console.WriteLine("Notiyfing Chat ID: {0}", chatId);
                    foreach (var result in newItems)
                    {
                        Console.WriteLine(" * New video: https://www.youtube.com/watch?v={0}", result.Id.VideoId);
                        await Bot.SendTextMessage(chatId, string.Format("New Neebs Gaming Video!\n\nhttps://www.youtube.com/watch?v={0}", result.Id.VideoId));
                    }
                }
            }
            catch (Exception ex)
            {
                // No nothing, just don't crash the app if service is down, or some unknown error happens.
                Console.WriteLine(ex.ToString());
            }
        }

        private static void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
