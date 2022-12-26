﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.IdentityModel.Tokens;
using WebPlatform.Auth;
using WebPlatform.Models.OptionsModels;
using WebPlatform.OPCUALayer;

namespace WebPlatform
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
            //Add service related to Jwt Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JwtOptions:Issuer"],
                    ValidAudience = Configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["JwtOptions:SecurityKey"]))
                };
            });

            //Add service related to IOptions feature in Controllers
            services.AddOptions();
            services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"));
            services.Configure<OPCUAServersOptions>(Configuration.GetSection("OPCUAServersOptions"));

            services.AddMvc();

            //Register server specific for the platform
            services.AddTransient<ITokenManager, JwtManager>();
            services.AddTransient<IAuth, StubAuthenticator>();

            //Register a singleton service managing OPC UA interactions
            services.AddSingleton<IUaClientSingleton, UaClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Check for invalid HTTP requests before the MVC
            //app.UseRequestValidator();

           
            app.UseRouting();
            app.UseAuthentication();
            app.UseRefreshToken();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
             });

        }
    }
}
