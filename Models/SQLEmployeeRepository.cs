
namespace EmployeeProject.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private AppDBContext _context;

        public SQLEmployeeRepository(AppDBContext appDBConetxt)
        {
            _context=appDBConetxt;
        }
        public Employee Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee= _context.Employees.Find(id);
            if(employee is not null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees;
        }

        public Employee GetEmployee(int id)
        {
            return _context.Employees.Find(id);
         
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee= _context.Employees.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return employeeChanges;
        }
    }
}
