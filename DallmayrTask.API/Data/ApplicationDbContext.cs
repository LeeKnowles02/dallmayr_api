using DallmayrTask.API.Enums;
using DallmayrTask.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DallmayrTask.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<TaskHistory> TaskHistories => Set<TaskHistory>();
    public DbSet<TaskPhoto> TaskPhotos => Set<TaskPhoto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).HasConversion<string>();
        });

        // Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
        });

        // Machine
        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.SerialNumber).IsUnique();
            entity.Property(m => m.MachineName).IsRequired().HasMaxLength(200);
            entity.Property(m => m.SerialNumber).IsRequired().HasMaxLength(100);
            entity.Property(m => m.MachineType).IsRequired().HasMaxLength(100);

            entity.HasOne(m => m.Customer)
                  .WithMany(c => c.Machines)
                  .HasForeignKey(m => m.CustomerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(300);
            entity.Property(t => t.TaskType).HasConversion<string>();
            entity.Property(t => t.Status).HasConversion<string>();
            entity.Property(t => t.Priority).HasConversion<string>();

            entity.HasOne(t => t.Customer)
                  .WithMany(c => c.Tasks)
                  .HasForeignKey(t => t.CustomerId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.Machine)
                  .WithMany(m => m.Tasks)
                  .HasForeignKey(t => t.MachineId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.Technician)
                  .WithMany(u => u.AssignedTasks)
                  .HasForeignKey(t => t.TechnicianId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // TaskHistory
        modelBuilder.Entity<TaskHistory>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Action).IsRequired().HasMaxLength(100);
            entity.Property(h => h.OldStatus).HasConversion<string>();
            entity.Property(h => h.NewStatus).HasConversion<string>();

            entity.HasOne(h => h.TaskItem)
                  .WithMany(t => t.History)
                  .HasForeignKey(h => h.TaskItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.ChangedBy)
                  .WithMany(u => u.TaskHistories)
                  .HasForeignKey(h => h.ChangedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskPhoto
        modelBuilder.Entity<TaskPhoto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FileName).IsRequired().HasMaxLength(300);
            entity.Property(p => p.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(p => p.ContentType).IsRequired().HasMaxLength(100);

            entity.HasOne(p => p.TaskItem)
                  .WithMany(t => t.Photos)
                  .HasForeignKey(p => p.TaskItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.UploadedBy)
                  .WithMany(u => u.TaskPhotos)
                  .HasForeignKey(p => p.UploadedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed default admin user (password: Admin@1234)
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            FullName = "System Admin",
            Email = "admin@dallmayr.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
