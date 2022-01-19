using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Host
{
    class Program
    {
        static void Main()
        {
            Uri uri = new Uri("http://localhost:3000");
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            WebHttpBinding binding = new WebHttpBinding();
            using (ServiceHost host = new ServiceHost(
                typeof(Base_service.UserManagement), uri))
            {
                ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(Base_service.IUserManagement), binding, "");
                endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
                ServiceEndpoint endpoint1 = host.AddServiceEndpoint(typeof(Base_service.IUserManagement), basicHttpBinding, "soap");
                host.Open();
                Console.WriteLine("A szerver elindult.. {0}", DateTime.Now);
                bool stop = true;
                while (stop)
                {
                    string line = Console.ReadLine();
                    switch (line)
                    {
                        case "exit":
                            {
                                stop = false; break;
                            }
                        case "help":
                            {
                                Console.WriteLine("List of commands:\n\texit\n\thelp");
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Ez egy ismeretlen parancs!");
                                break;
                            }
                    }
                }
                host.Close();
            }
        }
    }
}
