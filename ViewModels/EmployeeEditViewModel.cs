using EmployeeProject.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProject.ViewModels
{
    public class EmployeeEditViewModel : EmployeeCreateViewModel
    {
        public int Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
