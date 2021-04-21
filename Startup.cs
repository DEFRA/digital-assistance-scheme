using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SupplyChain.ClientApplication.Service;
using SupplyChain.ClientApplication.Service.Interface;

namespace SupplyChain.ClientApplication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHttpContextAccessor();
            services.AddScoped<IDefraAuthenticationService, DefraAuthenticationService>();
            services.AddScoped<IExportHealthCertificatesService, ExportHealthCertificatesService>();
            services.AddScoped<IReferenceDataService, ReferenceDataService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}