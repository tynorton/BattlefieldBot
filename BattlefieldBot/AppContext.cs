using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BattlefieldBot
{
    public class AppContext : DbContext
    {
        public const string SQLITE_FILENAME = "BattlefieldBot.db";

        public AppContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppContext>());
        }

        public DbSet<BattlelogUser> Users { get; set; }
        //public DbSet<EventData> EventDataValues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Database does not pluralize table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    } 
}
