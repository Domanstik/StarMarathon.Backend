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
    }
}
