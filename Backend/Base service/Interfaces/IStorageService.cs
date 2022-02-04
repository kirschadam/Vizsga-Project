using Base_service.JsonClasses;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service.Interfaces
{
    [ServiceContract]
    public interface IStockService
    {
        /*[OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/liststock?uid={uid}&name={name}&location={location}&quantity={quantity}&limit={limit}"
            )]
        Response_Stock ListStock(string uid, [Optional] string name, [Optional] string location, [Optional] string quantity, [Optional] string limit);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addstock?uid={uid}&name={name}&location={location}"
            )]
        string AddStock(string uid, string name, string location);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updatestock?uid={uid}&id={id}&name={name}&location={location}&quantity={quantity}"
            )]
        string UpdateStock(string uid, string id, [Optional] string name, [Optional] string location, [Optional] string quantity);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deletestock?uid={uid}&id={id}"
            )]
        string RemoveStock(string uid, [Optional] string id;*/

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/query?query={query}"
            )]
        Response_Product Query(string query);
    }
}
