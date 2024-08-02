using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Models
{
    public static class ModelBuilderExtentions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                Name = "Raed",
                Email = "raedbzour77@gmail.com",
                Department = Dept.Developer,
                PhotoPath = "r.jpg"
            },
                 new Employee
                 {
                     Id = 2,
                     Name = "Bahaa",
                     Email = "bahaaSamoudi@gmail.com",
                     Department = Dept.IT,
                     PhotoPath = "x.jpg"

                 });
        }
    }
}
