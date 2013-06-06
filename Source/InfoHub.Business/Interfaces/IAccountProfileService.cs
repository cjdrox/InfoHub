using InfoHub.Business.Models;
using InfoHub.Entity.Domain;

namespace InfoHub.Business.Interfaces
{
    public interface IAccountProfileService : IService<AccountProfile>
    {
        IAccountProfileResponse GetByUsername(string username);
        IAccountProfileResponse Login(ILoginRequest request);
    }
}