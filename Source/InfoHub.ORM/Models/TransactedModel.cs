using System.Collections.Generic;
using InfoHub.ORM.Interfaces;

namespace InfoHub.ORM.Models
{
    public class TransactedModel : DynamicModel
    {
        // Lets add two properties that we will reference in our overridden Execute method
        public System.Data.IDbTransaction Transaction;

        protected TransactedModel(IConfiguration configuration, ITable table): base(configuration, table)
        {
        }

        protected TransactedModel()
        {
        }

        // Lets override the Execute method and if we've supplied a Connection, then let's
        // using our own custom implementation otherwise use Massive's default implementation.
        public override int Execute(IEnumerable<System.Data.Common.DbCommand> commands)
        {
            if (Connection == null) return base.Execute(commands);

            var result = 0;

            foreach (var cmd in commands as IEnumerable<System.Data.IDbCommand>)
            {
                cmd.Connection = Connection;
                cmd.Transaction = Transaction;
                result += cmd.ExecuteNonQuery();
            }

            return result;
        }
    }
}
