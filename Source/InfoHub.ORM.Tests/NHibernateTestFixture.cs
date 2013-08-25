using System.Reflection;
using NUnit.Framework;
using NHibernate.Tool.hbm2ddl;

namespace InfoHub.ORM.Tests
{
    [TestFixture]
    public class NHibernateTestFixture
    {
        [Test]
        public void CanGenerateSchema()
        {
            var cfg = new NHibernate.Cfg.Configuration();
            cfg.Configure();
            cfg.AddAssembly(Assembly.Load("InfoHub.Entity"));
            
            new SchemaExport(cfg).Execute(false, true, false);
        }
    }
}
