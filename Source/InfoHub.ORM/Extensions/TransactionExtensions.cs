using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using InfoHub.ORM.Helpers;
using InfoHub.ORM.Interfaces;
using MySql.Data.MySqlClient;

namespace InfoHub.ORM.Extensions
{
    public static class TransactionExtensions
    {
        public static bool Query(this IDbTransaction transaction, string query, bool showSql = false)
        {
            //create command and assign the query and connection from the constructor
            var cmd = new MySqlCommand(query, (MySqlConnection) transaction.Connection);
            return transaction.Query(cmd, showSql);
        }

        public static bool Query(this IDbTransaction transaction, IDbCommand command, bool showSql = false)
        {
            var query = command.CommandText;
            var q = query.ToUpper();

            if (showSql)
            {
                Console.WriteLine(query);
            }

            //Execute command
            if (q.Contains("SELECT"))
            {
                command.ExecuteReader();
            }
            else if (q.Contains("COUNT") || q.Contains("SUM") || q.Contains("AVEARGE") || q.Contains("MIN")
                || q.Contains("MAX"))
            {
                command.ExecuteScalar();
            }
            else
            {
                command.ExecuteNonQuery();
            }

            return true;
        }


        public static IList<T> Query<T>(this IDbTransaction transaction, string query, bool showSql = true) where T : new()
        {
            var cmd = new MySqlCommand(query, (MySqlConnection) transaction.Connection);
            var reader = cmd.ExecuteReader();

            IList <T> list = new List<T>();

            while (reader.Read())
            {
                var item = new T();
                var t = item.GetType();

                foreach (var property in t.GetProperties())
                {
                    var type = property.PropertyType;
                    var readerValue = string.Empty;

                    if (reader[property.Name] != DBNull.Value)
                    {
                        readerValue = reader[property.Name].ToString();
                    }

                    if (!string.IsNullOrEmpty(readerValue))
                    {
                        property.SetValue(item, readerValue.ToType(type), null);
                    }

                }

                list.Add(item);
            }

            return list;
        }

        public static ITable Insert(this IDbTransaction transaction, ITable table)
        {
            var settings = (IDictionary<string, object>)table.ToExpando();
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();
            var counter = 0;

            var insertCommand = new MySqlCommand
                                    {
                                        CommandText = table.CreateInsertCommand(),
                                        Connection = (MySqlConnection) transaction.Connection,
                                        Transaction = (MySqlTransaction) transaction
                                    };

            

            settings = settings
                .Where(p => !p.Key.ToLower().Equals("name")
                            && !p.Key.ToLower().Equals("schema")
                            && !p.Key.ToLower().Equals("prototype")
                            && !p.Key.ToLower().Equals("tablename")
                            && !p.Key.ToLower().Equals("primarykeyfield")
                            && !p.Key.ToLower().Equals("columntypes")
                            && !p.Key.ToLower().Equals("factory")
                            
                ).ToDictionary(r => r.Key, r => r.Value);

            foreach (var item in settings)
            {
                sbKeys.AppendFormat("`{0}`,", item.Key);
                sbVals.AppendFormat("@{0},", counter);
                insertCommand.AddParam(item.Value);
                counter++;
            }

            return transaction.Query(insertCommand, true) ? table : null;
        }

        public static string CreateInsertCommand(this ITable table)
        {
            var expando = table.ToExpando();
            var properties = (IDictionary<string, object>)expando;
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();
            string result;

            const string stub = "INSERT INTO {0} ({1}) \r\n VALUES ({2})";
            var counter = 0;

            properties = properties
                .Where(p => !p.Key.ToLower().Equals("name")
                            && !p.Key.ToLower().Equals("schema")
                            && !p.Key.ToLower().Equals("prototype")
                            && !p.Key.ToLower().Equals("tablename")
                            && !p.Key.ToLower().Equals("primarykeyfield")
                            && !p.Key.ToLower().Equals("columntypes")
                            && !p.Key.ToLower().Equals("factory")
                ).ToDictionary(r => r.Key, r => r.Value);

            foreach (var property in properties)
            {
                sbKeys.AppendFormat("`{0}`,", property.Key);
                sbVals.AppendFormat("@{0},", counter);
                counter++;
            }

            if (counter > 0)
            {
                var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
                var values = sbVals.ToString().Substring(0, sbVals.Length - 1);
                var sql = string.Format(stub, table.TableName, keys, values);

                result = sql;
            }
            else
            {
                throw new InvalidOperationException("Can't parse this object to the database - there are no properties set");
            }

            return result;
        }
    }
}
