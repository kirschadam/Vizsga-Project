using Base_service.JsonClasses;
using System;
using System.Runtime.InteropServices;

namespace Base_service
{
    public class StockService : DatabaseManager.BaseDatabaseCommands, Interfaces.IStockService
    {
        public string AddProduct(string uid, string name, string unitPrice)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseInsert("products", "`name`, `unitPrice`", $"'{name}','{unitPrice}'");
            
            if (result.Item1 != null && result.Item1 > 0) return "Product successfully added!";
            else return result.Item2;
        }

        public string AddSalePurchase(string uid, string type, string product, string quantity, string location)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";
            if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

            //Checking the name of the product to find the corresponding product Id
            var result_read = BaseSelect("products", "`id`", $"WHERE `name` = '{product}'", "");

            string productId = "";
            if (result_read.Item1.Read()) productId = result_read.Item1["id"].ToString();
            if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
            if (result_read.Item2 != "") return result_read.Item2;

            //Checking the name of the location to find the corresponding location Id
            result_read = BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

            string locationId = "";
            if (result_read.Item1.Read()) locationId = result_read.Item1["id"].ToString();
            if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
            if (result_read.Item2 != "") return result_read.Item2;


            //Formatting current time to sql accepted format
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            int? id = current_users[uid].Id;

