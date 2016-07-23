using System.Collections.Generic;

namespace ObjectCompare.Linq
{
    public class DifferenceObjectsAdded : Difference
    {
        public int NumObjectsAdded { get; set; } = 0;
    }

    public class DifferenceObjectsAdded<T> : DifferenceObjectsAdded
    {
        public List<T> ObjectsAdded { get; set; } = new List<T>();
    }
}