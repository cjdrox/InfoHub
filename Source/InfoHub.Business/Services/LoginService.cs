using InfoHub.Business.Interfaces;
using InfoHub.Business.Models;
using InfoHub.Business.Requests;
using InfoHub.Business.Responses;
using InfoHub.Business.Types;
using InfoHub.Entity.Entities;
using Infohub.Repository.Interfaces;
using Infohub.Repository.Repositories;

namespace InfoHub.Business.Services
{
    public class LoginService : ServiceBase<SystemUser>, ILoginService
    {
        private readonly ISystemUserRepository _systemUserRepository;

        public LoginService() : this(new SystemUserRepository(null))
        {
            
        }

        public LoginService(ISystemUserRepository systemUserRepository)
        {
            _systemUserRepository = systemUserRepository;
        }

        public LoginResponse GetUserByName(LoginRequest request)
        {
            var response = _systemUserRepository.GetUserByUsername(request.Data.Username);

            return new LoginResponse
                       {
                           ServiceData = response,
                           Status = ResponseStatus.Success
                       };
        }
    }
}
