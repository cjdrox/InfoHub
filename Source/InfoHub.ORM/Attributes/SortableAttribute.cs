using System;

namespace InfoHub.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SortableAttribute : Attribute
    {
        private readonly bool _sortable;

        public SortableAttribute(bool sortable = true)
        {
            _sortable = sortable;
        }

        public override string ToString()
        {
            return _sortable.ToString();
        }
    }
}