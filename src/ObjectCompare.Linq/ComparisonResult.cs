using System.Collections.Generic;

namespace ObjectCompare.Linq
{
    public class ComparisonResult
    {
        public List<Difference> Differences { get; set; } = new List<Difference>();
    }
}