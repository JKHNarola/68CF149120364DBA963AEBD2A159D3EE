﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace App.BL
{
    public static class DynamicLinqHelper
    {
        public static async Task<DataSourceResult<T>> ToDatsSourceResultAsync<T>(this IQueryable<T> queryable, Q q)
        {
            try
            {
                queryable = PrepareWhereClause(queryable, q);
                var total = queryable.Count();
                var aggregate = PrepareAggregate(queryable, q);
                queryable = PrepareSort(queryable, q);
                if (q.PageNo > 0)
                    queryable = PreparePage(queryable, q.PageNo, q.PageSize);
                return new DataSourceResult<T>
                {
                    Data = await queryable.ToListAsync(),
                    Total = total,
                    Aggregates = aggregate
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static IQueryable<T> PrepareWhereClause<T>(IQueryable<T> queryable, Q q)
        {
            var wc = new StringBuilder();
            var values = new List<object>();
            var valueIndex = 0;
            foreach (var x in q.GetWhereClauseParts())
            {
                if (x.IsStartBracket)
                    wc.Append(" ( ");
                else if (x.IsEndBracket)
                    wc.Append(" ) ");
                else if (x.Logic.HasValue)
                    wc.Append(" " + x.Logic.ToString() + " ");
                else
                {
                    switch (x.Operator)
                    {
                        case Operator.Gt:
                            wc.Append(x.ColumnName + ">@" + valueIndex + " ");
                            break;
                        case Operator.Lt:
                            wc.Append(x.ColumnName + "<@" + valueIndex + " ");
                            break;
                        case Operator.Eq:
                            wc.Append(x.ColumnName + "=@" + valueIndex + " ");
                            break;
                        case Operator.Le:
                            wc.Append(x.ColumnName + "<=@" + valueIndex + " ");
                            break;
                        case Operator.Ge:
                            wc.Append(x.ColumnName + ">=@" + valueIndex + " ");
                            break;
                        case Operator.Ne:
                            wc.Append(x.ColumnName + "!=@" + valueIndex + " ");
                            break;
                        case Operator.Contains:
                            wc.Append(x.ColumnName + ".Contains(@" + valueIndex + ") ");
                            break;
                        case Operator.NotContains:
                            wc.Append("!" + x.ColumnName + ".Contains(@" + valueIndex + ") ");
                            break;
                        case Operator.StartsWith:
                            wc.Append(x.ColumnName + ".StartsWith(@" + valueIndex + ") ");
                            break;
                        case Operator.EndsWith:
                            wc.Append(x.ColumnName + ".EndsWith(@" + valueIndex + ") ");
                            break;
                        case Operator.In:
                            //TODO 
                            break;
                        default:
                            throw new NotImplementedException("Operator " + x.Operator.ToString() + " not implemented.");
                    }
                    values.Add(x.Value);
                    valueIndex++;
                }
            }
            if (wc.Length != 0)
            {
                var p = values.ToArray();
                return queryable.Where(wc.ToString(), p);
            }
            else
                return queryable;
        }
        private static IQueryable<T> PrepareSort<T>(IQueryable<T> queryable, Q q)
        {
            var ordering = "";
            var sorts = q.GetSorts();
            if (sorts != null)
                ordering = string.Join(",", sorts.Select(s => s.ColumnName + " " + s.Direction.ToString()));

            if (string.IsNullOrEmpty(ordering.Trim())) ordering = typeof(T).GetProperties()[0].Name;

            return queryable.OrderBy(ordering);
        }
        private static object PrepareAggregate<T>(IQueryable<T> queryable, Q q)
        {
            var aggregates = q.GetAggregators();
            if (aggregates != null && aggregates.Any())
            {
                var objProps = new Dictionary<DynamicProperty, object>();
                var groups = aggregates.GroupBy(g => g.ColumnName);
                Type type = null;
                foreach (var group in groups)
                {
                    var fieldProps = new Dictionary<DynamicProperty, object>();
                    foreach (var aggregate in group)
                    {
                        var prop = typeof(T).GetProperty(aggregate.ColumnName);
                        var param = Expression.Parameter(typeof(T), "s");
                        var selector = aggregate.Aggregate == "count" && (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                            ? Expression.Lambda(Expression.NotEqual(Expression.MakeMemberAccess(param, prop), Expression.Constant(null, prop.PropertyType)), param)
                            : Expression.Lambda(Expression.MakeMemberAccess(param, prop), param);
                        var mi = aggregate.MethodInfo(typeof(T));
                        if (mi == null)
                            continue;

                        var val = queryable.Provider.Execute(Expression.Call(null, mi,
                            aggregate.Aggregate == "count" && (Nullable.GetUnderlyingType(prop.PropertyType) == null)
                                ? new[] { queryable.Expression }
                                : new[] { queryable.Expression, Expression.Quote(selector) }));

                        fieldProps.Add(new DynamicProperty(aggregate.Aggregate, typeof(object)), val);
                    }
                    type = DynamicExpression.CreateClass(fieldProps.Keys);
                    var fieldObj = Activator.CreateInstance(type);
                    foreach (var p in fieldProps.Keys)
                        type.GetProperty(p.Name).SetValue(fieldObj, fieldProps[p], null);
                    objProps.Add(new DynamicProperty(group.Key, fieldObj.GetType()), fieldObj);
                }

                type = DynamicExpression.CreateClass(objProps.Keys);

                var obj = Activator.CreateInstance(type);

                foreach (var p in objProps.Keys)
                {
                    type.GetProperty(p.Name).SetValue(obj, objProps[p], null);
                }

                return obj;
            }
            else
            {
                return null;
            }
        }
        private static IQueryable<T> PreparePage<T>(IQueryable<T> queryable, int PageNum, int pageSize)
        {
            var sk = (PageNum - 1) * pageSize;
            return queryable.Skip(sk).Take(pageSize);
        }
    }
}