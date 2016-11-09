using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace AzureHF.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            Claim groupClaim = ClaimsPrincipal.Current.Claims.FirstOrDefault(
                c => c.Type == "groups");// &&
                //c.Value.Equals(GetGroupIdByName(Group), StringComparison.CurrentCultureIgnoreCase));

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}