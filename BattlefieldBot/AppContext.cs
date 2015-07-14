using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SQLite;

namespace BattlefieldBot
{
    public class AppContext : DbContext
    {
        public AppContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppContext>());

            SQLiteConnection conn = new SQLiteConnection("App.sqlite");
            conn.CreateTable<BattlelogUser>();
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
