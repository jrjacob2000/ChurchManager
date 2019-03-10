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
    [Authorize]
    public class GroupsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Groups
        [CustomAuthorize(Operations = Constants.Operation.Group_View)]
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
            return View(db.Groups.Where(x => x.OwnerGroupId == ownerId).ToList());
        }

        // GET: Groups/Details/5
        [CustomAuthorize(Operations = Constants.Operation.Group_View)]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            //var list = new List<Group>();
            //return View(list);

            
            var model = db.Groups.Where(x => x.Id == id).ProjectTo<GroupView>().FirstOrDefault(); // Or get model
            var ownerId = Operator().OwnerGroupId;
            var personList = db.People.Where(x => x.OwnerGroupId == ownerId)
                .ProjectTo<PersonView>().ToList()
                .Select(s => new SelectListItem() {Value = s.Id.ToString(), Text=s.Name}).ToList();
            model.PersonList = personList;

            

            var members = group.Persons.Select(x => new GroupMemberRow() { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName }).ToList();
            model.Members = members;
            return View(model);
        }
       

        // GET: Groups/Create
        [CustomAuthorize(Operations = Constants.Operation.Group_Create)]
        public ActionResult Create()
        {
            var group = new Group() { IsActive = true };
            return View(group);
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.Group_Create)]
        public ActionResult Create( Group group)
        {
            group.IsActive = true;
            group.DateEntered = DateTime.Now;
            group.EnteredBy = new Guid(Operator().Id);
            group.OwnerGroupId = Operator().OwnerGroupId;
            if (ModelState.IsValid)
            {
                group.Id = Guid.NewGuid();
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        [CustomAuthorize(Operations = Constants.Operation.Group_Edit)]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.Group_Edit)]
        public ActionResult Edit( Group group)
        {
            var dbGroup = db.Groups.Find(group.Id);
            dbGroup.Name = group.Name;
            dbGroup.Description = group.Description;
            dbGroup.DateLastEdited = DateTime.Now;
            dbGroup.EditedBy = new Guid(Operator().Id);
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        [CustomAuthorize(Operations = Constants.Operation.Group_Delete)]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            //return View(group);

            ModalDelete model = new ModalDelete();
            model.Action = "Delete";
            model.Controller = "Groups";
            model.Id = id.ToString();
            model.Name = group.Name;
            model.IsSubmit = true;

            return PartialView("_ModalDelete", model);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.Group_Delete)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            string error = string.Empty;
            string message = string.Empty;
            try
            {
                Group group = db.Groups.Find(id);
                db.Groups.Remove(group);
                db.SaveChanges();
                message = string.Format( "Successfully deleted the group {0}.", group.Name );
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return RedirectToAction("Index", new { successMessage = message, errorMessage = error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //------------------------------------------------------
        [RenderAjaxPartialScripts]
        [HttpPost]
        [CustomAuthorize(Operations = Constants.Operation.Group_Edit)]
        public ActionResult Add(GroupView model)
        {

            var ownerId = Operator().OwnerGroupId;

            var group = db.Groups.Find(model.Id);
            var person = db.People.Find(new Guid(model.SectedPersonIdToAdd));

            if (ModelState.IsValid)
            {
                group.Persons.Add(person);
                db.SaveChanges();
            }

            var members = group.Persons.Select(x => 
                                                new GroupMemberRow() {
                                                    Id = x.Id,
                                                    FirstName = x.FirstName,
                                                    LastName = x.LastName }
                                                ).ToList();



            model.Members = members;
                       

            ModelState.Clear();
            return PartialView("_GroupMember",model);
        }

        [RenderAjaxPartialScripts]
        [HttpPost]
        [CustomAuthorize(Operations = Constants.Operation.Group_Edit)]
        public ActionResult Remove(GroupView model)
        {
            var memberToDelete = model.Members.Where(x => x.IsDeleted).FirstOrDefault();

            var group = db.Groups.Find(model.Id);
            var person = db.People.Find(memberToDelete.Id);
            ModelState.Remove("SectedPersonIdToAdd");
            if (ModelState.IsValid)
            {
                group.Persons.Remove(person);
                db.SaveChanges();
            }

            var members = group.Persons.Select(x =>
                                               new GroupMemberRow()
                                               {
                                                   Id = x.Id,
                                                   FirstName = x.FirstName,
                                                   LastName = x.LastName
                                               }).ToList();

            model.Members = members;
            ModelState.Clear();
            return PartialView("_GroupMember", model);
        }

       
    }
}
