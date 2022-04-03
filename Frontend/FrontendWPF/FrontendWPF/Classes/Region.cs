using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
                string hostMessage = client.ListRegion(Shared.uid, id, region, limit).Message;
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
                    // string query = $"WHERE name='{regionsName}' AND unitPrice='{CreateMD5(unitprice)}'";
                    regionsArray = client.ListRegion(Shared.uid, id, region, limit).Regions;
                    regionsList = regionsArray.ToList();
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
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }
            // return location.ToList();
            return regionsList;
            
        }
        
    }
}