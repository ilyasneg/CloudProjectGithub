using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudProject.Models;
using NHibernate;
using NHibernate.Linq;

namespace CloudProject.Controllers
{
    public class DocumentsController : Controller
    {
        [HttpPost]
        [Authorize]
        public ActionResult Find(string text)
        {
            List<Document> documents = new List<Document>();
            if (!ModelState.IsValid || string.IsNullOrEmpty(text)) return View("All", documents);
            using (ISession session = NHibertnateSession.OpenSession())
            {
                documents = session.Query<Document>().Where(d => d.Author == User.Identity.Name && d.Name.Contains(text)).ToList();
            }

            return View("All", documents);
        }

        [HttpGet]
        [Authorize]
        public ActionResult All()
        {
            Session["NameDirectSort"] = true;
            Session["DateDirectSort"] = true;

            List<Document> documents = new List<Document>();
            if (!ModelState.IsValid) return View(documents);
            using (ISession session = NHibertnateSession.OpenSession())
            {
                documents = session.Query<Document>().Where(d => d.Author == User.Identity.Name).ToList();
            }

            return View(documents);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(Document model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                var file = Request.Files[0];
                if (file == null) return View(model);
                using (ISession session = NHibertnateSession.OpenSession())
                {
                    DateTime dateTime = DateTime.Now;
                    string timeStamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
                    var link = MD5Class.Calculate($"{timeStamp}{model.Name}").ToLower();
                    var fileExt = Path.GetExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), $"{link}{fileExt}");
                    file.SaveAs(path);
                    session.CreateSQLQuery("EXEC AddDocument @name = '" + model.Name + "',@date = '" + dateTime +
                                           "',@author = '" + User.Identity.Name + "',@link = '" + link +
                                           "', @contentPath = '" + path + "'")
                        .ExecuteUpdate();
                }
            }
            catch
            {
                // ignored
            }
            return RedirectToAction("All", "Documents");
        }

        [Authorize]
        public ActionResult Image(string link)
        {
            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), $"{link}.png");
            return File(path, "image/jpeg");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Sort(string sortBy, bool? directSort)
        {
            var model = Session["model"] as List<Document>;
            if (model == null) return View("All", new List<Document>());

            switch (sortBy)
            {
                case "Name":
                    if (directSort == true)
                    {
                        model = model.OrderBy(p => p.Name).ToList();
                        Session["NameDirectSort"] = false;
                    }
                    else
                    {
                        model = model.OrderByDescending(p => p.Name).ToList();
                        Session["NameDirectSort"] = true;
                    }
                    break;
                case "Date":
                    if (directSort == true)
                    {
                        model = model.OrderBy(p => p.Date).ToList();
                        Session["DateDirectSort"] = false;
                    }
                    else
                    {
                        model = model.OrderByDescending(p => p.Date).ToList();
                        Session["DateDirectSort"] = true;
                    }
                    break;
            }

            return View("All", model);
        }
    }
}