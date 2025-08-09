using Mapster;
using Microsoft.EntityFrameworkCore;
using StarMarathon.Domain.Entities;
using StarMarathon.Infrastructure;

using DomainTask = StarMarathon.Domain.Entities.Task;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StarDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StarDbContext>();
    Console.WriteLine("CS=" + builder.Configuration.GetConnectionString("Default"));
    db.Database.EnsureCreated();
    if (!db.Employees.Any()) { SeedDemo(db); }
}

TypeAdapterConfig<DomainTask, TaskDto>.NewConfig();
TypeAdapterConfig<Notification, NotificationDto>.NewConfig();
TypeAdapterConfig<Product, ProductDto>.NewConfig();

var api = app.MapGroup("/api");

api.MapGet("/tasks", (long tgId, StarDbContext db) =>
{
    var emp = db.Employees.Include(e => e.Groups).FirstOrDefault(e => e.TgId == tgId);
    if (emp is null) return Results.NotFound();
    var grpIds = emp.Groups.Select(g => g.GroupId).ToHashSet();
    var tasks = db.Tasks
        .Include(t => t.TaskGroups)
        .Include(t => t.EmployeeStatuses)
        .Where(t => t.Language == emp.Language && t.TaskGroups.Any(g => grpIds.Contains(g.GroupId)))
        .ToList()
        .Select(t => t.Adapt<TaskDto>() with { IsCompleted = t.EmployeeStatuses.Any(s => s.EmployeeId == emp.Id && s.IsCompleted) });
    return Results.Ok(tasks);
});

api.MapGet("/notifications", (long tgId, StarDbContext db) =>
    Results.Ok(db.Employees.Where(e => e.TgId == tgId)
        .SelectMany(e => db.Notifications.Where(n => n.EmployeeId == e.Id))
        .OrderByDescending(n => n.Created)
        .ProjectToType<NotificationDto>()));

api.MapGet("/products", (StarDbContext db) => db.Products.ProjectToType<ProductDto>());

api.MapPost("/tasks/{id:guid}/complete", (Guid id, long tgId, StarDbContext db) =>
{
    var emp = db.Employees.FirstOrDefault(e => e.TgId == tgId);
    if (emp is null) return Results.NotFound();
    var st = db.EmployeeTaskStatuses.Find(emp.Id, id);
    if (st is null)
    {
        st = new EmployeeTaskStatus { EmployeeId = emp.Id, TaskId = id, IsCompleted = true, CompletedAt = DateTime.UtcNow };
        db.EmployeeTaskStatuses.Add(st);
    }
    else if (!st.IsCompleted) { st.IsCompleted = true; st.CompletedAt = DateTime.UtcNow; }
    db.SaveChanges(); return Results.Ok();
});

app.Run();

static void SeedDemo(StarDbContext db)
{
    var g = new Group("all", "Все");
    var e = new Employee(123456789, "+79990001122", "ru");
    e.Groups.Add(new EmployeeGroup { Employee = e, Group = g });
    var t = new DomainTask("Фото рабочего места", "Отправь фото стола", "ru", 100, 10, DateTime.UtcNow.AddDays(5));
    t.TaskGroups.Add(new TaskGroup { Task = t, Group = g });
    var p = new Product("Кружка Star", "Фирменная кружка", 150, "https://placehold.co/100x100");
    db.AddRange(g, e, t, p, new Notification(e.Id, "Добро пожаловать в StarMarathon!"));
    db.SaveChanges();
}

record TaskDto(Guid Id, string Name, string Description, DateTime End, int WinningReward, int ParticipationReward, bool IsCompleted);
record NotificationDto(Guid Id, string Description, bool IsRead, DateTime Created);
record ProductDto(Guid Id, string Name, string Description, int Price, string ImageUrl);
