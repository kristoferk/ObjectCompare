using System;
using System.Collections.Generic;

namespace ObjectCompare.Linq.Expressions
{
    public class ListExpression<T>
    {
        private readonly List<Difference> _diffs;

        public ListExpression(List<Difference> diffs)
        {
            _diffs = diffs;
        }

        public ListExpression<T> FormatObjectsRemoved(Func<FormatRemovedObjects<T>, string> func)
        {
            foreach (var diff in _diffs)
            {
                DifferenceObjectsRemoved<T> removedDiff = diff as DifferenceObjectsRemoved<T>;
                if (removedDiff != null)
                {
                    diff.FormattedString = func(new FormatRemovedObjects<T> {
                        RemovedObjects = removedDiff.RemovedObjects,
                        Difference = removedDiff
                    });
                }
            }
            return this;
        }

        public ListExpression<T> FormatObjectsAdded(Func<FormatAddedObjects<T>, string> func)
        {
            foreach (var diff in _diffs)
            {
                DifferenceObjectsAdded<T> addedDiff = diff as DifferenceObjectsAdded<T>;
                if (addedDiff != null)
                {
                    diff.FormattedString = func(new FormatAddedObjects<T> {
                        AddedObjects = addedDiff.ObjectsAdded,
                        Difference = addedDiff
                    });
                }
            }

            return this;
        }
    }
}