using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchManager.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace ChurchManager.Controllers
{
    public class FamilyController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Family
        //[CustomAuthorize( Operations = "Family_View")]
        //public ActionResult Index()
        //{
        //    var list = db.Families.ProjectTo<FamilyView>().ToList();
        //    return View(list);
        //}

        [CustomAuthorize(Operations = Constants.Operation.Family_View)]
        public ActionResult Index(string successMessage, string errorMessage)
        {
            // Get Error message if exists.
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
            }

            // Get Success message if exists.
            if (!string.IsNullOrEmpty(successMessage))
            {
                ViewBag.SuccessMessage = successMessage;
            }
            var ownerId = Operator().OwnerGroupId;
            var list = db.Families.Where(x => x.OwnerGroupId == ownerId).ProjectTo<FamilyView>().ToList();            
           

            var familyListModel = new FamilyListView() { Items = list };
            familyListModel.CanAdd = UserManager.CanPerform(Constants.Operation.Family_Create);
            familyListModel.CanEdit = UserManager.CanPerform(Constants.Operation.Family_Edit);
            familyListModel.CanDelete = UserManager.CanPerform(Constants.Operation.Family_Delete);

            return View(familyListModel);
        }

        // GET: Family/Details/5
        [CustomAuthorize(Operations = Constants.Operation.Family_View)]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            
            var ownerId = Operator().OwnerGroupId;
            Family familyModel = db.Families.Where(x => x.OwnerGroupId == ownerId && x.ID == id).FirstOrDefault();

            if (familyModel == null)
            {
                return HttpNotFound();
            }

            var familyView = Mapper.Map<FamilyView>(familyModel);
            familyView.FamilyMembers = db.People.Where(x => x.Family.ID == familyModel.ID).ProjectTo<PersonView>().ToList();

            var personList = db.People.Where(x => x.OwnerGroupId == ownerId)
                .ProjectTo<PersonView>().ToList()
                .Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name }).ToList();
            familyView.PersonList = personList;


            return View(familyView);
        }

        // GET: Family/Create
        [CustomAuthorize(Operations = Constants.Operation.Family_Create)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Family/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [CustomAuthorize(Operations = Constants.Operation.Family_Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Family familyModel)
        {
            if (ModelState.IsValid)
            {
                familyModel.ID = Guid.NewGuid();
                familyModel.DateEntered = DateTime.Now;
                familyModel.EnteredBy = Guid.Parse(Operator().Id);
                familyModel.OwnerGroupId = Operator().OwnerGroupId;
                db.Families.Add(familyModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(familyModel);
        }

        // GET: Family/Edit/5
        [CustomAuthorize(Operations = Constants.Operation.Family_Edit)]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }          
            var ownerId = Operator().OwnerGroupId;
            Family familyModel = db.Families.Where(x => x.OwnerGroupId == ownerId && x.ID == id).FirstOrDefault();

            if (familyModel == null)
            {
                return HttpNotFound();
            }
            return View(familyModel);
        }

        // POST: Family/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [CustomAuthorize(Operations = Constants.Operation.Family_Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Family familyModel)
        {            
            var ownerId = Operator().OwnerGroupId;
            Family dbfamilyModel = db.Families.Where(x => x.OwnerGroupId == ownerId && x.ID == familyModel.ID).FirstOrDefault();

            dbfamilyModel.Name = familyModel.Name;
            dbfamilyModel.Address1 = familyModel.Address1;
            dbfamilyModel.Address2 = familyModel.Address2;
            dbfamilyModel.City = familyModel.City;
            dbfamilyModel.State = familyModel.State;
            dbfamilyModel.Zip = familyModel.Zip;
            dbfamilyModel.Country= familyModel.Country;
            dbfamilyModel.HomePhone= familyModel.HomePhone;
            dbfamilyModel.WorkPhone= familyModel.WorkPhone;
            dbfamilyModel.CellPhone= familyModel.CellPhone;
            dbfamilyModel.Email = familyModel.Email;
            dbfamilyModel.WeddingDate = familyModel.WeddingDate;
            dbfamilyModel.SendNewsLetter= familyModel.SendNewsLetter;
            dbfamilyModel.Latitude= familyModel.Latitude;
            dbfamilyModel.Longitude = familyModel.Longitude;            
            if (ModelState.IsValid)
            {
                dbfamilyModel.DateLastEdited = DateTime.Now;
                dbfamilyModel.EditedBy = Guid.Parse( Operator().Id);
                //db.Entry(familyModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(familyModel);
        }

        // GET: Family/Delete/5
        [RenderAjaxPartialScripts]
        [CustomAuthorize(Operations = Constants.Operation.Family_Delete)]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ownerId = Operator().OwnerGroupId;
            Family familyModel = db.Families.Where(x => x.OwnerGroupId == ownerId && x.ID == id).FirstOrDefault();
            if (familyModel == null)
            {
                return HttpNotFound();
            }
            //return View(familyModel);

            //***************

            ModalDelete model = new ModalDelete();
            model.Action = "Delete";
            model.Controller = "Family";
            model.Id = id.ToString();
            model.Name = familyModel.Name;
            model.IsSubmit = true;

            return PartialView("_ModalDelete", model);
        }

        // POST: Family/Delete/5
        [CustomAuthorize(Operations = Constants.Operation.Family_Delete)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            string error = string.Empty;
            string message = string.Empty;
            Family familyModel = db.Families.Find(id);

            var containsMember = db.People.Where(x => x.Family.ID == id).Count() > 0;
            if (containsMember)
            {
                error = "You cannot delete family containing members. Please remove all members first.";
            }
            else
            {

                try
                {

                    db.Families.Remove(familyModel);
                    db.SaveChanges();
                    message = "Successfully deleted family " + familyModel.Name;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return RedirectToAction("Index", new { successMessage = message, errorMessage = error });
        }

        [RenderAjaxPartialScripts]
        public ActionResult RemoveFromFamily(Guid? id, Guid? parentId)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Family family = db.Families.Find(parentId);
            Person person = db.People.Find(id);


            if (person == null || family == null)
            {
                //return HttpNotFound();
                throw new Exception("person or family cannot be found");
            }
            //return View(person);


            ModalDelete model = new ModalDelete();
            model.Action = "RemoveFromFamily";
            model.Controller = "Family";
            model.ParentId = id.ToString(); //parentId.ToString();
            model.Id = id.ToString();
            model.ModalTitle = "Remove Confirmation";
            model.ModalMessage = "Are you sure you want to remove " + person.FirstName + " from famliy " + family.Name + "?";

            ModelState.Clear();
            return PartialView("_ModalDelete", model);


        }

        
        [CustomAuthorize(Operations = Constants.Operation.Family_Edit + "," + Constants.Operation.Person_Edit)]
        [RenderAjaxPartialScripts]
        [HttpPost, ActionName("RemoveFromFamily")]
        [ValidateAntiForgeryToken]
        public ActionResult ComfirmRemoveFromFamily(ModalDelete deleteModel)
        {
           
            if (deleteModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Include("Family").Where(x => x.Id == new Guid( deleteModel.ParentId)).FirstOrDefault();
            if (person == null)
            {
                return HttpNotFound();
            }
            person.Family = null;
            db.SaveChanges();

            Family family = db.Families.Find(new Guid(deleteModel.Id));
            var model = Mapper.Map<FamilyView>(family);
            model.FamilyMembers = db.People.Where(x => x.Family.ID == family.ID).ProjectTo<PersonView>().ToList();

            ModelState.Clear();
            return PartialView("_FamilyMembersPartial", model);

        }

        [RenderAjaxPartialScripts]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToFamily(FamilyView familymodel)
        {

            var ownerId = Operator().OwnerGroupId;

            Family family = db.Families.Find(familymodel.ID);
            Person person = db.People.Find(new Guid(familymodel.SectedPersonIdToAdd));

            //if (ModelState.IsValid)
            //{
                person.Family = family;
                db.SaveChanges();
            //}

            
            var model = Mapper.Map<FamilyView>(family);
            model.FamilyMembers = db.People.Where(x => x.Family.ID == family.ID).ProjectTo<PersonView>().ToList();


            ModelState.Clear();
            return PartialView("_FamilyMembersPartial", model);
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
