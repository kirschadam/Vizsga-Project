using Base_service.JsonClasses;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service.Interfaces
{
    [ServiceContract]
    [MyBehavior]
    public interface IStockService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/liststock?uid={uid}&id={id}&product={product}&location={location}&qover={qover}&qunder={qunder}&limit={limit}"
            )]
        Response_Stock ListStock(string uid, [Optional] string id, [Optional] string product, [Optional] string location, [Optional] string qOver, [Optional] string qUnder, [Optional] string limit);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addstock?uid={uid}&product={product}&location={location}"
            )]
        string AddStock(string uid, string product, string location);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updatestock?uid={uid}&id={id}&product={product}&location={location}&quantity={quantity}"
            )]
        string UpdateStock(string uid, string id, [Optional] string product, [Optional] string location, [Optional] string quantity);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deletestock?uid={uid}&id={id}&location={location}"
            )]
        string RemoveStock(string uid, [Optional] string id, [Optional] string location);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listproduct?uid={uid}&id={id}&name={name}&qover={qover}&under={qunder}&limit={limit}"
            )]
        Response_Product ListProduct(string uid, [Optional] string id, [Optional] string name, [Optional] string qOver, [Optional] string qUnder, [Optional] string limit);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addproduct?uid={uid}&name={name}&unitprice={unitprice}"
            )]
        string AddProduct(string uid, string name, string unitPrice);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updateproduct?uid={uid}&id={id}&name={name}&unitprice={unitprice}"
            )]
        string UpdateProduct(string uid, string id, [Optional] string name, [Optional] string unitPrice);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deleteproduct?uid={uid}&id={id}"
            )]
        string RemoveProduct(string uid, string id);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listsalepurchase?uid={uid}&id={id}&type={type}&product={product}&qover={qover}&under={qunder}&before={before}&after={after}&location={location}&username={username}&limit={limit}"
            )]
        Response_SalePurchase ListSalePurchase(string uid, string type, [Optional] string id, [Optional] string product, [Optional] string qOver, [Optional] string qUnder, [Optional] string before, [Optional] string after, [Optional] string location, [Optional] string username, [Optional] string limit);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addsalepurchase?uid={uid}&type={type}&product={product}&quantity={quantity}&location={location}&date={date}"
            )]
        string AddSalePurchase(string uid, string type, string product, string quantity, string location, [Optional] string date);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updatesalepurchase?uid={uid}&id={id}&type={type}&product={product}&quantity={quantity}&price={price}&date={date}&location={location}&username={username}"
            )]
        string UpdateSalePurchase(string uid, string id, string type, [Optional] string product, [Optional] string quantity, [Optional] string price, [Optional] string date, [Optional] string location, [Optional] string username);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deletesalepurchase?uid={uid}&type={type}&id={id}&location={location}"
            )]
        string RemoveSalePurchase(string uid, string type, [Optional] string id, [Optional] string location);
    }
}
