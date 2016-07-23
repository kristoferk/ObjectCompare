using ObjectCompare.Linq.Copy;
using ObjectCompare.Linq.Expressions;
using System;

namespace ObjectCompare.Linq
{
    public interface IObjectComparer<T>
    {
        ObjectComparer<T> Config(Action<CompareConfig<T>> config);

        ComparisonResult Compare(T obj1, T obj2);

        Track<T> Track(T obj);
    }

    public class ObjectComparer<T> : IObjectComparer<T>
    {
        internal Action<CompareConfig<T>> ConfigAction { get; set; }

        internal T Object1 { get; set; }

        internal T Object2 { get; set; }

        public ObjectComparer<T> Config(Action<CompareConfig<T>> config)
        {
            ConfigAction = config;
            return this;
        }

        public ComparisonResult Compare(T obj1, T obj2)
        {
            var compareConfig = new CompareConfig<T>(obj1, obj2);
            ConfigAction(compareConfig);

            return new ComparisonResult {
                Differences = compareConfig.Differences
            };
        }

        public Track<T> Track(T obj)
        {
            Object1 = obj.Copy();
            Object2 = obj;
            return new Track<T>(this);
        }
    }
}