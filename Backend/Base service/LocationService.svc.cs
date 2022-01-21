using Base_service.DatabaseManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Base_service
{
    public class LocationService : BaseDatabaseCommands, ILocationService
    {
        public string Test()
        {
            return "Test successful!";
        }
    }
}
