using Microsoft.OpenApi.Models;
using WMS.EventBus;
using WMS.EventBus.Extensions;
using WMS.Picking.API.IntegrationEventHandlers;
using WMS.Picking.Application.Services;
using WMS.Picking.Infrastructure.Data;
using WMS.Picking.Infrastructure.Data.Repositories;

namespace WMS.Picking.API
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WMS.Picking.API", Version = "v1" });
            });
            
            // Add database context
            builder.Services.AddDbContext<PickingDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(PickingDbContext).Assembly.FullName)));
            
            // Add repositories
            builder.Services.AddScoped<IPickingListRepository, PickingListRepository>();
            
            // Add application services
            builder.Services.AddScoped<IPickingService, PickingService>();
            
            // Add event bus
            builder.Services.AddEventBus(
                builder.Configuration["EventBus:ConnectionString"],
                "wms_event_bus",
                "picking_service",
                5);
                
            // Add integration event handlers
            builder.Services.AddTransient<InventoryStockUpdatedIntegrationEventHandler>();
            
            // Add health checks
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<PickingDbContext>();
            
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WMS.Picking.API v1"));
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
            eventBus.Subscribe<InventoryStockUpdatedIntegrationEvent, InventoryStockUpdatedIntegrationEventHandler>();
        }
    }
}