using System;
using System.Data;
using System.Linq;
using InfoHub.ORM.Extensions;
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

            using(var transaction = _connection.BeginTransaction())
            {
                if (transaction.Query(query, _showSql) && useDatabase)
                {
                    return SwitchDatabase(name);
                }
                transaction.Commit();
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
            
            using(var transaction = _connection.BeginTransaction())
            {
                transaction.Query("USE " + name + ";\n", _showSql);
                transaction.Commit();
            }
            
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
            bool result;

            using(var transaction = _connection.BeginTransaction())
            {
                result = transaction.Query(query, _showSql);
                transaction.Commit();
            }

            return result;
        }

        public bool CreateTable(ITable source)
        {
            var query = String.Format("CREATE TABLE `{0}` (", source.TableName);
            bool result;

            foreach (var columnType in source.ColumnTypes)
            {
                var name = columnType.Key;
                var type = columnType.Value.Type;

                var implicitlyNull = type.IsImplicitlyNullable();
                var isEnumerable = type.IsEnumerable();
                var primary = columnType.Value.IsPrimary;

                var typeName = implicitlyNull ? Nullable.GetUnderlyingType(type).Name.ToMySQL() : type.Name.ToMySQL();
                
                //Chameera: TODO: Nullability error solved. Now onto collections.
                if (isEnumerable)
                {
                    var refType = type.GetGenericArguments()[0];
                    var refImplicitlyNull = refType.IsGenericType && refType.GetGenericTypeDefinition() == typeof(Nullable<>);
                    var refTypeName = refImplicitlyNull ? Nullable.GetUnderlyingType(refType).Name.ToMySQL() : refType.Name.ToMySQL();
                    query = String.Concat(query, String.Format("\n  `{0}_Id` {1},", refTypeName, "VARCHAR(255)"));
                    query = String.Concat(query, String.Format("\n  FOREIGN KEY(`{0}_Id`) REFERENCES {1}(Id) ON UPDATE CASCADE ON DELETE RESTRICT,", 
                        refTypeName, refTypeName));
                    continue;
                }

                var length = columnType.Value.Length;
                var notNull = columnType.Value.NotNull && !implicitlyNull;
                var defaultValue = columnType.Value.DefaultValue;

                query = String.Concat(query, String.Format("\n  `{0}` {1}", name, typeName));
                query = String.Concat(query, length > 0 ? String.Format("({0}) ", length) : "");
                query = String.Concat(query, (notNull ? "NOT NULL " : ""));
                query = String.Concat(query, (primary ? "PRIMARY KEY " : ""));
                query = String.Concat(query, (defaultValue != null ? String.Format("DEFAULT {0} ", defaultValue) : ""));

                if (!columnType.Equals(source.ColumnTypes.Last()))
                {
                    query = String.Concat(query, ",");
                }
            }

            query = String.Concat(query, "\n);\n");

            using(var transaction = _connection.BeginTransaction())
            {
                result = transaction.Query(query, _showSql);
                transaction.Commit();
            }

            return result;
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
