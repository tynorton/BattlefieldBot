namespace BattlefieldBot.BattlelogModel
{
    internal class bl_user_persona
    {
        public string clanTag { get; set; }
        public int games { get; set; }
        public string @namespace { get; set; }
        public int personaId { get; set; }
        public string personaName { get; set; }
        public string picture { get; set; }
        public int updatedAt { get; set; }
        public bl_user user { get; set; }
        public long userId { get; set; }
    }
}
