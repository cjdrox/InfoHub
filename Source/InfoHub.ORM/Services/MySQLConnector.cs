using System;
using System.Linq;
using InfoHub.ORM.Helpers;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using InfoHub.ORM.Types;
using MySql.Data.MySqlClient;

namespace InfoHub.ORM.Services
{
    public class MySQLConnector : IDatabaseConnector
    {
        private IConfiguration _configuration;
        private readonly MySqlConnection _connection;
        private readonly bool _showSql;

        public MySQLConnector(IConfiguration configuration, bool showSql = false)
        {
            if (!configuration.IsValid)
            {
                throw new ArgumentException("Configuration was not valid", "configuration");
            }

            _configuration = configuration;
            _showSql = showSql;

            var connectionString = "SERVER=" + _configuration.Host + ";" 
                + "DATABASE=" +_configuration.Database + ";" 
                + "UID=" + _configuration.Username + ";" 
                + "PASSWORD=" + _configuration.Password + ";";

            _connection = new MySqlConnection(connectionString);
        }

        private ConnectionResult OpenConnection()
        {
            try
            {
                _connection.Open();
                return ConnectionResult.Success;
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        return ConnectionResult.CannotConnect;
                    case 1045:
                        return ConnectionResult.InvalidCredentials;
                }
            }
            return ConnectionResult.UnknownFailure;
        }

        private void CloseConnection()
        {
            try
            {
                _connection.Close();
            }
            catch (MySqlException)
            {
            }
        }

        public bool Query(string query)
        {
            //open connection
            if (OpenConnection() != ConnectionResult.Success) return false;

            //create command and assign the query and connection from the constructor
            var cmd = new MySqlCommand(query, _connection);
            var q = query.ToUpper();

            if (_showSql)
            {
                Console.WriteLine(query);
            }

            //Execute command
            if (q.Contains("SELECT"))
            {
                cmd.ExecuteReader();
            }
            else if (q.Contains("COUNT") || q.Contains("SUM") || q.Contains("AVEARGE") || q.Contains("MIN") || q.Contains("MAX"))
            {
                cmd.ExecuteScalar();
            }
            else
            {
                cmd.ExecuteNonQuery();
            }

            //close connection
            CloseConnection();
            return true;
        }

        public bool CreateDatabase(string name, bool useDatabase = true)
        {
            var query = "CREATE DATABASE " + name + ";\n";

            if (Query(query))
            {
                Query("USE " + name + ";\n");
                _configuration = new Configuration(_configuration.Host, name, _configuration.Port, 
                    _configuration.Username, _configuration.Password);
                return true;
            }

            return false;
        }

        public bool DropDatabase(string name)
        {
            var query = "DROP DATABASE " + name + ";\n";

            return Query(query);
        }

        public bool CreateTable(ITable source)
        {
            var query = String.Format("CREATE TABLE `{0}` (", source.Name);

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

            return Query(query);
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
