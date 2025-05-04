using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

namespace WMS.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                               .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                               .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                               .AddEnvironmentVariables();
                               
            // Add Ocelot
            builder.Services.AddOcelot(builder.Configuration)
                .AddPolly()
                .AddCacheManager(x => x.WithDictionaryHandle());
                
            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            
            // Add health checks
            builder.Services.AddHealthChecks();
            
            var app = builder.Build();
            
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            
            app.UseRouting();
            app.UseAuthorization();
            
            app.MapHealthChecks("/health");
            
            // Use Ocelot
            app.UseOcelot().Wait();
            
            app.Run();
        }
    }
}