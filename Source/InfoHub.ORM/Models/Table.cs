﻿using System.Collections.Generic;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Types;

namespace InfoHub.ORM.Models
{
    public class Table : ITable
    {
        public string Name { get; set; }
        public IDictionary<string, ColumnData> ColumnTypes { get; set; }

        public Table(string name)
        {
            Name = name;
            ColumnTypes = new Dictionary<string, ColumnData>();
        }

        public Table() : this(null)
        {
        }

        public ITable WithName(string name)
        {
            Name = name;
            return this;
        }

        public ITable WithColumn<T>(string name)
        {
            ColumnTypes.Add(name, new ColumnData{Type = typeof (T)});
            return this;
        }

        public ITable WithColumn<T>(string name, long length)
        {
            ColumnTypes.Add(name, new ColumnData { Type = typeof(T), Length = length});
            return this;
        }

        public ITable WithColumn<T>(string name, long length, bool isPrimary)
        {
            ColumnTypes.Add(name, new ColumnData { Type = typeof(T), Length = length, IsPrimary = isPrimary});
            return this;
        }
    }
}
