using System;
using System.ServiceModel;

namespace Host
{
    class Program
    {
        static void Main()
        {
            using (ServiceHost host1 = new ServiceHost(typeof(Base_service.UserService)))
            using (ServiceHost host2 = new ServiceHost(typeof(Base_service.StockService)))
            using (ServiceHost host3 = new ServiceHost(typeof(Base_service.LocationService)))
            {
                host1.Open(); host2.Open(); host3.Open();
                Console.WriteLine("A szerver elindult.. {0}", DateTime.Now);
                Console.ReadKey();
            }
        }
    }
}
