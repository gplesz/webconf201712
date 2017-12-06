using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace testapi
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
            //services.AddMvc();
            //Nem kell a teljes MVC, elég az alap
            services.AddMvcCore()
                    //kell a válaszhoz a JSON formázás
                    .AddJsonFormatters()
                    //és a jogosultságkezelés is szükséges
                    .AddAuthorization();

            //És kell a felhasználóazonosítás,
            //amit az Identity Server végez
            services.AddAuthentication(
                IdentityServerAuthenticationDefaults
                    .AuthenticationScheme
                ).AddIdentityServerAuthentication(
                    o=>{
                        //IDS címe
                        o.Authority = "http://localhost:5000";
                        //fejlesztés van, nem kell ssl
                        o.RequireHttpsMetadata = false;
                        o.ApiName = "api1";
                        //o.ApiSecret = "secret";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
