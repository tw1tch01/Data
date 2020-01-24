using System;
using System.Linq.Expressions;

namespace Data.Extensions
{
    public static class LinqKitExtensions
    {
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
        }
    }
}