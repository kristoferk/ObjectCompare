using ObjectCompare.Linq.Expressions;

namespace ObjectCompare.Linq
{
    public interface ITrack
    {
        ComparisonResult GetDiff();
    }

    public class Track<T> : ITrack
    {
        private readonly ObjectComparer<T> _comparer;

        public Track(ObjectComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public ComparisonResult GetDiff()
        {
            var compareConfig = new CompareConfig<T>(_comparer.Object1, _comparer.Object2);
            _comparer.ConfigAction(compareConfig);
            return _comparer.Compare(_comparer.Object1, _comparer.Object2);
        }
    }
}