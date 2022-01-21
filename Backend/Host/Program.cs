using System;
using System.ServiceModel;

namespace Host
{
    class Program
    {
        static void Main()
        {
            using (ServiceHost host1 = new ServiceHost(typeof(Base_service.UserManagement)))
            using (ServiceHost host2 = new ServiceHost(typeof(Base_service.StorageManagement)))
            {
                host1.Open(); host2.Open();
                Console.WriteLine("A szerver elindult.. {0}", DateTime.Now);
                Console.ReadKey();
            }
        }
    }
}
