using System;

namespace BattlefieldBot.Battlelog.Proxies
{
    internal class bl_user_presence
    {
        public bool isOnline { get; set; }
        public bool isPlaying { get; set; }
        public int game { get; set; }
        public int[] gameExpansions { get; set; }
        public int gameId { get; set; }
        public bool isIdle { get; set; }
        public bool isOnlineOrbit { get; set; }
        public int personaId { get; set; }
        public int platform { get; set; }
        public int playingMode { get; set; }
        public Guid serverGuid { get; set; }
        public string serverName { get; set; }
        public long userId { get; set; }
    }
}
