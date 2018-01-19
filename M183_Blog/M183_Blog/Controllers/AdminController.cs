using M183_Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace M183_Blog.Controllers
{
    public class AdminController : Controller
    {
        private DataAccess db = new DataAccess();

        // GET: Admin
        public ActionResult Dashboard()
        {
            var userId = Session["userId"];
            if (userId != null)
            {
                Guid id = (Guid)userId;
                var user = db.User.FirstOrDefault(u => u.Id == id);
                if (user != null && user.Role == Role.Administrator)
                {
                    return View();
                }
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }
    }
}