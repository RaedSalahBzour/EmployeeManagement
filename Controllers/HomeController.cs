using Microsoft.AspNetCore.Mvc;
using EmployeeProject.Models;
using EmployeeProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize]
	public class HomeController: Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEmployeeRepository _employeeRepository;
        public HomeController(IEmployeeRepository employeeRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment=hostingEnvironment;
            _employeeRepository = employeeRepository;
        }
        [Route("~/Home")]
        [Route("~/")]
        public ViewResult Index()
        {
            
            var model= _employeeRepository.GetAll();
            return View(model);

        }
        [Route("{id?}")]
        public ViewResult Details(int? id)
        {
            Employee employee = _employeeRepository.GetEmployee(id.Value);
            if (employee is null)
            {
                Response.StatusCode = 404;
                return View("NotFound",id.Value);
            }

            HomeDetailsViewModels homeDetailsViewModels = new HomeDetailsViewModels()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };
            return View(homeDetailsViewModels);
        }
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee= _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath ,
            };
            return View(employeeEditViewModel);
        }
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqeFileName = ProcessUploadedFile(model);
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath=uniqeFileName
                };
                _employeeRepository.Add(newEmployee);
                return RedirectToAction("Details", new { id = newEmployee.Id });
            }
            return View();
        }
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Employee employee = _employeeRepository.GetEmployee(model.Id);
                    employee.Name = model.Name;
                    employee.Email = model.Email;
                    employee.Department = model.Department;

                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);

                    _employeeRepository.Update(employee);
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    // Log the error (uncomment ex variable name and write a log.)
                    Console.WriteLine($"An error occurred while saving the changes: {ex.InnerException?.Message}");

                    // Optionally, add a model state error and return the view
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(model);
        }


        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqeFileName = null;
            if (model.Photo is not null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                uniqeFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqeFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create)) 
                {

                model.Photo.CopyTo(fileStream);
                }
            }

            return uniqeFileName;
        }
    }
}
