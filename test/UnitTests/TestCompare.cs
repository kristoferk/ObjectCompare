using ObjectCompare;
using ObjectCompare.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Data;
using Xunit;

namespace UnitTests
{
    public class TestCompare
    {
        private static void ChangeOrder(Order obj2)
        {
            obj2.Created = new DateTime(2016, 6, 7);
            obj2.Sum = 1000;
            obj2.OrderHeader.OrderInfo.Invoice = "InvoiceTest";
            obj2.OrderType = OrderType.OrderB;
            obj2.Rows.RemoveAt(0);
            obj2.Rows.First().Quantity = 800;
            obj2.Rows.First().Product.Tags.Add("TestTag45");
            obj2.Rows.Add(new OrderRow());

            obj2.Rows.First(r => r.Product?.Meta != null && r.Product.Meta.Any()).Product.Meta.First().Id = "4";
            obj2.Rows.First(r => r.Product?.Meta != null && r.Product.Meta.Any()).Product.Meta.First().Value = "MetaTest45";
        }

        private static string FormatResult(ComparisonResult result)
        {
            List<string> sb = new List<string>();
            foreach (var d in result.Differences)
            {
                sb.Add(d.FormattedString);
            }

            string diff = string.Join("\n", sb.OrderBy(s => s));
            return diff;
        }

        [Fact]
        public void TestChange()
        {
            var comparer = new ObjectComparer<Order>();
            var obj1 = DataCreator.CreateOrder();
            var obj2 = DataCreator.CreateOrder();
            ChangeOrder(obj2);

            var result = comparer
                .Config(p => {
                    p.Fields(
                        f => f.OrderType,
                        f => f.Created,
                        f => f.OrderHeader.OrderInfo.Invoice
                        );
                    p.List(
                        l => l.Rows,
                        l => l.Id,
                        p2 => {
                            p2.Field(f => f.Quantity);
                            p2.List(
                                f => f.Product.Meta,
                                m => m.Id,
                                metaConfig => metaConfig.Field(mf => mf.Id)
                                );
                            p2.List(
                                f => f.Product.Tags,
                                m => m,
                                metaConfig => metaConfig.Field(mf => mf));
                        }
                        );
                })
                .Compare(obj1, obj2);

            var diff = FormatResult(result);

            List<string> expected = new List<string> {
                "Created: \"2016-07-06 00:00:00\" => \"2016-06-07 00:00:00\"",
                "OrderHeader.OrderInfo.Invoice: \"\" => \"InvoiceTest\"",
                "OrderType: \"OrderA\" => \"OrderB\"",
                "Quantity: \"2\" => \"800\"",
                "Rows.Product.Tags: 1 object added.",
                "Rows.Product.Meta: 1 object added.",
                "Rows.Product.Meta: 1 object removed.",
                "Rows: 1 object added.",
                "Rows: 1 object removed."
            };

            string expectedDiff = string.Join("\n", expected.OrderBy(s => s));
            Assert.Equal(expectedDiff, diff);
        }

