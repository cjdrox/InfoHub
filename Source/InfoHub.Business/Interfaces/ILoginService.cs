using InfoHub.Business.Requests;
using InfoHub.Business.Responses;
using InfoHub.Entity.Entities;

namespace InfoHub.Business.Interfaces
{
    public interface ILoginService : IService<SystemUser>
    {
        LoginResponse GetUserByName(LoginRequest request);
    }
}
