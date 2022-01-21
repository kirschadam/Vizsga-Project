using Base_service.DatabaseManagers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Base_service
{
    public class StockService : BaseDatabaseCommands, IStockService
    {
        const string join_products = "INNER JOIN `products` ON `stocks`.`productId` = `products`.`id`";
        const string join_locations = " INNER JOIN `locations` ON `stocks`.`locationId` = `locations`.`id`";
        public Response_Stock ListStock(string uid, [Optional] string name, [Optional] string location)
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

            return response;
        }
    }
}
