using System;

namespace BattlefieldBot
{
    public class UserPresenceSnapshot
    {
        public DateTime Date { get; set; }

        public Guid UserID { get; set; }

        public string UserName { get; set; }

        public bool IsOnline { get; set; }

        public bool IsPlaying { get; set; }

        public Guid ServerID { get; set; }
    }
}
