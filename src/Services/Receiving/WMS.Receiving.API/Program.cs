using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WMS.BuildingBlocks.EventBus;
using WMS.BuildingBlocks.EventBus.Extensions;
using WMS.Inventory.Application.IntegrationEvents.Events;
using WMS.Receiving.API.Filters;
using WMS.Receiving.API.IntegrationEventHandlers;
using WMS.Receiving.Application.Services;
using WMS.Receiving.Domain.Repositories;
using WMS.Receiving.Infrastructure.Data;
using WMS.Receiving.Infrastructure.Data.Repositories;

namespace WMS.Receiving.API
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
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });
            
            // Configure Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WMS.Receiving.API", Version = "v1" });
            });
            
            // Add database context
            builder.Services.AddDbContext<ReceivingDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ReceivingDbContext).Assembly.FullName)));
            
            // Add repositories
            builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
            
            // Add application services
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            
            // Add event bus
            builder.Services.AddEventBus(
                builder.Configuration["EventBus:ConnectionString"],
                "wms_event_bus",
                "receiving_service",
                5);
                
            // Add integration event handlers
            builder.Services.AddTransient<InventoryItemCreatedIntegrationEventHandler>();
            
            // Add health checks
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<ReceivingDbContext>();
            
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WMS.Receiving.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();
            
            app.MapControllers();
            app.MapHealthChecks("/health");
            
            // Configure event bus
            ConfigureEventBus(app);

            app.Run();
        }
        
        private static void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            
            // Subscribe to events
            eventBus.Subscribe<InventoryItemCreatedIntegrationEvent, InventoryItemCreatedIntegrationEventHandler>();
        }
    }
}