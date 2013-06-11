using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoHub.ORM.Interfaces;

namespace InfoHub.ORM.Models
{
    /// <summary>
    /// A class that wraps your database table in Dynamic Funtime
    /// </summary>
    public class DynamicModel : DynamicObject, IDynamicModel
    {
        private IConfiguration _configuration;
        const string ProviderName = "MySql.Data.MySqlClient";
        public DbProviderFactory Factory { get; protected set; }

        public static  DynamicModel Open(IConfiguration configuration)
        {
            dynamic dm = new DynamicModel(configuration, null);
            return dm;
        }

        public DynamicModel(IConfiguration configuration, ITable table)
        {
            _configuration = configuration;
            TableName = table.TableName;
            PrimaryKeyField = table.ColumnTypes.FirstOrDefault(column => column.Value.IsPrimary).ToString();

            SetFactory();
        }

        private void SetFactory()
        {
            try
            {
                Factory = DbProviderFactories.GetFactory(ProviderName);
            }
            catch (FileLoadException ex)
            {
                throw new MassiveException(
                    string.Format(
                        "Could not load the specified provider: {0}. Have you added a reference to the correct assembly?",
                        ProviderName), ex);
            }
            catch (ArgumentException e)
            {
                var foundClasses = "I did find these Factories:";
                var dt = DbProviderFactories.GetFactoryClasses();
                for (var i = 0; i < dt.Rows.Count; i++)
                    foundClasses += String.Format("|{0}|", dt.Rows[i][2]);

                throw new ArgumentException(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, foundClasses));
            }
        }

        protected DynamicModel()
        {
            SetFactory();
        }

