using System;
using System.Collections.Generic;

namespace ObjectCompare.Linq.Expressions
{
    public class FieldExpression<T>
    {
        private readonly List<Difference> _diffs;

        public FieldExpression(List<Difference> diffs)
        {
            _diffs = diffs;
        }

        public FieldExpression<T> Format(Func<FormatObject<T>, string> func)
        {
            foreach (var diff in _diffs)
            {
                DifferenceObjectChanged<T> changedDiff = (DifferenceObjectChanged<T>)diff;

                string formattedString = func(new FormatObject<T> {
                    Difference = changedDiff,
                    Object1Value = changedDiff.Object1Value,
                    Object2Value = changedDiff.Object2Value,
                    Object1 = changedDiff.Object1,
                    Object2 = changedDiff.Object2
                });

                diff.FormattedString = formattedString;
            }

            return this;
        }
    }
}