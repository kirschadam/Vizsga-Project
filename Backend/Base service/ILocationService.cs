using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILocationService" in both code and config file together.
    [ServiceContract]
    public interface ILocationService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/test"
            )]
        string Test();
    }
}
