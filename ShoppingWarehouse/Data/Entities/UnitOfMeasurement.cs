using ShoppingWarehouse.Resources.ErrorMessage;
using ShoppingWarehouse.Resources.UnitOfMeasurement;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWarehouse.Data.Entities
{
    public class UnitOfMeasurement : BaseEntity
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [MaxLength(200, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.MaxLength))]
        [Display(Name = nameof(UnitOfMeasurementResource.Description), ResourceType = typeof(UnitOfMeasurementResource))]
        public string Description { get; set; }

        public List<Article> Articles { get; set; }
        public List<ArticleRequest> ArticleRequests { get; set; }
        public List<PurchaseOrder> PurchaseOrders { get; set; }
    }
}