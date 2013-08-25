using System;

namespace InfoHub.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public abstract class OnActionAttribute : Attribute
    {
        public Do Action { get; protected set; }
    }

    public sealed class OnDeleteAttribute : OnActionAttribute
    {
        public OnDeleteAttribute(Do action)
        {
            Action = action ;
        }
    }

    public enum Do
    {
        Proceed,
        Restrict,
        Cascade
    }
}