using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Base_service.DatabaseManager
{
    public class BaseDatabaseCommands
    {
        public static Dictionary<string, JsonClasses.User> Current_users { get; } = new Dictionary<string, JsonClasses.User>();

        public static MySqlConnection BaseConnection { get; } = new MySqlConnection("SERVER=localhost; DATABASE=assets; UID=root; PASSWORD=; SSL MODE = none;");

        public Tuple<DataTable, string> BaseSelect(string table, string columns, string[,] condition_array, string inner_joins)
        {
            //put the conditions together into a string
            string conditions = "";
            for (int i = 0; i < condition_array.GetLength(0); i++)
            {
                if (condition_array[i, 2] == "''") continue;

                for (int j = 0; j < condition_array.GetLength(1); j++)
                {
                    conditions += condition_array[i, j];
                }
            }

            //If there are conditions, put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "'"
            if (conditions != "") conditions = conditions.Replace("'`", "' AND `").Replace("%`", "% AND `");
            else conditions = "1";

            string query = $"SELECT {columns} FROM `{table}` {inner_joins} WHERE {conditions};";

            DataTable result = new DataTable(); string message = "";

            using(MySqlDataAdapter adapter = new MySqlDataAdapter(new MySqlCommand(query, BaseConnection)))
            {
                try { adapter.Fill(result); }
                catch (Exception ex) { Console.WriteLine(query + "\n\n" + ex.Message); message = ex.Message; }
            }

            return new Tuple<DataTable, string>(result, message);
        }



        public Tuple<int?, string> BaseInsert(string table, string columns, string values)
        {
            string query = $"INSERT INTO `{table}`({columns}) VALUES ({values});";

            int? affected_rows = null; string message = "";

            using (MySqlCommand command = new MySqlCommand(query, BaseConnection))
            {
                try
                {
                    command.Connection.Open();
                    affected_rows = command.ExecuteNonQuery();
                }
                catch (Exception ex) { Console.WriteLine(query + "\n\n" + ex.Message); message = ex.Message; }
                finally
                {
                    if (command.Connection.State == ConnectionState.Open) command.Connection.Close();
                }
            }

            return new Tuple<int?, string>(affected_rows, message);
        }



        public Tuple<int?, string> BaseUpdate(string table, string[,] sets, string conditions)
        {
            //put the conditions together into a string
            string set = "";
            for (int i = 0; i < sets.GetLength(0); i++)
            {
                if (sets[i, 1] == "''") continue;
                if (set != "") set += ",";

                set += sets[i,0] + "=" + sets[i,1];
            }

            string query = $"UPDATE `{table}` SET {set} WHERE {conditions};";

            int? affected_rows = null; string message = "";

            using (MySqlCommand command = new MySqlCommand(query, BaseConnection))
            {
                try
                {
                    command.Connection.Open();
                    affected_rows = command.ExecuteNonQuery();
                }
                catch (Exception ex) { Console.WriteLine(query + "\n\n" + ex.Message); message = ex.Message; }
                finally
                {
                    if (command.Connection.State == ConnectionState.Open) command.Connection.Close();
                }
            }

            return new Tuple<int?, string>(affected_rows, message);
        }



        public Tuple<int?, string> BaseDelete(string table, string conditions)
        {
            string query = $"DELETE FROM `{table}` WHERE {conditions};";

            int? affected_rows = null; string message = "";

            using (MySqlCommand command = new MySqlCommand(query, BaseConnection))
            {
                try
                {
                    command.Connection.Open();
                    affected_rows = command.ExecuteNonQuery();
                }
                catch (Exception ex) { Console.WriteLine(query + "\n\n" + ex.Message); message = ex.Message; }
                finally
                {
                    if (command.Connection.State == ConnectionState.Open) command.Connection.Close();
                }
            }

            return new Tuple<int?, string>(affected_rows, message);
        }
    }
}