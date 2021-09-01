using ShoppingWarehouse.Resources.Department;
using ShoppingWarehouse.Resources.ErrorMessage;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWarehouse.Data.Entities
{
    public class Department : BaseEntity
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.MaxLength))]
        [Display(Name = nameof(DepartmentResource.Name), ResourceType = typeof(DepartmentResource))]
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }
    }
}