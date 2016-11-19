using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.ServiceBus.Messaging;

namespace AzureHF.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn()
        {
            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                string callbackUrl = Url.Action("SignInCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = callbackUrl },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {

                SendMessage("Signed out:" + User.Identity.Name);

                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult SignInCallback()
        {
            if (Request.IsAuthenticated)
            {

                SendMessage("Signed in: " + User.Identity.Name);

                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        private void SendMessage(string msg)
        {
            QueueClient client = QueueClient.CreateFromConnectionString("Endpoint=sb://azurehf.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8C4K/YMT6YPmsyJRp4e1IvRtDhIzcr0EMgi53ATSXf8=", "authlog");
            using (var bm = new BrokeredMessage(msg))
            {
                client.Send(bm);
            }
        }

    }
}
