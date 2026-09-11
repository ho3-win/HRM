using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mhrm.Data;
using Mhrm.Models;
using Mhrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Mhrm.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;




namespace Mhrm.Controllers
{
    [Authorize]
    public class NewAccountController : Controller
    {
        
        private readonly HrDbContext _context;
        
        public NewAccountController(HrDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult RegisterNew()
        {
            
            if (!PermissionHelper.HasPermission(User, "Add and delete"))
            {
                return Content("شما به این بخش دسترسی ندارید");
            }
            LoadDeptPosition();

            ViewBag.Employees = _context.Employees
                .Select(e => new
                {
                    e.Id,
                    FullName = e.FirstName + " " + e.LastName
                })
                .ToList();

            ViewBag.nameSubpage = "کارمند جدید";

            return View(new RegisterEmployeeViewModel());
        }


       
        [HttpPost]
        public IActionResult RegisterNew(RegisterEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDeptPosition();
                return View(model);
            }

            // Employeeساخت 
            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                National_id = model.National_id,
                Email = model.Email,
                Phone = model.Phone,
                BirthDate = model.BirthDate,
                HireDate = model.HireDate,
                InsuranceNumber = model.InsuranceNumber,
                DepartmentPositionId = model.DepartmentPositionId,
                Status = model.Status
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();

            //User ساخت 
            var user = new User
            {
                EmployeeId = employee.Id,
                Username = model.Username,
                Password = model.Password, 
                RoleId = model.RoleId,
                Bio = model.Bio
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "کارمند  موفقیت ثبت شد.";

            return RedirectToAction("RegisterNew");
        }


        // پر کردن لیست دپارتمان
        
        private void LoadDeptPosition()
        {
            ViewBag.DepartmentPosition = _context.DepartmentPosition
                .Select(dp => new SelectListItem
                {
                    Value = dp.Id.ToString(),
                    Text = dp.Department.Name + " - " + dp.Position.Title
                })
                .ToList();
        }

        


        //گرفتن اطلاعات کارکنان از دیتا بیس
        public IActionResult GetEmployeesList()
        {
            var employees = _context.Employees
                .Select(e => new Employee
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName
                })
                .ToList();

            return View(employees);
        }



        [HttpPost]
        public IActionResult DeleteEmployee([FromBody] int employeeId)
        {
            if (!PermissionHelper.HasPermission(User, "Add and delete"))
            {
                return Forbid();
            }
            var emp = _context.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (emp == null)
                return NotFound("کارمند یافت نشد.");

            
            var user = _context.Users.FirstOrDefault(u => u.EmployeeId == employeeId);
            if (user != null)
            {
                //  جلسه‌های مرتبط با آن کاربر حذف شوند
                var sessions = _context.UserSessions
                    .Where(s => s.UserId == user.Id)
                    .ToList();
                _context.UserSessions.RemoveRange(sessions);

                //  خود یوزر حذف شود
                _context.Users.Remove(user);
            }

            //  قراردادها 
            var contracts = _context.Contracts
                .Where(c => c.EmployeeId == employeeId)
                .ToList();
            _context.Contracts.RemoveRange(contracts);

           
            var documents = _context.Documents
                .Where(d => d.EmployeeId == employeeId)
                .ToList();
            _context.Documents.RemoveRange(documents);

            
            var requests = _context.Requests
                .Where(r => r.RequestBy == employeeId || r.ApprovedBy == employeeId)
                .ToList();
            _context.Requests.RemoveRange(requests);

           
           

            
            _context.Employees.Remove(emp);

            _context.SaveChanges();

            return Ok(new { message = "کارمند و تمام اطلاعات وابسته با موفقیت حذف شدند " });
        }


    }
}
