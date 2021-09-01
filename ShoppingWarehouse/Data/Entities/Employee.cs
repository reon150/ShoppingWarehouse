using ShoppingWarehouse.Resources.Employee;
using ShoppingWarehouse.Resources.ErrorMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWarehouse.Data.Entities
{
    public class Employee : BaseEntity
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [StringLength(11, MinimumLength = 11, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.StringLength))]
        [Display(Name = nameof(EmployeeResource.IdentityCard), ResourceType = typeof(EmployeeResource))]
        public string IdentityCard { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.MaxLength))]
        [Display(Name = nameof(EmployeeResource.FirstName), ResourceType = typeof(EmployeeResource))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.MaxLength))]
        [Display(Name = nameof(EmployeeResource.LastName), ResourceType = typeof(EmployeeResource))]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = nameof(EmployeeResource.DateOfBirth), ResourceType = typeof(EmployeeResource))]
        public DateTime DateOfBirth  { get; set; }

        [Display(Name = nameof(EmployeeResource.Department), ResourceType = typeof(EmployeeResource))]
        public Department Department { get; set; }
        [Display(Name = nameof(EmployeeResource.Department), ResourceType = typeof(EmployeeResource))]
        public int DepartmentId { get; set; }

        public List<ArticleRequest> ArticleRequests { get; set; }
    }
}