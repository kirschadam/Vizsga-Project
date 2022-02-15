using Base_service.JsonClasses;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service.Interfaces
{
    [ServiceContract]
    [MyBehavior]
    public interface IUserService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/list?uid={uid}&id={id}&username={username}&location={location}&region={region}&limit={limit}"
            )]
        Response_User ListUser(string uid, [Optional] string id, [Optional] string username, [Optional] string location, [Optional] string region, [Optional] string limit);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/login?username={username}&password={password}"
            )]
        Response_Login LoginUser(string username, string password);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/logout?uid={uid}"
            )]
        string LogoutUser(string uid);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/register?uid={uid}&username={username}&password={password}&locationid={locationId}&permission={permission}"
            )]
        //id and active status is given by default, so those do not need to be given
        string RegisterUser(string uid, string username, string password, string locationId, string permission);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/update?uid={uid}&id={id}&username={username}&password={password}&locationid={locationId}&permission={permission}&active={active}"
            )]
        //You can't update the id, it is used to search for the user to be updated
        string UpdateUser(string uid, string id, [Optional] string username, [Optional] string password, [Optional] string locationId, [Optional] string permission, [Optional] string active);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/delete?uid={uid}&id={id}&username={username}"
            )]
        string DeleteUser(string uid, [Optional] string id, [Optional] string username);
    }
}
