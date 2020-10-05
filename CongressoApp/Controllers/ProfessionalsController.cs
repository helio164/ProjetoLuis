using CongressoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CongressoApp.Controllers
{
    public class ProfessionalsController : Controller
    {
        private CongressoContext db = new CongressoContext();
        
        // GET: User/Professionals
        public ActionResult Index()
        {
            List<Professional> professionals = db.Professionals.ToList();
            return View(professionals);
        }

        // GET: User/Professionals/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Professional professional = await db.Professionals.FindAsync(id);
            if (professional == null)
            {
                return HttpNotFound();
            }
            return PartialView(professional);
        }

        // GET: User/Professionals/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Professionals/Create
        [HttpPost]
        public async Task<ActionResult> Create(Professional professional)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.Professionals.Add(professional);
                    await db.SaveChangesAsync();
                    return View("Details", professional);
                }
                return View(professional);
            }
            catch
            {
                return View(professional);
            }
        }

        // GET: User/Professionals/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Professionals/Edit/5
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

        // GET: User/Professionals/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Professionals/Delete/5
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
