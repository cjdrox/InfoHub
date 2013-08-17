using System;

namespace InfoHub.ORM.Interfaces
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