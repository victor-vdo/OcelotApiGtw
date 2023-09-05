using OcelotApiGtw.Auth.Controllers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var authenticationProviderKey = builder.Configuration.GetSection("APISettings:APIKey").Value ?? String.Empty;
var key = Encoding.ASCII.GetBytes(authenticationProviderKey);
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IOrderService, OrderService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
