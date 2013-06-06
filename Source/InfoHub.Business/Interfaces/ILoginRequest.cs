using InfoHub.Entity.Domain;

namespace InfoHub.Business.Interfaces
{
    public interface ILoginRequest : IServiceRequest<AccountProfile>
    {
        bool Sumbitted { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool Remember { get; set; }
        bool ForgottenPassword { get; set; }
    }
}