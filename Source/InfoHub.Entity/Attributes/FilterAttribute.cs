using System;

namespace InfoHub.Entity.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class FilterAttribute : Attribute
    {
        private readonly bool _sortable;

        public FilterAttribute(bool sortable = true)
        {
            _sortable = sortable;
        }

        public override string ToString()
        {
            return _sortable.ToString();
        }
    }
}