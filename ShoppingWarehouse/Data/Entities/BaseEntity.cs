using ShoppingWarehouse.Resources.BaseEntity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWarehouse.Data.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        [DefaultValue(true)]
        [Display(Name = nameof(BaseEntityResource.IsActive), ResourceType = typeof(BaseEntityResource))]
        public bool IsActive { get; set; }

        [Display(Name = nameof(BaseEntityResource.CreatedDate), ResourceType = typeof(BaseEntityResource))]
        public DateTime CreatedDate { get; set; }

        [Display(Name = nameof(BaseEntityResource.LastUpdatedDate), ResourceType = typeof(BaseEntityResource))]
        public DateTime LastUpdatedDate { get; set; }
    }
}