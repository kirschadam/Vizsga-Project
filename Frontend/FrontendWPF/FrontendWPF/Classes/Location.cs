using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontendWPF.Classes
{
    public class Location
    {
        public int Id;
        public string Name;
        public string Region;

        public Location()
        {
        }

        public Location(int id, string name, string region)
        {
            Id = id;
            Name = name;
            Region = region;
        }

        // returns a list of locations
        public static List<LocationService.Store> GetLocations(string id, string location, string region, string limit)
        {
            LocationService.LocationServiceClient client = new LocationService.LocationServiceClient();

            LocationService.Store[] locationsArray;
            List<LocationService.Store> locationsList = new List<LocationService.Store>();
 
            try
            {
                // string query = $"WHERE name='{locationName}' AND unitPrice='{CreateMD5(unitprice)}'";
                locationsArray = client.ListLocation(Shared.uid, id, location, region, limit).Locations;
                // UserService.Response_Location response_Location = new UserService.Response_Location();
                // string uid = response_Location.Uid;

                // if (locationsArray == null)
                if (locationsArray.Length == 0) // { FrontendWPF.UserService.Location[0]} // TODO: ez jön vissza akkor is, ha elérhető az adatbázis, de üres a lekérés! Módosítani kellene, hogy null értékkel térjen vissza, ha nem tud kapcsolódni az adatbázishoz!

                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    // return;
                }
                else
                {
                    locationsList = locationsArray.ToList();
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
            return locationsList;
            
        }
        
    }
}