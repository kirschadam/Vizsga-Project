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
        /// <summary>
        /// POST function for products table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="name"/>: Name of product.<br/>
        /// <paramref name="BuyUnitPrice"/>: Buying price of product.<br/>
        /// <paramref name="SellUnitPrice"/>: Selling price of product.
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addproduct?uid={uid}&name={name}&buyunitprice={buyunitprice}&sellunitprice={sellunitprice}"
            )]
        string AddProduct(string uid, string name, string BuyUnitPrice, string SellUnitPrice);

        /// <summary>
        /// POST function for sales and purchases tables.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="type"/>: The table you want to add to.<br/>
        /// <paramref name="product"/>: Name of product.<br/>
        /// <paramref name="quantity"/>: Quantity of product.<br/>
        /// <paramref name="location"/>: Location where sale/purchase is added to.<br/>
        /// <paramref name="unitPrice"/>: Buying or selling price of a single product.(Optional)<br/>
        /// <paramref name="date"/>: Date at which sale/purchase happened. (format: yyyy-MM-dd HH:mm:ss.fff) (Optional)<br/>
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addsalepurchase?uid={uid}&type={type}&product={product}&quantity={quantity}&location={location}&unitprice={unitprice}&date={date}"
            )]
        string AddSalePurchase(string uid, string type, string product, string quantity, string location, [Optional] string unitPrice, [Optional] string date);

        /// <summary>
        /// POST function for the stocks table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="product"/>: Name of product.<br/>
        /// <paramref name="location"/>: Location of Stock.
        /// </summary>
        /// <returns>Message or error of the query.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/addstock?uid={uid}&product={product}&location={location}"
            )]
        string AddStock(string uid, string product, string location);

        /// <summary>
        /// GET function for products table.
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of product. (Optional)<br/>
        /// <paramref name="name"/>: Name of product. (Optional)<br/>
        /// <paramref name="buyOver"/>: Products with buying price over this value. (Optional)<br/>
        /// <paramref name="buyUnder"/>: Products with buying price under this value. (Optional)<br/>
        /// <paramref name="sellOver"/>: Products with selling price over this value. (Optional)<br/>
        /// <paramref name="sellUnder"/>: Products with selling price under this value. (Optional)<br/>
        /// <paramref name="limit"/>: Limit query results to this value. (Optional)
        /// </summary>
        /// <returns>A message or error of the query and a List with Product classes.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listproduct?uid={uid}&id={id}&name={name}&buyover={buyover}&buyunder={buyunder}&sellover={sellover}&sellunder={sellunder}&limit={limit}"
            )]
        Response_Product ListProduct(string uid, [Optional] string id, [Optional] string name, [Optional] string buyOver, [Optional] string buyUnder, [Optional] string sellOver, [Optional] string sellUnder, [Optional] string limit);

        /// <summary>
        /// GET function for sales and purchases tables.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="type"/>: The table you want to add to.<br/>
        /// <paramref name="id"/>:  Id of sale or purchase.(Optional)<br/>
        /// <paramref name="product"/>:  Name of product in sale or purchase.(Optional)<br/>
        /// <paramref name="quantityOver"/>: Sale or purchase with quantity of products over this value. (Optional)<br/>
        /// <paramref name="quantityUnder"/>: Sale or purchase with quantity of products under this value. (Optional)<br/>
        /// <paramref name="priceOver"/>: Sale or purchase with total price over this value. (Optional)<br/>
        /// <paramref name="priceUnder"/>: Sale or purchase with total price under this value. (Optional)<br/>
        /// <paramref name="before"/>: Sale or purchase before this date. (format: yyyy-MM-dd HH:mm:ss.fff) (Optional)<br/>
        /// <paramref name="after"/>: Sale or purchase after this date. (format: yyyy-MM-dd HH:mm:ss.fff) (Optional)<br/>
        /// <paramref name="location"/>: Location of sale or purchase. (Optional)<br/>
        /// <paramref name="username"/>: User who uploaded sale or purchase into database. (Optional)<br/>
        /// <paramref name="limit"/>: Limit query results to this value. (Optional)<br/>
        /// </summary>
        /// <returns>A message or error of the query and a List with SalePurchase classes.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/listsalepurchase?uid={uid}&id={id}&type={type}&product={product}&quantityover={quantityover}&quantityunder={quantityunder}&priceover={priceover}&priceunder={priceunder}&before={before}&after={after}&location={location}&username={username}&limit={limit}"
            )]
        Response_SalePurchase ListSalePurchase(string uid, string type, [Optional] string id, [Optional] string product, [Optional] string quantityOver, [Optional] string quantityUnder, [Optional] string priceOver, [Optional] string priceUnder, [Optional] string before, [Optional] string after, [Optional] string location, [Optional] string username, [Optional] string limit);

        ///<summary>
        ///GET function for the stocks table.<br/>
        ///<paramref name="uid"/>: UID of current user.<br/>
        ///<paramref name="id"/>: Id of stock. (Optional)<br/>
        ///<paramref name="product"/>: Name of the product. (Optional)<br/>
        ///<paramref name="location"/>: Location of stock. (Optional)<br/>
        ///<paramref name="quantityOver"/>: Stock with number of products over this value. (Optional)<br/>
        ///<paramref name="quantityUnder"/>: Stock with number of products under this value. (Optional)<br/>
        ///<paramref name="limit"/>: Limit query results to this value. (Optional)
        ///</summary>
        ///<returns>A message or error of the query and a List with Stock classes.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/liststock?uid={uid}&id={id}&product={product}&location={location}&qover={qover}&qunder={qunder}&limit={limit}"
            )]
        Response_Stock ListStock(string uid, [Optional] string id, [Optional] string product, [Optional] string location, [Optional] string quantityOver, [Optional] string quantityUnder, [Optional] string limit);

        /// <summary>
        /// DELETE function for the product table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of product to delete.
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deleteproduct?uid={uid}&id={id}"
            )]
        string RemoveProduct(string uid, string id);

        /// <summary>
        /// DELETE function for sales and purchases tables.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="type"/>: The table you want to remove from.<br/>
        /// <paramref name="id"/>: Id of sale or purchase. (Optional)<br/>
        /// <paramref name="location"/>:Location where sales or purchases all need to be deleted, use with caution! (Optional) <br/><br/>
        /// Only the <paramref name="id"/> or <paramref name="location"/> needs to be given, function defaults to Id if both are given, but one of them must have a value.
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deletesalepurchase?uid={uid}&type={type}&id={id}&location={location}"
            )]
        string RemoveSalePurchase(string uid, string type, [Optional] string id, [Optional] string location);

        /// <summary>
        /// DELETE function for the stocks table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of stock to delete.(Optional)<br/>
        /// <paramref name="location"/>: Location where stocks all need to be deleted, use with caution! (Optional)<br/><br/>
        /// Only the <paramref name="id"/> or <paramref name="location"/> needs to be given, function defaults to Id if both are given, but one of them must have a value.
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/deletestock?uid={uid}&id={id}&location={location}"
            )]
        string RemoveStock(string uid, [Optional] string id, [Optional] string location);

        /// <summary>
        /// PUT function for products table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of stock to update.<br/>
        /// <paramref name="name"/>: Name of product. (Optional)<br/>
        /// <paramref name="buyPrice"/>: Buying price of product. (Optional)<br/>
        /// <paramref name="sellPrice"/>: Selling price of product. (Optional)
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updateproduct?uid={uid}&id={id}&name={name}&buyprice={buyprice}&sellprice={sellprice}"
            )]
        string UpdateProduct(string uid, string id, [Optional] string name, [Optional] string buyPrice, [Optional] string sellPrice);

        /// <summary>
        /// PUT function for sales and purchases tables.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of sale or purchase to update <br/>
        /// <paramref name="type"/>: The table you want to update in.<br/>
        /// <paramref name="product"/>: Name of product. (Optional)<br/>
        /// <paramref name="quantity"/>: Quantity of product. (Optional)<br/>
        /// <paramref name="totalPrice"/>: Buying or selling price of the whole transaction.(Optional)<br/>
        /// <paramref name="date"/>: Date at which sale/purchase happened. (format: yyyy-MM-dd HH:mm:ss.fff) (Optional)<br/>
        /// <paramref name="location"/>: Location where sale/purchase is added to. (Optional)<br/>
        /// <paramref name="username"/>: User who uploaded sale or purchase into database.
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updatesalepurchase?uid={uid}&id={id}&type={type}&product={product}&quantity={quantity}&totalprice={totalprice}&date={date}&location={location}&username={username}"
            )]
        string UpdateSalePurchase(string uid, string id, string type, [Optional] string product, [Optional] string quantity, [Optional] string totalPrice, [Optional] string date, [Optional] string location, [Optional] string username);

        /// <summary>
        /// PUT function for the stocks table.<br/>
        /// <paramref name="uid"/>: UID of current user.<br/>
        /// <paramref name="id"/>: Id of stock to update.<br/>
        /// <paramref name="product"/>: Name of product. (Optional)<br/>
        /// <paramref name="location"/>: Location of stock. (Optional)<br/>
        /// <paramref name="quantity"/>: Quantity of Stock.
        /// </summary>
        /// <returns>Message or error of query.</returns>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/updatestock?uid={uid}&id={id}&product={product}&location={location}&quantity={quantity}"
            )]
        string UpdateStock(string uid, string id, [Optional] string product, [Optional] string location, [Optional] string quantity);
    }
}
