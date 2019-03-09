using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchManager.Models;
using Microsoft.AspNet.Identity;

namespace ChurchManager.Controllers
{
    [Authorize]
    public class ChurchesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: Churches
        public ActionResult Index()
        {
            return View(db.Churches.ToList());
        }

        // GET: Churches/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Church church = db.Churches.Find(id);
            if (church == null)
            {
                return HttpNotFound();
            }
            return View(church);
        }

        // GET: Churches/Create
        public ActionResult Create()
        {
            var ownerId = Operator().OwnerGroupId;
            var churches = db.Churches.Where(x => x.OwnerGroupId == ownerId).ToList();
            if (churches != null)
            {                
                var churchExist = db.Churches.Where(x => x.OwnerGroupId == ownerId).Count() > 0;
                if (churchExist)
                {
                    ModelState.AddModelError("", "You're only allowed to have 1 church");
                    return RedirectToAction("Edit");
                }
            }
            return View();
        }

        // POST: Churches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Church church)
        {
            var ownerId = Operator().OwnerGroupId;
            var churches = db.Churches.Where(x => x.OwnerGroupId == ownerId).ToList();
            if (churches != null)
            {
                var churchExist = db.Churches.Where(x => x.OwnerGroupId == ownerId).Count() > 0;
                if (churchExist)
                {
                    ModelState.AddModelError("", "You're only allowed to have 1 church");
                    return RedirectToAction("Edit");
                }
            }

            church.Id = Guid.NewGuid();
            church.OwnerGroupId = Operator().OwnerGroupId;
            church.DateEntered = DateTime.Now;
            church.EnteredBy = Guid.Parse(Operator().Id);

            if (ModelState.IsValid)
            {
                
                db.Churches.Add(church);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return View(church);
        }

        // GET: Churches/Edit/5
        [CustomAuthorize(Operations ="Church_Edit")]
        public ActionResult Edit()
        {
            var id = Operator().OwnerGroupId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Church church = db.Churches.Where(x => x.OwnerGroupId == id).FirstOrDefault();
            if (church == null)
            {
                   return RedirectToAction("Create");
            }
            return View(church);
        }

        // POST: Churches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [CustomAuthorize(Operations = "Church_Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Church church)
        {
            var dbChurch = db.Churches.Find(church.Id);
            dbChurch.Name = church.Name;
            dbChurch.Address = church.Address;
            dbChurch.City = church.City;
            dbChurch.State = church.State;
            dbChurch.Country = church.Country;
            dbChurch.ZipCode = church.ZipCode;
            dbChurch.Phone = church.Phone;
            dbChurch.Email = church.Email;
            dbChurch.FbPage = church.FbPage;
            if (ModelState.IsValid)
            {
                dbChurch.DateLastEdited = DateTime.Now;
                dbChurch.EditedBy = Guid.Parse(Operator().Id);
                //db.Entry(church).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Success = true;
                return View(dbChurch);
            }
            else
            {
                ModelState.AddModelError("","Something went wrong while saving");               
            }
            return View(church);
        }

        // GET: Churches/Delete/5
        public ActionResult Delete(Guid? id)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Church church = db.Churches.Find(id);
            if (church == null)
            {
                return HttpNotFound();
            }
            return View(church);
        }

        // POST: Churches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Church church = db.Churches.Find(id);
            db.Churches.Remove(church);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
