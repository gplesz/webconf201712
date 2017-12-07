using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace mvcclient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //a default JWT claim típusokra 
            //('sub', 'idp', stb.) mapping törlése
            //hogy az IDS folyamatba ne szóljon bele
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAuthentication(o=>{
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = "oidc";
            }).AddCookie(o=>{
                o.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                o.Cookie.Name="is4mvcimplicitclient";
                //o.AccessDeniedPath = "/Home/AccesDenied";
            }).AddOpenIdConnect("oidc", o=>{
                //az IDS elérhetősége
                o.Authority="http://localhost:5000";
                //fejlesztéshez nem kell ssl
                o.RequireHttpsMetadata = false; 

                //Az App azonosítója az IDS config-ban
                o.ClientId = "mvc";
                o.ClientSecret="49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";

                o.ResponseType="code id_token";

                o.SignInScheme = "Cookies";

                //beállítjuk a scope-okat, amiket 
                //lekérünk/használunk a tokenben. 
                //Hogy biztosan csak ezek legyenek először mindent törlünk
                o.Scope.Clear();
                o.Scope.Add("openid");
                o.Scope.Add("profile");
                //o.Scope.Add("offline_access");
                //o.Scope.Add("api1");

                //ha true-t adunk, akkor az MVC kliens elmenti
                //a cookie-ba a tokeneket, így nagyobb lesz
                //ha nem, akkor a cookie kisebb lesz, de 
                //todo: kérdés, hogy ekkor minden körben elmegy-e az IDS-hez?
                o.SaveTokens = true;

                o.GetClaimsFromUserInfoEndpoint = true;

                //ezekkel fogjuk a tokent validálni
                //todo: ez pontosan mit jelent??
                //ezekből a claim-ekből olvassuk ki a nevet és a role-t?
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role
                };

            });
            
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
                app.UseExceptionHandler("/Home/Error");
            }

            //Ha ezt nem kapcsoljuk be, akkor 
            //a bejelentkezést nem fogja kezelni
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
