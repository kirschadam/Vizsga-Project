using Base_service.DatabaseManagers;
using System;
using System.Data;
using System.Runtime.InteropServices;

namespace Base_service
{
    public class UserManagement : BaseDatabaseCommands, IUserManagement
    {
        const string join_location = "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id`";
        const string join_region = " INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`";
        
        public Response_User ListUser([Optional]string id, [Optional] string username, [Optional] string location, [Optional] string region, [Optional] string limit)
        {
            Response_User response = new Response_User();
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

                BaseSelect(new string[] { 
                    "users", 
                    "`users`.`id`, `username`, `password`, `locations`.`name` AS 'location', `permission`, `active`", 
                    conditions, 
                    join_location + (region == null ? "" : join_region) 
                });

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



        public Response_User LoginUser(string username, string password)
        {
            Response_User response = new Response_User();
            
            try
            {
                BaseSelect(new string[] { 
                    "users", 
                    "`users`.`id`, `username`, `password`, `locations`.`name` AS 'location', `permission`, `active`", 
                    $"WHERE `username` = '{username}' AND `password` = '{password}'", 
                    join_location });

                if (BaseReader.Read())
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



        public string RegisterUser(string username, string password, string location, string permission)
        {
            int? result = null;

            try
            {
                string locationId = "";
                BaseSelect(new string[] { "locations", "`id`", $"WHERE `name` = '{location}'" });
                if(BaseReader.Read()) locationId = BaseReader["id"].ToString();

                Console.WriteLine(locationId);

                string values = "";
                foreach (var item in new string[] { username, password, locationId, permission})
                {
                    if (item == null || item == "") return "Please give every needed detail of the user being registered!";
                    values += $"'{item}',";
                }

                result = BaseInsert(new string[] { "users", "`username`, `password`, `locationId`, `permission`, `active`", values + "'1'" });
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



        public string UpdateUser(string id, [Optional] string username, [Optional] string password, [Optional] string locationId, [Optional] string permission, [Optional] string active)
        {
            int? result = null;

            try
            {
                string changes = "";
                string[] inputs = new string[] { username, password, locationId, permission, active };
                for (int i = 0; i < 5; i++)
                {
                    if (inputs[i] != null && inputs[i] != "")
                    {
                        if (changes != "") changes += ",";
                        switch (i)
                        {
                            case 0: { changes += $"`username`='{inputs[i]}' "; break; }
                            case 1: { changes += $"`password`='{inputs[i]}' "; break; }
                            case 2: { changes += $"`locationId`='{inputs[i]}' "; break; }
                            case 3: { changes += $"`permission`.`name`='{inputs[i]}' "; break; }
                            case 4: { changes += $"`active`='{inputs[i]}'"; break; }
                        }
                    }
                }

                result = BaseUpdate(new string[] { "users", changes, $"`id`='{id}'" });
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



        public string DeleteUser(string id)
        {
            int? result = null;

            try { result = BaseDelete(new string[] { "users", $"`id`='{id}'" }); }

            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == System.Data.ConnectionState.Open)
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
