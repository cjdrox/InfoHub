using System;
using InfoHub.ORM.Models;
using InfoHub.ORM.Services;

namespace InfoHub.Deployer
{
    public class Program
    {
        static void Main()
        {
            //IDatabaseDeployer deployer = new MySQLDeployerService(Assembly.Load("InfoHub.Entity"));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Deployment started!\n");

            //deployer.DeployAllClasses(null);
            //Console.WriteLine();

            //deployer.RunAllScripts(null);
            //Console.WriteLine();

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                var connector = new MySQLConnector(new Configuration("localhost", "", "3308", "root", ""), true);
                connector.CreateDatabase("blah");
                connector.CreateTable(t => t.WithName("blah").WithColumn<int>("ID"));
                connector.DropDatabase("blah");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Deployment failure: " + e.Message);
                Console.ReadLine();

                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Deployment successfully finished. Press any key to exit...");
            Console.ReadLine();
        }
    }
}
