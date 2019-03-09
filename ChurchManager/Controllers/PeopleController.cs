using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChurchManager.Models;

namespace ChurchManager.Controllers
{
    public class PeopleController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: People 
        [CustomAuthorize(Operations = Constants.Operation.Person_View)]
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
            var list = (from p in db.People.Include("Family")
                        .Where(x => x.OwnerGroupId == ownerId)
                       select new PersonView()
                            {
                                Id = p.Id,
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                Family = p.Family != null ? new FamilyView() { ID = p.Family.ID, Name = p.Family.Name } : null,
                                CellPhone = p.CellPhone,
                                BirthDay = p.BirthDay
                            }).ToList();

            var personListModel = new PersonListView() { Items = list };
            personListModel.CanAdd = UserManager.CanPerform(Constants.Operation.Person_Create);
            personListModel.CanEdit = UserManager.CanPerform(Constants.Operation.Person_Edit);
            personListModel.CanDelete = UserManager.CanPerform(Constants.Operation.Person_Delete);
            return View(personListModel);
        }

        // GET: People/Details/5
        [CustomAuthorize(Operations = Constants.Operation.Person_View)]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ownerId = Operator().OwnerGroupId;
            Person person = db.People.Where(x => x.OwnerGroupId == ownerId && x.Id == id).FirstOrDefault();
            var personView = Mapper.Map<PersonView>(person);
            if(person.Family != null)
                personView.FamilyMembers = db.People.Where(x => x.Family.ID == person.Family.ID && x.Id !=id).ProjectTo<PersonView>().ToList();
            if (person.Groups != null)
                personView.Groups = person.Groups.Where(x => x.IsActive).Select(s => new GroupView() { Id = s.Id, Name = s.Name }).ToList();
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(personView);
        }

        // GET: People/Create
        [CustomAuthorize(Operations = Constants.Operation.Person_Create)]
        public ActionResult Create()
        {
            var personView = new PersonView(db,Operator().OwnerGroupId);
            //personView.FamilyList = new SelectList(db.Families, "Id", "Name", personView.SelectedFamilyId).ToList();
            return View(personView);
        }

        // POST: People/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.Person_Create)]
        public ActionResult Create( PersonView personView)
        {
            var person = Mapper.Map<Person>(personView);
            person.FamilyRole = db.ReferenceList.Find(personView.SelectedFamilyRoleId);
                          
            
            if (personView.SelectedFamilyId == "CreateFamily")
            {
                var family = new Family();
                family.ID = Guid.NewGuid();
                family.Name = string.Format("{0}, {1}", person.LastName, person.FirstName);
                family.OwnerGroupId = Operator().OwnerGroupId;
                family.HomePhone = person.HomePhone;
                family.Address1 = person.Address1;
                family.Address2 = person.Address2;
                family.City = person.City;
                family.Country = person.Country;
                family.DateEntered = DateTime.Now;
                family.EnteredBy = new Guid(Operator().Id);

                db.Families.Add(family);
                person.Family = family;
            }
            else
            {
                person.Family = db.Families.Find(new Guid(personView.SelectedFamilyId));
            }

            if (ModelState.IsValid)
            {
                person.Id = Guid.NewGuid();
                person.DateEntered = DateTime.Now;
                person.EnteredBy = Guid.Parse( Operator().Id);
                person.OwnerGroupId = Operator().OwnerGroupId;
                db.People.Add(person);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(person);
        }

        // GET: People/Edit/5
        [CustomAuthorize(Operations = Constants.Operation.Person_Edit)]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ownerId = Operator().OwnerGroupId;
            Person person = db.People.Where(x => x.OwnerGroupId == ownerId && x.Id == id).FirstOrDefault();

            var personView = new PersonView(db,ownerId);
            personView = Mapper.Map<Person,PersonView>(person, personView);
            
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(personView);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.Person_Edit)]
        public ActionResult Edit( PersonView person)
        {
            var ownerId = Operator().OwnerGroupId;

            var dbPerson = db.People
                .Include("Family")
                .Include("FamilyRole")
                .Where(x => x.OwnerGroupId == ownerId && x.Id == person.Id).FirstOrDefault();

            if (dbPerson == null)
                throw new Exception("Cannot find the person");

            //dbPerson = Mapper.Map<Person>(person);
            dbPerson = Mapper.Map< PersonView, Person>(person, dbPerson);
            dbPerson.FamilyRole = db.ReferenceList.Find(person.SelectedFamilyRoleId);
                            
            if (person.SelectedFamilyId == "CreateFamily")
            {
                var family = new Family();
                family.ID = Guid.NewGuid();
                family.Name = string.Format("{0}, {1}", person.LastName, person.FirstName);
                family.OwnerGroupId = Operator().OwnerGroupId;
                family.HomePhone = person.HomePhone;
                family.Address1 = person.Address1;
                family.Address2 = person.Address2;
                family.City = person.City;
                family.Country = person.Country;
                family.DateEntered = DateTime.Now;
                family.EnteredBy = new Guid(Operator().Id);

                db.Families.Add(family);
                dbPerson.Family = family;
            }
            else if(!string.IsNullOrEmpty(person.SelectedFamilyId))
            {
                dbPerson.Family = db.Families.Where(x => x.OwnerGroupId == ownerId && x.ID == new Guid(person.SelectedFamilyId)).FirstOrDefault();
            }

            dbPerson.DateLastEdited = DateTime.Now;
            dbPerson.EditedBy = Guid.Parse(Operator().Id);
            //db.Entry(person).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        
        // GET: People/Delete/5
        [CustomAuthorize(Operations = Constants.Operation.Person_Delete)]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            //return View(person);


            ModalDelete model = new ModalDelete();
            model.Action = "Delete";
            model.Controller = "People";
            model.Id = id.ToString();
            model.Name =string.Format("{0}, {1}",person.LastName, person.FirstName);
            model.IsSubmit = true;

            return PartialView("_ModalDelete", model);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.Person_Delete)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            string error = string.Empty;
            string message = string.Empty;
            try
            {
                Person person = db.People.Find(id);
                db.People.Remove(person);
                db.SaveChanges();
                message = string.Format("Deleted successfully the person: {0}, {1}", person.LastName, person.FirstName);
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return RedirectToAction("Index", new { successMessage = message, errorMessage = error });
        }

        [RenderAjaxPartialScripts]
        public ActionResult RemoveFromGroup(Guid? id, Guid? parentId)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(parentId);
            Group group = db.Groups.Find(id);
            if (person == null || group ==null)
            {
                //return HttpNotFound();
                throw new Exception("person or group cannot be found");
            }
            //return View(person);


            ModalDelete model = new ModalDelete();
            model.Action = "RemoveFromGroup";
            model.Controller = "People";
            model.ParentId = parentId.ToString();
            model.Id = id.ToString();
            model.ModalTitle = "Remove Confirmation";
            model.ModalMessage = "Are you sure you want to remove " + person.FirstName + " from group " + group.Name + "?";

            return PartialView("_ModalDelete", model);


        }

        [RenderAjaxPartialScripts]
        [HttpPost, ActionName("RemoveFromGroup")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmRemoveFromGroup(Guid id, Guid parentId)
        {
            Person person = db.People.Find(parentId);
            Group group = db.Groups.Find(id);
            person.Groups.Remove(group);

            db.SaveChanges();

            var model = Mapper.Map<PersonView>(person);

            //return RedirectToAction("Details/" + parentId);
            return PartialView("_PersonGroupsPartial", model);
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
