using ObjectCompare.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectCompare.Linq.Comparing
{
    internal static class CompareFields
    {
        public static List<Difference> Compare<T>(T obj1, T obj2, List<Expression<Func<T, object>>> fieldsToCompare, string parentPath)
        {
            List<Difference> differences = new List<Difference>();

            foreach (var fieldToCompare in fieldsToCompare)
            {
                CompareField(differences, fieldToCompare, obj1, obj2, parentPath);
            }

            return differences;
        }

        public static List<Difference> CompareList<T, T2>(T obj1, T obj2, Expression<Func<T, IEnumerable<T2>>> expression, Func<T2, object> equality, Action<CompareConfig<T2>> config, string parentPath)
        {
            if (obj1 == null && obj2 == null)
            {
                return new List<Difference>();
            }

            List<Difference> differences = new List<Difference>();

            var compiledExpression = expression.Compile();
            List<T2> list1 = null;
            try
            {
                list1 = compiledExpression(obj1)?.ToList();
            }
            catch (NullReferenceException)
            {
            }

            if (list1 == null)
            {
                list1 = Activator.CreateInstance<List<T2>>();
            }

            List<T2> list2;
            try
            {
                list2 = compiledExpression(obj2)?.ToList();
            }
            catch (NullReferenceException)
            {
                list2 = Activator.CreateInstance<List<T2>>();
            }

            if (list2 == null)
            {
                list2 = Activator.CreateInstance<List<T2>>();
            }

            var navigationPropertyName = parentPath + "." + GetParentString(expression.Body);
            var propertyName = GetPropertyName(expression.Body);

            var dictionary1 = list1.ToDictionary(equality);
            var ids1 = dictionary1.Select(s => s.Key);
            var dictionary2 = list2.ToDictionary(equality);
            var ids2 = dictionary2.Select(s => s.Key);

            var newRows = dictionary2.Where(id => !ids1.Contains(id.Key)).ToList();
            var removedRows = dictionary1.Where(id => !ids2.Contains(id.Key)).ToList();

            if (newRows.Any())
            {
                differences.Add(new DifferenceObjectsAdded<T2> {
                    NavigationPropertyName = navigationPropertyName,
                    PropertyName = propertyName,
                    NumObjectsAdded = newRows.Count,
                    ObjectsAdded = newRows.Select(r => r.Value).ToList(),
                    FormattedString = $"{navigationPropertyName.TrimStart('.')}: {newRows.Count} {(newRows.Count == 1 ? "object" : "objects")} added."
                });
            }

            if (removedRows.Any())
            {
                differences.Add(new DifferenceObjectsRemoved<T2> {
                    NavigationPropertyName = navigationPropertyName,
                    PropertyName = propertyName,
                    NumObjectsRemoved = removedRows.Count,
                    RemovedObjects = removedRows.Select(r => r.Value).ToList(),
                    FormattedString = $"{navigationPropertyName.TrimStart('.')}: {removedRows.Count} {(removedRows.Count == 1 ? "object" : "objects")} removed."
                });
            }

            IOrderedEnumerable<KeyValuePair<object, T2>> equalRows = dictionary2.Where(x => dictionary1.ContainsKey(x.Key)).OrderBy(s => s.Key);

            foreach (KeyValuePair<object, T2> key in equalRows)
            {
                T2 innerObj1 = dictionary1[key.Key];
                T2 innerObj2 = dictionary2[key.Key];

                var config2 = new CompareConfig<T2>(innerObj1, innerObj2, navigationPropertyName);
                config(config2);
                differences.AddRange(config2.Differences);
            }

            return differences;
        }

        private static void CompareField<T>(List<Difference> differences, Expression<Func<T, object>> fieldToCompare, T obj1, T obj2, string parentPath)
        {
            var compiledExpression = fieldToCompare.Compile();
            var navigationPropertyName = GetParentString(fieldToCompare.Body);
            var propertyName = GetPropertyName(fieldToCompare.Body);

            object value1;
            try
            {
                value1 = compiledExpression(obj1);
            }
            catch (NullReferenceException)
            {
                value1 = null;
            }

            object value2;
            try
            {
                value2 = compiledExpression(obj2);
            }
            catch (NullReferenceException)
            {
                value2 = null;
            }

            if (value1 == null && value2 == null)
            {
                return;
            }

            if (value1 == null || !value1.Equals(value2))
            {
                differences.Add(new DifferenceObjectChanged<T> {
                    NavigationPropertyName = parentPath + "." + navigationPropertyName,
                    PropertyName = propertyName,
                    Object1Value = value1?.ToString(),
                    Object2Value = value2?.ToString(),
                    Object1 = obj1,
                    Object2 = obj2,
                    FormattedString = $"{navigationPropertyName}: \"{value1}\" => \"{value2}\""
                });
            }
        }

        private static string GetParentString(MemberExpression expression, string str)
        {
            if (expression == null)
            {
                return string.Empty;
            }

            var propertyExpression = expression.Expression as MemberExpression;
            if (propertyExpression != null)
            {
                str = propertyExpression.Member.Name + "." + str;
                return GetParentString(propertyExpression, str);
            }

            return str;
        }

        private static string GetParentString(Expression expression)
        {
            MemberExpression body = expression as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = expression as UnaryExpression;
                if (ubody != null)
                {
                    body = ubody.Operand as MemberExpression;
                }
            }

            return GetParentString(body, body?.Member.Name);
        }

        private static string GetPropertyName(Expression expression)
        {
            if (expression == null)
            {
                return string.Empty;
            }

            MemberExpression body = expression as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = expression as UnaryExpression;
                if (ubody != null)
                {
                    body = ubody.Operand as MemberExpression;
                }
            }

            string propertyName = string.Empty;
            if (body != null)
            {
                propertyName = body.Member.Name;
            }

            return propertyName;
        }
    }
}