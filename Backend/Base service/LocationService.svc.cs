using Base_service.JsonClasses;
using System;
using System.Runtime.InteropServices;

namespace Base_service
{
    public class LocationService : DatabaseManager.BaseDatabaseCommands, Interfaces.ILocationService
    {
        public string AddLocation(string uid, string location, string region)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Checking the name of the region to find the corresponding region Id
            var result_read = BaseSelect("regions", "`id`", $"WHERE `name` = '{region}'", "");

            string regionId = "";
            if (result_read.Item1.Read()) regionId = result_read.Item1["id"].ToString(); 
            if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
            if (result_read.Item2 != "") return result_read.Item2;

            var result = BaseInsert("locations", "`name`,`regionId`", $"'{location}','{regionId}'");

            if (result.Item1 != null && result.Item1 > 0) return "Location successfully added!";
            else return result.Item2;
        }




        public string AddRegion(string uid, string region)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseInsert("regions", "name", $"'{region}'");

            if (result.Item1 != null && result.Item1 > 0) return "Region successfully added!";
            else return result.Item2;
        }




        public Response_Location ListLocation(string uid, [Optional] string id, [Optional] string location, [Optional] string region, [Optional] string limit)
        {
            if (!current_users.ContainsKey(uid)) return new Response_Location() { Message = "Unauthorized user!" };

            Response_Location response = new Response_Location();

            string conditions = "";
            string[] inputs = new string[] { id, location, region, limit };
            for (int i = 0; i < inputs.Length; i++)
            {
                //If the given input holds information, make a condition about it
                if (inputs[i] != null && inputs[i].Length != 0)
                {
                    switch (i)
                    {
                        case 0: { conditions += $"`locations`.`id`='{inputs[i]}'"; break; }
                        case 1: { conditions += $"`locations`.`name`='{inputs[i]}'"; break; }
                        case 2: { conditions += $"`regions`.`name`='{inputs[i]}'"; break; }
                        case 3: { conditions += $" LIMIT {inputs[i]}"; break; }
                    }
                }
            }

            //If there are conditions, put the WHERE keyword to the beginning
            //put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "'"
            if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

            var result = BaseSelect(
                "locations",
                "`locations`.`id`, `locations`.`name`, `regions`.`name` as 'region'",
                conditions,
                "INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`"
                );

            var reader = result.Item1;
            while (reader.Read())
            {
                response.Locations.Add(new Store(
                    int.Parse(reader["id"].ToString()),
                    reader["name"].ToString(),
                    reader["region"].ToString()
                    ));
            }

            if (result.Item3.Connection.State == System.Data.ConnectionState.Open) result.Item3.Connection.Close();

            if (result.Item2 != "") response.Message = result.Item2;
            else response.Message = $"Number of regions found: {response.Locations.Count}";

            return response;
        }




        public Response_Region ListRegion(string uid, [Optional] string id, [Optional] string region, [Optional] string limit)
        {
            if (!current_users.ContainsKey(uid)) return new Response_Region() { Message = "Unauthorized user!" };

            Response_Region response = new Response_Region();

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
            //put and AND keyword between every condition, if there are multiple, every condition starts with "`" and ends with "'"
            if (conditions != "") conditions = conditions.Insert(0, "WHERE ").Replace("'`", "' AND `");

            var result = BaseSelect("regions", "*", conditions, "");
            var reader = result.Item1;

            while (reader.Read())
            {
                response.Regions.Add(new Region(
                    int.Parse(reader["id"].ToString()),
                    reader["name"].ToString()
                    ));
            }

            if (result.Item3.Connection.State == System.Data.ConnectionState.Open) result.Item3.Connection.Close();

            if (result.Item2 != "") response.Message = result.Item2;
            else response.Message = $"Number of regions found: {response.Regions.Count}";

            //If we found regions, find all the locations associated with them
            if (response.Regions.Count > 0)
            {
                foreach (var item in response.Regions)
                {
                    result = BaseSelect("locations", "*", $"WHERE `regions`.`name` = '{item.Name}'", "INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`");
                    reader = result.Item1;

                    while (reader.Read())
                    {
                        item.Locations.Add(new Store(int.Parse(reader["id"].ToString()), reader["name"].ToString(), item.Name));
                    }
                    if (result.Item2 != "") { response.Message = result.Item2; break; }
                }
            }

            if (result.Item3.Connection.State == System.Data.ConnectionState.Open) result.Item3.Connection.Close();

            return response;
        }




        public string RemoveLocation(string uid, [Optional] string id, [Optional] string location)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete("locations", $"`id`='{id}'");
            else if (location != null && location != "") result = BaseDelete("locations", $"`name`='{location}'");
            else return "Give either a location name or an id!";

            if (result.Item1 != null && result.Item1 > 0) return "Location successfully removed!"; 
            else return result.Item2;
        }




        public string RemoveRegion(string uid, [Optional] string id, [Optional] string region)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete("regions", $"`id`='{id}'");
            else if (region != null && region != "") result = BaseDelete("regions", $"`name`='{region}'");
            else return "Give either a region name or an id!";

            if (result.Item1 != null && result.Item1 > 0) return "Region successfully removed!";
            else return result.Item2;
        }




        public string UpdateLocation(string uid, string id, [Optional] string location, [Optional] string region)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            string regionId = "";
            if (region != null && region != "")
            {
                //Checking the name of the region to find the corresponding region Id
                var result_read = BaseSelect("regions", "`id`", $"WHERE `name` = '{region}'", "");

                if (result_read.Item1.Read()) regionId = result_read.Item1["id"].ToString();
                if (result_read.Item3.Connection.State == System.Data.ConnectionState.Open) result_read.Item3.Connection.Close();
                if (result_read.Item2 != "") return "Region not found in database!";
            }

            string changes = "";
            string[] inputs = new string[] { location, regionId };
            for (int i = 0; i < inputs.Length; i++)
            {
                //If the given input holds information, make a condition about it
                if (inputs[i] != null && inputs[i] != "")
                {
                    if (changes != "") changes += ",";
                    switch (i)
                    {
                        case 0: { changes += $"`name`='{inputs[i]}' "; break; }
                        case 1: { changes += $"`regionId`='{inputs[i]}' "; break; }
                    }
                }
            }

            var result = BaseUpdate("locations", changes, $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Location successfully updated!";
            else return result.Item2;
        }




        public string UpdateRegion(string uid, string id, string region)
        {
            if (!current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseUpdate("regions", $"`name` = '{region}'", $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0)
            {
                return "Region successfully updated!";
            }
            else return result.Item2;
        }
    }
}
