using System;
using System.Collections.Generic;
using System.Linq;

namespace BattlefieldBot
{
    public class UserPresenceRegistry
    {
        public DateTime? LastUpdated
        {
            get
            {
                var userPresenceSnapshot = this.HistoricalSnapshots.OrderBy(obj => obj.Date).FirstOrDefault();
                if (userPresenceSnapshot != null)
                {
                    return userPresenceSnapshot.Date;
                }

                return null;
            }
        }

        public IList<UserPresenceSnapshot> HistoricalSnapshots { get; set; } 
    }
}
