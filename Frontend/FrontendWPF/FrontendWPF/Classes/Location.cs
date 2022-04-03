using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FrontendWPF.Classes
{
    public class Location
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }

        public Location()
        {
        }

        public Location(string id, string name, string region)
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
                string hostMessage = client.ListLocation(Shared.uid, id, location, region, limit).Message;
                if (hostMessage.Contains("Unable to connect") || hostMessage.Contains("One or more errors occurred") || hostMessage.Contains("Egy vagy több hiba történt")) // returns 0 item (instead of null) if backend cannot connect to database
                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                else if (hostMessage == "Unauthorized user!")
                {
                    Shared.Logout();
                    return null;
                }
                else
                {
                    // string query = $"WHERE name='{locationName}' AND unitPrice='{CreateMD5(unitprice)}'";
                    locationsArray = client.ListLocation(Shared.uid, id, location, region, limit).Locations;
                    locationsList = locationsArray.ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server") || ex.ToString().Contains("EndpointNotFoundException"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            // return location.ToList();
            return locationsList;
            
        }
        
    }
}