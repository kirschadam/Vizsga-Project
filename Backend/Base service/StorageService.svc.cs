using Base_service.JsonClasses;
using System;
using System.Data;
using System.Runtime.InteropServices;

namespace Base_service
{
    /// <summary>
    /// Class for the various database queries related to products in the store catalog, stocks of various stores and sales/purchases of those stores.
    /// </summary>
    public class StockService : DatabaseManager.BaseDatabaseCommands, Interfaces.IStockService
    {
        public string AddProduct(string uid, string name, string buyUnitPrice, string sellUnitPrice)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseInsert("products", "`name`, `buyUnitPrice`, `sellUnitPrice`", $"'{name}','{buyUnitPrice}','{sellUnitPrice}'");

            if (result.Item2 != "") return result.Item2;
            else return "Product successfully added!";
        }




        public string AddSalePurchase(string uid, string type, string product, string quantity, string location, [Optional] string unitPrice , [Optional] string date)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";
            if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

            //Checking the name of the product to find the corresponding product Id
            var result_read = BaseSelect("products", "`id`", new string[,] { {"`name`", "=", $"'{product}'" } }, "");

            if(unitPrice != null && unitPrice != "")
            {
                unitPrice = (int.Parse(unitPrice) * int.Parse(quantity)).ToString();
            }

            string productId;
            if (result_read.Item1.Rows.Count != 0) 
            { 
                productId = result_read.Item1.Rows[0]["id"].ToString();
                if (unitPrice == null || unitPrice == "")
                {
                    unitPrice = result_read.Item1.Rows[0][$"{(type == "purchase" ? "buyUnitPrice" : "sellUnitPrice")}"].ToString();
                    unitPrice = (int.Parse(unitPrice) * int.Parse(quantity)).ToString();
                }
            }
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
            else date = DateTime.Parse(date).ToString("yyyy-MM-dd HH:mm:ss.fff");

            int? userId = Current_users[uid].Id;

            var result = BaseInsert(type + "s", "`productId`, `quantity`, `totalPrice`, `date`, `locationId`, `userId`", $"'{productId}','{quantity}','{unitPrice}','{date}','{locationId}','{userId}'");

            if (result.Item2 != "") return result.Item2;
            else return "Sale/Purchase successfully added!";
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


            var result = BaseInsert("stocks", "`productId`, `locationId`, `quantity`", $"'{productId}','{locationId}','0'");

