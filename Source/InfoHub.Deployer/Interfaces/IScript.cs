using System;

namespace InfoHub.Deployer.Interfaces
{
    public interface IScript : IDisposable
    {
        string Author { get; set; }
        DateTime CreatedAt { get; set; }
        bool Execute();
        bool RollBack();
    }
}