using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Mhrm.Data;
using Mhrm.Models;
using Mhrm.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Mhrm.Helpers;

namespace Mhrm.Controllers
{
    
    [Authorize]
    public class EmployeemenuController : Controller
    {
        
        private readonly HrDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeemenuController(
            HrDbContext context,
            IWebHostEnvironment env)
            {
                _context = context;
                _env = env;
            }
       

        public IActionResult Details(int id)
        {
            
            
            var employee = _context.Employees
                .Include(e => e.DepartmentPosition)
                .FirstOrDefault(e => e.Id == id);

            if (employee == null)
                return NotFound();

            // User Nullable
            var user = _context.Users.FirstOrDefault(u => u.EmployeeId == id);

            var contracts = _context.Contracts
                .Where(c => c.EmployeeId == id)
                .ToList();

            var documents = _context.Documents
                .Where(d => d.EmployeeId == id)
                .ToList();

            var leaveRequests = _context.LeaveRequests
                .Where(l => l.Request_by == user.Id)
                .ToList();

            var requests = _context.Requests
                .Where(r => r.RequestBy == id)
                .ToList();

           
            var sessions = new List<UserSession>();
            if (user != null)
            {
                sessions = _context.UserSessions
                    .Where(s => s.UserId == user.Id)
                    .ToList();
            }

            var vm = new EmployeeDetails
            {
                Employee = employee,
                User = user,
                Contracts = contracts,
                Documents = documents,
                LeaveRequests = leaveRequests,
                Requests = requests,
                Sessions = sessions
            };

            ViewBag.nameSubpage = $"{employee.FirstName} {employee.LastName}".Trim();

            return View("DetailsEmployee", vm);
        }
        
        [HttpPost]
        public IActionResult UpdateEmployee([FromBody] Employee dto)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emp = _context.Employees.FirstOrDefault(e => e.Id == dto.Id);
            if (emp == null)
                return NotFound();

            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.Phone = dto.Phone;
            emp.National_id = dto.National_id;

            emp.Email = dto.Email;
            emp.BirthDate = dto.BirthDate;
            emp.HireDate = dto.HireDate;

            emp.Status = dto.Status;
            emp.InsuranceNumber = dto.InsuranceNumber;
            emp.DepartmentPositionId = dto.DepartmentPositionId;

            _context.SaveChanges();

            return Ok();
        }


        public class UpdateUserDto
        {
            public int EmployeeId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string? Bio { get; set; }

            public string Password { get; set; } = string.Empty;
            public int RoleId { get; set; }

            public int Id { get; set; }
        }

        [HttpPost]
        public IActionResult UpdateUser([FromBody] UpdateUserDto dto)
        {
             //ایجاد مجوز
            var currentUserId = int.Parse(
                User.FindFirst("UserId")!.Value
            );

            var currentEmployeeId = _context.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => u.EmployeeId)
                .FirstOrDefault();

            if (!PermissionHelper.HasPermission(User, "Edit")
                && currentEmployeeId != dto.EmployeeId)
            {
                return Forbid();
            }
            //
            var user = _context.Users.FirstOrDefault(u => u.EmployeeId == dto.EmployeeId);
            if (user == null) return NotFound();

            user.Username = dto.Username;
            user.Bio = dto.Bio;