            if (result.Item2 != "") return result.Item2;
            else return "Stock successfully added!";
        }




        public Response_Product ListProduct(string uid, [Optional] string id, [Optional] string name, [Optional] string buyOver, [Optional] string buyUnder, [Optional] string sellOver, [Optional] string sellUnder, [Optional] string limit)
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
                { "`buyUnitPrice`", ">", $"'{buyOver}'" },
                { "`buyUnitPrice`", "<", $"'{buyUnder}'" },
                { "`sellUnitPrice`", ">", $"'{sellOver}'" },
                { "`sellUnitPrice`", "<", $"'{sellUnder}'"},
                { " LIMIT", " ", $"'{limit}'" }
            };

            var result = BaseSelect("products", "*", conditions, "");

            foreach (DataRow reader in result.Item1.Rows)
            {
                try
                {
                    response.Products.Add(new Product(
                        int.Parse(reader["id"].ToString()),
                        reader["name"].ToString(),
                        int.Parse(reader["buyPrice"].ToString()),
                        int.Parse(reader["sellPrice"].ToString())
                        ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    response.Message += $"Result with the id '{reader["id"]}' could not be converted correctly!";
                }
            }

            if (result.Item2 != "") response.Message += result.Item2;
            else response.Message += $"Number of products found: {response.Products.Count}";

            return response;
        }




        public Response_SalePurchase ListSalePurchase(string uid, string type, [Optional] string id, [Optional] string product, [Optional] string quantityOver, [Optional] string quantityUnder, [Optional] string priceOver, [Optional] string priceUnder, [Optional] string before, [Optional] string after, [Optional] string location, [Optional] string username, [Optional] string limit)
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
                { "`quantity`", ">", $"'{quantityOver}'" },
                { "`quantity`", "<", $"'{quantityUnder}'" },
                { "`totalPrice`", ">", $"'{priceOver}'" },
                { "`totalPrice`", "<", $"'{priceUnder}'" },
                { "`date`", ">", $"'{before}'" },
                { "`date`", "<", $"'{after}'" },
                { "`locations`.`name`", "=", $"'{location}'" },
                { "`users`.`username`", "=", $"'{username}'" },
                { " LIMIT", " ", $"'{limit}'" }
            };
            
            var result = BaseSelect(
                type + "s",
                $"`{type}s`.`id` AS 'id',`products`.`name` AS 'product',`totalPrice`,`quantity`,`date`,`locations`.`name` AS 'location',`users`.`username` AS 'user'",
                conditions,
                $"INNER JOIN `products` ON `{type}s`.`productId` = `products`.`id` INNER JOIN `locations` ON `{type}s`.`locationId` = `locations`.`id` INNER JOIN `users` ON `{type}s`.`userId` = `users`.`id`");


            foreach (DataRow reader in result.Item1.Rows)
            {
                try
                {
                    response.SalesPurchases.Add(new SalePurchase(
                        int.Parse(reader["id"].ToString()),
                        reader["product"].ToString(),
                        int.Parse(reader["quantity"].ToString()),
                        int.Parse(reader["totalPrice"].ToString()),
                        DateTime.Parse(reader["date"].ToString()),
                        reader["location"].ToString(),
                        reader["user"].ToString()
                        ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    response.Message += $"Result with the id '{reader["id"]}' could not be converted correctly!";
                }
            }

            if (result.Item2 != "") response.Message += result.Item2;
            else response.Message += $"Number of sales/purchases found: {response.SalesPurchases.Count}";

            return response;
        }




        public Response_Stock ListStock(string uid, [Optional] string id, [Optional] string product, [Optional] string location, [Optional] string quantityOver, [Optional] string quantityUnder, [Optional] string limit)
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
                {"`quantity`", ">", $"'{quantityOver}'" },
                {"`quantity`", "<", $"'{quantityUnder}'" },
                {" LIMIT", " ", $"'{limit}'" }
            };
            
            var result = BaseSelect(
                "stocks",
                "`stocks`.`id` AS 'id',`quantity`,`products`.`name` AS 'product',`locations`.`name` AS 'location'",
                conditions,
                "INNER JOIN `products` ON `stocks`.`productId` = `products`.`id` INNER JOIN `locations` ON `stocks`.`locationId` = `locations`.`id`");

            foreach (DataRow reader in result.Item1.Rows)
            {

                try
                {
                    response.Stocks.Add(new Stock(
                        int.Parse(reader["id"].ToString()),
                        int.Parse(reader["quantity"].ToString()),
                        reader["product"].ToString(),
                        reader["location"].ToString()
                        ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    response.Message += $"Result with the id '{reader["id"]}' could not be converted correctly!";
                }
            }

            if (result.Item2 != "") response.Message += result.Item2;
            else response.Message += $"Number of stocks found: {response.Stocks.Count}";

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

            if (result.Item2 != "") return result.Item2;
            else return "Sale(s)/purchase(s) successfully removed!";
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

            if (result.Item2 != "") return result.Item2;
            else return "Stock(s) successfully removed!";
        }




        public string UpdateProduct(string uid, string id, [Optional] string name, [Optional] string buyPrice, [Optional] string sellPrice)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            string[,] changes = 
            { 
                { "`name`", $"'{name}'" }, 
                { "`buyPrice`", $"'{buyPrice}'" },
                { "`sellPrice`", $"'{sellPrice}'" }
            };

            var result = BaseUpdate("products", changes, $"`id`='{id}'");

            if (result.Item2 != "") return result.Item2;
            else return "Product successfully updated!";
        }




        public string UpdateSalePurchase(string uid, string id, string type, [Optional] string product, [Optional] string quantity, [Optional] string totalPrice, [Optional] string date, [Optional] string location, [Optional] string username)
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

            date = DateTime.Parse(date).ToString("yyyy-MM-dd HH:mm:ss.fff");

            string[,] changes = 
            {
                { "`productId`", $"'{productId}'" },
                { "`quantity`", $"'{quantity}'" },
                { "`totalPrice`", $"'{totalPrice}'" },
                { "`date`", $"'{date}'" },
                { "`locationId`", $"'{locationId}'" },
                { "`userId`", $"'{userId}'" },
                { "`id`", $"'{id}'" }
            };

            var result = BaseUpdate(type + "s", changes, $"`id`='{id}'");

            if (result.Item2 != "") return result.Item2;
            else return "Sale/Purchase successfully updated!";
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

            if (result.Item2 != "") return result.Item2;
            else return "Stock successfully updated!";
        }
    }
}
