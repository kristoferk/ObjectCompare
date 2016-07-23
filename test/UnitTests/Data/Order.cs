using System;
using System.Collections.Generic;

namespace UnitTests.Data
{
    public class Order
    {
        public DateTime Created { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public OrderType OrderType { get; set; }

        public OrderHeader OrderHeader { get; set; }

        public decimal Sum { get; set; }

        public List<OrderRow> Rows { get; set; }
    }

    public class OrderHeader
    {
        public OrderInfo OrderInfo { get; set; }
    }

    public class OrderInfo
    {
        public string Invoice { get; set; }
    }

    public class OrderRow
    {
        public int Id { get; set; }

        public decimal Quantity { get; set; }

        public Product Product { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<string> Tags { get; set; }

        public List<MetaData> Meta { get; set; }
    }

    public class MetaData
    {
        public string Id { get; set; }

        public string Value { get; set; }
    }

    public enum OrderType
    {
        OrderA = 1,
        OrderB = 2
    }
}