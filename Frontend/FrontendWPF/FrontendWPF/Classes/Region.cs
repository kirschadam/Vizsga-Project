using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontendWPF.Classes
{
    public class Region
    {
        public int Id;
        public string Name;

        public Region()
        {
        }

        public Region(int id, string name)
        {
            Id = id;
            Name = name;
        }

        // returns a list of regions
        public static List<LocationService.Region> GetRegions(string id, string location, string region, string limit)
        {
            LocationService.LocationServiceClient client = new LocationService.LocationServiceClient();

            LocationService.Region[] regionsArray;
            List<LocationService.Region> regionsList = new List<LocationService.Region>();
 
            try
            {
                // string query = $"WHERE name='{regionsName}' AND unitPrice='{CreateMD5(unitprice)}'";
                regionsArray = client.ListRegion(Shared.uid, id, region, limit).Regions;
                // UserService.Response_Location response_Location = new UserService.Response_Location();
                // string uid = response_Location.Uid;

                // if (regionsArray == null)
                if (regionsArray.Length == 0) // { FrontendWPF.UserService.Location[0]} // TODO: ez jön vissza akkor is, ha elérhető az adatbázis, de üres a lekérés! Módosítani kellene, hogy null értékkel térjen vissza, ha nem tud kapcsolódni az adatbázishoz!

                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    // return;
                }
                else
                {
                    regionsList = regionsArray.ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server") || ex.ToString().Contains("EndpointNotFoundException"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    // return;
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message");
                    //  return;
                }
            }
            // return location.ToList();
            return regionsList;
            
        }
        
    }
}