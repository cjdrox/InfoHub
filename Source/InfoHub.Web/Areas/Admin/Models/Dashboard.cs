using InfoHub.Business.Interfaces;
using InfoHub.Web.Areas.Admin.Interfaces;

namespace InfoHub.Web.Areas.Admin.Models
{
    public class Dashboard : IDashboard
    {
        public ILoginRequest LoginRequest { get; set; }
        public IAccountProfileResponse LoginResponse { get; set; }
    }
}