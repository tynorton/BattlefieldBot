using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace BattlefieldBot
{
    class Program
    {
        public static Api Bot = new Api(ConfigurationManager.AppSettings["TelegramApiToken"]);

        static void Main(string[] args)
        {
            /*var timer = new System.Threading.Timer(
                e => RunYoutubeSearch().Wait(),
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMinutes(5));*/

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
                var updates = await Bot.GetUpdates(offset);
                bool changed = false;
                foreach (var update in updates)
                {
                    // Group Chats
                    if (update.Message.Chat.GetType() == typeof(GroupChat) && !chatIds.Contains(update.Message.Chat.Id))
                    {
                        chatIds.Add(update.Message.Chat.Id);
                        changed = true;
                    }

                    // User Chats
                    if (update.Message.Chat.GetType() == typeof (User))
                    {
                        var user = update.Message.Chat as User;
                        if (update.Message.Text.Trim().Equals("subscribe", StringComparison.OrdinalIgnoreCase) && !chatIds.Contains(update.Message.Chat.Id))
                        {
                            Console.WriteLine(string.Format("New Subscriber: {0} ({1}, ID: {2}", user.FirstName + " " + user.LastName, user.Username, update.Message.Chat.Id));
                            chatIds.Add(update.Message.Chat.Id);
                            changed = true;

                            await Bot.SendTextMessage(update.Message.Chat.Id, "You have been subscribed to Neebs Gaming notifications!");
                        }

                        if (update.Message.Text.Trim().Equals("unsubscribe", StringComparison.OrdinalIgnoreCase) && chatIds.Contains(update.Message.Chat.Id))
                        {
                            Console.WriteLine(string.Format("Removed Subscriber: {0} ({1}, ID: {2}", user.FirstName + " " + user.LastName, user.Username, update.Message.Chat.Id));
                            chatIds.Remove(update.Message.Chat.Id);
                            changed = true;

                            await Bot.SendTextMessage(update.Message.Chat.Id, "You have been unsubscribed from Neebs Gaming notifications.");
                        }
                    }

                    offset = update.Id + 1;
                }

                if (changed)
                {
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt"), string.Join(",", chatIds));
                }

                await Task.Delay(1000);
            }
        }

        private static async Task NotifyChats(List<dynamic> newItems)
        {
            List<int> chatIds = new List<int>();
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt")))
            {
                chatIds = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt")).Split(',').Select(int.Parse).ToList();
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
