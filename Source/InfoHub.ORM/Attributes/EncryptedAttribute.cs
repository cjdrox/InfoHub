using System;

namespace InfoHub.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EncryptedAttribute : Attribute
    {
    }
}