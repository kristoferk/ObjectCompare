using System.Collections.Generic;

namespace ObjectCompare.Linq
{
    public class DifferenceObjectsRemoved : Difference
    {
        public int NumObjectsRemoved { get; set; } = 0;
    }

    public class DifferenceObjectsRemoved<T> : DifferenceObjectsRemoved
    {
        public List<T> RemovedObjects { get; set; } = new List<T>();
    }
}