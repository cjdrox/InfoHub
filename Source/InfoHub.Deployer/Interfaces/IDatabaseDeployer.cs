using System;
using System.Reflection;
using InfoHub.Business.Interfaces;
using InfoHub.Entity.Models;

namespace InfoHub.Deployer.Interfaces
{
    public interface IDatabaseDeployer : IService<string>
    {
        void RunAllScripts(Assembly asm);
        void DeployClass(BaseEntity type);
        void DeployAllClasses(Assembly asm);
        void RunScript(Assembly asm, Type type);
    }
}