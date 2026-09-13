using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Mhrm.Data;
using Mhrm.Models;
using Mhrm.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mhrm.Helpers;

namespace Mhrm.Controllers
{
    [Authorize]
    public class WorkerController : Controller
    {
        private readonly HrDbContext _context;
        private readonly IWebHostEnvironment _env;

        public WorkerController(HrDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.nameSubpage = "اعلان‌ها";

            var news = await (
                     from n in _context.News
                     join u in _context.Users on n.PublishedBy equals u.Id
                     join e in _context.Employees on u.EmployeeId equals e.Id
                     where n.IsActive == true
                     orderby n.CreatedAt descending
                     select new NewsVM
                     {
                         Id = n.Id,
                         FilePath = n.FilePath,
                         Title = n.Title,
                         Content = n.Content,
                         CreatedAt = n.CreatedAt,
                         IsActive = n.IsActive,
                         IsImportant = n.IsImportant,
                         Category = n.Category,

                         EmployeeFirstName = e.FirstName,
                         EmployeeLastName = e.LastName,
                         EmployeeId = e.Id
                     }
                 ).ToListAsync();
            return View(news);
        }

        public async Task<IActionResult> NewsList()
        {
            var news = await (
                from n in _context.News
                join u in _context.Users on n.PublishedBy equals u.Id
                join e in _context.Employees on u.EmployeeId equals e.Id
                where n.IsActive == true
                orderby n.CreatedAt descending
                select new NewsVM
                {
                    Id = n.Id,
                    FilePath = n.FilePath,
                    Title = n.Title,
                    Content = n.Content,
                    CreatedAt = n.CreatedAt,
                    IsActive = n.IsActive,
                    IsImportant = n.IsImportant,
                    Category = n.Category,
                    EmployeeFirstName = e.FirstName,
                    EmployeeLastName = e.LastName,
                    EmployeeId = e.Id
                }
            ).ToListAsync();

            ViewBag.nameSubpage = "اخبار و اطلاعیه‌ها";
            return View(news);
        }


        public IActionResult Details()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            int employeeId = user.EmployeeId;

            var employee = _context.Employees
                .Include(e => e.DepartmentPosition)
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null)
                return NotFound();

            var contracts = _context.Contracts
                .Where(c => c.EmployeeId == employeeId)
                .ToList();

            var documents = _context.Documents
                .Where(d => d.EmployeeId == employeeId)
                .ToList();

            var leaveRequests = _context.LeaveRequests
                .Where(l => l.Request_by == user.Id)
                .ToList();


            var requests = _context.Requests
                .Where(r => r.RequestBy == employeeId)
                .ToList();

            var sessions = _context.UserSessions
                .Where(s => s.UserId == user.Id)
                .ToList();

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

            return View("Details", vm);
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
            if (!PermissionHelper.HasPermission(User, "Making changes"))
            {
                return Forbid();
            }
            var user = _context.Users.FirstOrDefault(u => u.EmployeeId == dto.EmployeeId);
            if (user == null) return NotFound();

            user.Username = dto.Username;
            user.Bio = dto.Bio;
            user.Password = dto.Password;
            // user.RoleId = dto.RoleId;

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
            if (!PermissionHelper.HasPermission(User, "Making changes"))
            {
                return Forbid();
            }

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
        public IActionResult CreateRequest([FromBody] Request model)
        {
            if (!PermissionHelper.HasPermission(User, "Making changes"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
                return BadRequest();

            _context.Requests.Add(model);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateRequest([FromBody] Request model)
        {
            if (!PermissionHelper.HasPermission(User, "Making changes"))
            {
                return Forbid();
            }
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
            if (!PermissionHelper.HasPermission(User, "Making changes"))
            {
                return Forbid();
            }
            var req = _context.Requests.FirstOrDefault(r => r.Id == id);
            if (req == null) return NotFound();

            _context.Requests.Remove(req);
            _context.SaveChanges();
            return Ok();
        }





        public IActionResult request()
        {
            ViewBag.nameSubpage = "درخواست ها";
            return View();
        }
    }
}
