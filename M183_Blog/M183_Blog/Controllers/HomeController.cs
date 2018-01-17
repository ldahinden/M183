using Liphsoft.Crypto.Argon2;
using M183_Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace M183_Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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

        public ActionResult Login()
        {
            ViewBag.Message = "Login";
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult DoLogin()
        {
            var email = Request["email"];
            var password = Request["password"];

            using (var db = new DataAccess())
            {
                var user = db.User.FirstOrDefault(u => u.Username == email);
                if (user != null)
                {
                    PasswordHasher hasher = new PasswordHasher();
                    if (hasher.Verify(user.Password, password))
                    {
                        SendSMS(user);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private void SendSMS(User user)
        {
            Random r = new Random();
            int token = r.Next(100000, 1000000);
            using (var db = new DataAccess())
            {
                db.Token.Add(new Token
                {
                    User = user,
                    Value = token,
                    TimeStamp = DateTime.Now
                });

                db.SaveChanges();
            }



        }
    }
}