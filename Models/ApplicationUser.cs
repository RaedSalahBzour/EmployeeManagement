using Microsoft.AspNetCore.Identity;

namespace EmployeeProject.Models
{
	public class ApplicationUser:IdentityUser
	{
        public string City { get; set; }
    }
}
