using MySql.Data.MySqlClient;

namespace Base_service.DatabaseManagers
{
    public class BaseDatabaseManager
    {
        public BaseDatabaseManager() { }

        private static MySqlConnection BaseConnection { get; } = new MySqlConnection()
        {
            ConnectionString = "SERVER=localhost; DATABASE=assets; UID=root; PASSWORD=; SSL MODE = none;"
        };

        public MySqlCommand BaseCommand { get; } = new MySqlCommand
        {
            CommandType = System.Data.CommandType.Text,
            Connection = BaseConnection
        };

        public MySqlDataReader BaseReader { get; set; }
    }
}