using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Infohub.Repository.Helpers
{
    public static class SessionFactoryHelper
    {
        private static ISessionFactory _sessionFactory;

        public static Configuration CurrentConfiguration { get; set; }

        // An ISessionFactory is threadsafe, many threads can access it concurrently and request ISessions
        public static ISession OpenSession()
        {
            return _sessionFactory.OpenSession();
        }

        public static ISessionFactory CreateSessionFactory(Assembly asm)
        {
            //var x = Fluently.Configure()
            //    .Database(MySQLConfiguration.Standard.ConnectionString(s=>s
            //        .Server("localhost")
            //        .Username("root")
            //        .Password("")
            //        .Database("InfoHub")));

            //var y = x.Mappings(m => m.AutoMappings.Add(CreateAutomappings)).ExposeConfiguration(BuildSchema);
            //_sessionFactory=y.BuildSessionFactory();
            
            return _sessionFactory;
        }

        public static void BuildSchema(Configuration configuration)
        {
            new SchemaExport(configuration).Create(false, true);
        }

        //private static AutoPersistenceModel CreateAutomappings()
        //{
        //    return AutoMap.AssemblyOf<SystemUser>().Conventions.Add<CascadeConvention>();
        //}
    }
}
