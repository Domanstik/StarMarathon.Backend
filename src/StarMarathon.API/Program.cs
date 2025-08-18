using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using StarMarathon.Application;
using StarMarathon.Application.Services;
using StarMarathon.Application.DTOs.Admin;
using StarMarathon.Infrastructure;
using StarMarathon.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// --- Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "StarMarathon API", Version = "v1" });
    var jwtScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введите токен вида: Bearer {token}"
    };
    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        [jwtScheme] = Array.Empty<string>()
    });
});


// --- AuthN/AuthZ
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key missing");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StarMarathon";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "StarMarathonClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

// --- DB migrate + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StarDbContext>();
    db.Database.Migrate();
    if (!db.Employees.Any()) SeedDemo(db);
}

app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api");

// ------------ DEV AUTH (в проде замените на ваш gateway) -----------------
api.MapPost("/auth/login", async (LoginRequest req, IConfiguration cfg, StarDbContext db) =>
{
    // auto-provision сотрудника (dev-упрощение)
    var emp = await db.Employees.FirstOrDefaultAsync(e => e.TgId == req.TgId);
    if (emp is null)
    {
        var lang = string.IsNullOrWhiteSpace(req.Language) ? "ru" : req.Language.Trim().ToLowerInvariant();
        emp = new Employee(req.TgId, req.Phone ?? "+000", lang);
        await db.Employees.AddAsync(emp);
        await db.SaveChangesAsync();
    }

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var expires = DateTime.UtcNow.AddMinutes(int.TryParse(cfg["Jwt:TokenLifetimeMinutes"], out var m) ? m : 120);

    var role = string.Equals(req.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Employee";

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, req.TgId.ToString()),
        new Claim("tg_id", req.TgId.ToString()),
        new Claim(ClaimTypes.NameIdentifier, req.TgId.ToString()),
        new Claim(ClaimTypes.Name, req.Username ?? $"tg_{req.TgId}"),
        new Claim(ClaimTypes.Role, role)
    };

    var token = new JwtSecurityToken(
        issuer: cfg["Jwt:Issuer"],
        audience: cfg["Jwt:Audience"],
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: expires,
        signingCredentials: creds
    );

    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = jwt, expiresAt = expires });
});
// ------------------------------------------------------------------------

// --------- Helpers
static long? GetTgId(ClaimsPrincipal user)
{
    var s = user.FindFirstValue("tg_id") ??
            user.FindFirstValue(ClaimTypes.NameIdentifier) ??
            user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    return long.TryParse(s, out var v) ? v : null;
}

// =================== USER-FACING (требует авторизации) ===================
api.MapGet("/tasks", async (ITaskService tasks, ClaimsPrincipal user, CancellationToken ct) =>
{
    var tgId = GetTgId(user);
    if (tgId is null) return Results.Unauthorized();
    try { return Results.Ok(await tasks.GetAvailableAsync(tgId.Value, ct)); }
    catch (KeyNotFoundException) { return Results.NotFound("Employee not found"); }
}).RequireAuthorization();

api.MapGet("/notifications", async (INotificationService svc, ClaimsPrincipal user, CancellationToken ct) =>
{
    var tgId = GetTgId(user);
    if (tgId is null) return Results.Unauthorized();
    try { return Results.Ok(await svc.GetAsync(tgId.Value, ct)); }
    catch (KeyNotFoundException) { return Results.NotFound("Employee not found"); }
}).RequireAuthorization();

api.MapPost("/tasks/{id:guid}/complete", async (Guid id, ITaskService tasks, ClaimsPrincipal user, CancellationToken ct) =>
{
    var tgId = GetTgId(user);
    if (tgId is null) return Results.Unauthorized();
    try { await tasks.CompleteAsync(id, tgId.Value, ct); return Results.Ok(); }
    catch (KeyNotFoundException) { return Results.NotFound("Employee not found"); }
}).RequireAuthorization();

// Магазин можно оставить публичным (или тоже закрыть)
api.MapGet("/products", (IProductService svc, CancellationToken ct) => svc.GetAsync(ct));

// =================== ADMIN AREA (защищено ролью Admin) ===================
var admin = app.MapGroup("/api/admin").RequireAuthorization("AdminOnly");

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

api.MapPost("/tasks/{id:guid}/participate", async (Guid id, ClaimsPrincipal user,
    IUserParticipationService svc, CancellationToken ct) =>
{
    var tgId = GetTgId(user);
    if (tgId is null) return Results.Unauthorized();
    try { await svc.JoinAsync(tgId.Value, id, ct); return Results.Ok(); }
    catch (KeyNotFoundException ex) { return Results.NotFound(ex.Message); }
}).RequireAuthorization();

admin.MapGet("/tasks/{id:guid}/participants", async (Guid id,
    IAdminParticipantService svc, CancellationToken ct) =>
{
    try { return Results.Ok(await svc.GetAsync(id, ct)); }
    catch (KeyNotFoundException) { return Results.NotFound("Task not found"); }
});

app.Run();

// ===== seed =====
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

// ===== contracts =====
public record LoginRequest(long TgId, string? Username, string? Phone, string? Role, string? Language);
