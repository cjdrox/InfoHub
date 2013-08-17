using System;
using System.Linq;
using System.Reflection;
using InfoHub.ORM.Attributes;
using InfoHub.ORM.Helpers;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using InfoHub.ORM.Types;

namespace InfoHub.ORM.Services
{
    public class MySQLDeployerService : IDatabaseDeployer
    {
        private readonly Assembly _assembly;
        private readonly IConfiguration _configuration;
        private readonly MySQLAdapter _adapter;

        public MySQLDeployerService(Assembly assembly, IConfiguration configuration)
        {
            _assembly = assembly;
            _configuration = configuration;
            _adapter = new MySQLAdapter(_configuration, true);
        }

        #region Script Runner Code
        public void RunAllScripts(Assembly assembly, bool runSilently = false)
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
                    RunScript(asm, type, runSilently);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(String.Concat("Execution of script failed! \n", e.Message));
                }
            }
        }

        public void RunScript(Assembly asm, Type type, bool runSilently = false)
        {
            var script = (IScript)asm.CreateInstance(type.FullName);

            if (script == null) return;
            Console.ForegroundColor = ConsoleColor.Yellow;

            var readLine = String.Empty;
            if (!runSilently)
            {
                for (readLine = null;
                     readLine == null || !(readLine.ToLower() == "n" || readLine.ToLower() == "y");
                     readLine = Console.ReadLine())
                {
                    Console.Write(String.Concat("Execute script: ", script, " by ", script.Author.ToUpper(), "? (Y/N)"));
                }   
            }
            
            if (runSilently || readLine.ToLower() == "y")
            {
                Console.ForegroundColor = ConsoleColor.White;

                if (script.Execute(_configuration))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Script {0} by {1} successfully executed!", script, script.Author);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Script {0} by {1} did not execute successfully.", script, script.Author);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Script {0} by {1} skipped!", script, script.Author);
            }
        }
        #endregion

        #region Class Deployer Code

        public void DeployClass(Type type)
        {
            ITable table = new Table(type.Name);
            var properties = type.GetProperties().Where(prop=>!Attribute.IsDefined(prop, typeof(UnmappedAttribute))).ToList();
            var primary = properties.Where(prop => Attribute.IsDefined(prop, typeof (PrimaryKeyAttribute))).ToList();
            var primaryColumnName = primary.Any() ? primary.First().Name : String.Empty;

            table.ColumnTypes = properties
                .Where(p => !p.Name.ToLower().Equals("name")
                            && !p.Name.ToLower().Equals("schema")
                            && !p.Name.ToLower().Equals("prototype")
                            && !p.Name.ToLower().Equals("tablename")
                            && !p.Name.ToLower().Equals("primarykeyfield")
                            && !p.Name.ToLower().Equals("columntypes")
                            && !p.Name.ToLower().Equals("factory")
                            && !p.Name.ToLower().Equals("connection"))
                .ToDictionary(r => r.Name, r => new ColumnData
                                                    {
                                                        Type = r.PropertyType,
                                                        IsPrimary = r.Name.Equals(primaryColumnName)
                                                    });

            _adapter.CreateTable(table);
            Console.WriteLine();
        }

        public void DeployAllClasses(Assembly asm = null, Type baseType = null)
        {
            asm = asm ?? _assembly;

            if (asm == null)
            {
                throw new ArgumentNullException("asm", "Assembly cannot be null");
            }

            Console.WriteLine("Deploying from source assembly:\n {0}", asm.FullName);
            Console.WriteLine();

            var types = asm.GetTypes();

            if (baseType!=null)
            {
                types = types.Where(t => baseType.IsAssignableFrom(t) && !t.Name.Equals(baseType.Name)).ToArray();
            }
            
            foreach (var type in types)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    DeployClass(type);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = (e is ArgumentException
                                                   ? ConsoleColor.White
                                                   : Console.ForegroundColor = ConsoleColor.Red);

                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                }
            }
        }

        #endregion

        public void Dispose()
        {
            _adapter.CloseConnection();
        }
    }
}
