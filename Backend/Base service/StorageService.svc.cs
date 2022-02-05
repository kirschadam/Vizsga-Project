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
        public string AddProduct(string uid, string name, string unitPrice)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                result = BaseInsert("products", "`name`,`unitPrice`", $"'{name}','{unitPrice}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Product successfully added!";
            }
            else return "There was an issue with your request!";
        }

        public string AddSalePurchase(string uid, string type, string product, string quantity, string location)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

                //Checking the name of the product to find the corresponding product Id
                BaseSelect("products", "`id`", $"WHERE `name` = '{product}'", "");

                string productId = "";
                if (BaseReader.Read()) productId = BaseReader["id"].ToString();
                else return "Location not found in database!";

                //Checking the name of the location to find the corresponding location Id
                BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                string locationId = "";
                if (BaseReader.Read()) locationId = BaseReader["id"].ToString();
                else return "Location not found in database!";

                string sqlFormattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                int? id = UserService.current_users[uid].Id;

                if (id == null) return "Something is wrong with user information!";

                result = BaseInsert( type + "s", "`productId`, `quantity`, `date`, `locationId`, `userId`", $"'{productId}','{quantity}','{sqlFormattedDate}','{locationId}','{id}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Sale/Purchase successfully added!";
            }
            else return "There was an issue with your request!";
        }

        public string AddStock(string uid, string product, string location)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                //Checking the name of the product to find the corresponding product Id
                BaseSelect("products", "`id`", $"WHERE `name` = '{product}'", "");

                string productId = "";
                if (BaseReader.Read()) productId = BaseReader["id"].ToString();
                else return "Location not found in database!";

                //Checking the name of the location to find the corresponding location Id
                BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                string locationId = "";
                if (BaseReader.Read()) locationId = BaseReader["id"].ToString();
                else return "Location not found in database!";

                result = BaseInsert("stocks", "`productId`,`locationId`,`quantity`", $"'{productId}','{locationId}','0'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Stock successfully added!";
            }
            else return "There was an issue with your request!";
        }

        public Response_Product ListProduct(string uid, [Optional] string id, [Optional] string name, [Optional] string qOver, [Optional] string qUnder, [Optional] string limit)
        {
            Response_Product response = new Response_Product();

            if (!UserService.current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            try
            {
                string conditions = "";

                string[] inputs = new string[] { id, name, qOver, qUnder, limit };
                for (int i = 0; i < 5; i++)
                {
                    //If the given input holds information, make a condition about it
                    if (inputs[i] != null && inputs[i].Length != 0)
                    {
                        switch (i)
                        {
                            case 0: { conditions += $"`id`='{inputs[i]}'"; break; }
                            case 1: { conditions += $"`name`='{inputs[i]}'"; break; }
                            case 2: { conditions += $"`unitPrice` > '{qOver}'"; break; }
                            case 3: { conditions += $"`unitPrice` < {qUnder}'"; break; }
                            case 4: { conditions += $" LIMIT {inputs[i]}"; break; }
                        }
                    }
                }

                if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

                BaseSelect( "products", "*", conditions, "");

                if (BaseReader == null) return null;

                while (BaseReader.Read())
                {
                    response.Products.Add(new Product(
                        int.Parse(BaseReader["id"].ToString()),
                        BaseReader["name"].ToString(),
                        int.Parse(BaseReader["unitPrice"].ToString())
                        ));
                }
                response.Message = $"Number of products found: {response.Products.Count}";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); response.Message = "There was a problem with your request!"; }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            return response;
        }

        public Response_SalePurchase ListSalePurchase(string uid, string type, [Optional] string id, [Optional] string product, [Optional] string qOver, [Optional] string qUnder, [Optional] string before, [Optional] string after, [Optional] string location, [Optional] string user, [Optional] string limit)
        {
            Response_SalePurchase response = new Response_SalePurchase();

            if (!UserService.current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            try
            {
                if (type == null || type == "") { response.Message = "State if the transaction is a sale or a purchase!"; return response; }

                string conditions = "";

                string[] inputs = new string[] { id, product, qOver, qUnder, before, after, location, user, limit };
                for (int i = 0; i < 9; i++)
                {
                    //If the given input holds information, make a condition about it
                    if (inputs[i] != null && inputs[i].Length != 0)
                    {
                        switch (i)
                        {
                            case 0: { conditions += $"`{type}s`.`id`='{id}'"; break; }
                            case 1: { conditions += $"`products`.`name`='{product}'"; break; }
                            case 2: { conditions += $"`quantity` > '{qOver}'"; break; }
                            case 3: { conditions += $"`quantity` < {qUnder}'"; break; }
                            case 4: { conditions += $"`date` < '{before}'"; break; }
                            case 5: { conditions += $"`date` > '{after}'"; break; }
                            case 6: { conditions += $"`locations`.`name`='{location}'"; break; }
                            case 7: { conditions += $"`users`.`username`='{user}'"; break; }
                            case 8: { conditions += $" LIMIT {limit}"; break; }
                        }
                    }
                }

                if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

                BaseSelect(
                    type + "s",
                    $"`{type}s`.`id` AS 'id', `products`.`name` AS 'product', `quantity`, `date`, `locations`.`name` AS 'location', `users`.`username` AS 'user'",
                    conditions,
                    $"INNER JOIN `products` ON `{type}s`.`productId` = `products`.`id` INNER JOIN `locations` ON `{type}s`.`locationId` = `locations`.`id` INNER JOIN `users` ON `{type}s`.`userId` = `users`.`id`");

                if (BaseReader == null) return null;

                while (BaseReader.Read())
                {
                    response.SalesPurchases.Add(new SalePurchase(
                        int.Parse(BaseReader["id"].ToString()),
                        BaseReader["product"].ToString(),
                        int.Parse(BaseReader["quantity"].ToString()),
                        DateTime.Parse(BaseReader["date"].ToString()),
                        BaseReader["location"].ToString(),
                        BaseReader["user"].ToString()
                        ));
                }
                response.Message = $"Number of sales/purchases found: {response.SalesPurchases.Count}";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); response.Message = "There was a problem with your request!"; }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }
            return response;
        }

        public Response_Stock ListStock(string uid, [Optional] string id, [Optional] string product, [Optional] string location, [Optional] string qOver, [Optional] string qUnder, [Optional] string limit)
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

                if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

                BaseSelect(
                    "stocks",
                    "`stocks`.`id` AS 'id', `quantity`, `products`.`name` AS 'product', `locations`.`name` AS 'location'",
                    conditions,
                    "INNER JOIN `products` ON `stocks`.`productId` = `products`.`id` INNER JOIN `locations` ON `stocks`.`locationId` = `locations`.`id`");

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

        public string RemoveProduct(string uid, string id)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                if (id != null && id != "")
                {
                    result = BaseDelete("products", $"`id`='{id}'");
                }
                else return "Give an id to delete!";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Product successfully removed!";
            }
            else return "There was an issue with your request!";
        }

        public string RemoveSalePurchase(string uid, string type, [Optional] string id, [Optional] string location)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                if(type == null || type == "") return "State if the transaction is a sale or a purchase!";

                if (id != null && id != "")
                {
                    result = BaseDelete( type + "s", $"`id`='{id}'");
                }
                else if(location != null && location != "")
                {
                    BaseSelect("locations", "id", $"`name`='{location}'", "");
                    if (BaseReader.Read())
                    {
                        BaseDelete( type + "s", $"`location`={BaseReader["id"]}'");
                    }
                }
                else return "Give an id or location to delete!";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Sale(s)/purchase(s) successfully removed!";
            }
            else return "There was an issue with your request!";
        }

        public string RemoveStock(string uid, [Optional] string id, [Optional] string location)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                if (id != null && id != "")
                {
                    result = BaseDelete("stocks", $"`id`='{id}'");
                }
                else if(location != null && location != "")
                {
                    BaseSelect("locations", "id", $"`name`='{location}'", "");
                    if (BaseReader.Read())
                    {
                        BaseDelete("stocks", $"`location`={BaseReader["id"]}'");
                    }
                } 
                else return "Give an id or location to delete!";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Stock(s) successfully removed!";
            }
            else return "There was an issue with your request!";
        }

        public string UpdateProduct(string uid, string id, [Optional] string name, [Optional] string unitPrice)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                string changes = "";
                string[] inputs = new string[] { name, unitPrice };
                for (int i = 0; i < 2; i++)
                {
                    if (inputs[i] != null && inputs[i] != "")
                    {
                        if (changes != "") changes += ",";
                        switch (i)
                        {
                            case 0: { changes += $"`name`='{inputs[0]}' "; break; }
                            case 1: { changes += $"`unitPrice`='{inputs[1]}' "; break; }
                        }
                    }
                }

                result = BaseUpdate("products", changes, $"`id`='{id}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Product successfully updated!";
            }
            else return "There was an issue with your request!";
        }

        public string UpdateSalePurchase(string uid, string id, string type, [Optional] string product, [Optional] string quantity, [Optional] string date, [Optional] string location, [Optional] string user)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                if (type == null || type == "") return "State if the transaction is a sale or a purchase!";

                string locationId = "";
                if (location != null && location != "")
                {
                    //Checking the name of the location to find the corresponding location Id
                    BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                    if (BaseReader.Read()) locationId = BaseReader["id"].ToString();
                    else return "Location not found in database!";
                }

                string productId = "";
                if (product != null && product != "")
                {
                    //Checking the name of the product to find the corresponding product Id
                    BaseSelect("products", "`id`", $"WHERE `name` = '{product}'", "");

                    if (BaseReader.Read()) productId = BaseReader["id"].ToString();
                    else return "Product not found in database!";
                }

                string userId = "";
                if (product != null && product != "")
                {
                    //Checking the name of the user to find the corresponding user Id
                    BaseSelect("users", "`id`", $"WHERE `name` = '{user}'", "");

                    if (BaseReader.Read()) userId = BaseReader["id"].ToString();
                    else return "User not found in database!";
                }

                string changes = "";
                string[] inputs = new string[] { productId, quantity, date, locationId, userId};
                for (int i = 0; i < 5; i++)
                {
                    if (inputs[i] != null && inputs[i] != "")
                    {
                        if (changes != "") changes += ",";
                        switch (i)
                        {
                            case 0: { changes += $"`productId`='{inputs[0]}' "; break; }
                            case 1: { changes += $"`quantity`='{inputs[1]}' "; break; }
                            case 2: { changes += $"`date`='{inputs[2]}' "; break; }
                            case 3: { changes += $"`locationId`='{inputs[3]}' "; break; }
                            case 4: { changes += $"`userId`='{inputs[4]}' "; break; }
                        }
                    }
                }

                result = BaseUpdate( type + "s", changes + $",`id`={id}", $"`id`='{id}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Sale/Purchase successfully updated!";
            }
            else return "There was an issue with your request!";
        }

        public string UpdateStock(string uid, string id, [Optional] string product, [Optional] string location, [Optional] string quantity)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                string locationId = "";
                if (location != null && location != "")
                {
                    //Checking the name of the location to find the corresponding location Id
                    BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                    if (BaseReader.Read()) locationId = BaseReader["id"].ToString();
                    else return "Location not found in database!";
                }

                string productId = "";
                if (product != null && product != "")
                {
                    //Checking the name of the product to find the corresponding product Id
                    BaseSelect("products", "`id`", $"WHERE `name` = '{product}'", "");

                    if (BaseReader.Read()) productId = BaseReader["id"].ToString();
                    else return "Product not found in database!";
                }

                string changes = "";
                string[] inputs = new string[] { productId, locationId, quantity };
                for (int i = 0; i < 3; i++)
                {
                    if (inputs[i] != null && inputs[i] != "")
                    {
                        if (changes != "") changes += ",";
                        switch (i)
                        {
                            case 0: { changes += $"`productId`='{inputs[0]}' "; break; }
                            case 1: { changes += $"`locationId`='{inputs[1]}' "; break; }
                            case 2: { changes += $"`quantity`='{inputs[2]}' "; break; }
                        }
                    }
                }

                result = BaseUpdate("stocks", changes, $"`id`='{id}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Stock successfully updated!";
            }
            else return "There was an issue with your request!";
        }
    }
}
