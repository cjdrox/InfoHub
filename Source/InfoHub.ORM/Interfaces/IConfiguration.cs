using System;

namespace InfoHub.ORM.Interfaces
{
    public interface IConfiguration : IDisposable
    {
        string Host { get; set; }
        string Database { get; set; }
        string Port { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool IsValid { get; }
    }
}