        [Fact]
        public void TestFormat()
        {
            var comparer = new ObjectComparer<Order>();
            var obj1 = DataCreator.CreateOrder();
            var obj2 = DataCreator.CreateOrder();
            ChangeOrder(obj2);

            var result = comparer
                .Config(x => {
                    x.Field(f => f.OrderType).Format(z => $"Order type changed from \"{z.Object1Value}\" to \"{z.Object2Value}\"");
                    x.Field(f => f.Created).Format(z => $"Creation date changed from \"{z.Object1Value}\" to \"{z.Object2Value}\"");
                    x.Field(f => f.OrderHeader.OrderInfo.Invoice).Format(z => $"Order invoice changed from \"{z.Object1Value}\" to \"{z.Object2Value}\"");
                    x.List(
                        l => l.Rows,
                        l => l.Id,
                        config => {
                            config.Field(f => f.Quantity).Format(z =>
                                $"Amount changed from \"{z.Object1Value}\" to \"{z.Object2Value}\" for product \"{z.Object2.Product.Name}\""
                                );
                            config.List(
                                f => f.Product.Tags,
                                m => m,
                                metaConfig => metaConfig.Field(mf => mf).Format(z =>
                                    $"Tag changed from \"{z.Object1Value}\" to \"{z.Object2Value}\" for \"{z.Difference.NavigationPropertyName}\""
                                    )
                                )
                                .FormatObjectsAdded(f => $"{f.AddedObjects.Count} {(f.AddedObjects.Count == 1 ? "tag" : "tags")} added: {string.Join(",", f.AddedObjects)}");
                            config.List(
                                f => f.Product.Meta,
                                m => m.Id,
                                metaConfig => metaConfig.Field(mf => mf.Id).Format(z =>
                                    $"Meta id changed from \"{z.Object1Value}\" to \"{z.Object2Value}\" for \"{z.Difference.NavigationPropertyName}\""
                                    )
                                )
                                .FormatObjectsRemoved(f => $"{f.RemovedObjects.Count} {(f.RemovedObjects.Count == 1 ? "meta value" : "meta values")} removed.")
                                .FormatObjectsAdded(f => $"{f.AddedObjects.Count} {(f.AddedObjects.Count == 1 ? "meta value" : "meta values")} added.");
                        }
                        )
                        .FormatObjectsRemoved(f => $"{f.RemovedObjects.Count} order {(f.RemovedObjects.Count == 1 ? "row" : "rows")} removed.")
                        .FormatObjectsAdded(f => $"{f.AddedObjects.Count} order {(f.AddedObjects.Count == 1 ? "row" : "rows")} added.");
                })
                .Compare(obj1, obj2);

            var diff = FormatResult(result);

            List<string> expected = new List<string> {
                "Creation date changed from \"2016-07-06 00:00:00\" to \"2016-06-07 00:00:00\"",
                "Order invoice changed from \"\" to \"InvoiceTest\"",
                "Order type changed from \"OrderA\" to \"OrderB\"",
                "Amount changed from \"2\" to \"800\" for product \"Product B\"",
                "1 tag added: TestTag45",
                "1 meta value added.",
                "1 meta value removed.",
                "1 order row added.",
                "1 order row removed."
            };

            string expectedDiff = string.Join("\n", expected.OrderBy(s => s));
            Assert.Equal(expectedDiff, diff);
        }

        [Fact]
        public void TestTrack()
        {
            var comparer = new ObjectComparer<Order>();
            var obj1 = DataCreator.CreateOrder();
            var track = comparer.Track(obj1);
            ChangeOrder(obj1);

            comparer.Config(p => {
                p.Fields(
                    f => f.OrderType,
                    f => f.Created,
                    f => f.OrderHeader.OrderInfo.Invoice
                    );
                p.List(
                    l => l.Rows,
                    l => l.Id,
                    p2 => {
                        p2.Field(f => f.Quantity);
                        p2.List(
                            f => f.Product.Meta,
                            m => m.Id,
                            metaConfig => metaConfig.Field(mf => mf.Id)
                            );
                        p2.List(
                            f => f.Product.Tags,
                            m => m,
                            metaConfig => metaConfig.Field(mf => mf));
                    }
                    );
            });

            var diff = FormatResult(track.GetDiff());

            List<string> expected = new List<string> {
                "Created: \"2016-07-06 00:00:00\" => \"2016-06-07 00:00:00\"",
                "OrderHeader.OrderInfo.Invoice: \"\" => \"InvoiceTest\"",
                "OrderType: \"OrderA\" => \"OrderB\"",
                "Quantity: \"2\" => \"800\"",
                "Rows.Product.Tags: 1 object added.",
                "Rows.Product.Meta: 1 object added.",
                "Rows.Product.Meta: 1 object removed.",
                "Rows: 1 object added.",
                "Rows: 1 object removed."
            };

            string expectedDiff = string.Join("\n", expected.OrderBy(s => s));
            Assert.Equal(expectedDiff, diff);
        }
    }
}