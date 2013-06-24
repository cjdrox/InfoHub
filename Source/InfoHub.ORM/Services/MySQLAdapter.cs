using System;
using System.Data;
using System.Linq;
using InfoHub.ORM.Helpers;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using MySql.Data.MySqlClient;

namespace InfoHub.ORM.Services
{
    public class MySQLAdapter : IDatabaseAdapter
    {
        private IConfiguration _configuration;
        private readonly MySqlConnection _connection;
        private readonly bool _showSql;

        public MySQLAdapter(IConfiguration configuration, bool showSql = false, bool open = true)
        {
            if (!configuration.IsValid)
            {
                throw new ArgumentException("Configuration was not valid", "configuration");
            }

            _configuration = configuration;
            _showSql = showSql;
            _connection = new MySqlConnection(_configuration.ConnectionString);

            if (open)
            {
                _connection.Open();
            }
        }

        public IDbConnection OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            
            return _connection;
        }

        public IDbConnection CloseConnection()
        {
            _connection.Close();
            return _connection;
        }
        
        public bool CreateDatabase(string name, bool useDatabase = true)
        {
            var query = "CREATE DATABASE " + name + ";\n";

            if (_connection.Query(query, _showSql) && useDatabase)
            {
                return SwitchDatabase(name);
            }

            return false;
        }

        public bool SwitchDatabase(string name)
        {
            var wasNotOpen = false;
            if (_connection.State != ConnectionState.Open)
            {
                wasNotOpen = true;
                _connection.Open();
            }
            
            _connection.Query("USE " + name + ";\n", _showSql);
            _configuration = new Configuration(_configuration.Host, name, _configuration.Port,
                _configuration.Username, _configuration.Password);

            if (wasNotOpen)
            {
                _connection.Close();
            }
            
            return true;
        }


        public bool DropDatabase(string name, bool checkExistence)
        {
            var query = "DROP DATABASE " + ( checkExistence ? "IF EXISTS " : "") + name + ";\n";

            return _connection.Query(query, _showSql);
        }

        public bool CreateTable(ITable source)
        {
            var query = String.Format("CREATE TABLE `{0}` (", source.TableName);

            foreach (var columnType in source.ColumnTypes)
            {
                var name = columnType.Key;
                var typeName = columnType.Value.Type.Name.ToMySQL();
                var length = columnType.Value.Length;
                var notNull = columnType.Value.NotNull;
                var defaultValue = columnType.Value.DefaultValue;

                query = String.Concat(query, String.Format("\n  {0} {1}", name, typeName));
                query = String.Concat(query, length > 0 ? String.Format("({0}) ", length) : "");
                query = String.Concat(query, (notNull ? "NOT NULL " : ""));
                query = String.Concat(query, (defaultValue != null ? String.Format("DEFAULT {0} ", defaultValue) : ""));

                if (!columnType.Equals(source.ColumnTypes.Last()))
                {
                    query = String.Concat(query, ",");
                }
            }

            query = String.Concat(query, "\n);\n");

            return _connection.Query(query, _showSql);
        }

        public bool CreateTable(Func<ITable, ITable> table)
        {
            var source = table(new Table());
            return CreateTable(source);
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
