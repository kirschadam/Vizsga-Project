using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service
{
    [ServiceContract]
    public interface IStockService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/list?uid={uid}&name={name}&location={location}"
            )]
        Response_Stock ListStock(string uid, [Optional] string name, [Optional] string location);
    }

    [DataContract]
    public class Response_Stock
    {
        private string message = null;
        private List<Stock> stocks = new List<Stock>();

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public List<Stock> Stocks
        {
            get { return stocks; }
            set { stocks = value; }
        }

        public Response_Stock() { }

        public Response_Stock(string message, List<Stock> stocks)
        {
            Message = message;
            Stocks = stocks;
        }
    }

    [DataContract]
    public class Stock
    {
        private int? id = null, quantity = null;
        private string name = null, location = null;

        [DataMember]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public int? Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public Stock() { }

        public Stock(int? id, int? quantity, string name, string location)
        {
            Id = id;
            Quantity = quantity;
            Name = name;
            Location = location;
        }
    }
}
