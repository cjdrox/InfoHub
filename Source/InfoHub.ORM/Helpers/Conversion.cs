using System;

namespace InfoHub.ORM.Helpers
{
    public static class Conversion
    {
        public static string ToMySQL(this string typeName)
        {
            switch (typeName)
            {
                case "Int32":
                    return "INT";
                case "String":
                    return "VARCHAR(255)";
                case "Guid":
                    return "VARCHAR(255)";
                case "DateTime":
                    return "DATETIME";
                case "Boolean":
                    return "BOOL";
                default:
                    return typeName;
            }
        }

        public static object ToType(this string value, Type t)
        {
            return Convert.ChangeType(value, t);
        }
    }
}
