using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry,not found";
                    break;

            }

            return View("NotFoundError");
        }
    }
}
