using System.Collections.Generic;

namespace ObjectCompare.Linq.Expressions
{
    public class FormatObject<T>
    {
        public T Object1 { get; set; }

        public T Object2 { get; set; }

        public object Object1Value { get; set; }

        public object Object2Value { get; set; }

        public DifferenceObjectChanged<T> Difference { get; set; }
    }

    public class FormatRemovedObjects<T>
    {
        public List<T> RemovedObjects { get; set; }

        public T Object2 { get; set; }

        public DifferenceObjectsRemoved<T> Difference { get; set; }
    }

    public class FormatAddedObjects<T>
    {
        public List<T> AddedObjects { get; set; }

        public T Object2 { get; set; }

        public DifferenceObjectsAdded<T> Difference { get; set; }
    }
}