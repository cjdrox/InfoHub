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
                default:
                    return typeName;
            }
        }
    }
}
