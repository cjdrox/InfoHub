using System;
using System.Reflection;
using Infohub.Repository.Helpers;
using NHibernate;
using NHibernate.Cfg;

namespace InfoHub.Deployer.Services
{
    public class FNHDeploymentService : NHDeploymentService
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly Assembly _assembly;

        public FNHDeploymentService() : this(Assembly.GetExecutingAssembly())
        {
        }

        public FNHDeploymentService(Assembly asm)
        {
            _assembly = asm;
            _sessionFactory = SessionFactoryHelper.CreateSessionFactory(_assembly);
        }

        public override void DeployClass(Configuration cfg, Type type)
        {
            throw new NotImplementedException();
        }

        public override void DeployAllClasses(Assembly assembly)
        {
            if (assembly==null)
            {
                SessionFactoryHelper.BuildSchema(SessionFactoryHelper.CurrentConfiguration);
            }
        }
    }
}
