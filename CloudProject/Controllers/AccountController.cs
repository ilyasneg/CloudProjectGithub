using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CloudProject.Models;
using NHibernate;

namespace CloudProject.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
                FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            User user = null;
            using (ISession session = NHibertnateSession.OpenSession())
            {
                user = session.Query<User>().FirstOrDefault(u => u.Login == model.Login && u.Password == MD5Class.Calculate(model.Password).ToLower());
            }
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(model.Login, true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Пользователя с таким логином и поролем нет");
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            try
            {
                User user = null;
                using (ISession session = NHibertnateSession.OpenSession())
                {
                    user = session.Query<User>().FirstOrDefault(u => u.Login == model.Login);
                }
                if (user == null)
                {
                    using (ISession session = NHibertnateSession.OpenSession())
                    {
                        using (ITransaction transaction = session.BeginTransaction())
                        {
                            session.Save(new User() {Login = model.Login, Password = MD5Class.Calculate(model.Password).ToLower()});
                            transaction.Commit();
                            FormsAuthentication.SetAuthCookie(model.Login, true);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                ModelState.AddModelError("", "Пользователь с таким логином уже существует");
            }
            catch
            {
                return View(model);
            }

            return View(model);
        }
    }
}