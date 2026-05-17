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
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(user => user.Id);
            user.HasIndex(user => user.Email).IsUnique();
            user.Property(user => user.FullName).IsRequired().HasMaxLength(150);
            user.Property(user => user.Email).IsRequired().HasMaxLength(200);
            user.Property(user => user.PasswordHash).IsRequired();
            user.Property(user => user.Role).HasConversion<string>();
        });

        // Customer
        modelBuilder.Entity<Customer>(customer =>
        {
            customer.HasKey(customer => customer.Id);
            customer.Property(customer => customer.Name).IsRequired().HasMaxLength(200);
        });

        // Machine
        modelBuilder.Entity<Machine>(machine =>
        {
            machine.HasKey(machine => machine.Id);
            machine.HasIndex(machine => machine.SerialNumber).IsUnique();
            machine.Property(machine => machine.MachineName).IsRequired().HasMaxLength(200);
            machine.Property(machine => machine.SerialNumber).IsRequired().HasMaxLength(100);
            machine.Property(machine => machine.MachineType).IsRequired().HasMaxLength(100);

            machine.HasOne(machine => machine.Customer)
                   .WithMany(customer => customer.Machines)
                   .HasForeignKey(machine => machine.CustomerId)
                   .OnDelete(DeleteBehavior.SetNull);
        });

        // TaskItem
        modelBuilder.Entity<TaskItem>(taskItem =>
        {
            taskItem.HasKey(taskItem => taskItem.Id);
            taskItem.Property(taskItem => taskItem.Title).IsRequired().HasMaxLength(300);
            taskItem.Property(taskItem => taskItem.TaskType).HasConversion<string>();
            taskItem.Property(taskItem => taskItem.Status).HasConversion<string>();
            taskItem.Property(taskItem => taskItem.Priority).HasConversion<string>();

            taskItem.HasOne(taskItem => taskItem.Customer)
                    .WithMany(customer => customer.Tasks)
                    .HasForeignKey(taskItem => taskItem.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);

            taskItem.HasOne(taskItem => taskItem.Machine)
                    .WithMany(machine => machine.Tasks)
                    .HasForeignKey(taskItem => taskItem.MachineId)
                    .OnDelete(DeleteBehavior.SetNull);

            taskItem.HasOne(taskItem => taskItem.Technician)
                    .WithMany(technician => technician.AssignedTasks)
                    .HasForeignKey(taskItem => taskItem.TechnicianId)
                    .OnDelete(DeleteBehavior.SetNull);
        });

        // TaskHistory
        modelBuilder.Entity<TaskHistory>(taskHistory =>
        {
            taskHistory.HasKey(taskHistory => taskHistory.Id);
            taskHistory.Property(taskHistory => taskHistory.Action).IsRequired().HasMaxLength(100);
            taskHistory.Property(taskHistory => taskHistory.OldStatus).HasConversion<string>();
            taskHistory.Property(taskHistory => taskHistory.NewStatus).HasConversion<string>();

            taskHistory.HasOne(taskHistory => taskHistory.TaskItem)
                       .WithMany(taskItem => taskItem.History)
                       .HasForeignKey(taskHistory => taskHistory.TaskItemId)
                       .OnDelete(DeleteBehavior.Cascade);

            taskHistory.HasOne(taskHistory => taskHistory.ChangedBy)
                       .WithMany(user => user.TaskHistories)
                       .HasForeignKey(taskHistory => taskHistory.ChangedByUserId)
                       .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskPhoto
        modelBuilder.Entity<TaskPhoto>(taskPhoto =>
        {
            taskPhoto.HasKey(taskPhoto => taskPhoto.Id);
            taskPhoto.Property(taskPhoto => taskPhoto.FileName).IsRequired().HasMaxLength(300);
            taskPhoto.Property(taskPhoto => taskPhoto.FilePath).IsRequired().HasMaxLength(500);
            taskPhoto.Property(taskPhoto => taskPhoto.ContentType).IsRequired().HasMaxLength(100);

            taskPhoto.HasOne(taskPhoto => taskPhoto.TaskItem)
                     .WithMany(taskItem => taskItem.Photos)
                     .HasForeignKey(taskPhoto => taskPhoto.TaskItemId)
                     .OnDelete(DeleteBehavior.Cascade);

            taskPhoto.HasOne(taskPhoto => taskPhoto.UploadedBy)
                     .WithMany(user => user.TaskPhotos)
                     .HasForeignKey(taskPhoto => taskPhoto.UploadedByUserId)
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