            user.Password = dto.Password;
            user.RoleId = dto.RoleId;

            
            //user.Id = dto.Id;

            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UpdateUserDto dto)
        {
            
            var existing = _context.Users.FirstOrDefault(u => u.EmployeeId == dto.EmployeeId);
            if (existing != null)
                return BadRequest("User already exists for this employee.");

            var user = new User
            {
                EmployeeId = dto.EmployeeId,
                Username = dto.Username,
                Bio = dto.Bio,
                Password = dto.Password,
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok();
        }


        [HttpPost]
        public IActionResult CreateContract([FromBody] Contract model)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
                return BadRequest();

            _context.Contracts.Add(model);
            _context.SaveChanges();
            return Ok();
        }


        [HttpPost]
        public IActionResult UpdateContract([FromBody] Contract model)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contract = _context.Contracts.FirstOrDefault(c => c.Id == model.Id);
            if (contract == null)
                return NotFound("قرارداد پیدا نشد");

            contract.StartDate = model.StartDate;
            contract.EndDate = model.EndDate;
            contract.Salary = model.Salary;
            contract.Status = model.Status;
            contract.HoursPerWeek = model.HoursPerWeek;
            contract.ContractType = model.ContractType;

            _context.SaveChanges();
            return Ok();
        }

        public class DeleteContractDto
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public IActionResult DeleteContract([FromBody] DeleteContractDto dto)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            var contract = _context.Contracts.FirstOrDefault(c => c.Id == dto.Id);
            if (contract == null)
                return NotFound();

            _context.Contracts.Remove(contract);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument(
            
            [FromForm] int EmployeeId,
            [FromForm] string Title,
            [FromForm] string Description,
            IFormFile file)
        {
             //ایجاد مجوز
            var currentUserId = int.Parse(
                User.FindFirst("UserId")!.Value
            );

            var currentEmployeeId = _context.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => u.EmployeeId)
                .FirstOrDefault();

            if (!PermissionHelper.HasPermission(User, "Edit")
                && currentEmployeeId != EmployeeId)
            {
                return Forbid();
            }
            //
            if (file == null || file.Length == 0)
                return BadRequest("file missing");

            var uploads = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var doc = new Document
            {
                EmployeeId = EmployeeId,
                Title = Title,
                Description = Description,
                FilePath = "/uploads/" + fileName
            };

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        public IActionResult CreateLeaveRequest([FromBody] LeaveRequest model)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
                return BadRequest();

            _context.LeaveRequests.Add(model);
            _context.SaveChanges();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> UpdateDocument(
            [FromForm] int Id,
            [FromForm] string Title,
            [FromForm] string Description,
            IFormFile? file)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            var doc = _context.Documents.FirstOrDefault(d => d.Id == Id);
            if (doc == null) return NotFound();

            doc.Title = Title;
            doc.Description = Description;

            // اگر فایل جدید انتخاب شد
            if (file != null && file.Length > 0)
            {
                // حذف فایل قبلی
                var oldPath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                // ذخیره فایل جدید
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                doc.FilePath = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteDocument([FromBody] int id)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            var doc = _context.Documents.FirstOrDefault(d => d.Id == id);
            if (doc == null) return NotFound();

            // حذف فایل از سرور
            var path = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            // حذف از دیتابیس
            _context.Documents.Remove(doc);
            _context.SaveChanges();

            return Ok();
        }


        [HttpPost]
        public IActionResult UpdateLeaveRequest([FromBody] LeaveRequest model)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            var leave = _context.LeaveRequests.FirstOrDefault(l => l.Id == model.Id);
            if (leave == null) return NotFound();

            leave.StartDate = model.StartDate;
            leave.EndDate = model.EndDate;
            leave.Status = model.Status;

            leave.LeaveType = model.LeaveType;
            leave.Description = model.Description;
            leave.ApprovedBy = model.ApprovedBy;

            _context.SaveChanges();

            return Ok();
        }

       


        [HttpPost]
        public IActionResult DeleteLeaveRequest([FromBody] int id)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            var leave = _context.LeaveRequests.FirstOrDefault(l => l.Id == id);
            if (leave == null) return NotFound();

            _context.LeaveRequests.Remove(leave);
            _context.SaveChanges();
            return Ok();
        }





        [HttpPost]
        public IActionResult CreateRequest([FromBody] Request model)
        {
            
            if (!ModelState.IsValid)
                return BadRequest();

            _context.Requests.Add(model);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateRequest([FromBody] Request model)
        {
            var req = _context.Requests.FirstOrDefault(r => r.Id == model.Id);
            if (req == null) return NotFound();

            req.Description = model.Description;
            req.Status = model.Status;

            req.LeaveRequest = model.LeaveRequest;
            req.StartDate = model.StartDate;
            req.EndDate = model.EndDate;
            req.ApprovedBy = model.ApprovedBy;

            _context.SaveChanges();

            return Ok();
        }


        [HttpPost]
        public IActionResult DeleteRequest([FromBody] int id)
        {
            
            var req = _context.Requests.FirstOrDefault(r => r.Id == id);
            if (req == null) return NotFound();

            _context.Requests.Remove(req);
            _context.SaveChanges();

            return Ok();
        }





        [HttpPost]
        public IActionResult CreateSession([FromBody] UserSession model)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            if (model.Logout_at == DateTime.MinValue)
                model.Logout_at = model.Login_at;

            if (model.Expirees_at == DateTime.MinValue)
                model.Expirees_at = model.Login_at.AddHours(24);

            model.Login_at    = DateTime.SpecifyKind(model.Login_at, DateTimeKind.Local);
            model.Logout_at   = DateTime.SpecifyKind(model.Logout_at, DateTimeKind.Local);
            model.Expirees_at = DateTime.SpecifyKind(model.Expirees_at, DateTimeKind.Local);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.UserSessions.Add(model);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateSession([FromBody] UserSession model)
        {
            if (!PermissionHelper.HasPermission(User, "Edit"))
            {
                return Forbid();
            }
            var session = _context.UserSessions
                .FirstOrDefault(x => x.Id == model.Id);

            if (session == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Session not found"
                });
            }

           
            session.Login_at    = DateTime.SpecifyKind(model.Login_at, DateTimeKind.Local);
            session.Logout_at   = DateTime.SpecifyKind(model.Logout_at, DateTimeKind.Local);
            session.Expirees_at = DateTime.SpecifyKind(model.Expirees_at, DateTimeKind.Local);

            session.Session_token = model.Session_token;
            session.Ip_address    = model.Ip_address;
            session.User_agent    = model.User_agent;

            _context.SaveChanges();

            return Json(new
            {
                success = true
            });
        }







        public class DeleteSessionDto
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public IActionResult DeleteSession(
            [FromBody] DeleteSessionDto dto)
        {
            var session = _context.UserSessions
                .FirstOrDefault(s => s.Id == dto.Id);

            if (session == null)
                return NotFound();

            _context.UserSessions.Remove(session);

            _context.SaveChanges();

            return Ok();
        }


        

        public IActionResult List()
        {
            ViewBag.nameSubpage = "لیست کارکنان";

            var employees = _context.Employees
                .Select(e => new EmployeeDetails
                {
                    Employee = e,
                    User = _context.Users.FirstOrDefault(u => u.EmployeeId == e.Id)
                })
                .ToList();

            return View(employees);
        }



        public IActionResult Requests()
        {
            ViewBag.nameSubpage = "درخواست‌های کارکنان";

            var requests = _context.Requests
                .OrderByDescending(r => r.Id)
                .ToList();

            var employees = _context.Employees
                .ToDictionary(e => e.Id, e => e.FirstName + " " + e.LastName);

            ViewBag.EmployeeNames = employees;

            return View(requests);
        }





    }













 



}
