using System;
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
        static void Main()
        {
            IConfiguration configuration = new Configuration("localhost", "", "3308", "root", "");
            IDatabaseDeployer deployer = new MySQLDeployerService(Assembly.Load("InfoHub.Entity"), configuration);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Deployment started!\n");

            var connector = new MySQLConnector(configuration, true);
            connector.SwitchDatabase("blah");

            Console.ForegroundColor = ConsoleColor.Yellow;

            deployer.DeployAllClasses(null);
            deployer.RunAllScripts(null);

            #region Fluent table creation

            //try
            //{
            //    Console.ForegroundColor = ConsoleColor.Yellow;
            //    var connector = new MySQLConnector(new Configuration("localhost", "", "3308", "root", ""), true);
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