        /// <summary>
        /// Creates a new Expando from a Form POST - white listed against the columns in the DB
        /// </summary>
        public dynamic CreateFrom(NameValueCollection coll)
        {
            dynamic result = new ExpandoObject();
            var dc = (IDictionary<string, object>)result;
            var schema = Schema.ToList();

            foreach (var item in coll.Keys)
            {
                var exists = schema.Any(x => x.COLUMN_NAME.ToLower() == item.ToString().ToLower());

                if (!exists) continue;

                var key = item.ToString();
                var val = coll[key];
                if (!String.IsNullOrEmpty(val))
                {
                    //what to do here? If it's empty... set it to NULL?
                    //if it's a string value - let it go through if it's NULLABLE?
                    //Empty? WTF?
                    dc.Add(key, val);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a default value for the column
        /// </summary>
        public dynamic DefaultValue(dynamic column)
        {
            dynamic result;

            var defaultValue = column.COLUMN_DEFAULT;
            if (string.IsNullOrEmpty(defaultValue))
            {
                result = null;
            }
            else if (defaultValue == "getdate()" || defaultValue == "(getdate())")
            {
                result = DateTime.Now.ToShortDateString();
            }
            else if (defaultValue == "newid()")
            {
                result = Guid.NewGuid().ToString();
            }
            else
            {
                result = defaultValue.Replace("(", "").Replace(")", "");
            }

            return result;
        }

        /// <summary>
        /// Creates an empty Expando set with defaults from the DB
        /// </summary>
        public dynamic Prototype
        {
            get
            {
                dynamic result = new ExpandoObject();
                var schema = Schema;
                foreach (var column in schema)
                {
                    var dc = (IDictionary<string, object>)result;
                    dc.Add(column.COLUMN_NAME, DefaultValue(column));
                }
                result._Table = this;
                return result;
            }
        }
        /// <summary>
        /// List out all the schema bits for use with ... whatever
        /// </summary>
        IEnumerable<dynamic> _schema;

        public IEnumerable<dynamic> Schema
        {
            get
            {
                return _schema ??
                       (_schema = Query("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @0", TableName));
            }
        }

        /// <summary>
        /// Enumerates the reader yielding the result - thanks to Jeroen Haegebaert
        /// </summary>
        public object QueryScalar(string sql, params object[] args)
        {
            using (var conn = OpenConnection())
            {
                var rdr = CreateCommand(sql, conn, args).ExecuteScalar();
                return rdr;
            }
        }

        /// <summary>
        /// Enumerates the reader yielding the result - thanks to Jeroen Haegebaert
        /// </summary>
        public IEnumerable<dynamic> Query(string sql, params object[] args)
        {
            using (var conn = OpenConnection())
            {
                var rdr = CreateCommand(sql, conn, args).ExecuteReader();
                while (rdr.Read())
                {
                    yield return rdr.RecordToExpando();
                }
            }
        }
        /// <summary>
        /// Executes the reader using SQL async API - thanks to Damian Edwards
        /// </summary>
        public void QueryAsync(string sql, Action<List<dynamic>> callback, params object[] args)
        {
            const string connectionString = "";
            using (var conn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.AddParams(args);
                conn.Open();

                var task = Task.Factory.FromAsync<IDataReader>(cmd.BeginExecuteReader, cmd.EndExecuteReader, null);
                task.ContinueWith(x => callback.Invoke(x.Result.ToExpandoList()));
            }
        }

        public IEnumerable<dynamic> Query(string sql, DbConnection connection, params object[] args)
        {
            using (var rdr = CreateCommand(sql, connection, args).ExecuteReader())
            {
                while (rdr.Read())
                {
                    yield return rdr.RecordToExpando();
                }
            }
        }
        /// <summary>
        /// Returns a single result
        /// </summary>
        public object Scalar(string sql, params object[] args)
        {
            object result;
            using (var conn = OpenConnection())
            {
                result = CreateCommand(sql, conn, args).ExecuteScalar();
            }
            return result;
        }

        /// <summary>
        /// Creates a DBCommand that you can use for loving your database.
        /// </summary>
        private DbCommand CreateCommand(string sql, DbConnection conn, params object[] args)
        {
            using (var result = Factory.CreateCommand())
            {
                if (result != null)
                {
                    result.Connection = conn;
                    result.CommandText = sql;
                    if (args.Length > 0)
                        result.AddParams(args);
                    return result;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns and OpenConnection
        /// </summary>
        public DbConnection OpenConnection(IConfiguration configuration = null)
        {
            var result = Factory.CreateConnection();
            configuration = configuration ?? _configuration;

            if (result != null)
            {
                result.ConnectionString = configuration.ConnectionString;
                result.Open();

                if (_configuration==null)
                {
                    _configuration = configuration;
                }
                return result;
            }

            return null;
        }

        /// <summary>
        /// Builds a set of Insert and Update commands based on the passed-on objects.
        /// These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects
        /// With a PK property (whatever PrimaryKeyField is set to) will be created at UPDATEs
        /// </summary>
        public List<DbCommand> BuildCommands(params object[] things)
        {
            return things.Select(item => HasPrimaryKey(item) 
                                             ? CreateUpdateCommand(item, GetPrimaryKey(item)) 
                                             : CreateInsertCommand(item)).ToList();
        }

        /// <summary>
        /// Executes a set of objects as Insert or Update commands based on their property settings, within a transaction.
        /// These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects
        /// With a PK property (whatever PrimaryKeyField is set to) will be created at UPDATEs
        /// </summary>
        public int Save(params object[] things)
        {
            var commands = BuildCommands(things);
            return Execute(commands);
        }

        /// <summary>
        /// Execute a single command
        /// </summary>
        public int Execute(DbCommand command)
        {
            return Execute(new[] { command });
        }

        public int Execute(string sql, params object[] args)
        {
            return Execute(CreateCommand(sql, null, args));
        }
        /// <summary>
        /// Executes a series of DBCommands in a transaction
        /// </summary>
        public virtual int Execute(IEnumerable<DbCommand> commands)
        {
            var result = 0;
            using (var conn = OpenConnection())
            {
                using (var tx = conn.BeginTransaction())
                {
                    foreach (var cmd in commands)
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = tx;
                        result += cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
            }
            return result;
        }
        public string PrimaryKeyField { get; set; }

        /// <summary>
        /// Conventionally introspects the object passed in for a field that 
        /// looks like a PK. If you've named your PrimaryKeyField, this becomes easy
        /// </summary>
        public bool HasPrimaryKey(object o)
        {
            return o.ToDictionary().ContainsKey(PrimaryKeyField);
        }

        /// <summary>
        /// If the object passed in has a property with the same name as your PrimaryKeyField
        /// it is returned here.
        /// </summary>
        public object GetPrimaryKey(object o)
        {
            object result;
            o.ToDictionary().TryGetValue(PrimaryKeyField, out result);
            return result;
        }

        public string TableName { get; set; }

        /// <summary>
        /// Creates a command for use with transactions - internal stuff mostly, but here for you to play with
        /// </summary>
        public DbCommand CreateInsertCommand(object o)
        {
            var expando = o.ToExpando();
            var settings = (IDictionary<string, object>)expando;
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();

            const string stub = "INSERT INTO {0} ({1}) \r\n VALUES ({2})";
            var result = CreateCommand(stub, null);
            var counter = 0;

            settings = settings
                .Where(p => !p.Key.ToLower().Equals("name")
                            && !p.Key.ToLower().Equals("schema")
                            && !p.Key.ToLower().Equals("prototype")
                            && !p.Key.ToLower().Equals("tablename")
                            && !p.Key.ToLower().Equals("primarykeyfield")
                            && !p.Key.ToLower().Equals("columntypes")
                            && !p.Key.ToLower().Equals("factory")
                ).ToDictionary(r=>r.Key, r=>r.Value);

            foreach (var item in settings)
            {
                sbKeys.AppendFormat("`{0}`,", item.Key);
                sbVals.AppendFormat("@{0},", counter);
                result.AddParam(item.Value);
                counter++;
            }

            if (counter > 0)
            {
                var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
                var values = sbVals.ToString().Substring(0, sbVals.Length - 1);
                var sql = string.Format(stub, TableName, keys, values);

                result.CommandText = sql;
            }
            else
            {
                throw new InvalidOperationException("Can't parse this object to the database - there are no properties set");
            }

            return result;
        }

        /// <summary>
        /// Creates a command for use with transactions - internal stuff mostly, but here for you to play with
        /// </summary>
        public DbCommand CreateUpdateCommand(object o, object key)
        {
            var expando = o.ToExpando();
            var settings = (IDictionary<string, object>)expando;
            var sbKeys = new StringBuilder();
            const string stub = "UPDATE {0} SET {1} WHERE {2} = @{3}";

            var result = CreateCommand(stub, null);
            var counter = 0;
            foreach (var item in settings)
            {
                var val = item.Value;
                if (!item.Key.Equals(PrimaryKeyField, StringComparison.CurrentCultureIgnoreCase) && item.Value != null)
                {
                    result.AddParam(val);
                    sbKeys.AppendFormat("{0} = @{1}, \r\n", item.Key, counter);
                    counter++;
                }
            }

            if (counter > 0)
            {
                result.AddParam(key);
                var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 4);//strip the last commas
                result.CommandText = string.Format(stub, TableName, keys, PrimaryKeyField, counter);
            }
            else
            {
                throw new InvalidOperationException("No parsable object was sent in - could not divine any name/value pairs");
            }

            return result;
        }

        /// <summary>
        /// Removes one or more records from the DB according to the passed-in WHERE
        /// </summary>
        public DbCommand CreateDeleteCommand(string where = "", object key = null, params object[] args)
        {
            var sql = string.Format("DELETE FROM {0} ", TableName);
            if (key != null)
            {
                sql += string.Format("WHERE {0}=@0", PrimaryKeyField);
                args = new[] { key };
            }
            else if (!string.IsNullOrEmpty(where))
            {
                sql += where.Trim().StartsWith("where", StringComparison.CurrentCultureIgnoreCase) ? where : "WHERE " + where;
            }

            return CreateCommand(sql, null, args);
        }

        /// <summary>
        /// Adds a record to the database. You can pass in an Anonymous object, an ExpandoObject,
        /// A regular old POCO, or a NameValueColletion from a Request.Form or Request.QueryString
        /// </summary>
        public object Insert(object o)
        {
            dynamic result;
            using (var conn = OpenConnection())
            {
                var cmd = CreateInsertCommand(o);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT @@IDENTITY as newID";
                result = cmd.ExecuteScalar();
            }

            return result;
        }

        /// <summary>
        /// Updates a record in the database. You can pass in an Anonymous object, an ExpandoObject,
        /// A regular old POCO, or a NameValueCollection from a Request.Form or Request.QueryString
        /// </summary>
        public int Update(object o, object key)
        {
            return Execute(CreateUpdateCommand(o, key));
        }

        /// <summary>
        /// Removes one or more records from the DB according to the passed-in WHERE
        /// </summary>
        public int Delete(object key = null, string where = "", params object[] args)
        {
            return Execute(CreateDeleteCommand(where, key, args));
        }

        /// <summary>
        /// Returns all records complying with the passed-in WHERE clause and arguments, 
        /// ordered as specified, limited (TOP) by limit.
        /// </summary>
        public IEnumerable<dynamic> All(string where = "", string orderBy = "", int limit = 0, string columns = "*", params object[] args)
        {
            string sql = BuildSelect(where, orderBy, limit);
            return Query(string.Format(sql, columns, TableName), args);
        }

        /// <summary>
        /// Returns the count from this table, using optional where statement
        /// </summary>
        /// <param name="where">query parameters (eg: "id>=100")</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Count(string where = "", params object[] args)
        {
            var sql = BuildCount(where);
            var ob = QueryScalar(string.Format(sql, TableName), args);
            return int.Parse(ob.ToString());
        }

        private static string BuildCount(string where)
        {
            var sql = "SELECT Count(*) FROM {0} ";
            if (!string.IsNullOrEmpty(where))
                sql += where.Trim().StartsWith("where", StringComparison.CurrentCultureIgnoreCase) ? where : "WHERE " + where;
            return sql;
        }

        private static string BuildSelect(string where, string orderBy, int limit)
        {
            var sql = "SELECT {0} FROM {1} ";
            if (!string.IsNullOrEmpty(where))
                sql += where.Trim().StartsWith("where", StringComparison.CurrentCultureIgnoreCase) ? where : "WHERE " + where;

            if (!string.IsNullOrEmpty(orderBy))
                sql += orderBy.Trim().StartsWith("order by", StringComparison.CurrentCultureIgnoreCase) ? orderBy : " ORDER BY " + orderBy;

            if (limit > 0)
                sql += " LIMIT " + limit;

            return sql;
        }

        /// <summary>
        /// Returns all records complying with the passed-in WHERE clause and arguments, 
        /// ordered as specified, limited (TOP) by limit.
        /// </summary>
        public void AllAsync(Action<List<dynamic>> callback, string where = "", string orderBy = "", int limit = 0, string columns = "*", params object[] args)
        {
            var sql = BuildSelect(where, orderBy, limit);
            QueryAsync(string.Format(sql, columns, TableName), callback, args);
        }

        /// <summary>
        /// Returns a dynamic PagedResult. Result properties are Items, TotalPages, and TotalRecords.
        /// </summary>
        public dynamic Paged(string where = "", string orderBy = "", string columns = "*", int pageSize = 20, int currentPage = 1, params object[] args)
        {
            dynamic result = new ExpandoObject();
            var countQuery = string.Format("SELECT COUNT({0}) FROM {1}", PrimaryKeyField, TableName);

            if (string.IsNullOrEmpty(orderBy))
                orderBy = PrimaryKeyField;

            if (!string.IsNullOrEmpty(where))
            {
                if (!where.Trim().StartsWith("where", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = "WHERE " + where;
                }
            }

            var sql = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS Row, {0} FROM {2} {3}) AS Paged ", columns, orderBy, TableName, where);
            var pageStart = (currentPage - 1) * pageSize;
            sql += string.Format(" WHERE Row > {0} AND Row <={1}", pageStart, (pageStart + pageSize));
            countQuery += where;
            result.TotalRecords = Scalar(countQuery, args);
            result.TotalPages = result.TotalRecords / pageSize;
            if (result.TotalRecords % pageSize > 0)
                result.TotalPages += 1;
            result.Items = Query(string.Format(sql, columns, TableName), args);
            return result;
        }

        /// <summary>
        /// Returns a single row from the database
        /// </summary>
        public dynamic Single(string where, params object[] args)
        {
            var sql = string.Format("SELECT * FROM {0} WHERE {1}", TableName, where);
            return Query(sql, args).FirstOrDefault();
        }

        /// <summary>
        /// Returns a single row from the database
        /// </summary>
        public dynamic Single(object key, string columns = "*")
        {
            var sql = string.Format("SELECT {0} FROM {1} WHERE {2} = @0", columns, TableName, PrimaryKeyField);
            return Query(sql, key).FirstOrDefault();
        }

        /// <summary>
        /// A helpful query tool
        /// </summary>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            //parse the method
            var constraints = new List<string>();
            var counter = 0;
            var info = binder.CallInfo;
            // accepting named args only... SKEET!
            if (info.ArgumentNames.Count != args.Length)
            {
                throw new InvalidOperationException("Please use named arguments for this type of query - the column name, orderby, columns, etc");
            }

            //first should be "FindBy, Last, Single, First"
            var op = binder.Name;
            var columns = " * ";
            var orderBy = string.Format(" ORDER BY {0}", PrimaryKeyField);
            var where = "";
            var whereArgs = new List<object>();

            //loop the named args - see if we have order, columns and constraints
            if (info.ArgumentNames.Count > 0)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    var name = info.ArgumentNames[i].ToLower();
                    switch (name)
                    {
                        case "orderby":
                            orderBy = " ORDER BY " + args[i];
                            break;
                        case "columns":
                            columns = args[i].ToString();
                            break;
                        default:
                            constraints.Add(string.Format(" {0} = @{1}", name, counter));
                            whereArgs.Add(args[i]);
                            counter++;
                            break;
                    }
                }
            }

            if (constraints.Count > 0)
            {
                where = " WHERE " + string.Join(" AND ", constraints.ToArray());
            }

            var sql = "SELECT TOP 1 " + columns + " FROM " + TableName + where;
            var singleResult = op.StartsWith("First") || op.StartsWith("Last") || op.StartsWith("Get");

            //Be sure to sort by DESC on the PK (PK Sort is the default)
            if (op.StartsWith("Last"))
            {
                orderBy = orderBy + " DESC ";
            }
            else
            {
                //default to multiple
                sql = "SELECT " + columns + " FROM " + TableName + where;
            }

            result = singleResult ? Query(sql + orderBy, whereArgs.ToArray()).FirstOrDefault() :
                                                                                                   Query(sql + orderBy, whereArgs.ToArray());
            return true;
        }

        internal class MassiveException : Exception
        {
            public MassiveException(string message, Exception innerException = null)
                : base(message, innerException)
            {

            }
        }
    }
}