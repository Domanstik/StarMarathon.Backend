using Microsoft.EntityFrameworkCore;
using StarMarathon.Domain.Entities;
namespace StarMarathon.Infrastructure;

public sealed class StarDbContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Domain.Entities.Task> Tasks => Set<Domain.Entities.Task>();
    public DbSet<EmployeeTaskStatus> EmployeeTaskStatuses => Set<EmployeeTaskStatus>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Product> Products => Set<Product>();
    public StarDbContext(DbContextOptions<StarDbContext> opts) : base(opts) { }
    public DbSet<TaskParticipant> TaskParticipants => Set<TaskParticipant>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<EmployeeGroup>().HasKey(x => new { x.EmployeeId, x.GroupId });
        b.Entity<TaskGroup>().HasKey(x => new { x.TaskId, x.GroupId });
        b.Entity<EmployeeTaskStatus>().HasKey(x => new { x.EmployeeId, x.TaskId });
        b.Entity<EmployeeGroup>().HasOne(e => e.Employee).WithMany(e => e.Groups).HasForeignKey(e => e.EmployeeId);
        b.Entity<EmployeeGroup>().HasOne(g => g.Group).WithMany(g => g.EmployeeGroups).HasForeignKey(g => g.GroupId);
        b.Entity<TaskGroup>().HasOne(t => t.Task).WithMany(t => t.TaskGroups).HasForeignKey(t => t.TaskId);
        b.Entity<TaskGroup>().HasOne(g => g.Group).WithMany(g => g.TaskGroups).HasForeignKey(g => g.GroupId);
        b.Entity<EmployeeTaskStatus>().HasOne(s => s.Employee).WithMany(e => e.TaskStatuses).HasForeignKey(s => s.EmployeeId);
        b.Entity<EmployeeTaskStatus>().HasOne(s => s.Task).WithMany(t => t.EmployeeStatuses).HasForeignKey(s => s.TaskId);

        b.Entity<Employee>().HasIndex(e => e.TgId).IsUnique();

        b.Entity<TaskGroup>()
          .HasOne(tg => tg.Task).WithMany(t => t.TaskGroups)
          .HasForeignKey(tg => tg.TaskId)
          .OnDelete(DeleteBehavior.Cascade);

        b.Entity<EmployeeTaskStatus>()
          .HasOne(s => s.Task).WithMany(t => t.EmployeeStatuses)
          .HasForeignKey(s => s.TaskId)
          .OnDelete(DeleteBehavior.Cascade);

        b.Entity<TaskParticipant>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Task).WithMany().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.TaskId, x.EmployeeId }).IsUnique();
        });


    }
}
