using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace App.BL
{
    public class Q
    {
        private List<ConditionPart> _whereClauseParts = null;
        private Dictionary<string, object> _extras = null;
        private List<Sort> _sorts = null;
        private List<Aggregator> _aggregates = null;
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public Q(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
            Init();
        }

        public Q()
        {
            Init();
        }

        private void Init()
        {
            _whereClauseParts = new List<ConditionPart>();
            _aggregates = new List<Aggregator>();
            _sorts = new List<Sort>();
            _extras = new Dictionary<string, object>();
        }

        public void AddStartBracket()
        {
            _whereClauseParts.Add(new ConditionPart() { IsStartBracket = true, IsEndBracket = false });
        }

        public void AddEndBracket()
        {
            _whereClauseParts.Add(new ConditionPart() { IsEndBracket = true, IsStartBracket = false });
        }

        public void AddCondition(string columnName, object value, Operator op = Operator.Eq)
        {
            _whereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, ColumnName = columnName, Operator = op, Value = value });
        }

        public void AddLogic(Logic logic = Logic.And)
        {
            _whereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, Logic = logic });
        }

        public void AddAnd()
        {
            _whereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, Logic = Logic.And });
        }

        public void AddOr()
        {
            _whereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, Logic = Logic.Or });
        }

        public void AddSort(string columnName, SortOrder direction)
        {
            //TODO Check not repeated
            _sorts.Add(new Sort()
            {
                ColumnName = columnName,
                Direction = direction
            });
        }

        public void AddAggregate(string columnName, string aggregate)
        {
            //TODO Check not repeated
            _aggregates.Add(new Aggregator()
            {
                ColumnName = columnName,
                Aggregate = aggregate
            });
        }

        public void AddExtra(string key, object value)
        {
            _extras.Add(key, value);
        }

        public List<Sort> GetSorts() => _sorts;

        public List<ConditionPart> GetWhereClauseParts() => _whereClauseParts;

        public List<Aggregator> GetAggregators() => _aggregates;
    }

    public class Sort
    {
        public string ColumnName { get; set; }
        public SortOrder Direction { get; set; }
    }

    public class ConditionPart
    {
        public bool IsStartBracket { get; set; }
        public bool IsEndBracket { get; set; }
        public string ColumnName { get; set; }
        public Operator? Operator { get; set; }
        public object Value { get; set; }
        public Logic? Logic { get; set; }
    }

    public class Aggregator
    {
        public string ColumnName { get; set; }
        public string Aggregate { get; set; }
        public MethodInfo MethodInfo(Type type)
        {
            var proptype = type.GetProperty(ColumnName).PropertyType;
            switch (Aggregate)
            {
                case "max":
                case "min":
                    return GetMethod(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Aggregate), MinMaxFunc().Method, 2).MakeGenericMethod(type, proptype);
                case "average":
                case "sum":
                    return GetMethod(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Aggregate),
                        ((Func<Type, Type[]>)this.GetType().GetMethod("SumAvgFunc", BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(proptype).Invoke(null, null)).Method, 1).MakeGenericMethod(type);
                case "count":
                    return GetMethod(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Aggregate),
                        Nullable.GetUnderlyingType(proptype) != null ? CountNullableFunc().Method : CountFunc().Method, 1).MakeGenericMethod(type);
            }
            return null;
        }

        private static MethodInfo GetMethod(string methodName, MethodInfo methodTypes, int genericArgumentsCount)
        {
            var methods = from method in typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                          let parameters = method.GetParameters()
                          let genericArguments = method.GetGenericArguments()
                          where method.Name == methodName &&
                            genericArguments.Length == genericArgumentsCount &&
                            parameters.Select(p => p.ParameterType).SequenceEqual((Type[])methodTypes.Invoke(null, genericArguments))
                          select method;
            return methods.FirstOrDefault();
        }

        private static Func<Type, Type[]> CountNullableFunc()
        {
            return (T) => new[]
                {
                    typeof(IQueryable<>).MakeGenericType(T),
                    typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(T, typeof(bool)))
                };
        }

        private static Func<Type, Type[]> CountFunc()
        {
            return (T) => new[]
                {
                    typeof(IQueryable<>).MakeGenericType(T)
                };
        }

        private static Func<Type, Type, Type[]> MinMaxFunc()
        {
            return (T, U) => new[]
                {
                    typeof (IQueryable<>).MakeGenericType(T),
                    typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, U))
                };
        }

        private static Func<Type, Type[]> SumAvgFunc<U>()
        {
            return (T) => new[]
                {
                    typeof (IQueryable<>).MakeGenericType(T),
                    typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof(U)))
                };
        }
    }
}
