using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TutorialApi.Models;

namespace TutorialApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:56315").
                        AllowAnyHeader().
                        AllowAnyMethod();
                });
            });
            
            services.AddMvc().AddJsonOptions(options => 
                {
                    //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    //options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }).
                AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()).
                SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(Configuration.GetConnectionString("NorthwindConnection")));
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRequestLocalization();
            app.UseCors();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
