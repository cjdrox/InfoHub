using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace InfoHub.ORM.Extensions
{
    public static class ReaderExtensions
    {
        /// <summary>
        /// Turns an IDataReader into a dynamic list of expando objects
        /// </summary>
        public static List<dynamic> ToExpandoList(this IDataReader rdr)
        {
            var result = new List<dynamic>();
            while (rdr.Read())
            {
                result.Add(rdr.RecordToExpando());
            }
            return result;
        }

        public static dynamic RecordToExpando(this IDataReader rdr)
        {
            dynamic e = new ExpandoObject();
            var d = e as IDictionary<string, object>;
            for (var i = 0; i < rdr.FieldCount; i++)
                d.Add(rdr.GetName(i), DBNull.Value.Equals(rdr[i]) ? null : rdr[i]);
            return e;
        }
    }
}
