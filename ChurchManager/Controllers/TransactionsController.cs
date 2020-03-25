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
    [Authorize]
    public class TransactionsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index(Guid? accountRegistryId)
        {
            
            ViewBag.AccountOptions = db.AccountCharts.ToList(); //populate reference options
            if(accountRegistryId != null && accountRegistryId.HasValue)
                ViewBag.AccountRegistryId = accountRegistryId;

            return View(new List<TransactionListView>());
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
                                  group new { t, tl, a, f } by new { Id = t.Id } into tgrp
                                  select new 
                                  {
                                      Id = tgrp.Key.Id,
                                      TransactionDate = tgrp.FirstOrDefault().t.TransactionDate,
                                      Payee = tgrp.FirstOrDefault().t.Payee,
                                      Comment = tgrp.FirstOrDefault().t.Comment,
                                      AccountRegistryId = tgrp.FirstOrDefault().t.AccountRegisterId,
                                      AccountId = tgrp.Count() > 1 ? (Guid?)null : tgrp.FirstOrDefault().tl.AccountId ,
                                      AccountName = tgrp.Count() > 1 ? "- split -" : tgrp.FirstOrDefault().a.Name ,
                                      AccountFundId = tgrp.Count() > 1 ? (Guid?)null : tgrp.FirstOrDefault().tl.FundId ,
                                      FundName = tgrp.Count() > 1 ?  "- split -" : tgrp.FirstOrDefault().f.Name ,
                                      Payment = tgrp.Sum(s => s.tl.Amount) > 0 ? tgrp.Sum(s => s.tl.Amount) : (decimal?)null,
                                      Deposit = tgrp.Sum(s => s.tl.Amount) < 0 ? tgrp.Sum(s => s.tl.Amount) * -1 : (decimal?)null,
                                      DateCreated = tgrp.FirstOrDefault().t.DateEntered
                                  });



            var transview = transviewquery
            .OrderByDescending(o => o.DateCreated)
            .Skip(start)
            .Take(10)
            .ToList()
            .Select(x => new TransactionListView
            {
                Id = x.Id,
                TransactionDate = x.TransactionDate.ToShortDateString(),
                Payee = x.Payee,
                Comment = x.Comment,
                AccountRegistryId = x.AccountRegistryId,
                AccountName = x.AccountName,
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
                             join ta in db.AccountCharts on t.AccountRegisterId equals ta.Id
                             join ue in db.Users on t.EnteredBy.ToString() equals ue.Id
                             join uu in db.Users on t.EditedBy.ToString() equals uu.Id
                             where tl.AccountId != t.AccountRegisterId && t.Id == id
                             group new { t, tl, a, f, ta, ue,uu } by new { Id = t.Id } into tgrp
                             select new TransactionDetailView
                             {
                                 Id = tgrp.Key.Id,
                                 TransactionDate = tgrp.FirstOrDefault().t.TransactionDate,
                                 Payee = tgrp.FirstOrDefault().t.Payee,
                                 Comment = tgrp.FirstOrDefault().t.Comment,
                                 AccountRegistryName = tgrp.FirstOrDefault().ta.Name,
                                 AccountName = tgrp.FirstOrDefault().a.Name,
                                 FundName = tgrp.FirstOrDefault().f.Name,
                                 Payment = tgrp.Sum(x => x.tl.Amount) > 0 ? tgrp.Sum(x => x.tl.Amount) : (decimal?)null,
                                 Deposit = tgrp.Sum(x => x.tl.Amount) > 0 ? (decimal?)null : tgrp.Sum(x => x.tl.Amount) * -1,
                                 EditedBy = tgrp.FirstOrDefault().uu.UserName ,
                                 DateLastEdited = tgrp.FirstOrDefault().t.DateLastEdited,
                                 EnteredBy = tgrp.FirstOrDefault().ue.UserName,
                                 DateEntered = tgrp.FirstOrDefault().t.DateEntered
                             }).FirstOrDefault();


            if (transview == null)
            {
                return HttpNotFound();
            }

            return View(transview);
        }
       
        // GET: Transactions/Create
        public ActionResult Create(Guid? accountRegistryId,DateTime? transDate, decimal? pAmout,decimal? dAmount,string payee)
        {
            try
            {
                var transactionView = new TransactionView();
                if (accountRegistryId != null && accountRegistryId.HasValue && accountRegistryId != Guid.Empty)
                    transactionView.AccountRegistryId = accountRegistryId.Value;

                if (transDate != null && transDate.HasValue)
                    transactionView.TransactionDate = transDate;

                if (pAmout != null && pAmout.HasValue)
                    transactionView.Payment = pAmout;

                if (dAmount != null && dAmount.HasValue)
                    transactionView.Deposit = dAmount;

                transactionView.Payee = payee;

                transactionView.AccountOptions = db.AccountCharts.ToList();
                transactionView.Splits = new List<Split>();
                return View(transactionView);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //// POST: Transactions/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(TransactionView transactionView)
        //{
        //    if (transactionView == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    if ((transactionView.Payment.HasValue && transactionView.Deposit.HasValue) ||
        //      (!transactionView.Payment.HasValue && !transactionView.Deposit.HasValue))
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    //if (transactionView.AccountFundId == Guid.Empty)
        //    //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //   var result = Upsert( transactionView);

        //    return View(result);
        //}

        [HttpPost]
        public JsonResult TransactionUpsert(TransactionView transactionView)
        {
            if (transactionView == null)
                return new JsonResult { Data = new { status = 400 } };

            if ((transactionView.Payment.HasValue && transactionView.Deposit.HasValue) ||
              (!transactionView.Payment.HasValue && !transactionView.Deposit.HasValue))
                return new JsonResult { Data = new { status = 400 } };

            List<string> validationErrors = new List<string>();
            var data= Upsert(transactionView, out validationErrors);

            return new JsonResult { Data = new { status = data != null ? 200 : 500, errors = validationErrors } };
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(Guid? id)
        {
            //TODO:Add validation for payment if there's still a balance in fund or register account.

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
            transview.Splits = transaction.TransactionLines
                                .Where(y => y.AccountId != transaction.AccountRegisterId)
                                .Select(x => new Split() { 
                                    Id = x.Id,
                                    SplitAccountId = x.AccountId,
                                    SplitAccountFundId = x.FundId,
                                    SplitAmount = x.Amount > 0 ? x.Amount :  x.Amount *-1
                                }).ToList();



            var amount = tranLines.Where(x => x.AccountId == transaction.AccountRegisterId).Sum(s => s.Amount);
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

            List<string> errors = new List<string>();
            var data =  Upsert( transactionView,out errors);

            return RedirectToAction("Index", new { accountRegistryId = data.AccountRegistryId });
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

        [HttpGet]
        public ActionResult Split(int index)
        {
            ViewBag.Index = index;
            var model = new Split();
            model.index = index;
            model.AccountOptions = db.AccountCharts.ToList();

            
            return PartialView("_Split", model);

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

        private TransactionView Upsert(TransactionView transactionView,out List<string> errors )
        {
            errors = new List<string>();
            //TODO:Add validation for payment its there's still a balance in fund or register account.
            var transactionId = transactionView.Id;
            Transaction transaction;
            if (transactionId != null && transactionId != Guid.Empty)
            {
                transaction = db.Transactions.Include("TransactionLines").Where(x => x.Id == transactionId).FirstOrDefault();
                if (transaction == null)
                {
                    return null;
                }

                transaction.DateLastEdited = DateTime.Now;
                transaction.EditedBy = new Guid(Operator().Id);
            }
            else
            {
                if (transactionView.Splits.Count() == 0)
                    errors.Add("At least 1 line is required");
                transaction = new Transaction();
                transaction.Id = Guid.NewGuid();
                transaction.DateEntered = DateTime.Now;
                transaction.EnteredBy = new Guid(Operator().Id);
            }

            try
            {

                transaction.AccountRegisterId = transactionView.AccountRegistryId;
                transaction.TransactionDate = transactionView.TransactionDate.Value;
                transaction.Comment = transactionView.Comment;
                transaction.Payee = transactionView.Payee;
                transaction.IsClosed = false;
                transaction.Deleted = false;
                transaction.OwnerGroupId = Operator().OwnerGroupId;

                var lines = new List<TransactionLine>();
                foreach(var item in transactionView.Splits)              
                {
                    if (transactionView.Deposit == null && transactionView.Payment == null)
                        errors.Add("amount cannot be empty");

                    if (item.SplitAccountId == Guid.Empty || transactionView.AccountRegistryId == Guid.Empty || item.SplitAccountFundId == Guid.Empty)
                        errors.Add("Either Account Registry, Account, or Fund are invalid");

                    bool isDeposit = transactionView.Deposit.HasValue;
                    Guid creditId = isDeposit ? item.SplitAccountId : transactionView.AccountRegistryId;
                    Guid debitId = isDeposit ? transactionView.AccountRegistryId : item.SplitAccountId ;
                    decimal amount = item.SplitAmount;

                    var creditAccount = db.AccountCharts.Find(creditId);
                    var debitAccount = db.AccountCharts.Find(debitId);

                    decimal assetBal = 0;
                    if (creditAccount.Type == AccountChartTypeEnum.Asset)
                        assetBal = GetAccountBalance(creditId, item.SplitAccountFundId);
                    
                    decimal liabilityBal = 0;
                    if (debitAccount.Type == AccountChartTypeEnum.Liability)
                        liabilityBal = GetAccountBalance(debitId, item.SplitAccountFundId);

                    if (debitAccount.Type == AccountChartTypeEnum.Liability)
                    {
                        if (liabilityBal < amount)
                            errors.Add(string.Format("You are over paying to your {0}. You current balance is only {1}.", debitAccount.Name, liabilityBal));                       
                    }

                    if (creditAccount.Type == AccountChartTypeEnum.Asset )
                    {
                        if (assetBal < amount)
                            errors.Add(string.Format("You only have {0} in your fund in {1}. its not enough for the payment of {2}", assetBal, creditAccount.Name, amount));
                    }


                    if (errors.Count() == 0)
                    {
                        var itemLines = GetDebitCreditPair(transaction.Id, creditId, debitId, item.SplitAccountFundId, amount);
                        lines.AddRange(itemLines);
                    }
                    
                };


                if (ModelState.IsValid && errors.Count() == 0)
                {

                    if (transactionId == null || transactionId == Guid.Empty)
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
                    return transactionView;
                }
            }
            catch (Exception ex)
            { 
                return null;
            }

            return null; 

        }

        private decimal GetAccountBalance(Guid registryAccountId, Guid fundId)
        {
            //Todo: registry account validation: should be assets and liability only

            var query = (from tl in db.TransactionLines
                         join a in db.AccountCharts on tl.AccountId equals a.Id
                         join f in db.AccountCharts on tl.FundId equals f.Id
                         where f.Id == fundId && a.Id == registryAccountId
                         //group new { tl, a, f } by new { Fund = f.Id, Account = a.Id } into grp
                         select a.Type == AccountChartTypeEnum.Liability ? tl.Amount * -1 : tl.Amount).ToList();

            return query == null? 0 : query.Sum();
        }

      

    }
}
