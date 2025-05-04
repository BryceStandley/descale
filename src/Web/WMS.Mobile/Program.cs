using descale.Web.WMS.Mobile.Services;

namespace WMS.Mobile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            
            // Add HTTP clients for microservices
            builder.Services.AddHttpClient("InventoryAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:InventoryAPI"]);
            });
            
            builder.Services.AddHttpClient("ReceivingAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ReceivingAPI"]);
            });
            
            builder.Services.AddHttpClient("PickingAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:PickingAPI"]);
            });
            
            builder.Services.AddHttpClient("ShippingAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ShippingAPI"]);
            });
            
            // Register services
            builder.Services.AddScoped<InventoryService>();
            builder.Services.AddScoped<ReceivingService>();
            builder.Services.AddScoped<PickingService>();
            builder.Services.AddScoped<ShippingService>();
            
            // Add authentication and authorization
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = builder.Configuration["IdentityUrl"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters.ValidateAudience = false;
                });
                
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "wms.api");
                });
            });
            
            // Add Blazored.LocalStorage for offline capabilities
            builder.Services.AddBlazoredLocalStorage();
            
            // Add BarcodeScannerService
            builder.Services.AddScoped<BarcodeScannerService>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            // Register service worker for PWA
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}