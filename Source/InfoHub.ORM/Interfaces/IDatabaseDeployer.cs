using System;
using System.Reflection;
using InfoHub.ORM.Models;

namespace InfoHub.ORM.Interfaces
{
    public interface IDatabaseDeployer
    {
        void RunAllScripts(Assembly asm);
        void DeployClass(Configuration cfg, Type type);
        void DeployAllClasses(Assembly asm);
        void RunScript(Assembly asm, Type type);
    }
}