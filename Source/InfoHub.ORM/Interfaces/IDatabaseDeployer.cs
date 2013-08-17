using System;
using System.Reflection;

namespace InfoHub.ORM.Interfaces
{
    public interface IDatabaseDeployer : IDisposable
    {
        void RunAllScripts(Assembly asm, bool runSilently);
        void DeployClass(Type type);
        void DeployAllClasses(Assembly asm, Type baseType);
        void RunScript(Assembly asm, Type type, bool runSilently);
    }
}