using System;

namespace InfoHub.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class MapAsAttribute : Attribute
    {
        private string Name { get; set; }
        private Type Type { get; set; }

        public MapAsAttribute(Type type)
        {
            Name = null;
            Type = type;
        }

        public MapAsAttribute(string name)
        {
            Name = name;
            Type = null;
        }

        public MapAsAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return Type.Name;
        }

        public bool IsCustomName
        {
            get { return String.IsNullOrEmpty(Name); }
        }

        public bool IsCustomType
        {
            get { return Type == null; }
        }
    }
}