using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace App.BL
{
    public class Query
    {
        public List<ConditionPart> WhereClauseParts = null;
        public Dictionary<string, object> Extras = null;
        public List<Sort> Sorts = null;
        public List<Aggregator> Aggregates = null;
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public Query(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
            Init();
        }

        public Query()
        {
            Init();
        }

        private void Init()
        {
            WhereClauseParts = new List<ConditionPart>();
            Aggregates = new List<Aggregator>();
            Sorts = new List<Sort>();
            Extras = new Dictionary<string, object>();
        }

        public void AddStartBracket(Logic? logic = null)
        {
            if (logic == null) logic = Logic.And;
            WhereClauseParts.Add(new ConditionPart() { IsStartBracket = true, IsEndBracket = false, Logic = logic });
        }

        public void AddEndBracket()
        {
            WhereClauseParts.Add(new ConditionPart() { IsEndBracket = true, IsStartBracket = false });
        }

        public void AddCondition(string columnName, object value, Operator op = Operator.Eq, Logic? logic = null)
        {
            if (logic == null) logic = Logic.And;
            WhereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, ColumnName = columnName, Operator = op, Value = value, Logic = logic });
        }

        public void AddConditionIsNull(string columnName, Logic? logic = null)
        {
            if (logic == null) logic = Logic.And;
            WhereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, ColumnName = columnName, Operator = Operator.IsNull, Value = null, Logic = logic });
        }

        public void AddConditionIsNotNull(string columnName, Logic? logic = null)
        {
            if (logic == null) logic = Logic.And;
            WhereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, ColumnName = columnName, Operator = Operator.IsNotNull, Value = null, Logic = logic });
        }

        public void AddConditionIsEmpty(string columnName, Logic? logic = null)
        {
            if (logic == null) logic = Logic.And;
            WhereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, ColumnName = columnName, Operator = Operator.IsEmpty, Value = null, Logic = logic });
        }

        public void AddConditionIsNotEmpty(string columnName, Logic? logic = null)
        {
            if (logic == null) logic = Logic.And;
            WhereClauseParts.Add(new ConditionPart() { IsEndBracket = false, IsStartBracket = false, ColumnName = columnName, Operator = Operator.IsNotEmpty, Value = null, Logic = logic });
        }

        public void AddSort(string columnName, SortOrder direction)
        {
            var s = Sorts.FirstOrDefault(x => x.ColumnName.ToLower() == columnName.ToLower());
            if (s != null) s.Direction = direction;
            else
                Sorts.Add(new Sort()
                {
                    ColumnName = columnName,
                    Direction = direction
                });
        }

        public void AddAggregate(string columnName, string aggregate)
        {
            var a = Aggregates.FirstOrDefault(x => x.ColumnName.ToLower() == columnName.ToLower());
            if (a != null) a.Aggregate = aggregate;
            else
                Aggregates.Add(new Aggregator()
                {
                    ColumnName = columnName,
                    Aggregate = aggregate
                });
        }

        public void AddExtra(string key, object value)
        {
            Extras.Add(key, value);
        }
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
