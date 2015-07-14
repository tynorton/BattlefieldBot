using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldBot
{
    public interface IRepository : IDisposable
    {
        BattlelogUser GetUser(Guid userId);

        IEnumerable<BattlelogUser> GetUsers();
    }
}