using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShoppingWarehouse.Resources.Article;
using ShoppingWarehouse.Resources.ErrorMessage; 

namespace ShoppingWarehouse.Data.Entities
{
    public class Article : BaseEntity
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.Required))]
        [MaxLength(100, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.MaxLength))]
        [Display(Name = nameof(ArticleResource.Description), ResourceType = typeof(ArticleResource))]
        public string Description { get; set; }

        [Range(0, 100000, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.InvalidRange))]
        [Display(Name = nameof(ArticleResource.Stock), ResourceType = typeof(ArticleResource))]
        public int Stock { get; set; }

        [Display(Name = nameof(ArticleResource.Brand), ResourceType = typeof(ArticleResource))]
        public Brand Brand { get; set; }
        [Display(Name = nameof(ArticleResource.Brand), ResourceType = typeof(ArticleResource))]
        public int BrandId { get; set; }

        [Display(Name = nameof(ArticleResource.UnitOfMeasurement), ResourceType = typeof(ArticleResource))]
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        [Display(Name = nameof(ArticleResource.UnitOfMeasurement), ResourceType = typeof(ArticleResource))]
        public int UnitOfMeasurementId { get; set; }

        [Display(Name = nameof(ArticleResource.Supplier), ResourceType = typeof(ArticleResource))]
        public Supplier Supplier { get; set; }
        [Display(Name = nameof(ArticleResource.Supplier), ResourceType = typeof(ArticleResource))]
        public int SupplierId { get; set; }

        public List<ArticleRequest> ArticleRequests { get; set; }

        public List<Article> Articles { get; set; }

        public List<PurchaseOrder> PurchaseOrders { get; set; }
    }
}