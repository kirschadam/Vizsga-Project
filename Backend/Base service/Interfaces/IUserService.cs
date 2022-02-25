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
        /// <summary>
        /// GET function for users table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of user. (Optional)<br/>
        /// <paramref name="username"/>: Username of user. (Optional)<br/>
        /// <paramref name="location"/>: Location of user. (Optional)<br/>
        /// <paramref name="region"/>: Region of user. (Optional)<br/>
        /// <paramref name="limit"/>: Limit query results to this value. (Optional)<br/>
        /// </summary>
        /// <returns>A message or error of the query and a <see cref="System.Collections.Generic.List{T}"/> filled with <see cref="User"/> classes.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listuser?uid={uid}&id={id}&username={username}&location={location}&region={region}&limit={limit}"
            )]
        Response_User ListUser(string uid, [Optional] string id, [Optional] string username, [Optional] string location, [Optional] string region, [Optional] string limit);

        /// <summary>
        /// POST function to add user to active user list and send back user data and a UID.<br/>
        /// <paramref name="username"/>: Username of user. <br/>
        /// <paramref name="password"/>: Password of user with MD5 encryption.
        /// </summary>
        /// <returns>A message or error of the query, the <see cref="System.Guid"/> of the user and a <see cref="User"/> class.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/loginuser?username={username}&password={password}"
            )]
        Response_Login LoginUser(string username, string password);

        /// <summary>
        /// POST function to remove user from active user list.
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// </summary>
        /// <returns>A message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/logoutuser?uid={uid}"
            )]
        string LogoutUser(string uid);

        /// <summary>
        /// POST function to add new user to users table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="username"/>: Username of user.<br/>
        /// <paramref name="password"/>: Password of user with MD5 encryption.<br/>
        /// <paramref name="location"/>: Location of user.<br/>
        /// <paramref name="permission"/>: Permission level of user.<br/>
        /// </summary>
        /// <returns>A message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/registeruser"
            )]
        string RegisterUser(string uid, string username, string password, string location, string permission);

        /// <summary>
        /// PUT function to update user(s) in users table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of user to update.<br/>
        /// <paramref name="username"/>: Username of user. (Optional)<br/>
        /// <paramref name="password"/>: Password of user with MD5 encryption. (Optional)<br/>
        /// <paramref name="location"/>: Location of user. (Optional)<br/>
        /// <paramref name="permission"/>: Permission level of user. (Optional)<br/>
        /// <paramref name="active"/>: Active status of user. (Optional)
        /// </summary>
        /// <returns>A message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updateuser"
            )]
        string UpdateUser(string uid, string id, [Optional] string username, [Optional] string password, [Optional] string location, [Optional] string permission, [Optional] string active);

        /// <summary>
        /// DELETE function to delete user in users table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/> Id of user to delete. (Optional)<br/>
        /// <paramref name="username"/> Username of user to delete. (Optional)
        /// </summary>
        /// <returns>A message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deleteuser"
            )]
        string DeleteUser(string uid, [Optional] string id, [Optional] string username);
    }
}
