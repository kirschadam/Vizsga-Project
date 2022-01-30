using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service
{
    [ServiceContract]
    public interface ILocationService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listlocation?uid={uid}&id={id}&location={location}&region={region}&limit={limit}"
            )]
        Response_Location ListLocation(string uid, [Optional] string id, [Optional] string location, [Optional] string region, [Optional] string limit);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addlocation?uid={uid}&location={location}&region={region}"
            )]
        string AddLocation(string uid, string location, string region);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updatelocation?uid={uid}&id={id}&location={location}&region={region}"
            )]
        string UpdateLocation(string uid, string id, [Optional] string location, [Optional] string region);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deletelocation?uid={uid}&id={id}&location={location}"
            )]
        string RemoveLocation(string uid, [Optional] string id, [Optional] string location);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listregion?uid={uid}&id={id}&region={region}&limit={limit}"
            )]
        Response_Region ListRegion(string uid, [Optional] string id, [Optional] string region, [Optional] string limit);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addregion?uid={uid}&region={region}"
            )]
        string AddRegion(string uid, string region);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updateregion?uid={uid}&id={id}&region={region}"
            )]
        string UpdateRegion(string uid, string id, string region);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deleteregion?uid={uid}&id={id}&region={region}"
            )]
        string RemoveRegion(string uid, [Optional] string id, [Optional] string region);
    }

    [DataContract]
    public class Response_Region
    {
        private string message = null;
        private List<Region> regions = new List<Region>();

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public List<Region> Regions
        {
            get { return regions; }
            set { regions = value; }
        }

        public Response_Region() { }

        public Response_Region(string message, List<Region> regions)
        {
            Message = message;
            Regions = regions;
        }
    }



    [DataContract]
    public class Region
    {
        private int? id = null;
        private string name = null;
        private List<Store> locations = new List<Store>();

        [DataMember]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public List<Store> Locations
        {
            get { return locations; }
            set { locations = value; }
        }

        public Region() { }

        public Region(int? id, string name)
        {
            Id = id;
            Name = name;
        }
    }



    [DataContract]
    public class Response_Location
    {
        private string message = null;
        private List<Store> locations = new List<Store>();

        [DataMember]
        public string Message 
        { 
            get { return message; } 
            set { message = value; } 
        }

        [DataMember]
        public List<Store> Locations
        {
            get { return locations; }
            set { locations = value; }
        }

        public Response_Location() { }

        public Response_Location(string message, List<Store> locations)
        {
            Message = message;
            Locations = locations;
        }
    }



    [DataContract]
    public class Store
    {
        private int? id = null;
        private string name = null, region = null;

        [DataMember]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Region
        {
            get { return region; } 
            set { region = value; }
        }

        public Store() { }

        public Store(int? id, string name, string region)
        {
            Id = id;
            Name = name;
            Region = region;
        }
    }
}
