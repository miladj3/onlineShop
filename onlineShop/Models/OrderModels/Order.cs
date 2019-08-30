using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace onlineShop.Models
{
    public class Order
    {
        public Order()
        {
            this.Items = new List<OrderItem>();
            this.ChangeHistory = new List<OrderChangeLog>();
            this.DeliveryDetails = new DeliveryDetails();
        }

        public Order(DeliveryMethodType deliveryMethod)
        {
            this.Items = new List<OrderItem>();
            this.ChangeHistory = new List<OrderChangeLog>();
            this.DeliveryDetails = new DeliveryDetails(deliveryMethod);
        }

        [Key]
        public int Id { get; set; }

        public OrderStatus Status { get; set; }
        public AppUser Customer { get; set; }
        public string CustomerId { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        public AppUser LastModifiedBy { get; set; }
        public string LastModifiedById { get; set; }

        public double OrderAmount { get { return this.Items.Sum(i => i.Amount); } }
        public double DeliveryFee { get; set; }
        public double OrderAmountTotal { get { return (this.OrderAmount + this.DeliveryFee); } }

        public PaymentMethod PaymentMethod { get; set; }

        public DeliveryDetails DeliveryDetails { get; set; }

        public List<OrderItem> Items { get; set; }
        public virtual List<OrderChangeLog> ChangeHistory { get; set; }

        public bool HasNoAccount { get { return this.Customer == null; }  }
    }
}
