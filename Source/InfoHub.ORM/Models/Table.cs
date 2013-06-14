using System.Collections.Generic;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Types;

namespace InfoHub.ORM.Models
{
    public class Table : TransactedModel, ITable
    {
        public IDictionary<string, ColumnData> ColumnTypes { get; set; }

        public Table(string name)
        {
            TableName = name;
            ColumnTypes = new Dictionary<string, ColumnData>();
        }

        public Table()
        {
            ColumnTypes = new Dictionary<string, ColumnData>();
        }

        #region Fluent Interface

        public ITable WithName(string name)
        {
            TableName = name;
            return this;
        }

        public ITable WithColumn<T>(string name)
        {
            ColumnTypes.Add(name, new ColumnData {Type = typeof (T)});
            return this;
        }

        public ITable WithColumn<T>(string name, long length)
        {
            ColumnTypes.Add(name, new ColumnData {Type = typeof (T), Length = length});
            return this;
        }

        public ITable WithColumn<T>(string name, long length, bool isPrimary)
        {
            ColumnTypes.Add(name, new ColumnData {Type = typeof (T), Length = length, IsPrimary = isPrimary});
            return this;
        }

        #endregion

    }
}
