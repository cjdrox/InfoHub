using System.Collections.Generic;
using InfoHub.ORM.Types;

namespace InfoHub.ORM.Interfaces
{
    public interface ITable
    {
        string TableName { get; set; }
        IDictionary<string, ColumnData> ColumnTypes { get; set; }
        ITable WithName(string name);
        ITable WithColumn<T>(string name);
        ITable WithColumn<T>(string name, long length);
        ITable WithColumn<T>(string name, long length, bool isPrimary);
    }
}