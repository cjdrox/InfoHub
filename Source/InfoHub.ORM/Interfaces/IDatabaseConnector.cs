using System;

namespace InfoHub.ORM.Interfaces
{
    public interface IDatabaseConnector : IDisposable
    {
        bool Query(string query);
        bool CreateDatabase(string name, bool useDatabase);
        bool DropDatabase(string name);
        bool CreateTable(Func<ITable,ITable> table);
    }
}