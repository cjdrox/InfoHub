using System;
using InfoHub.Deployer.Helpers;

namespace InfoHub.Deployer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ExecuteAttribute : Attribute
    {
        public override string ToString()
        {
            return Names.ExecuteName;
        }
    }
}