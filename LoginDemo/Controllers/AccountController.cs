using LoginDemo.Models;
using LoginDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web.Security;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Claims;
using System.Security.Principal;
using Newtonsoft.Json;

namespace LoginDemo.Controllers
{
    public class AccountController : Controller
    {

        Repository repository = new Repository();
        // GET: Account
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(LoginViewModel model) //Login
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var result = await repository.login(model);
            if (result.resultCode == 200 && !User.Identity.IsAuthenticated)
            {
                SessionAuthenticationModule sam = (SessionAuthenticationModule)this.HttpContext.ApplicationInstance.Modules["SessionAuthenticationModule"];
                IClaimsPrincipal principal = new Microsoft.IdentityModel.Claims.ClaimsPrincipal(new GenericPrincipal(new GenericIdentity(model.Email), null));
               
                List<User> user = new List<User>();
                user.Add(new Models.User { Email = "freecodespot@gmail.com",Date = DateTime.Now});
                user.Add(new Models.User { Email = "freecodespot@gmail.com", Date = DateTime.Now });
                user.Add(new Models.User { Email = "freecodespot@gmail.com", Date = DateTime.Now });
                user.Add(new Models.User { Email = "freecodespot@gmail.com", Date = DateTime.Now });
                string dummy = JsonConvert.SerializeObject(user);
                principal.Identities[0].Claims.Add(new Microsoft.IdentityModel.Claims.Claim(Microsoft.IdentityModel.Claims.ClaimTypes.Email, model.Email));
                principal.Identities[0].Claims.Add(new Microsoft.IdentityModel.Claims.Claim(Microsoft.IdentityModel.Claims.ClaimTypes.UserData, dummy));
                principal.Identities[0].Claims.Add(new Microsoft.IdentityModel.Claims.Claim(Microsoft.IdentityModel.Claims.ClaimTypes.Role, "Admin"));

                principal.Identities[0].Claims.Add(new Microsoft.IdentityModel.Claims.Claim("custom", "freecodespot custom claims"));

                var token = sam.CreateSessionSecurityToken(principal, null, DateTime.Now, DateTime.Now.AddMinutes(20), false);
                sam.WriteSessionTokenToCookie(token);

                //FormsAuthentication.SetAuthCookie(model.Email, false);

                return RedirectToAction("Index", "Home", result.Data); //redirect to login form
            }
            else if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("", result.message);
            }

            return View();
        }
      
        public  ActionResult Register() //Register
        {
            return View();
        }



        public ActionResult Logout() //Register
        {
            //FormsAuthentication.SignOut();
            var sam = FederatedAuthentication.SessionAuthenticationModule;
            
            sam.DeleteSessionTokenCookie();
           

            return RedirectToAction("Index"); //redirect to index
        }


        [HttpPost]
        public async Task<ActionResult> Register(LoginViewModel model) //Register
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var result = await repository.Register(model);

            if (result.resultCode == 200)
            {
                return RedirectToAction("Index"); //redirect to login form
            }
            else
            {
                ModelState.AddModelError("", result.message);
            }

            return View();
        }

      
    }
}