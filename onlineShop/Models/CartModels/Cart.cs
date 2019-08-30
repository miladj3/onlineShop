using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace onlineShop.Models
{
    [NotMapped]
    public class Cart
    {
        public Cart()
        {
            Items = new Collection<CartItem>();
        }

        public double CartAmount { get => Math.Round(this.Items.Select(i=>i.Amount).Sum(), 2); }
        public int ItemCount { get => this.Items.Count; }

        public ICollection<CartItem> Items { get; set; }
    }
}
