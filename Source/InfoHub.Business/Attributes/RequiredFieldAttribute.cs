using System;

namespace InfoHub.Business.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class RequiredFieldAttribute : Attribute
    {
    }
}
