using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Mhrm.Models;
using Mhrm.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Mhrm.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;
    private readonly HrDbContext _db;

    public HomeController(ILogger<HomeController> logger, HttpClient httpClient, HrDbContext db)
    {
        _logger = logger;
        _httpClient = httpClient;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var totalEmployees = await _db.Employees.CountAsync();
        ViewBag.EmployeeCount = totalEmployees;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Assistant()
    {
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> AskAi(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return Json(new { answer = "لطفاً یک سوال معتبر وارد کنید." });
        }

        try
        {
            var encodedQuestion = Uri.EscapeDataString(question.Trim());
            var url = $"https://avanex.ir/ai/api.php?question={encodedQuestion}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);

            if (doc.RootElement.TryGetProperty("answer", out var answerElement) &&
                answerElement.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(answerElement.GetString()))
            {
                return Json(new { answer = answerElement.GetString() });
            }

            return Json(new { answer = "پاسخی از سرور هوش مصنوعی دریافت نشد." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI API call failed for question: {Question}", question);
            return Json(new { answer = "در ارتباط با دستیار هوش مصنوعی خطایی رخ داد. لطفاً دوباره تلاش کنید." });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
