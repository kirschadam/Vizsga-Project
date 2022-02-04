using Base_service.DatabaseManagers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Linq;
using Base_service.JsonClasses;

namespace Base_service
{
    public class UserService : BaseDatabaseCommands, Interfaces.IUserService
    {
        public static Dictionary<string, User> current_users = new Dictionary<string, User>();

        const string join_location = "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id`";
        const string join_region = " INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`";
        
        public Response_User ListUser(string uid, [Optional]string id, [Optional] string username, [Optional] string location, [Optional] string region, [Optional] string limit)
        {
            Response_User response = new Response_User();

            if (!current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            try
            {
                string conditions = ""; 
                string[] inputs = new string[] { id, username, location, region, limit };
                for (int i = 0; i < 5; i++)
                {
                    //If the given input holds information, make a condition about it
                    if (inputs[i] != null && inputs[i].Length != 0)
                    {
                        switch (i)
                        {
                            case 0: { conditions += $"`users`.`id`='{inputs[i]}'"; break; }
                            case 1: { conditions += $"`username`='{inputs[i]}'"; break; }
                            case 2: { conditions += $"`locations`.`name`='{inputs[i]}'"; break; }
                            case 3: { conditions += $"`regions`.`name`='{inputs[i]}'"; break; }
                            case 4: { conditions += $" LIMIT {inputs[i]}"; break; }
                        }
                    }
                }

                //If there are conditions, put the WHERE keyword to the beginning
                //put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "' "
                if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

                BaseSelect(
                    "users", 
                    "`users`.`id`, `username`, `password`, `locations`.`name` AS 'location', `permission`, `active`", 
                    conditions, 
                    join_location + (region == null ? "" : join_region) 
                );

                if (BaseReader == null) return null;

                while (BaseReader.Read())
                {
                    response.Users.Add(new User(
                        int.Parse(BaseReader["id"].ToString()),
                        BaseReader["username"].ToString(),
                        BaseReader["password"].ToString(),
                        BaseReader["location"].ToString(),
                        int.Parse(BaseReader["permission"].ToString()),
                        int.Parse(BaseReader["active"].ToString())
                        ));
                }
                response.Message = $"Number of users found: {response.Users.Count}";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); response.Message = "There was a problem with your request!"; }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }
            return response;
        }



        public Response_Login LoginUser(string username, string password)
        {
            Response_Login response = new Response_Login();
            
            try
            {
                BaseSelect(
                    "users", 
                    "`users`.`id`, `username`, `password`, `locations`.`name` AS 'location', `permission`, `active`", 
                    $"WHERE `username` = '{username}' AND `password` = '{password}'", 
                    join_location );

                if (BaseReader.Read())
                {
                    response.User =new User(
                        int.Parse(BaseReader["id"].ToString()),
                        BaseReader["username"].ToString(),
                        BaseReader["password"].ToString(),
                        BaseReader["location"].ToString(),
                        int.Parse(BaseReader["permission"].ToString()),
                        int.Parse(BaseReader["active"].ToString())
                       );

                    //Generating unique id for user, removing previous instance if it wasn't before
                    current_users = current_users.TakeWhile(n => n.Value.Username != response.User.Username).ToDictionary(x => x.Key, y => y.Value);

                    response.Uid = Guid.NewGuid().ToString(); 
                    current_users.Add(response.Uid, response.User);

                    Console.WriteLine(response.Uid + "\t" + response.User.Username);
                    response.Message = $"Welcome {response.User.Username}!";
                }
                else response.Message = "Username or password incorrect!";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); response.Message = "There was a problem with your request!"; }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            return response;
        }

        public string LogoutUser(string uid)
        {
            if (current_users.Remove(uid))
            {
                return "You have logged out!";
            }
            else return "You are not logged in!";
        }



        public string RegisterUser(string uid, string username, string password, string location, string permission)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                //Checking the name of the location to find the corresponding location Id
                BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                string locationId = "";
                if (BaseReader.Read()) locationId = BaseReader["id"].ToString();
                else return "Location not found in database!";


                string values = "";
                foreach (var item in new string[] { username, password, locationId, permission})
                {
                    if (item == null || item == "") return "Please give every needed detail of the user being registered!";
                    values += $"'{item}',";
                }

                result = BaseInsert("users", "`username`, `password`, `locationId`, `permission`, `active`", values + "'1'");
            }

            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "User successfully registered!";
            }
            else return "There was an issue with your registration!";
        }



        public string UpdateUser(string uid, string id, [Optional] string username, [Optional] string password, [Optional] string location, [Optional] string permission, [Optional] string active)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                string locationId = "";

                //Checking the name of the location to find the corresponding location Id
                if (location != null || location != "")
                {
                    BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                    if (BaseReader.Read()) locationId = BaseReader["id"].ToString();
                    else return "Location not found in database!";
                }

                string changes = "";
                string[] inputs = new string[] { username, password, locationId, permission, active };
                for (int i = 0; i < 5; i++)
                {
                    if (inputs[i] != null && inputs[i] != "")
                    {
                        if (changes != "") changes += ",";
                        switch (i)
                        {
                            case 0: { changes += $"`username`='{inputs[0]}' "; break; }
                            case 1: { changes += $"`password`='{inputs[1]}' "; break; }
                            case 2: { changes += $"`locationId`='{inputs[2]}' "; break; }
                            case 3: { changes += $"`permission`='{inputs[3]}' "; break; }
                            case 4: { changes += $"`active`='{inputs[4]}'"; break; }
                        }
                    }
                }

                result = BaseUpdate("users", changes, $"`id`='{id}'");
            }

            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "User successfully updated!";
            }
            else return "There was an issue with updating the user!";
        }



        public string DeleteUser(string uid, [Optional] string id, [Optional] string username)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try 
            {
                if (id != null && id != "")
                {
                    result = BaseDelete("users", $"`id`='{id}'");
                }
                else if (username != null && username != "") result = BaseDelete("users", $"`username`='{username}'");
                else return "Give either a username or an id!";
            }

            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "User successfully deleted!";
            }
            else return "There was an issue with deleting the user!";
        }
    }
}
