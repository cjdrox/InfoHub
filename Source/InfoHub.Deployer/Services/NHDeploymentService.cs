using System;
using System.Linq;
using System.Reflection;
using InfoHub.Business.Models;
using InfoHub.Deployer.Helpers;
using InfoHub.Deployer.Interfaces;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace InfoHub.Deployer.Services
{
    public class NHDeploymentService : ServiceBase<string>, IDatabaseDeployer
    {
        public virtual void RunAllScripts(Assembly assembly = null)
        {
            var asm = (assembly ?? Assembly.GetExecutingAssembly());
            var types = asm.GetTypes();

            foreach (var type in from type in types 
                let atts = type.GetCustomAttributes(true) from att in atts
                where att.ToString() == Names.ScriptName && type.GetMethod(Names.ExecuteName) != null && type.IsClass
                orderby type.Name select type)
            {
                try
                {
                    RunScript(asm, type);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(String.Concat("Execution of script failed! \n" , e.Message));
                }
            }
        }

        public virtual void RunScript(Assembly asm, Type type)
        {
            var script = (IScript) asm.CreateInstance(type.FullName);

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

        public virtual void DeployAllClasses(Assembly assembly)
        {
            var asm = assembly ?? Assembly.Load("InfoHub.Entity");

            Console.WriteLine("Source assembly: {0}", asm.FullName);
            
            var cfg = new Configuration();
            cfg.Configure(@"D:\Projects\Visual Studio\InfoHub\Source\InfoHub.Deployer\bin\Debug\hibernate.cfg.xml");
            cfg.SetDefaultAssembly(asm.FullName);
            //cfg.AddAssembly(asm);

            #region Add manually

            //var types = asm.GetTypes().Where(t => t.IsClass);
            //foreach (var type in from type in types
            //                     where type.IsClass
            //                        && !type.IsSubclassOf(typeof(Attribute))
            //                        && !type.IsSubclassOf(typeof(Enum))
            //                     let atts = type.GetCustomAttributes(false)
            //                     from att in atts
            //                     where att.ToString() == "Table"
            //                     select type)
            //{
            //    DeployClass(cfg, type);
            //    //cfg.SetDefaultAssembly(type.Assembly.FullName);
            //    //cfg.SetDefaultNamespace(type.Namespace);
            //}

            #endregion
            
            try
            {
                cfg.BuildSessionFactory();
                new SchemaExport(cfg).Execute(false, false, false);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not deploy entity classes");
                Console.WriteLine(e.Message);
            }
            
        }

        public virtual void DeployClass(Configuration cfg, Type type)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(String.Concat("Deploying entity class: ", type.Name));

            try
            {
                cfg.AddResource("SystemUser.hbm.xml", type.Assembly);
                //cfg.SetDefaultAssembly(type.Assembly.FullName);
                //cfg.SetDefaultNamespace(type.Namespace);
                //cfg.AddClass(type);
                //cfg.AddFile("SystemUser.hbm.xml");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(String.Concat("Successfuly added entity class: ", type.Name));
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(String.Concat("Could not add entity class: ", type.Name));
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
