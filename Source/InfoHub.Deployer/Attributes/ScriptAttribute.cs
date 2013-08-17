using System;
using InfoHub.ORM.Helpers;

namespace InfoHub.Deployer.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class ScriptAttribute : Attribute
    {
        public override string ToString()
        {
            return Names.ScriptName;
        }
    }
}