            var result = BaseInsert(type + "s", "`productId`, `quantity`, `date`, `locationId`, `userId`", $"'{productId}','{quantity}','{date}','{locationId}','{id}'");
            if (result.Item1 != null && result.Item1 > 0) return "Sale/Purchase successfully added!";
            else return result.Item2;
        }

        public string AddStock(string uid, string product, string location)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Checking the name of the product to find the corresponding product Id
            var result_read = BaseSelect("products", "`id`", $"WHERE `name` = '{product}'", "");

            string productId = "";
            if (result_read.Item1.Read()) productId = result_read.Item1["id"].ToString();
            if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
            if (result_read.Item2 != "") return result_read.Item2;

            //Checking the name of the location to find the corresponding location Id
            result_read = BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

            string locationId = "";
            if (result_read.Item1.Read()) locationId = result_read.Item1["id"].ToString();
            if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
            if (result_read.Item2 != "") return result_read.Item2;


            var result = BaseInsert("stocks", "`productId`,`locationId`,`quantity`", $"'{productId}','{locationId}','0'");
            if (result.Item1 != null && result.Item1 > 0)  return "Stock successfully added!";
            else return result.Item2;
        }

        public Response_Product ListProduct(string uid, [Optional] string id, [Optional] string name, [Optional] string qOver, [Optional] string qUnder, [Optional] string limit)
        {
            Response_Product response = new Response_Product();

            if (!current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            string conditions = "";
            string[] inputs = new string[] { id, name, qOver, qUnder, limit };
            for (int i = 0; i < inputs.Length; i++)
            {
                //If the given input holds information, make a condition about it
                if (inputs[i] != null && inputs[i].Length != 0)
                {
                    switch (i)
                    {
                        case 0: { conditions += $"`id`='{inputs[i]}'"; break; }
                        case 1: { conditions += $"`name`='{inputs[i]}'"; break; }
                        case 2: { conditions += $"`unitPrice` > '{inputs[i]}'"; break; }
                        case 3: { conditions += $"`unitPrice` < {inputs[i]}'"; break; }
                        case 4: { conditions += $" LIMIT {inputs[i]}"; break; }
                    }
                }
            }

            //If there are conditions, put the WHERE keyword to the beginning
            //put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "'"
            if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

            var result = BaseSelect("products", "*", conditions, "");
            var reader = result.Item1;
            while (reader.Read())
            {
                response.Products.Add(new Product(
                    int.Parse(reader["id"].ToString()),
                    reader["name"].ToString(),
                    int.Parse(reader["unitPrice"].ToString())
                    ));
            }

            if (result.Item3.Connection.State == System.Data.ConnectionState.Open) result.Item3.Connection.Close();

            if (result.Item2 != "") response.Message = result.Item2;
            else response.Message = $"Number of products found: {response.Products.Count}";

            return response;
        }

        public Response_SalePurchase ListSalePurchase(string uid, string type, [Optional] string id, [Optional] string product, [Optional] string qOver, [Optional] string qUnder, [Optional] string before, [Optional] string after, [Optional] string location, [Optional] string user, [Optional] string limit)
        {
            Response_SalePurchase response = new Response_SalePurchase();

            if (!current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            if (type == null || type == "") { response.Message = "State if the transaction is a sale or a purchase!"; return response; }

            string conditions = "";
            string[] inputs = new string[] { id, product, qOver, qUnder, before, after, location, user, limit };
            for (int i = 0; i < inputs.Length; i++)
            {
                //If the given input holds information, make a condition about it
                if (inputs[i] != null && inputs[i].Length != 0)
                {
                    switch (i)
                    {
                        case 0: { conditions += $"`{type}s`.`id`='{inputs[i]}'"; break; }
                        case 1: { conditions += $"`products`.`name`='{inputs[i]}'"; break; }
                        case 2: { conditions += $"`quantity` > '{inputs[i]}'"; break; }
                        case 3: { conditions += $"`quantity` < {inputs[i]}'"; break; }
                        case 4: { conditions += $"`date` < '{inputs[i]}'"; break; }
                        case 5: { conditions += $"`date` > '{inputs[i]}'"; break; }
                        case 6: { conditions += $"`locations`.`name`='{inputs[i]}'"; break; }
                        case 7: { conditions += $"`users`.`username`='{inputs[i]}'"; break; }
                        case 8: { conditions += $" LIMIT {limit}"; break; }
                    }
                }
            }

            //If there are conditions, put the WHERE keyword to the beginning
            //put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "'"
            if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

            var result = BaseSelect(
                type + "s",
                $"`{type}s`.`id` AS 'id', `products`.`name` AS 'product', `quantity`, `date`, `locations`.`name` AS 'location', `users`.`username` AS 'user'",
                conditions,
                $"INNER JOIN `products` ON `{type}s`.`productId` = `products`.`id` INNER JOIN `locations` ON `{type}s`.`locationId` = `locations`.`id` INNER JOIN `users` ON `{type}s`.`userId` = `users`.`id`");

            var reader = result.Item1;
            while (reader.Read())
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

            if (result.Item3.Connection.State == System.Data.ConnectionState.Open) result.Item3.Connection.Close();

            if (response.SalesPurchases.Count != 0) response.Message = result.Item2;
            else response.Message = $"Number of sales/purchases found: {response.SalesPurchases.Count}";

            return response;
        }

        public Response_Stock ListStock(string uid, [Optional] string id, [Optional] string product, [Optional] string location, [Optional] string qOver, [Optional] string qUnder, [Optional] string limit)
        {
            Response_Stock response = new Response_Stock();

            if (!current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            string conditions = "";
            string[] inputs = new string[] { id, product, location, qOver, qUnder, limit };
            for (int i = 0; i < 6; i++)
            {
                //If the given input holds information, make a condition about it
                if (inputs[i] != null && inputs[i].Length != 0)
                {
                    switch (i)
                    {
                        case 0: { conditions += $"`stocks`.`id`='{inputs[i]}'"; break; }
                        case 1: { conditions += $"`products`.`name`='{inputs[i]}'"; break; }
                        case 2: { conditions += $"`locations`.`name`='{inputs[i]}'"; break; }
                        case 3: { conditions += $"`quantity` > '{inputs[i]}'"; break; }
                        case 4: { conditions += $"`quantity` < {inputs[i]}'"; break; }
                        case 5: { conditions += $" LIMIT {inputs[i]}"; break; }
                    }
                }
            }

            //If there are conditions, put the WHERE keyword to the beginning
            //put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "'"
            if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

            var result = BaseSelect(
                "stocks",
                "`stocks`.`id` AS 'id', `quantity`, `products`.`name` AS 'product', `locations`.`name` AS 'location'",
                conditions,
                "INNER JOIN `products` ON `stocks`.`productId` = `products`.`id` INNER JOIN `locations` ON `stocks`.`locationId` = `locations`.`id`");

            var reader = result.Item1;
            while (reader.Read())
            {
                response.Stocks.Add(new Stock(
                    int.Parse(reader["id"].ToString()),
                    int.Parse(reader["quantity"].ToString()),
                    reader["product"].ToString(),
                    reader["location"].ToString()
                    ));
            }

            if (result.Item3.Connection.State == System.Data.ConnectionState.Open) result.Item3.Connection.Close();

            if (response.Stocks.Count != 0) response.Message = result.Item2;
            else response.Message = $"Number of stocks found: {response.Stocks.Count}";

            return response;
        }

        public string RemoveProduct(string uid, string id)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseDelete("products", $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Product successfully removed!";
            else return result.Item2;
        }

        public string RemoveSalePurchase(string uid, string type, [Optional] string id, [Optional] string location)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";
            if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

            Tuple<int?, string> result = new Tuple<int?, string>(null, null);
            if (id != null && id != "") result = BaseDelete(type + "s", $"`id`='{id}'");
            else if (location != null && location != "")
            {
                var result_read = BaseSelect("locations", "id", $"`name`='{location}'", "");

                if (result_read.Item1.Read()) result = BaseDelete(type + "s", $"`location`={result_read.Item1["id"]}'");
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return result_read.Item2;
            }
            else return "Give an id or location to delete!";


            if (result.Item1 != null && result.Item1 > 0) return "Sale(s)/purchase(s) successfully removed!";
            else return result.Item2;
        }

        public string RemoveStock(string uid, [Optional] string id, [Optional] string location)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            Tuple<int?, string> result = new Tuple<int?, string>(null, null);
            if (id != null && id != "") result = BaseDelete("stocks", $"`id`='{id}'");
            else if (location != null && location != "")
            {
                var result_read = BaseSelect("locations", "id", $"`name`='{location}'", "");

                if (result_read.Item1.Read()) result = BaseDelete("stocks", $"`location`={result_read.Item1["id"]}'");
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return result_read.Item2;
            }
            else return "Give an id or location to delete!";

            if (result.Item1 != null && result.Item1 > 0)return "Stock(s) successfully removed!";
            else return result.Item2;
        }

        public string UpdateProduct(string uid, string id, [Optional] string name, [Optional] string unitPrice)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            string changes = "";
            string[] inputs = new string[] { name, unitPrice };
            for (int i = 0; i < inputs.Length; i++)
            {
                //If the given input holds information, make a condition about it
                if (inputs[i] != null && inputs[i] != "")
                {
                    if (changes != "") changes += ",";
                    switch (i)
                    {
                        case 0: { changes += $"`name`='{inputs[i]}' "; break; }
                        case 1: { changes += $"`unitPrice`='{inputs[i]}' "; break; }
                    }
                }
            }

            var result = BaseUpdate("products", changes, $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Product successfully updated!";
            else return result.Item2;
        }

        public string UpdateSalePurchase(string uid, string id, string type, [Optional] string product, [Optional] string quantity, [Optional] string date, [Optional] string location, [Optional] string user)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";
            if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

            string locationId = "";
            if (location != null && location != "")
            {
                //Checking the name of the location to find the corresponding location Id
                var result_read = BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                if (result_read.Item1.Read()) locationId = result_read.Item1["id"].ToString();
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return result_read.Item2;
            }

            string productId = "";
            if (product != null && product != "")
            {
                //Checking the name of the product to find the corresponding product Id
                var result_read = BaseSelect("products", "`id`", $"WHERE `name` = '{product}'", "");

                if (result_read.Item1.Read()) productId = result_read.Item1["id"].ToString();
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return result_read.Item2;
            }

            string userId = "";
            if (product != null && product != "")
            {
                //Checking the name of the user to find the corresponding user Id
                var result_read = BaseSelect("users", "`id`", $"WHERE `name` = '{user}'", "");

                if (result_read.Item1.Read()) userId = result_read.Item1["id"].ToString();
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return "User not found in database!";
            }

            string changes = "";
            string[] inputs = new string[] { productId, quantity, date, locationId, userId };
            for (int i = 0; i < inputs.Length; i++)
            {
                //If the given input holds information, make a condition about it
                if (inputs[i] != null && inputs[i] != "")
                {
                    if (changes != "") changes += ",";
                    switch (i)
                    {
                        case 0: { changes += $"`productId`='{inputs[i]}' "; break; }
                        case 1: { changes += $"`quantity`='{inputs[i]}' "; break; }
                        case 2: { changes += $"`date`='{inputs[i]}' "; break; }
                        case 3: { changes += $"`locationId`='{inputs[i]}' "; break; }
                        case 4: { changes += $"`userId`='{inputs[i]}' "; break; }
                    }
                }
            }

            var result = BaseUpdate(type + "s", changes + $",`id`={id}", $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Sale/Purchase successfully updated!";
            else return result.Item2;
        }

        public string UpdateStock(string uid, string id, [Optional] string product, [Optional] string location, [Optional] string quantity)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            string locationId = "";
            if (location != null && location != "")
            {
                //Checking the name of the location to find the corresponding location Id
                var result_read = BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                if (result_read.Item1.Read()) locationId = result_read.Item1["id"].ToString();
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return result_read.Item2;
            }

            string productId = "";
            if (product != null && product != "")
            {
                //Checking the name of the product to find the corresponding product Id
                var result_read = BaseSelect("products", "`id`", $"WHERE `name` = '{product}'", "");

                if (result_read.Item1.Read()) productId = result_read.Item1["id"].ToString();
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return result_read.Item2;
            }

            string changes = "";
            string[] inputs = new string[] { productId, locationId, quantity };
            for (int i = 0; i < inputs.Length; i++)
            {
                //If the given input holds information, make a condition about it
                if (inputs[i] != null && inputs[i] != "")
                {
                    if (changes != "") changes += ",";
                    switch (i)
                    {
                        case 0: { changes += $"`productId`='{inputs[i]}' "; break; }
                        case 1: { changes += $"`locationId`='{inputs[i]}' "; break; }
                        case 2: { changes += $"`quantity`='{inputs[i]}' "; break; }
                    }
                }
            }

            var result = BaseUpdate("stocks", changes, $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Stock successfully updated!";
            else return result.Item2;
        }
    }
}
