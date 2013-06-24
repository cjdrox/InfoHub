using System;
using System.Data;

namespace InfoHub.ORM.Interfaces
{
    public interface IDatabaseAdapter : IDisposable
    {
        IDbConnection OpenConnection();
        IDbConnection CloseConnection();
        bool CreateDatabase(string name, bool useDatabase);
        bool DropDatabase(string name, bool checkExistence );
        bool CreateTable(Func<ITable,ITable> table);
        bool SwitchDatabase(string name);
    }
}