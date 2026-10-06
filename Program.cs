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
using PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Media;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

using Scalar.AspNetCore;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Ports;
using PaginaVentasNet.Api.Modules.Identity.Infrastructure.Otp;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.UseCases;
using Resend;
using PaginaVentasNet.Api.Modules.Identity.Application.Profile.UseCases;
using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.Ports;
using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Ports;
using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.UseCases;
using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.UseCases;
using PaginaVentasNet.Api.Modules.Geography.Infrastructure;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.UseCases;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Users.UseCases;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using PaginaVentasNet.Api.Modules.Cart.Application.Ports;
using PaginaVentasNet.Api.Modules.Cart.Infrastructure;
using PaginaVentasNet.Api.Modules.Cart.Application.UseCases;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Orders.Infrastructure;
using PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

Env.Load(".env");

var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    })
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
builder.Services.AddScoped<UpdateCategoryUseCase>();
builder.Services.AddScoped<GetCategoryTreeUseCase>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<SearchProductsUseCase>();
builder.Services.AddScoped<AdminSearchProductsUseCase>();

// Identity
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<RegisterUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<ChangeRolUseCase>();

// Media
builder.Services.AddScoped<IProductMediaRepository, ProductMediaRepository>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<UploadProductMediaUseCase>();
builder.Services.AddScoped<GetProductMediaUseCase>();
builder.Services.AddScoped<DeleteProductMediaUseCase>();


// Otp
builder.Services.AddResend(options =>
{
    options.ApiToken = Environment.GetEnvironmentVariable("RESEND_API_KEY")!;
});
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IEmailService, ResendEmailService>();
builder.Services.AddScoped<SendOtpUseCase>();
builder.Services.AddScoped<VerifyOtpUseCase>();

// Profile
builder.Services.AddScoped<GetProfileUseCase>();
builder.Services.AddScoped<UpdateProfileUseCase>();
builder.Services.AddScoped<UpdatePasswordUseCase>();

// Geography
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<IMunicipioRepository, MunicipioRepository>();
builder.Services.AddScoped<GetDepartamentosUseCase>();
builder.Services.AddScoped<GetMunicipiosByDepartamentoUseCase>();

// Addresses
builder.Services.AddScoped<IDireccionRepository, DireccionRepository>();
builder.Services.AddScoped<CreateDireccionUseCase>();
builder.Services.AddScoped<GetDireccionesUseCase>();
builder.Services.AddScoped<DeleteDireccionUseCase>();
builder.Services.AddScoped<SetDireccionPrincipalUseCase>();

// Cart
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<GetCartUseCase>();
builder.Services.AddScoped<AddCartItemUseCase>();
builder.Services.AddScoped<UpdateCartItemUseCase>();
builder.Services.AddScoped<RemoveCartItemUseCase>();
builder.Services.AddScoped<ClearCartUseCase>();

// Orders
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<CreateOrderUseCase>();
builder.Services.AddScoped<GetOrdersUseCase>();
builder.Services.AddScoped<CancelOrderUseCase>();
builder.Services.AddScoped<UpdateOrderStatusUseCase>();


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

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Para endpoints de autenticación — 5 intentos por minuto por IP
    options.AddFixedWindowLimiter("auth", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueLimit = 0;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Para envío de OTP — 3 intentos por minuto por IP
    options.AddFixedWindowLimiter("otp", config =>
    {
        config.PermitLimit = 3;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueLimit = 0;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Respuesta cuando se excede el límite
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync("""
            {
                "success": false,
                "statusCode": 429,
                "data": null,
                "error": {
                    "code": "RATE_LIMIT_EXCEEDED",
                    "message": "Demasiados intentos. Por favor espera un minuto antes de intentar de nuevo."
                }
            }
            """, token);
    };
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
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();
app.Run();