using Microsoft.EntityFrameworkCore;
using System;

namespace onlineShop.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string PropertyName { get; set; }
        public string PrimaryKeyValue { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime DateChanged { get; set; }
        public AppUser Employee { get; set; }
        public string EmployeeId { get; set; }
        public EntityState ChangeType { get; set; }
    }
}
