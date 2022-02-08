using Base_service.JsonClasses;
using System;
using System.Data;
using System.Runtime.InteropServices;

namespace Base_service
{
    public class StockService : DatabaseManager.BaseDatabaseCommands, Interfaces.IStockService
    {
        public string AddProduct(string uid, string name, string unitPrice)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseInsert("products", "`name`, `unitPrice`", $"'{name}','{unitPrice}'");
            
            if (result.Item1 != null && result.Item1 > 0) return "Product successfully added!";
            else return result.Item2;
        }




        public string AddSalePurchase(string uid, string type, string product, string quantity, string location, [Optional] string date)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";
            if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

            //Checking the name of the product to find the corresponding product Id
            var result_read = BaseSelect("products", "`id`", new string[,] { {"`name`", "=", $"'{product}'" } }, "");

            string productId;
            if (result_read.Item1.Rows.Count != 0) productId = result_read.Item1.Rows[0]["id"].ToString();
            else if (result_read.Item2 != "") return result_read.Item2;
            else return "Product not found in database!";

            //Checking the name of the location to find the corresponding location Id
            result_read = BaseSelect("locations", "`id`", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

            string locationId;
            if (result_read.Item1.Rows.Count != 0) locationId = result_read.Item1.Rows[0]["id"].ToString();
            else if (result_read.Item2 != "") return result_read.Item2;
            else return "Location not found in database!";

            //Formatting current time to sql accepted format if user didn't give date
            if (date == null || date == "") date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            int? id = Current_users[uid].Id;


            var result = BaseInsert(type + "s", "`productId`, `quantity`, `date`, `locationId`, `userId`", $"'{productId}','{quantity}','{date}','{locationId}','{id}'");
            if (result.Item1 != null && result.Item1 > 0) return "Sale/Purchase successfully added!";
            else return result.Item2;
        }




        public string AddStock(string uid, string product, string location)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Checking the name of the product to find the corresponding product Id
            var result_read = BaseSelect("products", "`id`", new string[,] { { "`name`", "=", $"'{product}'" } }, "");

            string productId;
            if (result_read.Item1.Rows.Count != 0) productId = result_read.Item1.Rows[0]["id"].ToString();
            else if (result_read.Item2 != "") return result_read.Item2;
            else return "Product not found in database!";

            //Checking the name of the location to find the corresponding location Id
            result_read = BaseSelect("locations", "`id`", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

            string locationId;
            if (result_read.Item1.Rows.Count != 0) locationId = result_read.Item1.Rows[0]["id"].ToString();
            else if (result_read.Item2 != "") return result_read.Item2;
            else return "Location not found in database!";


            var result = BaseInsert("stocks", "`productId`,`locationId`,`quantity`", $"'{productId}','{locationId}','0'");
            if (result.Item1 != null && result.Item1 > 0)  return "Stock successfully added!";
            else return result.Item2;
        }




