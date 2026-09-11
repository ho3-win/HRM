using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mhrm.Models;
using Microsoft.AspNetCore.Authorization;


namespace Mhrm.Controllers;

[Authorize]
public class Survey : Controller
{
    public IActionResult Index()
    {
        ViewBag.nameSubpage="نظرسنجی ";
        return View();
    }

  
}

