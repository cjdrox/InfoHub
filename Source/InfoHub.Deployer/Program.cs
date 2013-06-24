using System;
using System.Linq;
using System.Reflection;
using InfoHub.Deployer.Services;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using InfoHub.ORM.Services;
using IDatabaseDeployer = InfoHub.Deployer.Interfaces.IDatabaseDeployer;

namespace InfoHub.Deployer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var runAllScripts = args.Select(arg=>arg.ToLower()).Contains("/runscripts");

            IConfiguration configuration = new Configuration("localhost", "", "3308", "root", "");
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Deployment started!\n");

            using (var adapter = new MySQLAdapter(configuration, true))
            {
                adapter.DropDatabase("blah", true);
                adapter.CreateDatabase("blah");
                configuration.Database = "blah";
            }
            
            Console.ForegroundColor = ConsoleColor.Yellow;

            using(IDatabaseDeployer deployer = new MySQLDeployerService(Assembly.Load("InfoHub.Entity"), configuration))
            {
                deployer.DeployAllClasses(null);
                deployer.RunAllScripts(Assembly.GetExecutingAssembly(), runAllScripts);
            }
            
            #region Fluent table creation

            //try
            //{
            //    Console.ForegroundColor = ConsoleColor.Yellow;
            //    var connector = new MySQLAdapter(new Configuration("localhost", "", "3308", "root", ""), true);
            //    connector.CreateDatabase("blah");
            //    connector.CreateTable(t => t.WithName("blah").WithColumn<int>("ID"));
            //    connector.DropDatabase("blah");
            //}
            //catch (Exception e)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine("Deployment failure: " + e.Message);
            //    Console.ReadLine();

            //    return;
            //}

            #endregion

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Deployment successfully finished. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
