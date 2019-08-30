using onlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace onlineShop.DTOs
{
    public class OrderDTO
    {
        public OrderDTO()
        {
            Items = new List<OrderItemDTO>();
            DeliveryDetails = new OrderDetailsDTO();
        }

        public int Id { get; set; }

        public OrderDetailsDTO DeliveryDetails { get; set; }

        public List<OrderItemDTO> Items { get; set; }

        public bool NoUserAccount { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        public string LastModifiedById { get; set; }
        public string LastModifiedByName { get; set; }


        public OrderStatus Status { get; set; }
        public double OrderAmount { get { return this.Items.Sum(i => i.Amount); } }
        public double DeliveryFee { get; set; }
        public double OrderAmountTotal { get { return (this.OrderAmount + this.DeliveryFee); } }
        public List<OrderChangeLog> ChangeHistory { get; set; }
    }
}
