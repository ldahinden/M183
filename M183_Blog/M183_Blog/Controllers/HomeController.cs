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
        private DataAccess db = new DataAccess();

        public ActionResult Index()
        {
            /*var argon2 = new PasswordHasher();
            db.User.Add(new User
            {
                Username = " user@user.com ",
                Password = argon2.Hash("user"),
                PhoneNumber = "000000000",
                FirstName = "Test",
                LastName = "User",
                Role = Role.User,
                Active = true
            });
            db.User.Add(new User
            {
                Username = " admin@admin.com ",
                Password = argon2.Hash("admin"),
                PhoneNumber = "000000000",
                FirstName = "Test",
                LastName = "Admin",
                Role = Role.Administrator,
                Active = true
            });
            db.SaveChanges();*/

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

            return RedirectToAction(nameof(Login));
        }

        public ActionResult Logout()
        {
            Session.Abandon();
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

            var user = db.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var token = db.Token.OrderByDescending(t => t.TimeStamp).FirstOrDefault(t => t.User.Id == user.Id && t.Active);
            if (token == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (token.TimeStamp < DateTime.Now.AddMinutes(-5) || token.Value != pin)
            {
                return RedirectToAction(nameof(Pin));
            }

            Session["loggedin"] = true;
            token.Active = false;

            db.Userlog.Add(new Userlog
            {
                User = user,
                Action = "Login successful"
            });

            db.UserLogin.Add(new UserLogin
            {
                User = user,
                Ip = Request.UserHostAddress,
                SessionId = Session.SessionID,
                Active = true,
                Timestamp = DateTime.Now
            });

            db.SaveChanges();

            if (user.Role == Role.User)
            {
                return RedirectToAction(nameof(UserController.Dashboard), "User");
            }
            else if (user.Role == Role.Administrator)
            {
                return RedirectToAction(nameof(AdminController.Dashboard), "Admin");
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
                TimeStamp = DateTime.Now,
                Active = true
            };
            
            db.Token.Add(token);
            db.SaveChanges();

            var sms = new NexmoRequest
            {
                from = "m183",
                text = token.Value,
                to = "0000000000",
                api_key = "",
                api_secret = ""
            };

            sms.Send();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }
    }
}