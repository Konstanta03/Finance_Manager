using Microsoft.AspNetCore.Identity;

namespace FinanceManager2._0.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? CreatedAt { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Settings> Settings { get; set; } = new List<Settings>();
    }
}
