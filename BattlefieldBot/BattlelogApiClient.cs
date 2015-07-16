using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using BattlefieldBot.Battlelog.Proxies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BattlefieldBot
{
    public class BattlelogApiClient : MyWebClient
    {
        public bool Login(string username, string password)
        {
            HttpWebResponse response;

            // Autenticate User
            this.PostPage("https://battlelog.battlefield.com/bf3/gate/login/", new Dictionary<string, string>()
            {
                {"email", ConfigurationManager.AppSettings["BattlelogUserName"]},
                {"password", ConfigurationManager.AppSettings["BattlelogPassword"]},
                {"redirect", String.Empty},
                {"submit", "Sign+In"}
            }, out response);

            // TODO: Check to see if response was invalid
            return true;
        }

        public IEnumerable<BattlelogUser> GetComCenterStatuses(bool includeSelf = false)
        {
            HttpWebResponse response;

            // After auth, get friends list status
            var doc = this.GetPage("http://battlelog.battlefield.com/bf3/comcenter/sync/", out response);

            var json = JsonConvert.DeserializeObject<JObject>(doc.DocumentNode.OuterHtml);

            // get JSON result objects into a list
            List<JToken> friends = json["data"]["friendscomcenter"].Children().ToList();

            if (includeSelf)
            {
                var userProfile = this.GetPage("http://battlelog.battlefield.com/bf3/profile/edit/", out response,
                    new Dictionary<string, string>()
                    {
                        {"X-Requested-With", "XMLHttpRequest"},
                        {"X-AjaxNavigation", "1"}
                    });

                json = JObject.Parse(userProfile.DocumentNode.OuterHtml);
                var user = json["context"]["profileCommon"]["user"];
                friends.Add(user);
            }

            var users = new List<BattlelogUser>();
            foreach (var friend in friends)
            {
                var blUser = JsonConvert.DeserializeObject<bl_user>(friend.ToString());
                users.Add(new BattlelogUser(blUser));
            }

            return users;
        }


        public static string GetGameName(GameType type)
        {
            switch (type)
            {
                case GameType.Battlefield3:
                    return "Battlefield 3";
                case GameType.Battlefield4:
                    return "Battlefield 4";
                default:
                    return "Unknown";
            }
        }

        public static string GetGameUrl(GameType type, GamePlatformType platform, string serverGuid)
        {
            switch (type)
            {
                case GameType.Battlefield3:
                    return String.Format("http://battlelog.battlefield.com/bf3/servers/show/pc/{0}", serverGuid);
                case GameType.Battlefield4:
                    switch (platform)
                    {
                        case GamePlatformType.PC:
                            return String.Format("http://battlelog.battlefield.com/bf4/servers/show/pc/{0}", serverGuid);
                        case GamePlatformType.XboxOne:
                            return String.Format("http://battlelog.battlefield.com/bf4/servers/show/XBOXONE/{0}", serverGuid);
                        default:
                            return string.Empty;

                    }
                default:
                    return string.Empty;
            }
        }
    }
}
