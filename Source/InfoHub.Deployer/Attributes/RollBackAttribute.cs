using System;
using InfoHub.ORM.Helpers;

namespace InfoHub.Deployer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class RollBackAttribute : Attribute
    {
        public override string ToString()
        {
            return Names.RollBackName;
        }
    }
}