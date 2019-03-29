using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeMobile.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult Index(string name, string email, string subject, string message)
        //{
        //    return View();
        //}
    }
}