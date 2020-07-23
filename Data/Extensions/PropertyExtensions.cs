using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Data.Extensions
{
    public static class PropertyExtensions
    {
        public static TType SetProperty<TType, TProperty>
        (
            this TType type,
            Expression<Func<TType, TProperty>> lambda,
            object value
        ) where TType : class
        {
            if (type == null) return type;

            if (!(lambda.Body is MemberExpression selector)) throw new ArgumentException(nameof(lambda));

            var property = selector.Member as PropertyInfo;

            if (!property.CanWrite) throw new InvalidOperationException("Cannot set a restricted property.");

            if (value != null && property.PropertyType != value.GetType()) throw new InvalidOperationException("Value type does not match the Property type.");

            if (property.GetValue(type)?.ToString() == value?.ToString()) return type;

            property.SetValue(type, value);

            return type;
        }

        public static TType TrySetProperty<TType>
        (
            this TType type,
            string propertyName,
            object value
        ) where TType : class
        {
            if (type == null) return type;

            var property = type.GetType().GetProperty(propertyName);

            if (property == null) return type;

            if (!property.CanWrite) throw new InvalidOperationException("Cannot set a restricted property.");

            if (value != null && !property.PropertyType.IsAssignableFrom(value.GetType())) throw new InvalidOperationException("Property type is not assignable from the Value type.");

            property.SetValue(type, value);

            return type;
        }
    }
}