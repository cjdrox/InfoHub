using InfoHub.Entity.Entities;
using System.Linq;
using InfoHub.ORM.Interfaces;
using InfoHub.Security.Models;
using Infohub.Repository.Interfaces;

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
                && user.Passhash.Equals(password.SHA1Hash())).FirstOrDefault();
        }
    }
}
