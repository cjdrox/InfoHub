using System;
using System.Collections;
using System.Collections.Generic;

namespace InfoHub.ORM.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable<>).IsAssignableFrom(type) || typeof(IList).IsAssignableFrom(type);
        }

        public static bool IsImplicitlyNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
