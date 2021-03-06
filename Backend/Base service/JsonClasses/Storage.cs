using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Base_service.JsonClasses
{
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
        private string product = null, location = null;

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
        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        [DataMember]
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public Stock() { }

        public Stock(int? id, int? quantity, string product, string location)
        {
            Id = id;
            Quantity = quantity;
            Product = product;
            Location = location;
        }
    }

    [DataContract]
    public class Response_Product
    {
        private string message = null;
        private List<Product> products = new List<Product>();

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public List<Product> Products
        {
            get { return products; }
            set { products = value; }
        }

        public Response_Product() { }

        public Response_Product(string message, List<Product> products)
        {
            Message = message;
            Products = products;
        }
    }

    [DataContract]
    public class Product
    {
        private int? id = null, buyUnitPrice = null, sellUnitPrice = null;
        private string name = null;

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
        public int? BuyUnitPrice
        {
            get { return buyUnitPrice; }
            set { buyUnitPrice = value; }
        }

        [DataMember]
        public int? SellUnitPrice
        {
            get { return sellUnitPrice; }
            set { sellUnitPrice = value; }
        }

        public Product() { }

        public Product(int? id, string name, int? buyUnitPrice, int? sellUnitPrice)
        {
            Id = id;
            Name = name;
            BuyUnitPrice = buyUnitPrice;
            SellUnitPrice = sellUnitPrice;
        }
    }

    [DataContract]
    public class Response_SalePurchase
    {
        private string message = null;
        private List<SalePurchase> salespurchases = new List<SalePurchase>();

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public List<SalePurchase> SalesPurchases
        {
            get { return salespurchases; }
            set { salespurchases = value; }
        }

        public Response_SalePurchase() { }

        public Response_SalePurchase(string message, List<SalePurchase> salespurchases)
        {
            Message = message;
            SalesPurchases = salespurchases;
        }
    }

    [DataContract]
    public class SalePurchase
    {
        private int? id = null, quantity = null, totalPrice = null;
        private string product = null, location = null, username = null;
        private DateTime? date = null;

        [DataMember]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        [DataMember]
        public int? Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [DataMember]
        public int? TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }

        [DataMember]
        public DateTime? Date
        {
            get { return date; }
            set { date = value; }
        }

        [DataMember]
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        [DataMember]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }


        public SalePurchase() { }

        public SalePurchase(int? id, string product, int? quantity, int? totalPrice, DateTime? date, string location, string username)
        {
            Id = id;
            Product = product;
            Quantity = quantity;
            TotalPrice = totalPrice;
            Date = date;
            Location = location;
            Username = username;
        }
    }
}