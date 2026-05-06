using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure.Data;
using UrlShortener.Core.Entities;
using UrlShortener.Core.Interfaces;
using UrlShortener.Application.Services;
using UrlShortener.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=urls.db"));

builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// --- DB auto-migrate ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();   // wwwroot

// -----------------------------------------------------------
// POST /api/shorten  → create short link
// -----------------------------------------------------------
app.MapPost("/api/shorten", async (ShortenRequest req, IUrlShortenerService svc, HttpContext ctx) =>
{
    try
    {
        var entry = await svc.ShortenAsync(req.Url, req.Alias, req.ExpiresAt);
        var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
        return Results.Ok(ToResponse(entry, baseUrl));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new ErrorResponse(ex.Message));
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new ErrorResponse(ex.Message));
    }
});

// -----------------------------------------------------------
// GET /api/links  → list all active links
// -----------------------------------------------------------
app.MapGet("/api/links", async (IUrlShortenerService svc, HttpContext ctx) =>
{
    var links = await svc.GetAllAsync();
    var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
    return Results.Ok(links.Select(l => ToResponse(l, baseUrl)));
});

// -----------------------------------------------------------
// DELETE /api/links/{code}
// -----------------------------------------------------------
app.MapDelete("/api/links/{code}", async (string code, IUrlShortenerService svc) =>
{
    var deleted = await svc.DeleteAsync(code);
    return deleted ? Results.NoContent() : Results.NotFound();
});

// -----------------------------------------------------------
// GET /{code}  → redirect (keep this LAST)
// -----------------------------------------------------------
app.MapGet("/{code}", async (string code, IUrlShortenerService svc, ILogger<Program> logger) =>
{
    logger.LogInformation("Resolving code: {Code}", code);
    if (string.IsNullOrEmpty(code) || code == "index.html" || code == "favicon.ico")
    {
        return Results.NotFound();
    }

    var entry = await svc.ResolveAsync(code);
    if (entry is null)
    {
        logger.LogWarning("Code not found: {Code}", code);
        return Results.Redirect("/?error=not_found");
    }

    logger.LogInformation("Redirecting to: {Url}", entry.OriginalUrl);
    return Results.Redirect(entry.OriginalUrl, permanent: false);
});

// SPA fallback
app.MapFallbackToFile("index.html");

app.Run();

// --- helper ---
static ShortenResponse ToResponse(ShortUrl e, string baseUrl) => new(
    e.Code,
    $"{baseUrl}/{e.Alias ?? e.Code}",
    e.OriginalUrl,
    e.Alias,
    e.ClickCount,
    e.CreatedAt,
    e.ExpiresAt
);
