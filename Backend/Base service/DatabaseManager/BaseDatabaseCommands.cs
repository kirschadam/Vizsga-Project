using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Base_service.DatabaseManager
{
    public class BaseDatabaseCommands
    {
        public static Dictionary<string, JsonClasses.User> current_users = new Dictionary<string, JsonClasses.User>();

        public static MySqlConnection BaseConnection { get; } = new MySqlConnection()
        {
            ConnectionString = "SERVER=localhost; DATABASE=assets; UID=root; PASSWORD=; SSL MODE = none;"
        };

        public Tuple<MySqlDataReader, string, MySqlCommand> BaseSelect(string table, string columns, string conditions, string inner_joins)
        {
            string query = $"SELECT {columns} FROM `{table}` {inner_joins} {conditions};";

            MySqlCommand command = new MySqlCommand(query, connection: BaseConnection);
            MySqlDataReader reader = null;
            string message = "";

            try
            {
                BaseConnection.Open();
                reader = command.ExecuteReader();
            }
            catch(Exception ex) { Console.WriteLine(query + "\n\n" + ex.Message); message = ex.Message; }

            return new Tuple<MySqlDataReader, string, MySqlCommand>(reader, message, command);
        }



        public Tuple<int?, string> BaseInsert(string table, string columns, string values)
        {
            string query = $"INSERT INTO `{table}`({columns}) VALUES ({values});";

            int? affected_rows = null;
            MySqlCommand command = new MySqlCommand(query, BaseConnection);
            string message = "";

            try
            {
                command.Connection.Open();
                affected_rows = command.ExecuteNonQuery();
            }
            catch (Exception ex) { Console.WriteLine(query + "\n\n" + ex.Message); message = ex.Message; }
            finally
            {
                if (command.Connection.State == ConnectionState.Open)
                    command.Connection.Close();
            }

            return new Tuple<int?, string>(affected_rows, message);
        }



        public Tuple<int?, string> BaseUpdate(string table, string sets, string conditions)
        {
            string query = $"UPDATE `{table}` SET {sets} WHERE {conditions};";

            int? affected_rows = null;
            MySqlCommand command = new MySqlCommand(query, BaseConnection);
            string message = "";

            try
            {
                command.Connection.Open();
                affected_rows = command.ExecuteNonQuery();
            }
            catch (Exception ex) { Console.WriteLine(query + "\n\n" + ex.Message); message = ex.Message; }
            finally
            {
                if (command.Connection.State == ConnectionState.Open)
                    command.Connection.Close();
            }

            return new Tuple<int?, string>(affected_rows, message);
        }



        public Tuple<int?, string> BaseDelete(string table, string conditions)
        {
            string query = $"DELETE FROM `{table}` WHERE {conditions};";

            int? affected_rows = null;
            MySqlCommand command = new MySqlCommand(query, BaseConnection);
            string message = "";

            try
            {
                command.Connection.Open();
                affected_rows = command.ExecuteNonQuery();
            }
            catch (Exception ex) { Console.WriteLine(query + "\n\n" + ex.Message); message = ex.Message; }
            finally
            {
                if (command.Connection.State == ConnectionState.Open)
                    command.Connection.Close();
            }

            return new Tuple<int?, string>(affected_rows, message);
        }
    }
}