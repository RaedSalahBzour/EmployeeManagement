﻿using System.ComponentModel.DataAnnotations;

namespace EmployeeProject.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20,ErrorMessage ="Name cannot exceed 20 characters")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address.")]
        [Display(Name="Office Email")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        
        public string PhotoPath { get; set; }
    }
}
