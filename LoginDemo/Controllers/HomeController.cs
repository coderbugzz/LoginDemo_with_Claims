using LoginDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace LoginDemo.Controllers
{
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            var identity = (Microsoft.IdentityModel.Claims.ClaimsIdentity)HttpContext.User.Identity;
            var claim = identity.Claims.FirstOrDefault(c => c.ClaimType == Microsoft.IdentityModel.Claims.ClaimTypes.UserData).Value;
            var custom_claim = identity.Claims.FirstOrDefault(c => c.ClaimType == "custom").Value;
           
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