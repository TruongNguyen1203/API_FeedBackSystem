using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var connection = _config.GetConnectionString("DefaultConnection");
            services.AddDbContext<StoreContext>(options => options.UseSqlServer(connection));
            //for identity
            services.AddIdentity<AppUser,Role>()
            .AddEntityFrameworkStores<StoreContext>()
            .AddDefaultTokenProviders();

            //for authentication
             services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;

                })
                //adding jwt bearer
                .AddJwtBearer(options=>
                {
                    options.SaveToken=true;
                    options.RequireHttpsMetadata=false;
                    options.TokenValidationParameters= new TokenValidationParameters()
                    {
                        ValidateIssuer=true,
                        ValidateAudience=true,
                        ValidAudience=_config["JWT:ValidAudience"],
                        ValidIssuer=_config["JWT:ValidIssuer"],
                        IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]))
                    };
                });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
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
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
