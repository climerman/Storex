using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotations;
using Domain.Orders;

namespace Domain.Storage
{
    public class Product : BaseEntity
    {
        public int ProductId { get; set; }

        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resources.Common))]
        [MinLength(1, ErrorMessageResourceName = "FieldMinLength", ErrorMessageResourceType = typeof(Resources.Common))]
        [MaxLength(64, ErrorMessageResourceName = "FieldMaxLength", ErrorMessageResourceType = typeof(Resources.Common))]
        [Display(Name = "ProductName", ResourceType = typeof(Resources.Domain))]
        public string ProductName { get; set; }

        [MaxLength(255, ErrorMessageResourceName = "FieldMaxLength", ErrorMessageResourceType = typeof(Resources.Common))]
        public string ProductDescription { get; set; }
        
        [Precision(8, 2)]
        public decimal? ProductValue { get; set; } //Product intrinsic value (omaväärtus laos)

        public bool ProductActive { get; set; }

        [ForeignKey(nameof(ProductCategory))]
        public int ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }

        public virtual List<StoredProduct> StoredProducts { get; set; }
    }
}