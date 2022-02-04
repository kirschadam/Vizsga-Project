using Base_service.DatabaseManagers;
using Base_service.JsonClasses;
using System;
using System.Data;
using System.Runtime.InteropServices;

namespace Base_service
{
    public class LocationService : BaseDatabaseCommands, Interfaces.ILocationService
    {
        const string join_region = " INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`";

        public string AddLocation(string uid, string location, string region)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try 
            {
                //Checking the name of the region to find the corresponding region Id
                BaseSelect("regions", "`id`", $"WHERE `name` = '{region}'", "");

                string regionId = "";
                if (BaseReader.Read()) regionId = BaseReader["id"].ToString();
                else return "Region not found in database!";

                result = BaseInsert("locations", "`name`,`regionId`", $"'{location}','{regionId}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Location successfully added!";
            }
            else return "There was an issue with your request!";
        }




        public string AddRegion(string uid, string region)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                if(region == null || region == "") return "Give the name of the region!";
                result = BaseInsert("regions", "name", $"'{region}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Region successfully added!";
            }
            else return "There was an issue with your request!";
        }




        public Response_Location ListLocation(string uid, [Optional] string id, [Optional] string location, [Optional] string region, [Optional] string limit)
        {
            if (!UserService.current_users.ContainsKey(uid)) return new Response_Location() { Message = "Unauthorized user!" };

            Response_Location response = new Response_Location();

            try
            {
                string conditions = "";
                string[] inputs = new string[] { id, location, region, limit };
                for (int i = 0; i < 4; i++)
                {
                    //If the given input holds information, make a condition about it
                    if (inputs[i] != null && inputs[i].Length != 0)
                    {
                        switch (i)
                        {
                            case 0: { conditions += $"`locations`.`id`='{inputs[i]}'"; break; }
                            case 1: { conditions += $"`locations`.`name`='{inputs[i]}'"; break; }
                            case 2: { conditions += $" `regions`.`name`='{inputs[i]}'"; break; }
                            case 3: { conditions += $" LIMIT {inputs[i]}"; break; }
                        }
                    }
                }

                //If there are conditions, put the WHERE keyword to the beginning
                //put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "' "
                if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

                BaseSelect(
                    "locations", 
                    "`locations`.`id`, `locations`.`name`, `regions`.`name` as 'region'", 
                    conditions, 
                    join_region
                    );

                if (BaseReader == null) return null;

                while (BaseReader.Read())
                {
                    response.Locations.Add(new Store(
                        int.Parse(BaseReader["id"].ToString()),
                        BaseReader["name"].ToString(),
                        BaseReader["region"].ToString()
                        ));
                }
                response.Message = $"Number of regions found: {response.Locations.Count}";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); response.Message = "There was a problem with your request!"; }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            return response;
        }




        public Response_Region ListRegion(string uid, [Optional] string id, [Optional] string region, [Optional] string limit)
        {
            if (!UserService.current_users.ContainsKey(uid)) return new Response_Region() { Message = "Unauthorized user!" };

            Response_Region response = new Response_Region();

            try
            {
                string conditions = "";
                string[] inputs = new string[] { id, region, limit };
                for (int i = 0; i < 3; i++)
                {
                    //If the given input holds information, make a condition about it
                    if (inputs[i] != null && inputs[i].Length != 0)
                    {
                        switch (i)
                        {
                            case 0: { conditions += $"`id`='{inputs[i]}'"; break; }
                            case 1: { conditions += $"`name`='{inputs[i]}'"; break; }
                            case 2: { conditions += $" LIMIT {inputs[i]}"; break; }
                        }
                    }
                }

                //If there are conditions, put the WHERE keyword to the beginning
                //put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "' "
                if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

                BaseSelect("regions", "*", conditions, "");

                if (BaseReader == null) return null;

                while (BaseReader.Read())
                {
                    response.Regions.Add(new Region(
                        int.Parse(BaseReader["id"].ToString()),
                        BaseReader["name"].ToString()
                        ));
                }
                response.Message = $"Number of regions found: {response.Regions.Count}";

                //If we found regions, find all the locations associated with them
                if(response.Regions.Count > 0)
                {
                    foreach (var item in response.Regions)
                    {
                        BaseSelect("locations", "*", $"WHERE `regions`.`name` = '{item.Name}'", join_region);
                        while (BaseReader.Read())
                        {
                            item.Locations.Add(new Store(int.Parse(BaseReader["id"].ToString()), BaseReader["name"].ToString(), item.Name));
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); response.Message = "There was a problem with your request!"; }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            return response;
        }




        public string RemoveLocation(string uid, [Optional] string id, [Optional] string location)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                if (id != null && id != "")
                {
                    result = BaseDelete("locations", $"`id`='{id}'");
                }
                else if (location != null && location != "") result = BaseDelete("locations", $"`name`='{location}'");
                else return "Give either a location name or an id!";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Location successfully removed!";
            }
            else return "There was an issue with your request!";
        }




        public string RemoveRegion(string uid, [Optional] string id, [Optional] string region)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                if (id != null && id != "")
                {
                    result = BaseDelete("regions", $"`id`='{id}'");
                }
                else if (region != null && region != "") result = BaseDelete("regions", $"`name`='{region}'");
                else return "Give either a region name or an id!";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Region successfully removed!";
            }
            else return "There was an issue with your request!";
        }




        public string UpdateLocation(string uid, string id, [Optional] string location, [Optional] string region)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                string regionId = "";
                if (region != null && region != "")
                {
                    //Checking the name of the region to find the corresponding region Id
                    BaseSelect("regions", "`id`", $"WHERE `name` = '{region}'", "");

                    if (BaseReader.Read()) regionId = BaseReader["id"].ToString();
                    else return "Region not found in database!";
                }

                string changes = "";
                string[] inputs = new string[] { location, regionId};
                for (int i = 0; i < 2; i++)
                {
                    if (inputs[i] != null && inputs[i] != "")
                    {
                        if (changes != "") changes += ",";
                        switch (i)
                        {
                            case 0: { changes += $"`name`='{inputs[0]}' "; break; }
                            case 1: { changes += $"`regionId`='{inputs[1]}' "; break; }
                        }
                    }
                }

                result = BaseUpdate("locations", changes, $"`id`='{id}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Location successfully updated!";
            }
            else return "There was an issue with your request!";
        }




        public string UpdateRegion(string uid, string id, string region)
        {
            if (!UserService.current_users.ContainsKey(uid)) return "Unauthorized user!";

            int? result = null;

            try
            {
                result = BaseUpdate("regions", $"`name` = '{region}'", $"`id`='{id}'");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                if (BaseCommand.Connection.State == ConnectionState.Open)
                    BaseCommand.Connection.Close();
            }

            if (result != null && result > 0)
            {
                return "Region successfully updated!";
            }
            else return "There was an issue with your request!";
        }
    }
}
