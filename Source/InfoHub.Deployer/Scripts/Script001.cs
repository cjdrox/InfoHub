using System;
using InfoHub.Deployer.Attributes;
using InfoHub.Deployer.Models;
using InfoHub.Entity.Entities;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Services;
using Infohub.Repository.Repositories;

namespace InfoHub.Deployer.Scripts
{
    [Script]
    public class Script001 : ScriptBase
    {
        public Script001()
        {
            Author = "cjdrox";
            CreatedAt = DateTime.Now;
        }

        [Execute]
        public override bool Execute(IConfiguration configuration)
        {
            Configuration = configuration;

            var connector = new MySQLConnector(Configuration);
            connector.SwitchDatabase("blah");

            var adminUser = new SystemUser
                                {
                                    Username = "admin",
                                    Passhash = "test",
                                    IsDeleted = false,
                                    CreatedAt = DateTime.Now,
                                    ModifiedAt = DateTime.Now,
                                };

            var repository = new SystemUserRepository(Configuration);

            try
            {
                repository.Add(adminUser);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        [RollBack]
        public override bool RollBack()
        {
            return false;
        }
    }
}
