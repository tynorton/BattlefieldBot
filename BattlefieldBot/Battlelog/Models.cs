using System;
using System.ComponentModel.DataAnnotations.Schema;
using BattlefieldBot.Battlelog.Proxies;
using LiteDB;

namespace BattlefieldBot
{
    public enum GameType
    {
        Unknown = 0, // XB1 showed this
        Battlefield3 = 2,
        Battlefield4 = 2048
        // BattlefieldHardline = ??
        // MOH = ??
    }

    public enum GamePlatformType
    {
        Unknown = 0, // XB1 showed this
        PC = 1,
        Xbox360 = 2,
        // ? PlayStation3 = 3,
        XboxOne = 64
        // PlayStation4 = ?
    }

    public class BattlelogUser
    {
        [Obsolete]
        public BattlelogUser() { }

        internal BattlelogUser(bl_user user)
        {
            this.UserID = user.userId;
            this.UserName = user.username;
            this.IsOnline = user.presence.isOnline;
            this.IsPlaying = user.presence.isPlaying;
            this.ServerID = null != user.presence.playingMp ? user.presence.playingMp.serverGuid.ToString() : user.presence.serverGuid.ToString();
            this.ServerName = null != user.presence.playingMp ? user.presence.playingMp.serverName : user.presence.serverName;
            this.GameType = null != user.presence.onlineGame ? user.presence.onlineGame.game : user.presence.game;
            this.Platform = null != user.presence.onlineGame ? user.presence.onlineGame.platform : user.presence.platform;
            this.LastUpdated = DateTime.UtcNow;
        }

        public object ServerName { get; set; }

        [BsonId]
        public long UserID { get; set; }

        public string UserName { get; set; }

        public bool IsOnline { get; set; }

        public bool IsPlaying { get; set; }

        public GameType GameType { get; set; }

        public string ServerID { get; set; }

        [NotMapped]
        public virtual ServerDetail Server { get; private set; }

        public DateTime LastUpdated { get; set; }
        public DateTime LastSeen { get; set; }
        public GamePlatformType Platform { get; set; }

        public override string ToString()
        {
            return this.UserName;
        }
    }

    public class ServerDetail
    {
        [Obsolete]
        public ServerDetail() { }

        public ServerDetail(string serverName, Guid serverGuid, GameType type, GamePlatformType platform)
        {
            this.Name = serverName;
            this.ServerID = serverGuid.ToString();
            this.GameType = type;
            this.Platform = platform;
        }

        internal ServerDetail(bl_user_presence presence)
        {
            this.Name = presence.serverName;
            this.ServerID = presence.serverGuid.ToString();
            this.GameType = presence.game;
            this.Platform = presence.platform;
        }

        public string Name { get; set; }

        [BsonId]
        public string ServerID { get; set; }

        public GameType GameType { get; set; }

        public GamePlatformType Platform { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
