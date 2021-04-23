using Manage_Projects_API.Data;
using Manage_Projects_API.Data.Models;
using Manage_Projects_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API
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
            services.AddDbContext<ProjectManagementContext>(option =>
            option.UseSqlServer(@"data source=.\sqlserverdev;initial catalog=nococid;
                    persist security info=true;integrated security=false;trustservercertificate=false;
                    uid=sa;password=maxsulapro0701;trusted_connection=false;multipleactiveresultsets=true;"));

            services.AddScoped<IContext<Permission>, Context<Permission>>();
            services.AddScoped<IContext<Project>, Context<Project>>();
            services.AddScoped<IContext<ProjectType>, Context<ProjectType>>();
            services.AddScoped<IContext<Role>, Context<Role>>();
            services.AddScoped<IContext<User>, Context<User>>();

            services.AddSingleton<IErrorHandlerService, ErrorHandlerService>();
            services.AddSingleton<IJwtAuthService, JwtAuthService>();

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectTypeService, ProjectTypeService>();
            services.AddScoped<IUserService, UserService>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(option => {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config => {
                config.RequireHttpsMetadata = false;
                config.SaveToken = false;
                config.TokenValidationParameters = JwtAuth.TokenValidationParameters;
            });

            services.AddCors();

            services.AddSwaggerDocument(c =>
            {
                c.DocumentName = "Project-Management-Api-Docs";
                c.Title = "Project-Management-API";
                c.Version = "v1";
                c.Description = "The Project Management API documentation description";
                c.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Copy 'Bearer ' + valid JWT token into field",
                    In = OpenApiSecurityApiKeyLocation.Header
                }));
                c.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().WithMethods("PATCH").AllowAnyHeader().Build());
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi(o => o.DocumentName = "Project-Management-Api-Docs");
            app.UseAuthentication();
            app.UseSwaggerUi3();
        }
    }
}
