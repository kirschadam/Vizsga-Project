using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Base_service.JsonClasses
{
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