using InfoHub.Entity.Entities;
using System.Linq;
using InfoHub.ORM.Interfaces;
using Infohub.Repository.Interfaces;
using InfoHub.Infrastructure.Security.Models;
namespace Infohub.Repository.Repositories
{
    public class SystemUserRepository : RepositoryBase<SystemUser>, ISystemUserRepository
    {
        public SystemUserRepository(IDatabaseAdapter adapter) : base(adapter)
        {
        }

        public SystemUser GetUserByUsername(string username)
        {
            return Query(user => user.Username == username).FirstOrDefault();
        }

        public SystemUser GetUserByUsernamePassword(string username, string password)
        {
            return Query(user => user.Username.Equals(username) 
                && user.Passhash.Equals(password.Hash())).FirstOrDefault();
        }
    }
}
