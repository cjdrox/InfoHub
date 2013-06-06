using System;
using InfoHub.Deployer.Attributes;
using InfoHub.Deployer.Models;
using InfoHub.Entity.Entities;
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
        public override bool Execute()
        {
            var adminUser = new SystemUser{ Username = "admin", Passhash = "test"};
            var repository = new SystemUserRepository();

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
