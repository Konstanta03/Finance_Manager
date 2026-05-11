using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace FinanceManager2._0.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? CreatedAt { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Settings> Settings { get; set; } = new List<Settings>();
        public ICollection<RecurringPayment> RecurringPayments { get; set; } = new List<RecurringPayment>();
    }
}