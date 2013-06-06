using System.Web.Mvc;
using InfoHub.Business.Interfaces;
using InfoHub.Business.Requests;
using InfoHub.Business.Services;
using InfoHub.Web.Areas.Admin.Interfaces;
using InfoHub.Web.Areas.Admin.Models;

namespace InfoHub.Web.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboard _dashboard;
        private readonly IAccountProfileService _accountProfileService;

        public DashboardController()
        {
            _dashboard = new Dashboard {LoginRequest = new LoginRequest()};
            _accountProfileService = new AccountProfileService();
        }

        public ActionResult Index()
        {
            return View(_dashboard);
        }

        public ActionResult Logout()
        {
            _dashboard.LoginRequest.Sumbitted = false;
            _dashboard.LoginResponse = null;
            return RedirectToAction("Index");
        }

        public ActionResult Login(ILoginRequest loginRequest)
        {
            _dashboard.LoginResponse = _accountProfileService.Login(loginRequest);
            return RedirectToAction("Index");
        }
    }
}
