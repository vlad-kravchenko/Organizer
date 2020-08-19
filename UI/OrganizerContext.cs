using System.Data.Entity;
using System.Data.SQLite;

namespace UI
{
    public class OrganizerContext : DbContext
    {
        public OrganizerContext()
        {
            if (!System.IO.File.Exists("Organizer.db"))
            {
                SQLiteConnection.CreateFile("Organizer.db");

                using (var sqlite2 = new SQLiteConnection("Data Source=Organizer.db"))
                {
                    sqlite2.Open();
                    string sql = @"CREATE TABLE [Tasks] 
                            ([Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
                            [Text] text NOT NULL, 
                            [RepeatRule] text NOT NULL, 
                            [FixedTime] bigint NOT NULL, 
                            [Hour] bigint NOT NULL, 
                            [Minute] bigint NOT NULL, 
                            [StartDate] text NOT NULL);";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                    command.ExecuteNonQuery();
                }
            }
            Database.SetInitializer<OrganizerContext>(null);
        }

        public DbSet<UI.Task> Tasks { get; set; }
    }
}