using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;

namespace InfoHub.ORM.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Turns an object into an ExpandoObject
        /// </summary>
        public static dynamic ToExpando(this object o)
        {
            var result = new ExpandoObject();
            var properties = result as IDictionary<string, object>;

            if (o is ExpandoObject)
                return o; //shouldn't have to... but just in case

            if (o.GetType() == typeof(NameValueCollection) || o.GetType().IsSubclassOf(typeof(NameValueCollection)))
            {
                var nv = (NameValueCollection)o;
                nv.Cast<string>().Select(key => new KeyValuePair<string, object>(key, nv[key])).ToList().ForEach(properties.Add);
            }
            else
            {
                var props = o.GetType().GetProperties();
                foreach (var item in props)
                {
                    properties.Add(item.Name, item.GetValue(o, null));
                }
            }
            return result;
        }
        /// <summary>
        /// Turns an object into a Dictionary
        /// </summary>
        public static IDictionary<string, object> ToDictionary(this object thingy)
        {
            return (IDictionary<string, object>)thingy.ToExpando();
        }
    }
}