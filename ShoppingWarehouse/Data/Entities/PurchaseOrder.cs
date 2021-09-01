using ShoppingWarehouse.Resources.ErrorMessage;
using ShoppingWarehouse.Resources.PurchaseOrder;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWarehouse.Data.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.InvalidRange))]
        [Display(Name = nameof(PurchaseOrderResource.Quantity), ResourceType = typeof(PurchaseOrderResource))]
        public int Quantity { get; set; }

        [Range(0.0, 999999.99, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.InvalidRange))]
        [Display(Name = nameof(PurchaseOrderResource.UnitCost), ResourceType = typeof(PurchaseOrderResource))]
        public decimal UnitCost { get; set; }

        [Display(Name = nameof(PurchaseOrderResource.Article), ResourceType = typeof(PurchaseOrderResource))]
        public Article Article { get; set; }
        [Display(Name = nameof(PurchaseOrderResource.Article), ResourceType = typeof(PurchaseOrderResource))]
        public int ArticleId { get; set; }
    }
}