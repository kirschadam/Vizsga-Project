using System;
using System.Runtime.InteropServices;
using System.Linq;
using Base_service.JsonClasses;

namespace Base_service
{
    public class UserService : DatabaseManager.BaseDatabaseCommands, Interfaces.IUserService
    {
        public Response_User ListUser(string uid, [Optional] string id, [Optional] string username, [Optional] string location, [Optional] string region, [Optional] string limit)
        {
            Response_User response = new Response_User();

            if (!current_users.ContainsKey(uid))
            {
                response.Message = "Unauthorized user!";
                return response;
            }

            string conditions = "";
            string[] inputs = new string[] { id, username, location, region, limit };
            for (int i = 0; i < inputs.Length; i++)
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

            var result = BaseSelect(
                "users",
                "`users`.`id`, `username`, `password`, `locations`.`name` AS 'location', `permission`, `active`",
                conditions,
                "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id` INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`"
                );

            var reader = result.Item1;
            while (reader.Read())
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

            if (result.Item3.Connection.State == System.Data.ConnectionState.Open) result.Item3.Connection.Close();

            return response;
        }



        public Response_Login LoginUser(string username, string password)
        {
            Response_Login response = new Response_Login();

            var result = BaseSelect(
                    "users",
                    "`users`.`id`, `username`, `password`, `locations`.`name` AS 'location', `permission`, `active`",
                    $"WHERE `username` = '{username}' AND `password` = '{password}'",
                    "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id`"
                    );

            var reader = result.Item1;
            if (reader.Read())
            {
                response.User = new User(
                    int.Parse(reader["id"].ToString()),
                    reader["username"].ToString(),
                    reader["password"].ToString(),
                    reader["location"].ToString(),
                    int.Parse(reader["permission"].ToString()),
                    int.Parse(reader["active"].ToString())
                   );

                //Generating unique id for user, removing previous instance if it wasn't before
                current_users = current_users.TakeWhile(n => n.Value.Username != response.User.Username).ToDictionary(x => x.Key, y => y.Value);

                response.Uid = Guid.NewGuid().ToString();
                current_users.Add(response.Uid, response.User);

                Console.WriteLine(response.Uid + "\t" + response.User.Username);
                response.Message = $"Welcome {response.User.Username}!";
            }
            else response.Message = "Username or password incorrect!";

            if (result.Item3.Connection.State == System.Data.ConnectionState.Open) result.Item3.Connection.Close();

            return response;
        }

        public string LogoutUser(string uid)
        {
            if (current_users.Remove(uid)) return "You have logged out!";
            else return "You are not logged in!";
        }



        public string RegisterUser(string uid, string username, string password, string location, string permission)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Checking the name of the location to find the corresponding location Id
            var result_read = BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

            string locationId = "";
            if (result_read.Item1.Read()) locationId = result_read.Item1["id"].ToString();
            if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
            if (result_read.Item2 != "") return result_read.Item2;


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
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            string locationId = "";

            //Checking the name of the location to find the corresponding location Id
            if (location != null || location != "")
            {
                var result_read = BaseSelect("locations", "`id`", $"WHERE `name` = '{location}'", "");

                if (result_read.Item1.Read()) locationId = result_read.Item1["id"].ToString();
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return "Location not found in database!";
            }

            string changes = "";
            string[] inputs = new string[] { username, password, locationId, permission, active };
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] != null && inputs[i] != "")
                {
                    if (changes != "") changes += ",";
                    switch (i)
                    {
                        case 0: { changes += $"`username`='{inputs[i]}' "; break; }
                        case 1: { changes += $"`password`='{inputs[i]}' "; break; }
                        case 2: { changes += $"`locationId`='{inputs[i]}' "; break; }
                        case 3: { changes += $"`permission`='{inputs[i]}' "; break; }
                        case 4: { changes += $"`active`='{inputs[i]}'"; break; }
                    }
                }
            }

            var result = BaseUpdate("users", changes, $"`id`='{id}'");
            if (result.Item1 != null && result.Item1 > 0) return "User successfully updated!";
            else return result.Item2;
        }



        public string DeleteUser(string uid, [Optional] string id, [Optional] string username)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete("users", $"`id`='{id}'"); 
            else if (username != null && username != "") result = BaseDelete("users", $"`username`='{username}'");
            else return "Give either a username or an id!";


            if (result.Item1 != null && result.Item1 > 0) return "User successfully deleted!";
            else return result.Item2;
        }
    }
}
