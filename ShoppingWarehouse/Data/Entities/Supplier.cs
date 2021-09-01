using Microsoft.EntityFrameworkCore;
using ShoppingWarehouse.Resources.ErrorMessage;
using ShoppingWarehouse.Resources.Supplier;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWarehouse.Data.Entities
{
    [Index(nameof(NationalTaxPayerRegistry), IsUnique = true, Name = "IX_NationalTaxPayerRegistry")]
    public class Supplier : BaseEntity
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [StringLength(11, MinimumLength = 11, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.StringLength))] 
        [Display(Name = nameof(SupplierResource.NationalTaxPayerRegistry), ResourceType = typeof(SupplierResource))]
        public string NationalTaxPayerRegistry { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.StringLength))]
        [Display(Name = nameof(SupplierResource.CommercialName), ResourceType = typeof(SupplierResource))]
        public string CommercialName { get; set; }

        public List<Article> Articles { get; set; }
    }
}