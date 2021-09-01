using ShoppingWarehouse.Resources.Brand;
using ShoppingWarehouse.Resources.ErrorMessage;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWarehouse.Data.Entities
{
    public class Brand : BaseEntity
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.MaxLength))]
        [Display(Name = nameof(BrandResource.Description), ResourceType = typeof(BrandResource))]
        public string Description { get; set; }
        public List<Article> Articles { get; set; }
        public List<PurchaseOrder> PurchaseOrder { get; set; }
    }
}