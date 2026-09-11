using Microsoft.AspNetCore.Mvc;
using Mhrm.Models;
using Mhrm.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Mhrm.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly HrDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(HrDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // صفحه نمایش اخبار
        public async Task<IActionResult> Index()
        {
            ViewBag.nameSubpage = "اعلان‌ها";
            
            var news = await (
                    from n in _context.News
                    join u in _context.Users on n.PublishedBy equals u.Id
                    join e in _context.Employees on u.EmployeeId equals e.Id
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

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
                return NotFound();

            news.IsActive = !news.IsActive; //  تغییر وضعیت

            _context.News.Update(news);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // نمایش فرم افزودن خبر (GET)
        [HttpGet]
        public IActionResult Createnew()
        {
            ViewBag.nameSubpage = "افزودن اعلان جدید";
            return View(new News()); 
        }

        // ذخیره خبر جدید (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Createnew(News model, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                // ذخیره فایل تصویر
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string fileName = await SaveImageAsync(ImageFile);
                    model.FilePath = fileName;
                }

                model.CreatedAt = DateTime.Now;
                model.PublishedAt = DateTime.Now;
                model.PublishedBy = 1; // موقتاً مقدار 1 می‌دهیم

                _context.News.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "خبر با موفقیت اضافه شد";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // متد کمکی برای ذخیره تصویر
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "news");
            
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/uploads/news/" + uniqueFileName;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _context.News.FindAsync(id);

            if (news == null)
                return NotFound();

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            TempData["Success"] = "خبر با موفقیت حذف شد";
            return RedirectToAction(nameof(Index));
        }
    }
}