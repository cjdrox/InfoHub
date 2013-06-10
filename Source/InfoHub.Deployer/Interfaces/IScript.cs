using System;
using InfoHub.ORM.Interfaces;

namespace InfoHub.Deployer.Interfaces
{
    public interface IScript : IDisposable
    {
        IConfiguration Configuration { get; set; }
        string Author { get; set; }
        DateTime CreatedAt { get; set; }
        bool Execute(IConfiguration configuration);
        bool RollBack();
    }
}