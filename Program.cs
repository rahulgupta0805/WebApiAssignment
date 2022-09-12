using Assignment.Service.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var configuration = new ConfigurationBuilder()
                             .AddJsonFile("appsettings.json", false, true)
                             .AddEnvironmentVariables()
                             .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
                             .Build();
var serviceTitle = "User Service";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = configuration.GetConnectionString("DbConnection");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
                option.SwaggerDoc("v1", new OpenApiInfo { Title = serviceTitle, Version = "v1" , Description = "Update user information"}));
builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString,
                dbContextOptionBuilder =>
                {
                    dbContextOptionBuilder.CommandTimeout(5 * 60);
                    dbContextOptionBuilder.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromMilliseconds(500),
                        errorNumbersToAdd: null);
                }
), ServiceLifetime.Scoped);
builder.Services.AddMediatR(typeof(Program));

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
