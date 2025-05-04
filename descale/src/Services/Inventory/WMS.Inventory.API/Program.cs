using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WMS.Inventory.Infrastructure.Data;
using WMS.Inventory.API.Filters;
using WMS.Inventory.Application.Services;
using System.Text.Json.Serialization;

namespace WMS.Inventory.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            
            // Configure Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WMS.Inventory.API", Version = "v1" });
            });
            
            // Add database context
            builder.Services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName)));
            
            // Add repositories
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            
            // Add application services
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            
            // Add MediatR for domain events
            builder.Services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(InventoryItemCreatedEvent).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(InventoryItemCreatedEventHandler).Assembly);
            });
            
            // Add health checks
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<InventoryDbContext>();
            
            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WMS.Inventory.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();
            
            app.MapControllers();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}