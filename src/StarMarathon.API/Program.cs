using StarMarathon.Application;
using StarMarathon.Application.Services;
using StarMarathon.Infrastructure;
using StarMarathon.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StarDbContext>();
    db.Database.EnsureCreated();
    if (!db.Employees.Any()) SeedDemo(db);
}

var api = app.MapGroup("/api");

api.MapGet("/tasks", async (long tgId, ITaskService tasks, CancellationToken ct) =>
{
    try { return Results.Ok(await tasks.GetAvailableAsync(tgId, ct)); }
    catch (KeyNotFoundException) { return Results.NotFound("Employee not found"); }
});

api.MapGet("/notifications", async (long tgId, INotificationService svc, CancellationToken ct) =>
{
    try { return Results.Ok(await svc.GetAsync(tgId, ct)); }
    catch (KeyNotFoundException) { return Results.NotFound("Employee not found"); }
});

api.MapGet("/products", (IProductService svc, CancellationToken ct) => svc.GetAsync(ct));

api.MapPost("/tasks/{id:guid}/complete", async (Guid id, long tgId, ITaskService tasks, CancellationToken ct) =>
{
    try { await tasks.CompleteAsync(id, tgId, ct); return Results.Ok(); }
    catch (KeyNotFoundException) { return Results.NotFound("Employee not found"); }
});

app.Run();

static void SeedDemo(StarDbContext db)
{
    var g = new Group("all", "Все");
    var e = new Employee(123456789, "+79990001122", "ru");
    e.Groups.Add(new EmployeeGroup { Employee = e, Group = g });

    var t = new StarMarathon.Domain.Entities.Task(
        "Фото рабочего места", "Отправь фото стола", "ru", 100, 10, DateTime.UtcNow.AddDays(5));
    t.TaskGroups.Add(new TaskGroup { Task = t, Group = g });

    var p = new Product("Кружка Star", "Фирменная кружка", 150, "https://placehold.co/100x100");

    db.AddRange(g, e, t, p, new Notification(e.Id, "Добро пожаловать в StarMarathon!"));
    db.SaveChanges();
}
