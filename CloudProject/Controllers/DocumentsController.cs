using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudProject.Models;
using NHibernate;

namespace CloudProject.Controllers
{
    public class DocumentsController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult All()
        {
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
                session.CreateSQLQuery("EXEC AddDocument @name = '" + model.Name + "',@date = '"+ dateTime +"',@author = '" + User.Identity.Name + "',@link = '" + link + "'")
                    .ExecuteUpdate();
            }
            return RedirectToAction("All", "Documents");
        }

        [Authorize]
        public ActionResult Image(string link)
        {
            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), $"{link}.png");
            return base.File(path, "image/jpeg");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Sort(string sortBy)
        {
            var model = (List<Document>)Session["model"];
            if (model == null) return View("All", new List<Document>());
            //TODO сортировка в одном направлении (нужно сделать в обе стороны)
            switch (sortBy)
            {
                case "Name":
                    model = model.OrderBy(p => p.Name).ToList();
                    break;
                case "Date":
                    model = model.OrderBy(p => p.Date).ToList();
                    break;
            }

            return View("All", model);
        }
    }
}