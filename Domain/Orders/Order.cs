using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotations;
using Domain.Contacts;
using Domain.Identity;
using Domain.Storage;

namespace Domain.Orders
{
    public class Order : BaseEntity
    {
        public int OrderId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OrderPaymentDate { get; set; } //Deadline date for payment

        [Precision(6, 2)]
        public decimal? OrderDiscountPercent { get; set; } //Discount percent for this order

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public virtual Person Client { get; set; }

        [ForeignKey(nameof(OrderType))]
        public int OrderTypeId { get; set; }
        public virtual OrderType OrderType { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual List<StoredProduct> StoredProducts { get; set; }
    }
}