using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchManager.Models;

namespace ChurchManager.Controllers
{
    public class AccountChartsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AccountCharts
        public ActionResult Index()
        {
            return View(db.AccountCharts.ToList());
        }

        // GET: AccountCharts/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountChart accountChart = db.AccountCharts.Find(id);
            if (accountChart == null)
            {
                return HttpNotFound();
            }
            return View(accountChart);
        }

        // GET: AccountCharts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountCharts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountChart accountChart)
        {
          
            accountChart.IsActive = true;
            accountChart.DateEntered = DateTime.Now;
            accountChart.EnteredBy = new Guid(Operator().Id);
            accountChart.OwnerGroupId = Operator().OwnerGroupId;

            if (ModelState.IsValid)
            {
                accountChart.Id = Guid.NewGuid();
                db.AccountCharts.Add(accountChart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accountChart);
        }

        // GET: AccountCharts/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountChart accountChart = db.AccountCharts.Find(id);


            if (accountChart == null)
            {
                return HttpNotFound();
            }
            return View(accountChart);
        }

        // POST: AccountCharts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,IsActive,Type")] AccountChart accountChart)
        {
            AccountChart accountChartDb = db.AccountCharts.Find(accountChart.Id);
            accountChartDb.Code = accountChart.Code;
            accountChartDb.Name = accountChart.Name;
            accountChartDb.Type = accountChart.Type;
            accountChartDb.IsActive = true;
            accountChartDb.DateLastEdited = DateTime.Now;
            accountChartDb.EditedBy = new Guid(Operator().Id);

            if (ModelState.IsValid)
            {
                db.Entry(accountChartDb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accountChartDb);
        }

        // GET: AccountCharts/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountChart accountChart = db.AccountCharts.Find(id);
            if (accountChart == null)
            {
                return HttpNotFound();
            }

            ModalDelete model = new ModalDelete();
            model.Action = "Delete";
            model.Controller = "AccountCharts";
            model.Id = id.ToString();
            model.Name = accountChart.Name;
            model.IsSubmit = true;

            return PartialView("_ModalDelete", model);
            //return View(accountChart);
        }

        // POST: AccountCharts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            AccountChart accountChart = db.AccountCharts.Find(id);
            db.AccountCharts.Remove(accountChart);
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
