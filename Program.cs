using EmployeeProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
namespace MyMvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            // Add MVC services to the container
            builder.Services.AddControllersWithViews(options => {
                var policy =
                new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                });
            
            builder.Services.AddAuthorization(options =>
            {
                


                 options.AddPolicy("AdminRolePolicy", policy =>
                policy.RequireAssertion(context => context.User.IsInRole("Admin") ||
                context.User.IsInRole("Super Admin")));

                options.AddPolicy("CreateRolePolicy", policy =>
                policy.RequireAssertion(context => context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Create Role" && claim.Value == "true") ||
                context.User.IsInRole("Super Admin")));

                options.AddPolicy("EditRolePolicy", policy =>
                policy.RequireAssertion(context => context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                context.User.IsInRole("Super Admin")));

                options.AddPolicy("DeleteRolePolicy", policy =>
                policy.RequireAssertion(context => context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Delete Role" && claim.Value == "true") ||
                context.User.IsInRole("Super Admin")));
            });
            //to change the default access denied path
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            builder.Services.AddRazorPages();
            builder.Services.AddDbContextPool<AppDBContext>(options=>
            options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDBConnection")));
       


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();
            var app = builder.Build();
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
			app.MapControllers();


			app.Run();
        }
    }
}
