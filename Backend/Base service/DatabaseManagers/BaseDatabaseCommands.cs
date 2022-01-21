using System;
using System.Data;

namespace Base_service.DatabaseManagers
{
    public class BaseDatabaseCommands : BaseDatabaseManager
    {
        public void BaseSelect(string table, string columns, string conditions, string inner_joins)
        {
            if (BaseCommand.Connection.State == ConnectionState.Open) BaseCommand.Connection.Close();

            string query = $"SELECT {columns} FROM `{table}` {inner_joins} {conditions};";

            Console.WriteLine(query);

            BaseCommand.CommandText = query;

            BaseCommand.Connection.Open();

            BaseReader = BaseCommand.ExecuteReader();
        }



        public int? BaseInsert(string table, string columns, string values)
        {
            if (BaseCommand.Connection.State == ConnectionState.Open) BaseCommand.Connection.Close();

            int? affected_rows;
            BaseCommand.CommandText = $"INSERT INTO `{table}`({columns}) VALUES ({values});";

            BaseCommand.Connection.Open();

            affected_rows = BaseCommand.ExecuteNonQuery();
            BaseCommand.Connection.Close();

            return affected_rows;
        }



        public int? BaseUpdate(string table, string sets, string conditions)
        {
            if (BaseCommand.Connection.State == ConnectionState.Open) BaseCommand.Connection.Close();

            int? affected_rows;

            BaseCommand.CommandText = $"UPDATE `{table}` SET {sets} WHERE {conditions};";

            BaseCommand.Connection.Open();

            affected_rows = BaseCommand.ExecuteNonQuery(); BaseCommand.Connection.Close();

            return affected_rows;
        }



        public int? BaseDelete(string table, string conditions)
        {
            if (BaseCommand.Connection.State == ConnectionState.Open) BaseCommand.Connection.Close();

            int? affected_rows;

            BaseCommand.CommandText = $"DELETE FROM `{table}` WHERE {conditions};";

            BaseCommand.Connection.Open();

            affected_rows = BaseCommand.ExecuteNonQuery(); BaseCommand.Connection.Close();

            return affected_rows;
        }
    }
}