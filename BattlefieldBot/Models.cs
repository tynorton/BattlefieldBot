using System;
using BattlefieldBot.BattlelogModel;

namespace BattlefieldBot
{
    public enum GameType
    {
        Battlefield3 = 2,
        Battlefield4 = 2048,
        // BattlefieldHardline = ??
        // MOH = ??
        Unknown = int.MaxValue
    }

    public enum GamePlatformType
    {
        PC = 1,
        Xbox360 = 2,
        PlayStation3 = 3
        // XboxOne = ?
        // PlayStation4 = ?
    }

    public class BattlelogUser
    {
        internal BattlelogUser(bl_user user)
        {
            this.UserName = user.username;
            this.IsOnline = user.presence.isOnline;
            this.IsPlaying = user.presence.isPlaying;
            this.ServerDetail = new ServerDetail(user.presence);
            this.GameType = (GamePlatformType) user.presence.game;
        }

        public string UserName { get; set; }
        public bool IsOnline { get; set; }
        public bool IsPlaying { get; set; }
        public GamePlatformType GameType { get; set; }
        public ServerDetail ServerDetail { get; set; }

    }

    public class ServerDetail
    {
        public ServerDetail(string serverName, Guid serverGuid, GameType type, GamePlatformType platform)
        {
            this.Name = serverName;
            this.ID = serverGuid;
            this.GameType = type;
            this.Platform = platform;
        }

        internal ServerDetail(bl_user_presence presence)
        {
            this.Name = presence.serverName;
            this.ID = presence.serverGuid;
            this.GameType = (GameType) presence.game;
            this.Platform = (GamePlatformType) presence.platform;
        }

        public string Name { get; set; }
        public Guid ID { get; set; }
        public GameType GameType { get; set; }
        public GamePlatformType Platform { get; set; }
    }
}
