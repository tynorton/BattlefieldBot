namespace BattlefieldBot.Battlelog.Proxies
{
    internal class bl_user
    {
        public int createdAt { get; set; }
        public string gravatarMd5 { get; set; }
        public bl_user_presence presence { get; set; }
        public long userId { get; set; }
        public string username { get; set; }
    }
}
