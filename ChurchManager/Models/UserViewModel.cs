using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;

namespace ChurchManager.Models
{
    public class UserViewModel 
    {
        public string ID { get; set; }

        private string _userName;

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [Required]
        public string UserName {
            get
            {
                if (string.IsNullOrEmpty(_userName))
                    return this.Email;

                return _userName;
            }
            set {
                _userName = value;
            }
        }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool ChangePasswordOnFirstLogin { get; set; }

        public bool IsAccountOwner { get; set; }

        public List<string> SelectedRoles { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }       

    }

    public class UserListView : CanPerformOperation
    {
        public IEnumerable<ChurchManager.Models.UserViewModel> Items { get; set; }
    }
}