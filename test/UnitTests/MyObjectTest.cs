using ObjectCompare.Linq;
using System;
using System.Linq;
using UnitTests.Data;
using Xunit;

namespace UnitTests
{
    public class MyObjectTest
    {
        [Fact]
        public void TestDoc()
        {
            var myObject = new MyObject { MyProperty = 3 };

            var comparer = new ObjectComparer<MyObject>();
            comparer.Config(p => {
                p.Field(f => f.MyProperty);
            });

            var track = comparer.Track(myObject);
            myObject.MyProperty = 4;

            var result = track.GetDiff();

            Console.Write(string.Join("\n", result.Differences.Select(d => d.FormattedString)));
        }
    }
}