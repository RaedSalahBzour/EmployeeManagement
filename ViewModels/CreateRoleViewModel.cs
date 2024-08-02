using System.ComponentModel.DataAnnotations;

namespace EmployeeProject.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]

        public string RoleName { get; set; }
    }
}
