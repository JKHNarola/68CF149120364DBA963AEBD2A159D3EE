using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace App.BL
{
    public static class DynamicLinqHelper
    {
        public static Expression<Func<TEntity, bool>> CreateDynamicExpression<TEntity>(string propertyName, Operator op, string value, Type valueType)
        {
            Type type = typeof(TEntity);
            object asType = AsType(value, valueType);
            var p = Expression.Parameter(type, "x");
            var property = Expression.Property(p, propertyName);
            MethodInfo method;
            Expression q;

            switch (op)
            {
                case Operator.Gt:
                    q = Expression.GreaterThan(property, Expression.Constant(asType));
                    break;
                case Operator.Lt:
                    q = Expression.LessThan(property, Expression.Constant(asType));
                    break;
                case Operator.Eq:
                    q = Expression.Equal(property, Expression.Constant(asType));
                    break;
                case Operator.Le:
                    q = Expression.LessThanOrEqual(property, Expression.Constant(asType));
                    break;
                case Operator.Ge:
                    q = Expression.GreaterThanOrEqual(property, Expression.Constant(asType));
                    break;
                case Operator.Ne:
                    q = Expression.NotEqual(property, Expression.Constant(asType));
                    break;
                case Operator.Contains:
                    method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    q = Expression.Call(property, method ?? throw new InvalidOperationException(),
                        Expression.Constant(asType, typeof(string)));
                    break;
                case Operator.StartsWith:
                    method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                    q = Expression.Call(property, method ?? throw new InvalidOperationException(),
                        Expression.Constant(asType, typeof(string)));
                    break;
                case Operator.EndsWith:
                    method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                    q = Expression.Call(property, method ?? throw new InvalidOperationException(),
                        Expression.Constant(asType, typeof(string)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }

            return Expression.Lambda<Func<TEntity, bool>>(q, p);
        }

        private static object AsType(string value, Type type)
        {
            //TODO: This method needs to be expanded to include all appropriate use cases
            string v = value;
            if (value.StartsWith("'") && value.EndsWith("'"))
                v = value.Substring(1, value.Length - 2);

            if (type == typeof(string))
                return v;
            if (type == typeof(DateTime))
                return DateTime.Parse(v);
            if (type == typeof(DateTime?))
                return DateTime.Parse(v);
            if (type == typeof(int))
                return int.Parse(v);
            if (type == typeof(int?))
                return int.Parse(v);

            throw new ArgumentException("A filter was attempted for a field with value '" + value + "' and type '" +
                                        type + "' however this type is not currently supported");
        }

        public static IQueryable<T> PrepareWhereClause<T>(Q q)
        {
            var wc = new StringBuilder();
            for (var i = 0; i < q.WhereClause.Count(); i++)
            {
                var x = q.WhereClause[i];
                if (x.IsStartBracket)
                    wc.Append(" ( ");
                else if (x.IsEndBracket)
                    wc.Append(" ) ");
                else
                {
                    switch (x.Operator)
                    {
                        case Operator.Gt:
                            wc.Append(x.ColumnName + ">" + x.Value);
                            break;
                        case Operator.Lt:
                            wc.Append(x.ColumnName + "<" + x.Value);
                            break;
                        case Operator.Eq:
                            wc.Append(x.ColumnName + "=" + x.Value);
                            break;
                        case Operator.Le:
                            wc.Append(x.ColumnName + "<=" + x.Value);
                            break;
                        case Operator.Ge:
                            wc.Append(x.ColumnName + ">=" + x.Value);
                            break;
                        case Operator.Ne:
                            wc.Append(x.ColumnName + "!=" + x.Value);
                            break;
                        case Operator.Contains:
                            wc.Append(x.ColumnName + ".Contains(" + x.Value + ")");
                            break;
                        case Operator.StartsWith:
                            wc.Append(x.ColumnName + ".StartsWith(" + x.Value + ")");
                            break;
                        case Operator.EndsWith:
                            wc.Append(x.ColumnName + ".EndsWith(" + x.Value + ")");
                            break;
                        case Operator.In:
                            wc.Append(x.ColumnName + ".EndsWith(" + x.Value + ")");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(x.Operator), x.Operator, null);
                    }
                }
            }
        }

    }
}
