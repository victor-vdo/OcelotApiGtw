using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotApiGtw;
using OcelotApiGtw.Domain.Interfaces;
using OcelotApiGtw.Domain.Services;
using System.Text;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("configuration.json").AddEnvironmentVariables();
builder.Configuration.AddJsonFile("swaggerendpoints.json").AddEnvironmentVariables();
var authenticationProviderKey = builder.Configuration.GetSection("APISettings:APIKey").Value ?? String.Empty;

// Add services to the container.
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IOrderService, OrderService>();

builder.Services
    .AddAuthentication()
    .AddJwtBearer("order_auth_scheme", options =>
    {
        options.TokenValidationParameters = new
                                         TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey
                  (Encoding.UTF8.GetBytes(authenticationProviderKey)),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    })
    .AddJwtBearer("payment_auth_scheme", options =>
    {
        options.TokenValidationParameters = new
                                         TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey
                  (Encoding.UTF8.GetBytes(authenticationProviderKey)),
            ValidAudience = "paymentAudience",
            ValidIssuer = "paymentIssuer",
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var routes = "/";
builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x =>
        {
            x.WithDictionaryHandle();
        });

builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = routes;
});

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddOcelot(routes, builder.Environment)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger for ocelot
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ocelot API Gateway",
        Version = "v1",
        Description = "An API to perform an Gateway operations",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Soft Finan�as",
            Email = "soft.financas@softfinancas.com",
            Url = new Uri("https://softfinancas.com"),
        },
        License = new OpenApiLicense
        {
            Name = "Ocelot API LICX",
            Url = new Uri("https://example.com/license"),
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
    options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;

}).UseOcelot().Wait();

app.UseRouting();
app.MapControllers();

app.Run();