using Microsoft.EntityFrameworkCore;
using StarMarathon.Application;
using StarMarathon.Application.Services;
using StarMarathon.Application.DTOs.Admin;
using StarMarathon.Infrastructure;
using StarMarathon.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

// ensure db + seed ...
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StarDbContext>();
    db.Database.EnsureCreated();
    if (!db.Employees.Any()) SeedDemo(db);
}

var api = app.MapGroup("/api");

// ====== USER-FACING (как было) ======
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

// ====== ADMIN AREA (CRUD) ======
var admin = app.MapGroup("/api/admin");

// --- Tasks ---
admin.MapGet("/tasks", async (IAdminTaskService svc, CancellationToken ct) =>
    Results.Ok(await svc.ListAsync(ct)));

admin.MapGet("/tasks/{id:guid}", async (Guid id, IAdminTaskService svc, CancellationToken ct) =>
{
    try { return Results.Ok(await svc.GetAsync(id, ct)); }
    catch (KeyNotFoundException) { return Results.NotFound("Task not found"); }
});

admin.MapPost("/tasks", async (TaskAdminCreateRequest req, IAdminTaskService svc, CancellationToken ct) =>
{
    var id = await svc.CreateAsync(req, ct);
    return Results.Created($"/api/admin/tasks/{id}", new { id });
});

admin.MapPut("/tasks/{id:guid}", async (Guid id, TaskAdminUpdateRequest req, IAdminTaskService svc, CancellationToken ct) =>
{
    try { await svc.UpdateAsync(id, req, ct); return Results.NoContent(); }
    catch (KeyNotFoundException) { return Results.NotFound("Task not found"); }
});

admin.MapDelete("/tasks/{id:guid}", async (Guid id, IAdminTaskService svc, CancellationToken ct) =>
{
    try { await svc.DeleteAsync(id, ct); return Results.NoContent(); }
    catch (KeyNotFoundException) { return Results.NotFound("Task not found"); }
});

// --- Products ---
admin.MapGet("/products", async (IAdminProductService svc, CancellationToken ct) =>
    Results.Ok(await svc.ListAsync(ct)));

admin.MapGet("/products/{id:guid}", async (Guid id, IAdminProductService svc, CancellationToken ct) =>
{
    try { return Results.Ok(await svc.GetAsync(id, ct)); }
    catch (KeyNotFoundException) { return Results.NotFound("Product not found"); }
});

admin.MapPost("/products", async (ProductAdminCreateRequest req, IAdminProductService svc, CancellationToken ct) =>
{
    var id = await svc.CreateAsync(req, ct);
    return Results.Created($"/api/admin/products/{id}", new { id });
});

admin.MapPut("/products/{id:guid}", async (Guid id, ProductAdminUpdateRequest req, IAdminProductService svc, CancellationToken ct) =>
{
    try { await svc.UpdateAsync(id, req, ct); return Results.NoContent(); }
    catch (KeyNotFoundException) { return Results.NotFound("Product not found"); }
});

admin.MapDelete("/products/{id:guid}", async (Guid id, IAdminProductService svc, CancellationToken ct) =>
{
    try { await svc.DeleteAsync(id, ct); return Results.NoContent(); }
    catch (KeyNotFoundException) { return Results.NotFound("Product not found"); }
});

app.Run();

// Seed как раньше...
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
