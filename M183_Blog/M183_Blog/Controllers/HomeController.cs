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

        /*
            Fragen

            1   Username nicht gehasht da E-Mail für Informationszwecke oder Password Reset Funktion verwendet werden könnte
                Argon2 ist einer der besten hash Algorithmen mit KeyDerivation
                Bei diesem Algorithmus kann memory cost und iteratinoen der Zeit und Leistung der Computer angepasst werden
            
            2   Session Hijacking, da die Session nur gültig ist mit der Ip wo sich der User eingeloggt hat

            3   Cookie mit SessionId wird gestohlen und verwendet um sich als diesen user zu identifizieren.
                Die Session wird nur erfolgreich herausgegeben wenn die Ip des Clients die gleiche ist wie zum Zeitpunkt des Logins

        */

        private DataAccess db = new DataAccess();

        public ActionResult Index()
        {
            var argon2 = new PasswordHasher();
            if (db.User.FirstOrDefault(u => u.Username == "user@user.com") == null)
            {
                db.User.Add(new User
                {
                    Username = "user@user.com",
                    Password = argon2.Hash("user"),
                    PhoneNumber = "000000000",
                    FirstName = "Test",
                    LastName = "User",
                    Role = Role.User,
                    Active = true
                });
            }
            if (db.User.FirstOrDefault(u => u.Username == "admin@admin.com") == null)
            {
                db.User.Add(new User
                {
                    Username = "admin@admin.com",
                    Password = argon2.Hash("admin"),
                    PhoneNumber = "000000000",
                    FirstName = "Test",
                    LastName = "Admin",
                    Role = Role.Administrator,
                    Active = true
                });
            }
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
            ViewBag.Error = TempData["error"];
            ViewBag.Title = "Login";
            return View();
        }

        public ActionResult Pin()
        {
            ViewBag.Error = TempData["error"];
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
                    if (!user.Active)
                    {
                        TempData["error"] = "User Blocked";
                        return RedirectToAction(nameof(Login));
                    }

                    Session.Remove("tries");
                    SendSMS(user);
                    Session.Add("userId", user.Id);
                    return RedirectToAction(nameof(Pin));
                }

                if (Session["tries"] == null)
                {
                    Session.Add("tries", 0);
                }
                else
                {
                    int tries = (int)Session["tries"];
                    Session["tries"] = ++tries;

                    if (tries > 2)
                    {
                        if (user.Active)
                        {
                            user.Active = false;

                            db.Userlog.Add(new Userlog
                            {
                                User = user,
                                Action = "User Blocked"
                            });
                        }
                    }
                }

                db.Userlog.Add(new Userlog
                {
                    User = user,
                    Action = $"Wrong Password {password}"
                });
            }

            TempData["error"] = "Wrong Credentials";


            return RedirectToAction(nameof(Login));
        }

        public ActionResult Logout()
        {
            var userLogin = db.UserLogin.FirstOrDefault(ul => ul.SessionId == Session.SessionID);
            if (userLogin != null)
            {
                userLogin.Active = false;
            }
            db.Userlog.Add(new Userlog
            {
                User = userLogin.User,
                Action = "Logout"
            });
            Session.Abandon();
            return RedirectToAction(nameof(Login));
        }

        public ActionResult CheckPin()
        {
            var pin = Request["pin"];
            var userId = Session["userId"];
            if (userId == null)
            {
                // dont show any error message, because someone wants to access pin page without login
                return RedirectToAction(nameof(Login));
            }

            Guid id = (Guid)userId;

            var user = db.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                // dont show any error message, because someone wants to access pin page without login
                return RedirectToAction(nameof(Login));
            }

            var token = db.Token.OrderByDescending(t => t.TimeStamp).FirstOrDefault(t => t.User.Id == user.Id && t.Active);
            if (token == null)
            {
                // dont show any error message, because someone wants to access pin page without login
                return RedirectToAction(nameof(Login));
            }

            if (token.TimeStamp < DateTime.Now.AddMinutes(-5) || token.Value != pin)
            {
                if (Session["tries"] == null)
                {
                    Session.Add("tries", 0);
                }
                else
                {
                    int tries = (int)Session["tries"];
                    Session["tries"] = ++tries;

                    if (tries > 2)
                    {
                        if (user.Active)
                        {
                            user.Active = false;
                            token.Active = false;

                            db.Userlog.Add(new Userlog
                            {
                                User = user,
                                Action = "User Blocked"
                            });
                        }
                    }
                }

                TempData["error"] = "Token invalid or expired";
                db.Userlog.Add(new Userlog
                {
                    User = user,
                    Action = "Pin invalid or expired"
                });
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

            var sms = new NexmoRequest
            {
                from = "m183",
                text = token.Value,
                to = user.PhoneNumber,
                api_key = "",
                api_secret = ""
            };

            sms.Send();
        }

        protected override void Dispose(bool disposing)
        {
            db?.SaveChanges();
            db?.Dispose();
            base.Dispose(disposing);
        }
    }
}