using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager.Models
{
    public class PersonView
    {
        private ApplicationDbContext _db;
        private Guid _ownerId;

        public PersonView() { }
        public PersonView(ApplicationDbContext db,Guid ownerId)
        {
            _db = db;
            _ownerId = ownerId;
        }

        private ApplicationDbContext DbContext {
            get {
                if (_db == null)
                {
                    _db = new ApplicationDbContext();
                    return _db;
                }
                else
                    return _db;

            }
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Name {
            get {
                return string.Format("{0}, {1}", LastName, FirstName);
            }
        }
    
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
       
        public string LastName { get; set; }
        public string Suffix { get; set; }
        
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string WorkEmail { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? BirthDay { get; set; }
        public DateTime? MembershipDate { get; set; }
        [Required]
        public string Gender { get; set; }
        public int Envelope { get; set; }
        public string Classification { get; set; }

        public bool IsDeleted { get; set; }


        #region Foreign Key
        //*************************************************
        //Foreign Key navigation
        //*************************************************
        public ReferenceList FamilyRole { get; set; }
        public virtual FamilyView Family { get; set; }
        #endregion

        #region Helper Properties
        //*************************************************
        //Helper Properties
        //*************************************************
        public string SelectedFamilyRoleId { get; set; }
        public string SelectedFamilyId { get; set; }
        public string FamilyRoleDisplay { get; set; }

        #endregion

        #region Children
        public IEnumerable<PersonView> FamilyMembers { get; set; }

        public IEnumerable<GroupView> Groups { get; set; }
        #endregion


        #region reference list
        //*************************************************
        //reference list
        //*************************************************
        public IEnumerable<SelectListItem> FamilyRoleList {
            get
            {                
                var list = ReferenceListManager.GetFamilyRole(DbContext, SelectedFamilyRoleId).ToList();
                
                return list;
            }
        }     
        public IEnumerable<SelectListItem> FamilyList {
            get {
                var list = new SelectList(DbContext.Families.Where(x => x.OwnerGroupId == _ownerId), "Id", "Name", SelectedFamilyId).ToList();
                list.Insert(0, new SelectListItem() { Value = "CreateFamily", Text = "Create new Family (Using fullname) " });
                list.Insert(1, new SelectListItem() { Value = "--", Text = "-----------------------------------------------", Disabled = true });

                return list;
            }
        }

        #endregion
    }

    public class PersonListView : CanPerformOperation
    {
        public IEnumerable<PersonView> Items { get; set; }
    }
}