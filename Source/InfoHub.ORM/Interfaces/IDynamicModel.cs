using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Dynamic;
using System.Linq.Expressions;

namespace InfoHub.ORM.Interfaces
{
    public interface IDynamicModel
    {
        /// <summary>
        /// Creates a new Expando from a Form POST - white listed against the columns in the DB
        /// </summary>
        dynamic CreateFrom(NameValueCollection coll);

        /// <summary>
        /// Gets a default value for the column
        /// </summary>
        dynamic DefaultValue(dynamic column);

        /// <summary>
        /// Creates an empty Expando set with defaults from the DB
        /// </summary>
        dynamic Prototype { get; }

        IEnumerable<object> Schema { get; }
        string PrimaryKeyField { get; set; }
        string TableName { get; set; }

        /// <summary>
        /// Enumerates the reader yielding the result - thanks to Jeroen Haegebaert
        /// </summary>
        object QueryScalar(string sql, params object[] args);

        /// <summary>
        /// Enumerates the reader yielding the result - thanks to Jeroen Haegebaert
        /// </summary>
        IEnumerable<object> Query(string sql, params object[] args);

        /// <summary>
        /// Executes the reader using SQL async API - thanks to Damian Edwards
        /// </summary>
        void QueryAsync(string sql, Action<List<object>> callback, params object[] args);

        IEnumerable<object> Query(string sql, DbConnection connection, params object[] args);

        /// <summary>
        /// Returns a single result
        /// </summary>
        object Scalar(string sql, params object[] args);

        /// <summary>
        /// Returns and OpenConnection
        /// </summary>
        DbConnection OpenConnection(IConfiguration configuration);

        /// <summary>
        /// Builds a set of Insert and Update commands based on the passed-on objects.
        /// These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects
        /// With a PK property (whatever PrimaryKeyField is set to) will be created at UPDATEs
        /// </summary>
        List<DbCommand> BuildCommands(params object[] things);

        /// <summary>
        /// Executes a set of objects as Insert or Update commands based on their property settings, within a transaction.
        /// These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects
        /// With a PK property (whatever PrimaryKeyField is set to) will be created at UPDATEs
        /// </summary>
        int Save(params object[] things);

        /// <summary>
        /// Execute a single command
        /// </summary>
        int Execute(DbCommand command);

        int Execute(string sql, params object[] args);

        /// <summary>
        /// Executes a series of DBCommands in a transaction
        /// </summary>
        int Execute(IEnumerable<DbCommand> commands);

        /// <summary>
        /// Conventionally introspects the object passed in for a field that 
        /// looks like a PK. If you've named your PrimaryKeyField, this becomes easy
        /// </summary>
        bool HasPrimaryKey(object o);

        /// <summary>
        /// If the object passed in has a property with the same name as your PrimaryKeyField
        /// it is returned here.
        /// </summary>
        object GetPrimaryKey(object o);

        /// <summary>
        /// Creates a command for use with transactions - internal stuff mostly, but here for you to play with
        /// </summary>
        DbCommand CreateInsertCommand(object o);

        /// <summary>
        /// Creates a command for use with transactions - internal stuff mostly, but here for you to play with
        /// </summary>
        DbCommand CreateUpdateCommand(object o, object key);

        /// <summary>
        /// Removes one or more records from the DB according to the passed-in WHERE
        /// </summary>
        DbCommand CreateDeleteCommand(string where = "", object key = null, params object[] args);

        /// <summary>
        /// Adds a record to the database. You can pass in an Anonymous object, an ExpandoObject,
        /// A regular old POCO, or a NameValueColletion from a Request.Form or Request.QueryString
        /// </summary>
        object Insert(object o);

        /// <summary>
        /// Updates a record in the database. You can pass in an Anonymous object, an ExpandoObject,
        /// A regular old POCO, or a NameValueCollection from a Request.Form or Request.QueryString
        /// </summary>
        int Update(object o, object key);

        /// <summary>
        /// Removes one or more records from the DB according to the passed-in WHERE
        /// </summary>
        int Delete(object key = null, string where = "", params object[] args);

        /// <summary>
        /// Returns all records complying with the passed-in WHERE clause and arguments, 
        /// ordered as specified, limited (TOP) by limit.
        /// </summary>
        IEnumerable<object> All(string where = "", string orderBy = "", int limit = 0, string columns = "*", params object[] args);

        /// <summary>
        /// Returns the count from this table, using optional where statement
        /// </summary>
        /// <param name="where">query parameters (eg: "id>=100")</param>
        /// <param name="args"></param>
        /// <returns></returns>
        int Count(string where = "", params object[] args);

        /// <summary>
        /// Returns all records complying with the passed-in WHERE clause and arguments, 
        /// ordered as specified, limited (TOP) by limit.
        /// </summary>
        void AllAsync(Action<List<object>> callback, string where = "", string orderBy = "", int limit = 0, string columns = "*", params object[] args);

        /// <summary>
        /// Returns a dynamic PagedResult. Result properties are Items, TotalPages, and TotalRecords.
        /// </summary>
        dynamic Paged(string where = "", string orderBy = "", string columns = "*", int pageSize = 20, int currentPage = 1, params object[] args);

        /// <summary>
        /// Returns a single row from the database
        /// </summary>
        dynamic Single(string where, params object[] args);

        /// <summary>
        /// Returns a single row from the database
        /// </summary>
        dynamic Single(object key, string columns = "*");

        /// <summary>
        /// A helpful query tool
        /// </summary>
        bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result);

        bool TryGetMember(GetMemberBinder binder, out object result);
        bool TrySetMember(SetMemberBinder binder, object value);
        bool TryDeleteMember(DeleteMemberBinder binder);
        bool TryConvert(ConvertBinder binder, out object result);
        bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result);
        bool TryInvoke(InvokeBinder binder, object[] args, out object result);
        bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result);
        bool TryUnaryOperation(UnaryOperationBinder binder, out object result);
        bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result);
        bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value);
        bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes);
        IEnumerable<string> GetDynamicMemberNames();
        DynamicMetaObject GetMetaObject(Expression parameter);
    }
}