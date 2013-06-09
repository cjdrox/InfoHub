using System;
using System.Linq;
using System.Reflection;
using InfoHub.Business.Models;
using InfoHub.Deployer.Helpers;
using InfoHub.Deployer.Interfaces;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using InfoHub.ORM.Services;
using IDatabaseDeployer = InfoHub.Deployer.Interfaces.IDatabaseDeployer;

namespace InfoHub.Deployer.Services
{
    public class MySQLDeployerService : ServiceBase<string>, IDatabaseDeployer
    {
        private readonly Assembly _assembly;
        private readonly IConfiguration _configuration;
        private readonly MySQLConnector _connector;

        public MySQLDeployerService(Assembly assembly, IConfiguration configuration)
        {
            _assembly = assembly;
            _configuration = configuration;
            _connector = new MySQLConnector(_configuration, true);
        }

        #region Script Runner Code
        public void RunAllScripts(Assembly assembly)
        {
            var asm = (assembly ?? _assembly);
            var types = asm.GetTypes();

            foreach (var type in from type in types
                                 let atts = type.GetCustomAttributes(true)
                                 from att in atts
                                 where att.ToString() == Names.ScriptName
                                       && type.GetMethod(Names.ExecuteName) != null && type.IsClass
                                 orderby type.Name
                                 select type)
            {
                try
                {
                    RunScript(asm, type);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(String.Concat("Execution of script failed! \n", e.Message));
                }
            }
        }

        public void RunScript(Assembly asm, Type type)
        {
            var script = (IScript)asm.CreateInstance(type.FullName);

            if (script == null) return;
            Console.ForegroundColor = ConsoleColor.Yellow;

            string readLine;
            for (readLine = null;
                 readLine == null || !(readLine.ToLower() == "n" || readLine.ToLower() == "y");
                 readLine = Console.ReadLine())
            {
                Console.Write(String.Concat("Execute script: ", script, " by ", script.Author.ToUpper(), "? (Y/N)"));
            }

            if (readLine.ToLower() == "y")
            {
                Console.ForegroundColor = ConsoleColor.White;

                if (script.Execute())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Script {0} successfully executed!", script);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Script {0} did not execute successfully.", script);
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Script " + script + " skipped!");
            }
        }
        #endregion
        
        public void DeployClass(Type type)
        {
            _connector.CreateTable(new Table(type.Name));
        }

        public void DeployAllClasses(Assembly asm)
        {
            asm = asm ?? _assembly;

            if (asm==null)
            {
                throw new ArgumentNullException("asm", "Assembly cannot be null");
            }

            Console.WriteLine("Source assembly: {0}", asm.FullName);
        }
    }
}
