using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PaginaVentasNet.Api.Common.Middleware;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;
using PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Categories;
using PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Products;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.UseCases;
using PaginaVentasNet.Api.Modules.Identity.Infrastructure;
using PaginaVentasNet.Api.Modules.Pokemon.Application.Ports;
using PaginaVentasNet.Api.Modules.Pokemon.Application.UseCases;
using PaginaVentasNet.Api.Modules.Pokemon.Infrastructure;
using Scalar.AspNetCore;

Env.Load(".env");

var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = ApiResponse<object>.Fail(
                "VALIDATION_ERROR",
                errors.First(),
                400);

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable("JWT_SECRET")!))
        };
    });

builder.Services.AddAuthorization();

// Catalog
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<GetCategoriesUseCase>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<GetProductsUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<SearchProductsUseCase>();

// Identity
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<RegisterUseCase>();
builder.Services.AddScoped<LoginUseCase>();


// Pokemon Api de prueba
builder.Services.AddHttpClient<IPokemonProvider, PokeApiAdapter>();
builder.Services.AddScoped<GetPokemonUseCase>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, _) =>
    {
        document.Info.Title = "PaginaVentasNet API";
        document.Info.Version = "v1";
        document.Info.Description = "API de la plataforma de ventas";

        document.Components ??= new();
        document.Components.SecuritySchemes.Add("Bearer", new()
        {
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Ingresa el token JWT"
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "PaginaVentasNet API";
        options.AddHttpAuthentication("Bearer", bearer =>
        {
            bearer.Token = "tu-token-aqui";
        });
    });
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();