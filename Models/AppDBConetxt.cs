using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Models
{
	public class AppDBContext : IdentityDbContext<ApplicationUser>
	{
		public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
		{
		}

		public DbSet<Employee> Employees { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder); // Ensure the default Identity configurations are applied

			// Configure primary key for IdentityUserLogin<string>
			modelBuilder.Entity<IdentityUserLogin<string>>()
				.HasKey(login => new { login.LoginProvider, login.ProviderKey });

			// Other custom configurations can be added here
			modelBuilder.Seed();

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    
	}
}
