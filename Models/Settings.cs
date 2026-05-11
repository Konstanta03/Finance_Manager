using System;

namespace FinanceManager2._0.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public string Currency { get; set; } = "USD";
        public string Language { get; set; } = "uk";
        public string Theme { get; set; } = "light";
        public bool SaveSettings { get; set; } = true;
        public string ExportData { get; set; } = "";
    }
}