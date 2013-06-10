using InfoHub.Entity.Entities;
using System.Linq;
using InfoHub.ORM.Interfaces;
using Infohub.Repository.Interfaces;

namespace Infohub.Repository.Repositories
{
    public class SystemUserRepository : RepositoryBase<SystemUser>, ISystemUserRepository
    {
        public SystemUserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public SystemUser GetUserByUsername(string username)
        {
            return Query(user => user.Username == username).FirstOrDefault();
        }
    }
}
