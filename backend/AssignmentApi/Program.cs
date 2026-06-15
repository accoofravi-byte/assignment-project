using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using AssignmentApi.Data;
using AssignmentApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AssignmentApi.DTOs;
using AssignmentApi.Models;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        )
    );
});

builder.Services.AddScoped<JwtService>();

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAngular",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();

var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedOptions.KnownIPNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/healthz", () => Results.Ok("OK"));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();    
}

app.MapPost("/api/auth/login", (LoginDto dto, JwtService JwtService, ILogger<Program> logger) =>
{
    try
    {
        Console.WriteLine($"Received Username: {dto.Username}");
        Console.WriteLine($"Received Password: {dto.Password}");        

        var userName = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(dto.Username));
        var password = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(dto.Password));

        Console.WriteLine($"Decoded Username: {userName}");
        Console.WriteLine($"Decoded Password: {password}");

        if(userName == "admin" && password == "admin123")
        {
            var token = JwtService.GenerateToken(dto.Username);
            logger.LogInformation("User {Username} logged in successfully", dto.Username);
            return Results.Ok(new { token });
        }
        else
        {
            logger.LogWarning("Failed login attempt for user {Username}", dto.Username);
            return Results.Unauthorized();
        } 
    }
    catch (Exception)
    {
        return Results.BadRequest("Invalid credentials");
    }

      
    
}).AllowAnonymous();

app.MapGet("/api/products", async (AppDbContext db)=>
{
    return await db.Products.ToListAsync();
}).RequireAuthorization();

app.MapGet("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var products = await db.Products.FindAsync(id);
    return products is null ? Results.NotFound() : Results.Ok(products);
}).RequireAuthorization();

app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
}).RequireAuthorization();

app.MapPut("/api/products/{id}", async (int id, Product product, AppDbContext db) =>
{
    var existing = await db.Products.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.Name = product.Name;
    existing.Price = product.Price;
    existing.Quantity = product.Quantity;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.MapDelete("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var existing = await db.Products.FindAsync(id);
    if (existing is null) return Results.NotFound();

    db.Products.Remove(existing);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();
    
app.Run();