        public Response_Product ListProduct(string uid, [Optional] string id, [Optional] string name, [Optional] string qOver, [Optional] string qUnder, [Optional] string limit)
        {
            Response_Product response = new Response_Product();

            if (!Current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            string[,] conditions = 
            {
                { "`id`", "=", $"'{id}'" },
                { "`name`", "=", $"'{name}'" },
                { "`unitPrice`", ">", $"'{qOver}'" },
                { "`unitPrice`", "<", $"'{qUnder}'" },
                { " LIMIT", " ", $"'{limit}'" }
            };

            var result = BaseSelect("products", "*", conditions, "");

            foreach (DataRow reader in result.Item1.Rows)
            {
                response.Products.Add(new Product(
                    int.Parse(reader["id"].ToString()),
                    reader["name"].ToString(),
                    int.Parse(reader["unitPrice"].ToString())
                    ));
            }

            if (result.Item2 != "") response.Message = result.Item2;
            else response.Message = $"Number of products found: {response.Products.Count}";

            return response;
        }




        public Response_SalePurchase ListSalePurchase(string uid, string type, [Optional] string id, [Optional] string product, [Optional] string qOver, [Optional] string qUnder, [Optional] string before, [Optional] string after, [Optional] string location, [Optional] string username, [Optional] string limit)
        {
            Response_SalePurchase response = new Response_SalePurchase();

            if (!Current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            if (type == null || type == "") { response.Message = "State if the transaction is a sale or a purchase!"; return response; }

            string[,] conditions = 
            {
                { $"`{type}s`.`id`", "=", $"'{id}'" },
                { "`products`.`name`", "=", $"'{product}'" },
                { "`quantity`", ">", $"'{qOver}'" },
                { "`quantity", "<", $"'{qUnder}'" },
                { "`date`", ">", $"'{before}'" },
                { "`date`", "<", $"'{after}'" },
                { "`locations`.`name`", "=", $"'{location}'" },
                { "`users`.`username`", "=", $"'{username}'" },
                { " LIMIT", " ", $"'{limit}'" }
            };
            
            var result = BaseSelect(
                type + "s",
                $"`{type}s`.`id` AS 'id',`products`.`name` AS 'product',`quantity`,`date`,`locations`.`name` AS 'location',`users`.`username` AS 'user'",
                conditions,
                $"INNER JOIN `products` ON `{type}s`.`productId` = `products`.`id` INNER JOIN `locations` ON `{type}s`.`locationId` = `locations`.`id` INNER JOIN `users` ON `{type}s`.`userId` = `users`.`id`");

            foreach (DataRow reader in result.Item1.Rows)
            {
                response.SalesPurchases.Add(new SalePurchase(
                    int.Parse(reader["id"].ToString()),
                    reader["product"].ToString(),
                    int.Parse(reader["quantity"].ToString()),
                    DateTime.Parse(reader["date"].ToString()),
                    reader["location"].ToString(),
                    reader["user"].ToString()
                    ));
            }

            if (response.SalesPurchases.Count != 0) response.Message = result.Item2;
            else response.Message = $"Number of sales/purchases found: {response.SalesPurchases.Count}";

            return response;
        }




        public Response_Stock ListStock(string uid, [Optional] string id, [Optional] string product, [Optional] string location, [Optional] string qOver, [Optional] string qUnder, [Optional] string limit)
        {
            Response_Stock response = new Response_Stock();

            if (!Current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            string[,] conditions =
            {
                {"`stocks`.`id`", "=", $"'{id}'" },
                {"`products`.`name`", "=", $"'{product}'" },
                {"`locations`.`name`", "=", $"'{location}'" },
                {"`quantity`", ">", $"'{qOver}'" },
                {"`quantity`", "<", $"'{qUnder}'" },
                {" LIMIT", " ", $"'{limit}'" }
            };
            
            var result = BaseSelect(
                "stocks",
                "`stocks`.`id` AS 'id',`quantity`,`products`.`name` AS 'product',`locations`.`name` AS 'location'",
                conditions,
                "INNER JOIN `products` ON `stocks`.`productId` = `products`.`id` INNER JOIN `locations` ON `stocks`.`locationId` = `locations`.`id`");

            foreach (DataRow reader in result.Item1.Rows)
            {
                response.Stocks.Add(new Stock(
                    int.Parse(reader["id"].ToString()),
                    int.Parse(reader["quantity"].ToString()),
                    reader["product"].ToString(),
                    reader["location"].ToString()
                    ));
            }

            if (response.Stocks.Count != 0) response.Message = result.Item2;
            else response.Message = $"Number of stocks found: {response.Stocks.Count}";

            return response;
        }




        public string RemoveProduct(string uid, string id)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseDelete("products", $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Product successfully removed!";
            else return result.Item2;
        }




        public string RemoveSalePurchase(string uid, string type, [Optional] string id, [Optional] string location)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";
            if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

            //Check for case under which we delete
            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete(type + "s", $"`id`='{id}'");
            else if (location != null && location != "")
            {
                var result_read = BaseSelect("locations", "id", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

                if (result_read.Item1.Rows.Count != 0) result = BaseDelete(type + "s", $"`location`={result_read.Item1.Rows[0]["id"]}'");
                else if (result_read.Item2 != "") return result_read.Item2;
                else return "Location not found in database!";
            }
            else return "Give an id or location to delete!";


            if (result.Item1 != null && result.Item1 > 0) return "Sale(s)/purchase(s) successfully removed!";
            else return result.Item2;
        }




        public string RemoveStock(string uid, [Optional] string id, [Optional] string location)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Check for case under which we delete
            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete("stocks", $"`id`='{id}'");
            else if (location != null && location != "")
            {
                var result_read = BaseSelect("locations", "id", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

                if (result_read.Item1.Rows.Count != 0) result = BaseDelete("stocks", $"`location`={result_read.Item1.Rows[0]["id"]}'");
                else if (result_read.Item2 != "") return result_read.Item2;
                else return "Location not found in database!";
            }
            else return "Give an id or location to delete!";

            if (result.Item1 != null && result.Item1 > 0)return "Stock(s) successfully removed!";
            else return result.Item2;
        }




        public string UpdateProduct(string uid, string id, [Optional] string name, [Optional] string unitPrice)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            string[,] changes = 
            { 
                { "`name`", $"'{name}'" }, 
                { "`unitPrice`", $"'{unitPrice}'" } 
            };

            var result = BaseUpdate("products", changes, $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Product successfully updated!";
            else return result.Item2;
        }




        public string UpdateSalePurchase(string uid, string id, string type, [Optional] string product, [Optional] string quantity, [Optional] string date, [Optional] string location, [Optional] string username)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";
            if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

            //Checking the name of the location to find the corresponding location Id
            string locationId = "";
            if (location != null && location != "")
            {
                var result_read = BaseSelect("locations", "`id`", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

                if (result_read.Item1.Rows.Count != 0) locationId = result_read.Item1.Rows[0]["id"].ToString();
                else if (result_read.Item2 != "") return result_read.Item2;
                else return "Location not found in database!";
            }

            //Checking the name of the product to find the corresponding product Id
            string productId = "";
            if (product != null && product != "")
            {
                var result_read = BaseSelect("products", "`id`", new string[,] { { "`name`", "=", $"'{product}'" } }, "");

                if (result_read.Item1.Rows.Count != 0) productId = result_read.Item1.Rows[0]["id"].ToString();
                else if (result_read.Item2 != "") return result_read.Item2;
                else return "Product not found in database!";
            }

            //Checking the name of the user to find the corresponding user Id
            string userId = "";
            if (product != null && product != "")
            {
                var result_read = BaseSelect("users", "`id`", new string[,] { { "`username`", "=", $"'{username}'" } }, "");

                if (result_read.Item1.Rows.Count != 0) userId = result_read.Item1.Rows[0]["id"].ToString();
                else if (result_read.Item2 != "") return result_read.Item2;
                else return "User not found in database!";
            }

            string[,] changes = 
            {
                { "`productId`", $"'{productId}'" },
                { "`quantity`", $"'{quantity}'" },
                { "`date`", $"'{date}'" },
                { "`locationId`", $"'{locationId}'" },
                { "`userId`", $"'{userId}'" },
                { "`id`", $"'{id}'" }
            };

            var result = BaseUpdate(type + "s", changes, $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Sale/Purchase successfully updated!";
            else return result.Item2;
        }




        public string UpdateStock(string uid, string id, [Optional] string product, [Optional] string location, [Optional] string quantity)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Checking the name of the location to find the corresponding location Id
            string locationId = "";
            if (location != null && location != "")
            {
                var result_read = BaseSelect("locations", "`id`", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

                if (result_read.Item1.Rows.Count != 0) locationId = result_read.Item1.Rows[0]["id"].ToString();
                else if (result_read.Item2 != "") return result_read.Item2;
                else return "Location not found in database!";
            }

            //Checking the name of the product to find the corresponding product Id
            string productId = "";
            if (product != null && product != "")
            {
                var result_read = BaseSelect("products", "`id`", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

                if (result_read.Item1.Rows.Count != 0) productId = result_read.Item1.Rows[0]["id"].ToString();
                else if (result_read.Item2 != "") return result_read.Item2;
                else return "Product not found in database!";
            }

            string[,] changes = 
            { 
                { "`productId`", $"'{productId}'" },
                { "`locationId`", $"'{locationId}'" },
                { "`quantity`", $"'{quantity}'" } 
            };

            var result = BaseUpdate("stocks", changes, $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Stock successfully updated!";
            else return result.Item2;
        }
    }
}
