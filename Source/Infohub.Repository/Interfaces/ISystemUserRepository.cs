using InfoHub.Entity.Entities;

namespace Infohub.Repository.Interfaces
{
    public interface ISystemUserRepository
    {
        SystemUser GetUserByUsername(string username);
        SystemUser GetUserByUsernamePassword(string username, string password);
    }
}
