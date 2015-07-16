using System;

namespace BattlefieldBot.Battlelog.Proxies
{
    internal class bl_user_presence
    {
        public bl_user_presence_playingMp playingMp;
        public bl_user_presence_onlineGame onlineGame;
        public bool isOnline { get; set; }
        public bool isPlaying { get; set; }
        public GameType game { get; set; }
        public int[] gameExpansions { get; set; }
        public int gameId { get; set; }
        public bool isIdle { get; set; }
        public bool isOnlineOrbit { get; set; }
        public int personaId { get; set; }
        public GamePlatformType platform { get; set; }
        public int playingMode { get; set; }
        public Guid serverGuid { get; set; }
        public string serverName { get; set; }
        public long userId { get; set; }
    }

    internal class bl_user_presence_onlineGame
    {
        public GamePlatformType platform { get; set; }
        public GameType game { get; set; }
        public long personaId { get; set; }
    }

    internal class bl_user_presence_playingMp
    {
        public Guid serverGuid { get; set; }
        public GamePlatformType platform { get; set; }
        public long personaId { get; set; }
        public long gameId { get; set; }
        public int role { get; set; }
        public int[] gameExpansions { get; set; }
        public string serverName { get; set; }
        public long gameMode { get; set; }
        public GameType game { get; set; }
        public string levelName { get; set; }
    }
}
