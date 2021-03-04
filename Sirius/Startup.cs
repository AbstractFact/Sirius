using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4jClient;
using System;
using System.Text.Json;
using Sirius.Services.Redis;
using Sirius.Hubs;
using Sirius.Services;

namespace Sirius
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
            services.AddControllers();

            var neo4jclient = new GraphClient(new Uri("http://localhost:7474/"), "neo4j", "SiriusAdmin0");
            neo4jclient.ConnectAsync();
            services.AddSingleton<IGraphClient>(neo4jclient);

            services.AddSingleton<IRedisService, RedisService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<PersonService>();
            services.AddSingleton<SeriesService>();
            services.AddSingleton<AwardService>();
            services.AddSingleton<RoleService>();
            services.AddSingleton<AwardedService>();
            services.AddSingleton<DirectedService>();
            services.AddSingleton<UserSeriesListService>();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            }).AddMvcOptions(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CORS", builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .SetIsOriginAllowed((host) => true)
                           .AllowCredentials(); 
                });
            });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CORS");

            app.UseMvc();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("sirius");
            });
        }
    }
}
