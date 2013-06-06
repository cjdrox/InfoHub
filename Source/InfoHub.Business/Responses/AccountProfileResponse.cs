using InfoHub.Business.Interfaces;
using InfoHub.Business.Models;
using InfoHub.Entity.Domain;

namespace InfoHub.Business.Responses
{
    public sealed class AccountProfileResponse : ServiceResponse<AccountProfile>, IAccountProfileResponse
    {
        public AccountProfileResponse(AccountProfile accountProfile)
        {
            ServiceData = accountProfile;
        }
    }
}
