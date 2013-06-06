using System.Web.Mvc;
using InfoHub.Web.Models;

namespace InfoHub.Web.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("See");
        }

        public ActionResult See(Complex complex)
        {
            if (complex==null)
            {
                complex = new Complex();
            }

            return View("Complex", complex);
        }
    }
}
