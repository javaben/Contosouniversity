using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using web2.Models;

namespace web2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { 
            //services.AddControllers(); //for web api,View ���Q Container �޲z�A�L�k�ϥΪ`�J
            services.AddControllersWithViews(); //for Mvc ���U Controller & View

            services.Configure<Profile>(Configuration.GetSection("Profile"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //internal request
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts(); //�]�w�����u�ਫ Https�AProduction ����ĳ�ϥ�
            }
            app.UseHttpsRedirection(); // �j�������s�� Https

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthorization(); //�]�w���v

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapDefaultControllerRoute();
                //endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
