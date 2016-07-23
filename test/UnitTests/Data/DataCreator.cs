using System;
using System.Collections.Generic;

namespace UnitTests.Data
{
    public static class DataCreator
    {
        public static Order CreateOrder()
        {
            var rows = new List<OrderRow> {
                new OrderRow {
                    Id = 1,
                    Quantity = 1,
                    Product = new Product {
                        Id = "1",
                        Name = "Product A"
                    }
                },
                new OrderRow {
                    Id = 2,
                    Quantity = 2,
                    Product = new Product {
                        Id = "2",
                        Name = "Product B",
                        Meta = new List<MetaData> {
                            new MetaData { Id = "1", Value = "MetaValue" },
                            new MetaData { Id = "2", Value = "MetaValue" }
                        },
                        Tags = new List<string> { "Test", "Testar" }
                    }
                }
            };

            return new Order {
                Created = new DateTime(2016, 7, 6),
                OrderType = OrderType.OrderA,
                Sum = 500,
                TimeStamp = new DateTimeOffset(new DateTime(2016, 7, 6)),
                Rows = rows,
                OrderHeader = new OrderHeader {
                    OrderInfo = new OrderInfo { Invoice = string.Empty }
                }
            };
        }
    }
}