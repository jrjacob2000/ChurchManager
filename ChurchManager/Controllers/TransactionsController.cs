﻿using System;
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
    public class TransactionsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index(Guid? accountRegistryId)
        {

           //var transview = (from t in db.Transactions
           //             join tl in db.TransactionLines on t.Id equals tl.TransactionId 
           //             join a in db.AccountCharts on tl.AccountId equals a.Id
           //             join f in db.AccountCharts on tl.FundId equals f.Id
           //             where tl.AccountId != t.AccountRegisterId && (t.AccountRegisterId == accountRegistryId || accountRegistryId == null)
           //             orderby t.DateEntered descending
           //              select new TransactionView
           //              {
           //                  Id = t.Id,
           //                  TransactionDate = t.TransactionDate,
           //                  Payee = t.Payee,
           //                  Comment = t.Comment,
           //                  AccountRegistryId = t.AccountRegisterId,
           //                  AccountId = tl.AccountId,
           //                  AccountName = a.Name,
           //                  AccountFundId = tl.FundId,
           //                  FundName = f.Name,
           //                  Payment = tl.Amount > 0 ? tl.Amount : (decimal?)null,
           //                  Deposit = tl.Amount < 0 ? tl.Amount *-1 : (decimal?)null,
           //              })
           //              .ToList();

            ViewBag.AccountOptions = db.AccountCharts.ToList(); //populate reference options
            ViewBag.AccountRegistryId = accountRegistryId;

            return View(new List< TransactionView>());
        }

        public JsonResult GetList(Guid? acctRegisterId, int? draw, int start, int length)
        {
            string search = Request.QueryString["search[value]"];
            var sDraw = draw;

            var transviewquery = (from t in db.Transactions
                                  join tl in db.TransactionLines on t.Id equals tl.TransactionId
                                  join a in db.AccountCharts on tl.AccountId equals a.Id
                                  join f in db.AccountCharts on tl.FundId equals f.Id
                                  where tl.AccountId != t.AccountRegisterId &&
                                  (t.AccountRegisterId == acctRegisterId || acctRegisterId == null)
                                  orderby t.DateEntered descending
                                  select new TransactionView
                                  {
                                      Id = t.Id,
                                      TransactionDate = t.TransactionDate,
                                      Payee = t.Payee,
                                      Comment = t.Comment,
                                      AccountRegistryId = t.AccountRegisterId,
                                      AccountId = tl.AccountId,
                                      AccountName = a.Name,
                                      AccountFundId = tl.FundId,
                                      FundName = f.Name,
                                      Payment = tl.Amount > 0 ? tl.Amount : (decimal?)null,
                                      Deposit = tl.Amount < 0 ? tl.Amount * -1 : (decimal?)null,
                                  });

            var transview = transviewquery
            .Skip(start)
            .Take(10)
            .ToList()
            .Select(x => new TransactionView
            {
                Id = x.Id,
                TransactionDate = x.TransactionDate,
                TransactionDateString = x.TransactionDate.Value.ToShortDateString(),
                Payee = x.Payee,
                Comment = x.Comment,
                AccountRegistryId = x.AccountRegistryId,
                AccountId = x.AccountId,
                AccountName = x.AccountName,
                AccountFundId = x.AccountFundId,
                FundName = x.FundName,
                Payment = x.Payment,
                Deposit = x.Deposit,
            });

            //ViewBag.AccountOptions = db.AccountCharts.ToList(); //populate reference options
            var result = new {
                draw = sDraw,
                recordsTotal = transviewquery.Count(),
                recordsFiltered = transviewquery.Count(),
                data = transview
            };
           
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        // GET: Transactions/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var transview = (from t in db.Transactions
                             join tl in db.TransactionLines on t.Id equals tl.TransactionId
                             join a in db.AccountCharts on tl.AccountId equals a.Id
                             join f in db.AccountCharts on tl.FundId equals f.Id
                             where tl.AccountId != t.AccountRegisterId && t.Id == id
                             select new TransactionView
                             {
                                 Id = t.Id,
                                 TransactionDate = t.TransactionDate,
                                 Payee = t.Payee,
                                 Comment = t.Comment,
                                 AccountRegistryId = t.AccountRegisterId,
                                 AccountId = tl.AccountId,
                                 AccountName = a.Name,
                                 AccountFundId = tl.FundId,
                                 FundName = f.Name,
                                 Payment = tl.Amount > 0 ? tl.Amount : tl.Amount * -1,
                                 EditedBy = t.EditedBy,
                                 DateLastEdited = t.DateLastEdited,
                                 EnteredBy = t.EnteredBy,
                                 DateEntered = t.DateEntered
                             }).FirstOrDefault();
                         

            if (transview == null)
            {
                return HttpNotFound();
            }

            return View(transview);
        }
       
        // GET: Transactions/Create
        public ActionResult Create(string accountRegistryId)
        {
            var transactionView = new TransactionView();
            if(!string.IsNullOrEmpty(accountRegistryId))
                transactionView.AccountRegistryId = new Guid( accountRegistryId);
            transactionView.AccountOptions = db.AccountCharts.ToList();
            return View(transactionView);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransactionView transactionView)
        {
            if (transactionView == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if ((transactionView.Payment.HasValue && transactionView.Deposit.HasValue) ||
              (!transactionView.Payment.HasValue && !transactionView.Deposit.HasValue))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (transactionView.AccountFundId == Guid.Empty)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

           return Upsert(null, transactionView);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(Guid? id)
        {
            //TODO:Add validation for payment its there's still a balance in fund or register account.

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Include("TransactionLines")
                .Where(x => x.Id == id).FirstOrDefault();
            if (transaction == null)
            {
                return HttpNotFound();
            }

            var tranLines = transaction.TransactionLines;

            var transview = new TransactionView();
            transview.Id = transaction.Id;
            transview.TransactionDate = transaction.TransactionDate;
            transview.Payee = transaction.Payee;
            transview.Comment = transaction.Comment;
            transview.AccountRegistryId = transaction.AccountRegisterId;

            if (tranLines.Count() == 2)//handle split (single transaction =2)
            {
                transview.AccountId = tranLines.Where(x => x.AccountId != transaction.AccountRegisterId).First().AccountId;
                transview.AccountFundId = tranLines.First().FundId;
            }

            var amount = tranLines.Where(x => x.AccountId == transaction.AccountRegisterId).First().Amount;
            if (amount > 0)
                transview.Deposit = amount;
            else
                transview.Payment = amount * -1; //diplay as positive

            transview.AccountOptions = db.AccountCharts.ToList(); //populate reference options

            return View(transview);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TransactionView transactionView)
        {
            if (transactionView == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if ((transactionView.Payment.HasValue && transactionView.Deposit.HasValue) ||
              (!transactionView.Payment.HasValue && !transactionView.Deposit.HasValue))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (transactionView.AccountFundId == Guid.Empty)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return Upsert(transactionView.Id, transactionView);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }


            ModalDelete model = new ModalDelete();
            model.Action = "Delete";
            model.Controller = "Transactions";
            model.Id = id.ToString();
            model.Name = transaction.TransactionDate.ToString();
            model.IsSubmit = true;

            return PartialView("_ModalDelete", model);
            
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                ModelState.AddModelError("","Record not found");
                return RedirectToAction("Index");
            }
            db.Transactions.Remove(transaction);
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

        private List<TransactionLine> GetDebitCreditPair(Guid transactionId, Guid creditAccountId, Guid debitAccountId, Guid fundId, decimal amount)
        {
            var lines = new List<TransactionLine>() {
                new TransactionLine(){
                    Id =Guid.NewGuid(),
                    TransactionId = transactionId,
                    AccountId = debitAccountId,
                    FundId =fundId,
                    Amount = amount //debit is always positive
                },
                 new TransactionLine(){
                    Id =Guid.NewGuid(),
                    TransactionId = transactionId,
                    AccountId = creditAccountId,
                    FundId = fundId,
                    Amount = amount * -1//credit is always negative
                },
            };

            return lines;
        }

        private ActionResult Upsert(Guid? transactionId, TransactionView transactionView)
        {
            //TODO:Add validation for payment its there's still a balance in fund or register account.

            Transaction transaction;
            if (transactionId.HasValue)
            {
                transaction = db.Transactions.Include("TransactionLines").Where(x => x.Id == transactionId).FirstOrDefault();
                if(transaction == null)
                    return HttpNotFound();
            }
            else
            {
                transaction = new Transaction();
                transaction.Id = Guid.NewGuid();
            }

            
            transaction.AccountRegisterId = transactionView.AccountRegistryId;
            transaction.TransactionDate = transactionView.TransactionDate.Value;
            transaction.Comment = transactionView.Comment;
            transaction.Payee = transactionView.Payee;
            transaction.IsClosed = false;
            transaction.Deleted = false;
            transaction.DateEntered = DateTime.Now;
            transaction.EnteredBy = new Guid(Operator().Id);
            transaction.OwnerGroupId = Operator().OwnerGroupId;


            Guid creditAcc;
            Guid debitAcc;
            decimal amount;
            if (transactionView.Payment.HasValue)
            {
                creditAcc = transactionView.AccountRegistryId;
                debitAcc = transactionView.AccountId;
                amount = transactionView.Payment.Value;
            }
            else
            {
                creditAcc = transactionView.AccountId;
                debitAcc = transactionView.AccountRegistryId;
                amount = transactionView.Deposit.Value;
            }


            var lines = GetDebitCreditPair(transaction.Id, creditAcc, debitAcc, transactionView.AccountFundId, amount);


            if (ModelState.IsValid)
            {

                if (transactionId == null)
                {
                    transaction.TransactionLines = lines;
                    db.Transactions.Add(transaction);
                }
                else
                {
                    transaction.TransactionLines.ToList().ForEach(x =>
                        db.TransactionLines.Remove(x)
                    ); 
                    
                    transaction.TransactionLines = lines;
                }

                    db.SaveChanges();
                return RedirectToAction("Index",new { accountRegistryId = transactionView.AccountRegistryId});
            }

            return View(new TransactionView() );

        }
    }
}