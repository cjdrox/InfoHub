using System;
using System.Reflection;
using InfoHub.Business.Interfaces;

namespace InfoHub.Deployer.Interfaces
{
    public interface IDatabaseDeployer : IService<string>
    {
        void RunAllScripts(Assembly asm, bool runSilently);
        void DeployClass(Type type);
        void DeployAllClasses(Assembly asm);
        void RunScript(Assembly asm, Type type, bool runSilently);
    }
}