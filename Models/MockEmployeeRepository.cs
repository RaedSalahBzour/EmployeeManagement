
namespace EmployeeProject.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        public List<Employee> _employeeList;
        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee> {
                new Employee() {Id=1,Name="Raed",Email="raedbzour77@gmail.com",Department=Dept.Developer},
                new Employee() {Id=2,Name="Ahmed",Email="ahmad2@gmail.com",Department=Dept.HR},
                new Employee() {Id=3,Name="Mohammed",Email="mosalah@gmail.com",Department=Dept.IT},
                new Employee() {Id=4,Name="Bahaa",Email="bahaa@gmail.com",Department=Dept.Developer},
              
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id=_employeeList.Max(x => x.Id)+1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = _employeeList.FirstOrDefault(x => x.Id==id);
            if (employee is not null)
            {
                _employeeList.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(e=>e.Id == Id);
        }

        public Employee Update(Employee employeeChanges)
        {
            Employee employee = _employeeList.FirstOrDefault(x => x.Id == employeeChanges.Id);
            if (employee is not null)
            {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            return employee;
        }
    }
}
