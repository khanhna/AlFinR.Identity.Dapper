using System.Data;
using System.Data.SqlClient;
using AspNetCore.Identity.Dapper.SqlServer;
using AspNetCore.Identity.Dapper.SqlServer.Repositories;
using Demo.Identity.Dapper.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo.Identity.Dapper
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
            #region Identity Dapper snippet

            services.AddTransient<IDbConnection, SqlConnection>(e =>
                new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddUserStore<UserStoreDapper<ApplicationUser, ApplicationRole, int>>()
                .AddRoleStore<RoleStoreDapper<ApplicationRole, int>>()
                .AddDefaultTokenProviders();

            services
                .AddTransient<IIdentityUserDapperRepository<ApplicationUser, int>,
                    IdentityUserDapperRepository<ApplicationUser, int>>();
            services
                .AddTransient<IIdentityUserLoginDapperRepository<ApplicationUser, int, ApplicationUserLogin>,
                    IdentityUserLoginDapperRepository<ApplicationUser, int, ApplicationUserLogin>>();
            services
                .AddTransient<IIdentityUserClaimDapperRepository<ApplicationUser, int, ApplicationUserClaim>,
                    IdentityUserClaimDapperRepository<ApplicationUser, int, ApplicationUserClaim>>();
            services
                .AddTransient<IIdentityUserTokenDapperRepository<ApplicationUser, int, ApplicationUserToken>,
                    IdentityUserTokenDapperRepository<ApplicationUser, int, ApplicationUserToken>>();
            services
                .AddTransient<IIdentityRoleDapperRepository<ApplicationRole, int>,
                    IdentityRoleDapperRepository<ApplicationRole, int>>();
            services
                .AddTransient<IIdentityUserRoleDapperRepository<ApplicationUser, int, ApplicationUserRole>,
                    IdentityUserRoleDapperRepository<ApplicationUser, int, ApplicationUserRole>>();
            services
                .AddTransient<IIdentityRoleClaimDapperRepository<ApplicationRole, int, ApplicationRoleClaim>,
                    IdentityRoleClaimDapperRepository<ApplicationRole, int, ApplicationRoleClaim>>();

            //Setup Stores
            services
                .AddTransient<IUserStore<ApplicationUser>, UserStoreDapper<ApplicationUser, ApplicationRole, int,
                    ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken,
                    ApplicationRoleClaim>>();
            services
                .AddTransient<IRoleStore<ApplicationRole>,
                    RoleStoreDapper<ApplicationRole, int, ApplicationUserRole, ApplicationRoleClaim>>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;
            });
            #endregion

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
