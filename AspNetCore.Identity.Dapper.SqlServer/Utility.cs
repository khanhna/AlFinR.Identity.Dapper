using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCore.Identity.Dapper.SqlServer
{
    internal class Utility
    {
        public static string GetPropertyNames<T>(params Expression<Func<T, object>>[] expressions)
        {
            var memberNames = expressions.Select(expression =>
            {
                var member = expression.Body as MemberExpression;
                var unary = expression.Body as UnaryExpression;
                return member ?? (unary?.Operand as MemberExpression);
            }).Select(memberExpression => memberExpression.Member.Name).AsEnumerable();

            return string.Join(",", memberNames);
        }

        public static IEnumerable<string> GetEnumerablePropertyNames<T>(params Expression<Func<T, object>>[] expressions)
        {
            return expressions.Select(expression =>
            {
                var member = expression.Body as MemberExpression;
                var unary = expression.Body as UnaryExpression;
                return member ?? (unary?.Operand as MemberExpression);
            }).Select(memberExpression => memberExpression.Member.Name).AsEnumerable();
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            var member = expression.Body as MemberExpression;
            var unary = expression.Body as UnaryExpression;

            if(member == null)
                member = unary?.Operand as MemberExpression;

            return member.Member.Name;
        }
    }
}
