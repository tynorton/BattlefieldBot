using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace BattlefieldBot
{
    class Program
    {
        public static Api Bot = new Api(ConfigurationManager.AppSettings["TelegramApiToken"]);
        public const ConsoleColor DefaultColor = ConsoleColor.White;

        static void Main(string[] args)
        {
            Console.ForegroundColor = DefaultColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Hello my name is BattlefieldBot\n");
            Console.ForegroundColor = DefaultColor;

            using (var db = new LiteDatabase(AppContext.SQLITE_FILENAME))
            {
                var users = db.GetCollection<BattlelogUser>("users").FindAll();
                Console.WriteLine("[{0}] Found {1} users saved from previous runs.", DateTime.UtcNow.ToShortTimeString(), users.Count());
                foreach (var user in users)
                {
                    // HACK: LiteDB seems to not save the kind of datetime.
                    var lastSeenStr = user.LastSeen == DateTime.MinValue
                        ? "never"
                        : StringFunctions.GetAgeString(user.LastSeen.ToUniversalTime());

                    Console.Write(" * {0} [Last seen {1} playing {2} ({3}) on {4} ({5})]",
                        user.UserName,
                        lastSeenStr,
                        BattlelogApiClient.GetGameName(user.GameType),
                        user.Platform,
                        user.ServerName,
                        BattlelogApiClient.GetGameUrl(user.GameType, user.Platform, user.ServerID));

                    /*
                    if (user.IsOnline && user.IsPlaying)
                    {
                        Console.Write(", playing {0} on {1}", BattlelogApiClient.GetGameName(user.Server.GameType), user.Server.Name);
                    }*/

                    Console.WriteLine();
                }
            }

            Console.WriteLine("[{0}] Requesting user status from battelog...", DateTime.UtcNow.ToShortTimeString());
            var timer = new System.Threading.Timer(
                e => RunBattlelogSearchAsync().Wait(),
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMinutes(5));

            // Start main loop
            Run().Wait();
        }

        static async Task Run()
        {
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
                        if (update.Message.Chat.GetType() == typeof(GroupChat) && !chatIds.Contains(update.Message.Chat.Id))
                        {
                            chatIds.Add(update.Message.Chat.Id);
                            changed = true;
                        }

                        // User Chats
                        if (update.Message.Chat.GetType() == typeof(User))
                        {
                            var user = update.Message.Chat as User;
                            if (update.Message.Text.Trim().Equals("subscribe", StringComparison.OrdinalIgnoreCase) && !chatIds.Contains(update.Message.Chat.Id))
                            {
                                Console.WriteLine(string.Format("[{0}] New Subscriber: {1} ({2}, ID: {3})", DateTime.UtcNow.ToShortTimeString(), user.FirstName + " " + user.LastName, user.Username, update.Message.Chat.Id));
                                chatIds.Add(update.Message.Chat.Id);
                                changed = true;

                                await Bot.SendTextMessage(update.Message.Chat.Id, "You have been subscribed to notifications. You can unsubscribe at any time by typing unsubscribe.");
                            }
                            else if (update.Message.Text.Trim().Equals("unsubscribe", StringComparison.OrdinalIgnoreCase) && chatIds.Contains(update.Message.Chat.Id))
                            {
                                Console.WriteLine(string.Format("[{0}] Removed Subscriber: {1} ({2}, ID: {3})", DateTime.UtcNow.ToShortTimeString(), user.FirstName + " " + user.LastName, user.Username, update.Message.Chat.Id));
                                chatIds.Remove(update.Message.Chat.Id);
                                changed = true;

                                await Bot.SendTextMessage(update.Message.Chat.Id, "You have been unsubscribed to notifications.");
                            }

                            else
                            {
                                await Bot.SendTextMessage(update.Message.Chat.Id, "This bot only understands two commands:\n\n [subscribe] - Subscribe to notifications\n[unsubscribe] - Unsubscribe from notifications");
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
                catch (Exception ex)
                {
                    // Don't crash the loop, just print error
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ForegroundColor = DefaultColor;
                }
            }
        }

        private static async Task NotifyChats(string msg)
        {
            try
            {
                var chatIds = new List<int>();
                if (File.Exists(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt")))
                {
                    chatIds = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "subscribedchats.txt")).Split(',').Select(int.Parse).ToList();
                }

                foreach (var chatId in chatIds)
                {
                    Console.WriteLine("[{0} - Broadcast] {1}", DateTime.UtcNow.ToShortTimeString(), msg);
                    await Bot.SendTextMessage(chatId, msg);
                }
            }
            catch (Exception ex)
            {
                // No nothing, just don't crash the app if service is down, or some unknown error.
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

        public static async Task RunBattlelogSearchAsync()
        {
            var client = new BattlelogApiClient();

            LiteCollection<BattlelogUser> userDocs;
            IList<BattlelogUser> storedUsers;

            // Can't get user list, let's just load up our last known users list and hope for the best
            using (var db = new LiteDatabase(AppContext.SQLITE_FILENAME))
            {
                userDocs = db.GetCollection<BattlelogUser>("users");
                storedUsers = userDocs.FindAll().ToList();
            }

            // Autenticate User
            client.Login(ConfigurationManager.AppSettings["BattlelogUserName"],
                ConfigurationManager.AppSettings["BattlelogPassword"]);

            string includeSelfStr = ConfigurationManager.AppSettings["MonitorCurrentUser"];

            // Download ComCenter Statuses
            var users =
                client.GetComCenterStatuses(!string.IsNullOrEmpty(includeSelfStr) && bool.Parse(includeSelfStr)) ??
                storedUsers;

            using (var db = new LiteDatabase(AppContext.SQLITE_FILENAME))
            {
                bool showSavingMsg = false;

                // Add/Update found users locally
                foreach (var user in users)
                {
                    var userRepo = db.GetCollection<BattlelogUser>("users");
                    var matchingUser = userRepo.Find(obj => obj.UserName == user.UserName).SingleOrDefault();
                    if (null != matchingUser)
                    {
                        if (user.IsOnline)
                        {
                            showSavingMsg = true;
                            user.LastSeen = DateTime.UtcNow;
                        }

                        bool isOnlineStatusUpdated = matchingUser.IsOnline != user.IsOnline;
                        if (isOnlineStatusUpdated)
                        {
                            showSavingMsg = true;
                            // User has changed online state since last update
                            Console.WriteLine("[{0}] {1} {2}", DateTime.UtcNow.ToShortTimeString(), user.UserName,
                                user.IsOnline ? "is online" : "has gone offline");
                        }

                        bool isPlayingStatusUpdated = matchingUser.IsPlaying != user.IsPlaying;
                        if (isPlayingStatusUpdated)
                        {
                            showSavingMsg = true;
                            if (user.IsPlaying && matchingUser.IsPlaying && (matchingUser.ServerID != user.ServerID))
                            {
                                // User has changed server since last update
                                await NotifyChats(
                                    string.Format("{0} has changed servers, and is now playing {1} ({2}) on {3} ({4})",
                                        user.UserName,
                                        BattlelogApiClient.GetGameName(user.GameType),
                                        user.Platform,
                                        user.ServerName,
                                        BattlelogApiClient.GetGameUrl(user.GameType, user.Platform, user.ServerID)));
                            }
                            else if (user.IsPlaying && !matchingUser.IsPlaying)
                            {
                                await NotifyChats(
                                    string.Format("{0} has started playing {1} ({2}) on {3} ({4})",
                                        user.UserName,
                                        BattlelogApiClient.GetGameName(user.GameType),
                                        user.Platform,
                                        user.ServerName,
                                        BattlelogApiClient.GetGameUrl(user.GameType, user.Platform, user.ServerID)));
                            }
                            else if (!user.IsPlaying && matchingUser.IsPlaying)
                            {
                                await NotifyChats(
                                    string.Format("{0} has stopped playing {1} ({2})",
                                        user.UserName,
                                        BattlelogApiClient.GetGameName(user.GameType),
                                        user.Platform));
                            }
                        }

                        // Delete stale record
                        userRepo.Delete(obj => obj.UserID == user.UserID);
                    }
                    else
                    {
                        Console.WriteLine("[{0}] {1} {2}", DateTime.UtcNow.ToShortTimeString(), user.UserName,
                            users.Any() ? "has accepted your friend request" : "discovered as friend");
                    }

                    // Insert user record
                    userRepo.Insert(user);
                }

                if (showSavingMsg)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] Saving new user statuses to disk", DateTime.UtcNow.ToShortTimeString());
                    Console.ForegroundColor = DefaultColor;
                }

                db.Commit();
            }
        }
    }
}
