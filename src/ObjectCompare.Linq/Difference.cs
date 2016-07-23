namespace ObjectCompare.Linq
{
    public class Difference
    {
        public string NavigationPropertyName { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public string Object1TypeName { get; set; }

        public string Object2TypeName { get; set; }

        public string FormattedString { get; set; }
    }
}