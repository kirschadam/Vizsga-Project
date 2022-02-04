using Base_service.DatabaseManagers;
using Base_service.JsonClasses;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Runtime.InteropServices;

namespace Base_service
{
    public class StockService : BaseDatabaseCommands, Interfaces.IStockService
    {
        /*const string join_products = "INNER JOIN `products` ON `stocks`.`productId` = `products`.`id`";
        const string join_locations = " INNER JOIN `locations` ON `stocks`.`locationId` = `locations`.`id`";
        public Response_Stock ListStock(string uid, [Optional] string name, [Optional] string location, [Optional] string quantity, [Optional] string limit)
        {
            Response_Stock response = new Response_Stock();

            if (!UserService.current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            try
            {
                string conditions = "";
                if(name != null && name != "")
                {
                    conditions += $"`products`.`name` = '{name}'";
                }

                if (location != null && location != "")
                {
                    conditions += $"`locations`.`name` = '{location}'";
                }

                if(conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

                BaseSelect(
                    "stocks", 
                    "`stocks`.`id` AS 'id', `quantity`, `products`.`name` AS 'product', `locations`.`name` AS 'location'", 
                    conditions, 
                    join_products + join_locations);

                if (BaseReader == null) return null;

                while (BaseReader.Read())
                {
                    response.Stocks.Add(new Stock(
                        int.Parse(BaseReader["id"].ToString()),
                        int.Parse(BaseReader["quantity"].ToString()),
                        BaseReader["product"].ToString(),
                        BaseReader["location"].ToString()
                        ));
                }
                response.Message = $"Number of stocks found: {response.Stocks.Count}";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); response.Message = "There was a problem with your request!"; }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            return response;*/
        public static MySqlDataReader db_reader;
        public static MySqlConnection databaseConnection;
        public static MySqlCommand CommandDatabase;

        public Response_Product Query(string query)
        {
            if (db_reader != null) db_reader.Dispose();
            Response_Product response = new Response_Product();
            databaseConnection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=assets;");
            CommandDatabase = new MySqlCommand(query, databaseConnection);
            try
            {
                databaseConnection.Open();

                if (query.StartsWith("UPDATE") || query.StartsWith("INSERT") || query.StartsWith("DELETE"))
                {
                    CommandDatabase.Prepare();
                    if(CommandDatabase.ExecuteNonQuery() > 0)
                    {
                        response.Message = "Sikeres lekérés!";
                    }
                }

                else
                {
                    db_reader = CommandDatabase.ExecuteReader();

                    while (db_reader.Read())
                    {
                        response.Products.Add(new Product(
                                int.Parse(db_reader["id"].ToString()),
                                db_reader["name"].ToString(),
                                int.Parse(db_reader["unitPrice"].ToString())
                            ));
                    }

                    response.Message = $"{response.Products.Count} products found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            if (databaseConnection.State == ConnectionState.Open){ databaseConnection.Close(); }

            return response;
        }
    }
}
