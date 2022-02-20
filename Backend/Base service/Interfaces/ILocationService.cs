using Base_service.JsonClasses;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service.Interfaces
{
    /// <summary>
    /// Interface for the various database queries related to store locations and administrative regions.
    /// </summary>
    [ServiceContract]
    [MyBehavior]
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
}
