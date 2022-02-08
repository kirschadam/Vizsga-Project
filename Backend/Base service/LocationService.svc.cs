using Base_service.JsonClasses;
using System;
using System.Data;
using System.Runtime.InteropServices;

namespace Base_service
{
    public class LocationService : DatabaseManager.BaseDatabaseCommands, Interfaces.ILocationService
    {
        public string AddLocation(string uid, string location, string region)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Checking the name of the region to find the corresponding region Id
            var result_read = BaseSelect("regions", "`id`", new string[,] { { "`name`", "=", $"'{region}'" } }, "");

            string regionId = "";
            if (result_read.Item1.Rows.Count != 0) regionId = result_read.Item1.Rows[0]["id"].ToString();
            if (result_read.Item2 != "") return result_read.Item2;


            var result = BaseInsert("locations", "`name`,`regionId`", $"'{location}','{regionId}'");

            if (result.Item1 != null && result.Item1 > 0) return "Location successfully added!";
            else return result.Item2;
        }




        public string AddRegion(string uid, string region)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseInsert("regions", "name", $"'{region}'");

            if (result.Item1 != null && result.Item1 > 0) return "Region successfully added!";
            else return result.Item2;
        }




        public Response_Location ListLocation(string uid, [Optional] string id, [Optional] string location, [Optional] string region, [Optional] string limit)
        {
            if (!Current_users.ContainsKey(uid)) return new Response_Location() { Message = "Unauthorized user!" };

            Response_Location response = new Response_Location();

            string[,] conditions = 
            {
                { "`locations`.`id`", "=", $"'{id}'" },
                { "`locations`.`name`", "=", $"'{location}'" },
                { "`regions`.`name`", "=", $"'{region}'"  },
                { " LIMIT", " ", $"'{limit}'" }
            };

            var result = BaseSelect(
                "locations",
                "`locations`.`id`, `locations`.`name`, `regions`.`name` as 'region'",
                conditions,
                "INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`"
                );

            foreach(DataRow reader in result.Item1.Rows)
            {
                response.Locations.Add(new Store(
                    int.Parse(reader["id"].ToString()),
                    reader["name"].ToString(),
                    reader["region"].ToString()
                    ));
            }

            if (result.Item2 != "") response.Message = result.Item2;
            else response.Message = $"Number of regions found: {response.Locations.Count}";

            return response;
        }




        public Response_Region ListRegion(string uid, [Optional] string id, [Optional] string region, [Optional] string limit)
        {
            if (!Current_users.ContainsKey(uid)) return new Response_Region() { Message = "Unauthorized user!" };

            Response_Region response = new Response_Region();

            string[,] conditions = 
            {
                { "`id`", "=", $"'{id}'" },
                { "`name`", "=", $"'{region}'" },
                { " LIMIT", " ", $"'{limit}'" }
            };

            var result = BaseSelect("regions", "*", conditions, "");

            foreach (DataRow reader in result.Item1.Rows)
            {
                response.Regions.Add(new Region(
                    int.Parse(reader["id"].ToString()),
                    reader["name"].ToString()
                    ));
            }

            if (result.Item2 != "") response.Message = result.Item2;
            else response.Message = $"Number of regions found: {response.Regions.Count}";

            //If we found regions, find all the locations associated with them
            if (response.Regions.Count > 0)
            {
                foreach (var item in response.Regions)
                {
                    result = BaseSelect("locations", "*", new string[,] { { "`regions`.`name`", "=", $"'{item.Name}'" } }, "INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`");

                    foreach (DataRow reader in result.Item1.Rows)
                    {
                        item.Locations.Add(new Store(int.Parse(reader["id"].ToString()), reader["name"].ToString(), item.Name));
                    }
                    if (result.Item2 != "") { response.Message = result.Item2; break; }
                }
            }

            return response;
        }




        public string RemoveLocation(string uid, [Optional] string id, [Optional] string location)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete("locations", $"`id`='{id}'");
            else if (location != null && location != "") result = BaseDelete("locations", $"`name`='{location}'");
            else return "Give either a location name or an id!";

            if (result.Item1 != null && result.Item1 > 0) return "Location successfully removed!"; 
            else return result.Item2;
        }




        public string RemoveRegion(string uid, [Optional] string id, [Optional] string region)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Check for case under which we delete
            Tuple<int?, string> result;
            if (id != null && id != "") result = BaseDelete("regions", $"`id`='{id}'");
            else if (region != null && region != "") result = BaseDelete("regions", $"`name`='{region}'");
            else return "Give either a region name or an id!";

            if (result.Item1 != null && result.Item1 > 0) return "Region successfully removed!";
            else return result.Item2;
        }




        public string UpdateLocation(string uid, string id, [Optional] string location, [Optional] string region)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            //Checking the name of the region to find the corresponding region Id
            string regionId = "";
            if (region != null && region != "")
            {
                var result_read = BaseSelect("regions", "`id`", new string[,] { { "`name`", "=", $"'{region}'"} }, "");

                if (result_read.Item1.Rows.Count != 0) regionId = result_read.Item1.Rows[0]["id"].ToString();
                if (result_read.Item2 != "") return result_read.Item2;
            }

            string[,] changes = 
            {
                { "`name`", $"'{location}'" },
                { "`regionId`", $"'{regionId}'" }
            };

            var result = BaseUpdate("locations", changes, $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0) return "Location successfully updated!";
            else return result.Item2;
        }




        public string UpdateRegion(string uid, string id, string region)
        {
            if (!Current_users.ContainsKey(uid)) return "Unauthorized user!";

            var result = BaseUpdate("regions", new string[,]{ { "`name`", $"'{region}'" } }, $"`id`='{id}'");

            if (result.Item1 != null && result.Item1 > 0)
            {
                return "Region successfully updated!";
            }
            else return result.Item2;
        }
    }
}
