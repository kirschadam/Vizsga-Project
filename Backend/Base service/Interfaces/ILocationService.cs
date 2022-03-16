using Base_service.JsonClasses;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service.Interfaces
{
    [ServiceContract]
    [MyBehavior]
    public interface ILocationService
    {
        /// <summary>
        /// POST function for locations table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="location"/>: Name of the location.<br/>
        /// <paramref name="region"/>: Region which the location is tied to.<br/>
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "*",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addlocation"
            )]
        string AddLocation(string uid, string location, string region);

        /// <summary>
        /// POST function for regions table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="region"/>: Name of the region.<br/>
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "*",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addregion"
            )]
        string AddRegion(string uid, string region);

        /// <summary>
        /// GET function for locations table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of location. (Optional)<br/>
        /// <paramref name="location"/>: Name of location. (Optional)<br/>
        /// <paramref name="region"/>: Region of location. (Optional)<br/>
        /// <paramref name="limit"/>: Limit query results to this value. (Optional)
        /// </summary>
        /// <returns>A message or error of the query and a <see cref="System.Collections.Generic.List{T}"/> filled with <see cref="Store"/> classes.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listlocation?uid={uid}&id={id}&location={location}&region={region}&limit={limit}"
            )]
        Response_Location ListLocation(string uid, [Optional] string id, [Optional] string location, [Optional] string region, [Optional] string limit);

        /// <summary>
        /// GET function for regions table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of region. (Optional)<br/>
        /// <paramref name="region"/>: Name of region. (Optional)<br/>
        /// <paramref name="limit"/>: Limit query results to this value. (Optional)
        /// </summary>
        /// <returns>A message or error of the query and a <see cref="System.Collections.Generic.List{T}"/> filled with <see cref="Region"/> classes.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listregion?uid={uid}&id={id}&region={region}&limit={limit}"
            )]
        Response_Region ListRegion(string uid, [Optional] string id, [Optional] string region, [Optional] string limit);

        /// <summary>
        /// DELETE function for locations table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of location to delete. (Optional)<br/>
        /// <paramref name="location"/>: Name of location to delete. (Optional)<br/><br/>
        /// Only the <paramref name="id"/> or <paramref name="location"/> needs to be given, function defaults to Id if both are given, but one of them must have a value.
        /// </summary>
        /// <returns>A message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "*",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deletelocation"
            )]
        string RemoveLocation(string uid, [Optional] string id, [Optional] string location);

        /// <summary>
        /// DELETE function for regions table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of region to delete. (Optional)<br/>
        /// <paramref name="region"/>: Name of region to delete. (Optional)<br/><br/>
        /// Only the <paramref name="id"/> or <paramref name="location"/> needs to be given, function defaults to Id if both are given, but one of them must have a value.
        /// </summary>
        /// <returns>A message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "*",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deleteregion"
            )]
        string RemoveRegion(string uid, [Optional] string id, [Optional] string region);

        /// <summary>
        /// PUT function for locations table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of location to update.<br/>
        /// <paramref name="location"/> Name of location. (Optional)<br/>
        /// <paramref name="region"/> Region of location. (Optional)
        /// </summary>
        /// <returns>A message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "*",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updatelocation"
            )]
        string UpdateLocation(string uid, string id, [Optional] string location, [Optional] string region);

        /// <summary>
        /// PUT function for regionss table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of region to update. (Optional)<br/>
        /// <paramref name="region"/>: Name of region. (Optional)
        /// </summary>
        /// <returns>A message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "*",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updateregion"
            )]
        string UpdateRegion(string uid, string id, string region);
    }
}
