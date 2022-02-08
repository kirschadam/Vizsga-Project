using System;
using System.Runtime.InteropServices;
using System.Linq;
using Base_service.JsonClasses;
using System.Data;

namespace Base_service
{
    public class UserService : DatabaseManager.BaseDatabaseCommands, Interfaces.IUserService
    {
        public Response_User ListUser(string uid, [Optional] string id, [Optional] string username, [Optional] string location, [Optional] string region, [Optional] string limit)
        {
            Response_User response = new Response_User();

            try
            {
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
                    { " LIMIT", " ", $"'{limit}'" }
                };

                var result = BaseSelect(
                    "users",
                    "`users`.`id`,`username`,`password`,`locations`.`name` AS 'location',`permission`,`active`",
                    conditions,
                    "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id` INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`"
                    );

                foreach (DataRow reader in result.Item1.Rows)
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

                if (result.Item2 != "") response.Message = result.Item2;
                response.Message = $"Number of users found: {response.Users.Count}";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return response;
        }




        public Response_Login LoginUser(string username, string password)
        {
            Response_Login response = new Response_Login();

            var result = BaseSelect(
                    "users",
                    "`users`.`id`,`username`,`password`,`locations`.`name` AS 'location',`permission`,`active`",
                    new string[,] { { "`username`", "=", $"'{username}'" }, { "`password`", "=", $"'{password}'" } },
                    "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id`"
                    );

            if (result.Item1.Rows.Count != 1) response.Message = "Username or password incorrect!";
            else 
            {
                DataRow reader = result.Item1.Rows[0];
                response.User = new User(
                    int.Parse(reader["id"].ToString()),
                    reader["username"].ToString(),
                    reader["password"].ToString(),
                    reader["location"].ToString(),
                    int.Parse(reader["permission"].ToString()),
                    int.Parse(reader["active"].ToString())
                   );

                //Generating unique id for user, removing previous instance if it wasn't before
                if(Current_users.Where(n => n.Value.Username == response.User.Username).Count() != 0)
                {
                    Current_users.Remove(Current_users.Where(n => n.Value.Username == response.User.Username).FirstOrDefault().Key);
                }
                
                response.Uid = Guid.NewGuid().ToString();
                Current_users.Add(response.Uid, response.User);

                Console.WriteLine(response.Uid + "\t" + response.User.Username);
                response.Message = $"Welcome {response.User.Username}!";
            }

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
            if (result.Item2 != null && result.Item1 > 0) return "User successfully registered!";
            else return result.Item2;
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
            if (result.Item1 != null && result.Item1 > 0) return "User successfully updated!";
            else return result.Item2;
        }




        public string DeleteUser(string uid, [Optional] string id, [Optional] string username)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Check for case under which we delete
            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete("users", $"`id`='{id}'"); 
            else if (username != null && username != "") result = BaseDelete("users", $"`username`='{username}'");
            else return "Give either a username or an id!";


            if (result.Item1 != null && result.Item1 > 0) return "User successfully deleted!";
            else return result.Item2;
        }
    }
}
