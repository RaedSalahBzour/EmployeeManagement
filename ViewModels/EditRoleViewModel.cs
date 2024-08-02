using System.ComponentModel.DataAnnotations;

namespace EmployeeProject.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }
        [Required(ErrorMessage ="Role Name Is Required")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
