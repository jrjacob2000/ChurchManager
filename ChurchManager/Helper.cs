using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager
{
    public enum ReferenceType
    {
        FamilyRole,
        Country
    }

    public static class ReferenceListManager
    {
        public static SelectList GetFamilyRole(ChurchManager.Models.ApplicationDbContext db)
        {
            var list = db.ReferenceList.Where(x => x.ReferenceType == ReferenceType.FamilyRole.ToString()).ToList();
            return new SelectList(list, "Id", "Description");
        }
        public static SelectList GetFamilyRole(ChurchManager.Models.ApplicationDbContext db, string selectedValue)
        {
            var list = db.ReferenceList.Where(x => x.ReferenceType == ReferenceType.FamilyRole.ToString()).ToList();
            return new SelectList(list, "Id", "Description", selectedValue);
        }

        public static SelectList GetCountries(ChurchManager.Models.ApplicationDbContext db)
        {
            var list = db.ReferenceList.Where(x => x.ReferenceType == ReferenceType.Country.ToString()).ToList();
            return new SelectList(list, "Id", "Description");
        }
    }

    public abstract class CanPerformOperation
    {
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }


    public static class Constants
    {
        public static class Operation
        {           
            public const string Family_Create = "Family_Create";
            public const string Family_Delete = "Family_Delete";
            public const string Family_Edit = "Family_Edit";
            public const string Family_View = "Family_View";

            public const string Person_Create = "Person_Create";
            public const string Person_Delete = "Person_Delete";
            public const string Person_Edit = "Person_Edit";
            public const string Person_View = "Person_View";

            //group
            public const string Group_Create = "Group_Create";
            public const string Group_Delete = "Group_Delete";
            public const string Group_Edit = "Group_Edit";
            public const string Group_View = "Group_View";

            //administrator
            public const string Role_Create = "Role_Create";
            public const string Role_Delete = "Role_Delete";
            public const string Role_Edit = "Role_Edit";
            public const string Role_View = "Role_View";
            public const string User_Create = "User_Create";
            public const string User_Delete = "User_Delete";
            public const string User_Edit = "User_Edit";
            public const string User_View = "User_View";
            public const string Church_Edit = "Church_Edit";

        }
        public static string RoleOperations {
            get
            {
                return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", Operation.Role_Create, Operation.Role_Delete, Operation.Role_Edit, Operation.Role_View,
                Operation.User_Create, Operation.User_Delete, Operation.User_Edit, Operation.User_View, Operation.Church_Edit);
            }
        }
    }
}