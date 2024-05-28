using DepartmentAndSubDepartmentsMVC.Attribute;
using System.ComponentModel.DataAnnotations;

namespace DepartmentAndSubDepartmentsMVC.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department Name is required")]
        [StringLength(100, ErrorMessage = "Department Name cannot be longer than 100 characters")]
        public string Name { get; set; } = null!;
        public string Logo { get; set; } = string.Empty!;
        [NoSelfParentingAttribute]
        public int? ParentDepartmentId { get; set; } = null!;

        //To enable lazy loading, Entity Framework Core requires that your navigation properties are marked as virtual.
        //This is necessary because lazy loading is achieved by dynamically generating a proxy class at runtime that
        //overrides these virtual navigation properties. The overridden properties intercept access to the navigation
        //properties and trigger the lazy loading mechanism.      
        public virtual Department? ParentDepartment { get; set; } = null!;
        public virtual ICollection<Department>? SubDepartments { get; set; } = null!;
    }
}
