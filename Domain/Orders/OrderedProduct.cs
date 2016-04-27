﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotations;
using Domain.Storage;

namespace Domain.Orders
{
    public class OrderedProduct : BaseEntity
    {
        public int OrderedProductId { get; set; }

        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resources.Common))]
        public int OrderedProductQuantity { get; set; }
       
        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resources.Common))]
        [Precision(8, 2)]
        public decimal OrderedProductValue { get; set; } //Ordered product value(toote hind arvel / pakkumisel)

        [DataType(DataType.Date)]
        public DateTime? ProductReservedStartDate { get; set; } //Reserveeringu alguskuupäev

        [DataType(DataType.Date)]
        public DateTime? ProductReservedEndDate { get; set; } //Reserveeringu lõppkuupäev


        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}