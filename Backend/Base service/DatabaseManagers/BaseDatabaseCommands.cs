using System;
using System.Data;

namespace Base_service.DatabaseManagers
{
    public class BaseDatabaseCommands : BaseDatabaseManager
    {
        public void BaseSelect(string[] parts)
        {
            if (BaseCommand.Connection.State == ConnectionState.Open) BaseCommand.Connection.Close();

            string query = null;

            //Switch case, decides what the Select funtion that called this has in it
            //the parts in order are: 0 - table name, 1 - requested columns, 2 - request conditions, 3 - inner joins, if needed
            switch (parts.Length)
            {
                case 1:
                    {
                        query = $"SELECT * FROM `{parts[0]}`;";
                        break;
                    }
                case 2:
                    {
                        query = $"SELECT {parts[1]} FROM `{parts[0]}`;";
                        break;
                    }
                case 3:
                    {
                        query = $"SELECT {parts[1]} FROM `{parts[0]}` {parts[2]};";
                        break;
                    }
                case 4:
                    {
                        query = $"SELECT {parts[1]} FROM `{parts[0]}` {parts[3]} {parts[2]};";
                        break;
                    }
            }
            Console.WriteLine(query);

            BaseCommand.CommandText = query;

            BaseCommand.Connection.Open();

            BaseReader = BaseCommand.ExecuteReader();
        }



        public int? BaseInsert(string[] parts)
        {
            if (BaseCommand.Connection.State == ConnectionState.Open) BaseCommand.Connection.Close();

            int? affected_rows;
            BaseCommand.CommandText = $"INSERT INTO `{parts[0]}`({parts[1]}) VALUES ({parts[2]});";

            BaseCommand.Connection.Open();

            affected_rows = BaseCommand.ExecuteNonQuery();
            BaseCommand.Connection.Close();

            return affected_rows;
        }



        public int? BaseUpdate(string[] parts)
        {
            if (BaseCommand.Connection.State == ConnectionState.Open) BaseCommand.Connection.Close();

            int? affected_rows;

            BaseCommand.CommandText = $"UPDATE `{parts[0]}` SET {parts[1]} WHERE {parts[2]};";

            BaseCommand.Connection.Open();

            affected_rows = BaseCommand.ExecuteNonQuery(); BaseCommand.Connection.Close();

            return affected_rows;
        }



        public int? BaseDelete(string[] parts)
        {
            if (BaseCommand.Connection.State == ConnectionState.Open) BaseCommand.Connection.Close();

            int? affected_rows;

            BaseCommand.CommandText = $"DELETE FROM `{parts[0]}` WHERE {parts[1]};";

            BaseCommand.Connection.Open();

            affected_rows = BaseCommand.ExecuteNonQuery(); BaseCommand.Connection.Close();

            return affected_rows;
        }
    }
}