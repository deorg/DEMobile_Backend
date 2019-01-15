using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeMobile.Controllers
{
    public class SuccessController : Controller
    {
        // GET: Success
        public ActionResult Index()
        {
            return View();
        }

        // GET: Success/Details/5
        public ActionResult Redirect()
        {
            return View("Redirect");
        }

        // GET: Success/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Success/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Success/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Success/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Success/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Success/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
