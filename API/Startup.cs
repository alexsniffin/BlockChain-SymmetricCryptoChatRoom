using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domainlogic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Models;
using StructureMap;

namespace API
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            var container = new Container();
            
            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    // Registering to allow for Interfaces to be dynamically mapped
                    _.AssemblyContainingType(typeof(Startup));
                    _.Assembly("DomainLogic");
                    _.Assembly("Models");
                    _.WithDefaultConventions();
                });
                
                config.Populate(services);
            });
                        
            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"); });

            app.UseFileServer();
            
            app.UseSignalR(route =>
            {
                route.MapHub<MessageHub>("/chat");
            });
        }
    }
}