using Microsoft.AspNetCore.Mvc;
using TrainingTest.Models.Pages;

namespace TrainingTest.Controllers;

public class StartPageController : PageControllerBase<StartPage>
{
    public IActionResult Index(StartPage currentPage)
    {
        ViewData["PageCss"] = "/index.css";
        return View(currentPage);
    }
}