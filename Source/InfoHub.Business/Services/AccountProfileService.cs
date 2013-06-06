using InfoHub.Business.Interfaces;
using InfoHub.Business.Models;
using InfoHub.Business.Responses;
using InfoHub.Entity.Domain;
using InfoHub.Entity.Interfaces;
using InfoHub.Entity.Repositories;

namespace InfoHub.Business.Services
{
    public sealed class AccountProfileService : ServiceBase<AccountProfile>, IAccountProfileService
    {
        private readonly IAccountProfileRepository _accountProfileRepository;

        public AccountProfileService() : this(new AccountProfileRepository())
        {   
        }

        public AccountProfileService(IAccountProfileRepository accountProfileRepository)
        {
            _accountProfileRepository = accountProfileRepository;
        }

        public IAccountProfileResponse GetByUsername(string username)
        {
            var entity = _accountProfileRepository.GetUserByUsername(username);
            return new AccountProfileResponse(entity);
        }

        public IAccountProfileResponse Login(ILoginRequest request)
        {
            return GetNullResponse() as IAccountProfileResponse;
        }
    }
}