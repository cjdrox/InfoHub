using System;

namespace InfoHub.Entity.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class MapAsAttribute : Attribute
    {
        private readonly Type _type;

        public MapAsAttribute(Type type)
        {
            _type = type;
        }

        public override string ToString()
        {
            return _type.Name;
        }
    }
}