using Liphsoft.Crypto.Argon2;
using M183_Blog.Models;
using M183_Blog.Nexmo;
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
            ViewBag.Title = "Login";
            return View();
        }

        public ActionResult Pin()
        {
            ViewBag.Title = "Pin";
            return View();
        }

        [HttpPost]
        public ActionResult DoLogin()
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
                        Session.Add("userId", user.Id);
                        return RedirectToAction(nameof(Pin));
                    }
                }
            }

            return RedirectToAction(nameof(Login));
        }

        public ActionResult CheckPin()
        {
            var pin = Request["pin"];
            var userId = Session["userId"];
            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            Guid id = (Guid)userId;

            using (var db = new DataAccess())
            {
                var user = db.User.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                var token = db.Token.FirstOrDefault(t => t.User == user);
                if (token == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                if (token.TimeStamp < DateTime.Now.AddMinutes(-5) || token.Value != pin)
                {
                    return RedirectToAction(nameof(Login));
                }

                Session["loggedin"] = true;
            }

            return RedirectToAction(nameof(Index));
        }

        private void SendSMS(User user)
        {
            Random r = new Random();
            var token = new Token
            {
                User = user,
                Value = r.Next(100000, 1000000).ToString(),
                TimeStamp = DateTime.Now
            };
            
            using (var db = new DataAccess())
            {
                db.Token.Add(token);
                db.SaveChanges();
            }

            var sms = new NexmoRequest
            {
                from = "M183 Blog Engine",
                text = token.Value,
                to = token.User.PhoneNumber,
                api_key = "",
                api_secret = ""
            };

            sms.Send();
        }
    }
}