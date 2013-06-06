using InfoHub.Business.Interfaces;

namespace InfoHub.Web.Areas.Admin.Interfaces
{
    public interface IDashboard
    {
        ILoginRequest LoginRequest { get; set; }
        IAccountProfileResponse LoginResponse { get; set; }
    }
}