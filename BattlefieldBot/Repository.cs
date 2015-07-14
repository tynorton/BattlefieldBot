using System;
using System.Collections.Generic;

namespace BattlefieldBot
{
    [LifecycleTransient]
    public class Repository : IRepository
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public BattlelogUser GetUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BattlelogUser> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
