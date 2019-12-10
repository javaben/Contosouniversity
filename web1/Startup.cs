using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace web1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<HelloMessage>(new HelloMessage()
            //{
            //    Message = " Pica"
            //});

            services.Configure<HelloMessage>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) => {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Start\r\n");
                await next();
                await context.Response.WriteAsync("\r\nEnd");
            });

            app.UseHelloWorld();

            //app.Use(async (context, next) => {
            //    await context.Response.WriteAsync("456");
            //    await next();
            //    await context.Response.WriteAsync("789");
            //});

            app.Run(async (context) => {
                    await context.Response.WriteAsync(" ");
                });

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage(); //���㪺Exception description
            //}

            ////app.UseStatusCodePages((builder) => builder.)

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }
    }
}
