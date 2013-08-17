using System;

namespace InfoHub.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        public TableAttribute(string tableName = "")
        {
            TableName = tableName;
        }

        public string TableName { get; private set; }

        public override string ToString()
        {
            return "Table";
        }
    }
}