using System;

namespace InfoHub.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
        private readonly bool _sortable;

        public PrimaryKeyAttribute(bool sortable = true)
        {
            _sortable = sortable;
        }

        public override string ToString()
        {
            return _sortable.ToString();
        }
    }
}