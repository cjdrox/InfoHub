using System;

namespace InfoHub.Business.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OperativeFieldAttribute : Attribute
    {
        public bool Sortable { get; set; }
        public bool Discrete { get; set; }
        public bool Continuous { get; set; }
    }
}