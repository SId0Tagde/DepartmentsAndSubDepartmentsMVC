using DepartmentAndSubDepartmentsMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace DepartmentAndSubDepartmentsMVC.Attribute
{
    public class NoSelfParentingAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var department = (Department)validationContext.ObjectInstance;
            // Check if the department is its own parent directly
            if (department.ParentDepartmentId == department.Id)
            {
                return new ValidationResult("A department cannot be its own parent.");
            }
            return ValidationResult.Success;
        }
    }
}
