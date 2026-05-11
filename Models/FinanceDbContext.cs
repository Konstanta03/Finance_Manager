using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FinanceManager2._0.Models
{
    public class FinanceDbContext : IdentityDbContext<ApplicationUser>
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Settings> Settings { get; set; } = null!;
        public DbSet<RecurringPayment> RecurringPayments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Таблиці
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Transaction>().ToTable("transactions");
            modelBuilder.Entity<Settings>().ToTable("settings");
            modelBuilder.Entity<RecurringPayment>().ToTable("recurring_payments");

            // 🔹 Авто-конвертер UTC для всіх DateTime
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            // Для nullable DateTime
            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            );

            // 🔹 Transaction.Date
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Date)
                .HasConversion(dateTimeConverter);

            // 🔹 RecurringPayment.StartDate
            modelBuilder.Entity<RecurringPayment>()
                .Property(r => r.StartDate)
                .HasConversion(dateTimeConverter);

            // 🔹 ApplicationUser.CreatedAt
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.CreatedAt)
                .HasConversion(nullableDateTimeConverter);

            // 🔹 Продовжуємо ваші властивості, зв'язки і колонки як раніше
            modelBuilder.Entity<Category>().Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Category>().Property(c => c.Name).HasColumnName("name");
            modelBuilder.Entity<Category>().Property(c => c.IsExpense).HasColumnName("is_expense");

            modelBuilder.Entity<Transaction>().Property(t => t.Id).HasColumnName("id");
            modelBuilder.Entity<Transaction>().Property(t => t.UserId).HasColumnName("user_id");
            modelBuilder.Entity<Transaction>().Property(t => t.CategoryId).HasColumnName("category_id");
            modelBuilder.Entity<Transaction>().Property(t => t.Amount).HasColumnName("amount");
            modelBuilder.Entity<Transaction>().Property(t => t.Note).HasColumnName("note");

            modelBuilder.Entity<Settings>().Property(s => s.Id).HasColumnName("id");
            modelBuilder.Entity<Settings>().Property(s => s.UserId).HasColumnName("user_id");
            modelBuilder.Entity<Settings>().Property(s => s.Currency).HasColumnName("currency");
            modelBuilder.Entity<Settings>().Property(s => s.Language).HasColumnName("language");
            modelBuilder.Entity<Settings>().Property(s => s.Theme).HasColumnName("theme");
            modelBuilder.Entity<Settings>().Property(s => s.SaveSettings).HasColumnName("save_settings");
            modelBuilder.Entity<Settings>().Property(s => s.ExportData).HasColumnName("export_data");

            modelBuilder.Entity<RecurringPayment>().Property(r => r.Id).HasColumnName("id");
            modelBuilder.Entity<RecurringPayment>().Property(r => r.Name).HasColumnName("name");
            modelBuilder.Entity<RecurringPayment>().Property(r => r.Amount).HasColumnName("amount");
            modelBuilder.Entity<RecurringPayment>().Property(r => r.CategoryId).HasColumnName("category_id");
            modelBuilder.Entity<RecurringPayment>().Property(r => r.UserId).HasColumnName("user_id");
            modelBuilder.Entity<RecurringPayment>().Property(r => r.Frequency).HasColumnName("frequency");
            modelBuilder.Entity<RecurringPayment>().Property(r => r.IsActive).HasColumnName("is_active");

            // 🔹 Зв’язки
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId);

            modelBuilder.Entity<Settings>()
                .HasOne(s => s.User)
                .WithMany(u => u.Settings)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecurringPayment>()
                .HasOne(r => r.User)
                .WithMany(u => u.RecurringPayments)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecurringPayment>()
                .HasOne(r => r.Category)
                .WithMany()
                .HasForeignKey(r => r.CategoryId);
        }
    }
}