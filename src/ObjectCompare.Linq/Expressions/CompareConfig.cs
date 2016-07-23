using ObjectCompare.Linq.Comparing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectCompare.Linq.Expressions
{
    public class CompareConfig<T>
    {
        private readonly T _obj1;
        private readonly T _obj2;
        private readonly string _propertyPath;

        public CompareConfig(T obj1, T obj2, string propertyPath = null)
        {
            _obj1 = obj1;
            _obj2 = obj2;
            _propertyPath = propertyPath;
        }

        internal List<Difference> Differences { get; set; } = new List<Difference>();

        public FieldExpression<T> Field(Expression<Func<T, object>> expression)
        {
            return Fields(expression);
        }

        public FieldExpression<T> Fields(params Expression<Func<T, object>>[] expressions)
        {
            List<Difference> diffs = CompareFields.Compare(_obj1, _obj2, expressions.ToList(), _propertyPath);
            Differences.AddRange(diffs);
            return new FieldExpression<T>(diffs);
        }

        public ListExpression<T2> List<T2>(
            Expression<Func<T, IEnumerable<T2>>> expression,
            Func<T2, object> equality,
            Action<CompareConfig<T2>> config)
        {
            var listDiffs = CompareFields.CompareList(_obj1, _obj2, expression, equality, config, _propertyPath);
            Differences.AddRange(listDiffs);
            return new ListExpression<T2>(listDiffs);
        }
    }
}