using System;
using System.Runtime.InteropServices;
using System.Linq;
using Base_service.JsonClasses;
using System.Data;

namespace Base_service
{
    /// <summary>
    /// Class for the various database queries related to user management, and handling logins and logouts of users.
    /// </summary>
    public class UserService : DatabaseManager.BaseDatabaseCommands, Interfaces.IUserService
    {
        public Response_User ListUser(string uid, [Optional] string id, [Optional] string username, [Optional] string location, [Optional] string permissionover, [Optional] string permissionunder, [Optional] string active, [Optional] string region, [Optional] string limit)
        {
            Response_User response = new Response_User();

            if (!Current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            string[,] conditions = new string[,]
            {
                    {"`users`.`id`", "=" , $"'{id}'" },
                    { "`username`", "=", $"'{username}'" },
                    { "`locations`.`name`", "=", $"'{location}'" },
                    { "`regions`.`name`", "=", $"'{region}'" },
                    { "`permission`", ">", $"'{permissionover}'" },
                    { "`permission`", "<", $"'{permissionunder}'" },
                    { "`active`", "=", $"'{active}'" },
                    { " LIMIT", " ", $"{limit}" }
            };

            var result = BaseSelect(
                "users",
                "`users`.`id`,`username`,`password`,`locations`.`name` AS 'location',`permission`,`active`",
                conditions,
                "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id` INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`"
                );

            foreach (DataRow reader in result.Item1.Rows)
            {
                try
                {
                    response.Users.Add(new User(
                        int.Parse(reader["id"].ToString()),
                        reader["username"].ToString(),
                        reader["password"].ToString(),
                        reader["location"].ToString(),
                        int.Parse(reader["permission"].ToString()),
                        int.Parse(reader["active"].ToString())
                        ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    response.Message += $"Result with the id '{reader["id"]}' could not be converted correctly!";
                }
            }

            if (result.Item2 != "") response.Message += result.Item2;
            response.Message += $"Number of users found: {response.Users.Count}";

            return response;
        }




        public Response_Login LoginUser(string username, string password)
        {
            Response_Login response = new Response_Login();

            foreach (var item in Current_users)
            {
                if(item.Value.Username == username && item.Value.Password == password)
                {
                    //Let in user if they logged in less than 8 hours ago,
                    //do not refresh the LoggedInAt variable, user will get a new uid if they try logging in again and it's later than 4 hours
                    if(item.Value.LoggedInAt > DateTime.Now.AddHours(-8))
                    {
                        response.Uid = item.Key;
                        response.User = new User(item.Value.Id, item.Value.Username, item.Value.Password, item.Value.Location, item.Value.Permission, item.Value.Active);
                        response.Message = $"Welcome {item.Value.Username}!";
                        return response;
                    }
                    //Clear currently logging user if they logged in the Current_users dictionary more than 8 hours ago
                    else
                    {
                        Current_users.Remove(item.Key);
                        break;
                    }
                }
            }

            var result = BaseSelect(
                    "users",
                    "`users`.`id`,`username`,`password`,`locations`.`name` AS 'location',`permission`,`active`",
                    new string[,] { { "`username`", "=", $"'{username}'" }, { "`password`", "=", $"'{password}'" } },
                    "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id`"
                    );

            if (result.Item1.Rows.Count == 1)
            {
                DataRow reader = result.Item1.Rows[0];
                try
                {
                    response.User = new User(
                        int.Parse(reader["id"].ToString()),
                        reader["username"].ToString(),
                        reader["password"].ToString(),
                        reader["location"].ToString(),
                        int.Parse(reader["permission"].ToString()),
                        int.Parse(reader["active"].ToString())
                       );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    response.Message += $"User could not be converted correctly!";
                    return response;
                }
                
                response.Uid = Guid.NewGuid().ToString();

                Current_users.Add(response.Uid, new CurrentUser(response.User.Id, response.User.Username, response.User.Password,
                    response.User.Location, response.User.Permission, response.User.Active) { LoggedInAt = DateTime.UtcNow, Uid = response.Uid });

                Console.WriteLine(response.Uid + "\t" + response.User.Username);
                response.Message = $"Welcome {response.User.Username}!";
            }
            else if (result.Item2 != "") response.Message = result.Item2;
            else response.Message = "Username or password incorrect!";

            return response;
        }





        public string LogoutUser(string uid)
        {
            if (Current_users.Remove(uid)) return "You have logged out!";
            else return "You are not logged in!";
        }




        public string RegisterUser(string uid, string username, string password, string location, string permission)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Checking the name of the location to find the corresponding location Id
            var result_read = BaseSelect("locations", "`id`", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

            string locationId;
            if (result_read.Item1.Rows.Count != 0) locationId = result_read.Item1.Rows[0]["id"].ToString();
            else if (result_read.Item2 != "") return result_read.Item2;
            else return "Location not found in database!";

            //Piece together and check for empty values
            string values = "";
            foreach (var item in new string[] { username, password, locationId, permission })
            {
                if (item == null || item == "") return "Please give every needed detail of the user being registered!";
                values += $"'{item}',";
            }

            var result = BaseInsert("users", "`username`, `password`, `locationId`, `permission`, `active`", values + "'1'");

            if (result.Item2 != "") return result.Item2;
            else return "User successfully registered!";
        }




        public string UpdateUser(string uid, string id, [Optional] string username, [Optional] string password, [Optional] string location, [Optional] string permission, [Optional] string active)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            string locationId = "";

            //Checking the name of the location to find the corresponding location Id
            if (location != null || location != "")
            {
                var result_read = BaseSelect("locations", "`id`", new string[,] { { "`name`", "=", $"'{location}'" } }, "");

                if (result_read.Item1.Rows.Count != 0) locationId = result_read.Item1.Rows[0]["id"].ToString();
                else if (result_read.Item2 != "") return result_read.Item2; 
                else return "Location not found in database!";
            }

            string[,] changes = 
            {
                { "`username`", $"'{username}'" },
                { "`password`", $"'{password}'" },
                { "`locationId`", $"'{locationId}'" },
                { "`permission`", $"'{permission}'" },
                { "`active`", $"'{active}'" }
            };

            var result = BaseUpdate("users", changes, $"`id`='{id}'");

            if (result.Item2 != "") return result.Item2;
            else return "User successfully updated!";
        }




        public string DeleteUser(string uid, [Optional] string id, [Optional] string username)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Check for case under which we delete
            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete("users", $"`id`='{id}'"); 
            else if (username != null && username != "") result = BaseDelete("users", $"`username`='{username}'");
            else return "Give either a username or an id!";

            if (result.Item2 != "") return result.Item2;
            else return "User successfully deleted!";
        }
    }
}
