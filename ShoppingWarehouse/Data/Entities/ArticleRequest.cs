using ShoppingWarehouse.Resources.ArticleRequest;
using ShoppingWarehouse.Resources.ErrorMessage;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWarehouse.Data.Entities
{
    public class ArticleRequest : BaseEntity
    {
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(ErrorMessageResource), ErrorMessageResourceName = nameof(ErrorMessageResource.InvalidRange))]
        [Display(Name = nameof(ArticleRequestResource.Quantity), ResourceType = typeof(ArticleRequestResource))]
        public int Quantity { get; set; } 

        [Display(Name = nameof(ArticleRequestResource.Employee), ResourceType = typeof(ArticleRequestResource))]
        public Employee Employee { get; set; }
        [Display(Name = nameof(ArticleRequestResource.Employee), ResourceType = typeof(ArticleRequestResource))]
        public int EmployeeId { get; set; }

        [Display(Name = nameof(ArticleRequestResource.Article), ResourceType = typeof(ArticleRequestResource))]
        public Article Article { get; set; }
        [Display(Name = nameof(ArticleRequestResource.Article), ResourceType = typeof(ArticleRequestResource))]
        public int ArticleId { get; set; }
    }
}
