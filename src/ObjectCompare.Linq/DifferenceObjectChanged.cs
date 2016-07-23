namespace ObjectCompare.Linq
{
    public class DifferenceObjectChanged : Difference
    {
        public string Object1Value { get; set; }

        public string Object2Value { get; set; }
    }

    public class DifferenceObjectChanged<T> : DifferenceObjectChanged
    {
        public T Object1 { get; set; }

        public T Object2 { get; set; }
    }
}