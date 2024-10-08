using Database.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebPush;

using WebPushNotificationsApp.PushService;
using static System.Net.WebRequestMethods;

namespace WebPushNotificationApp.Mvc.Controllers;

public class HomeController(
    ILogger<HomeController> _logger, 
    IConfiguration _configuration, 
    IUserRepository _userRepository,
    UserManager<AplicationUser> _userManager) : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        ViewBag.PublicKey = _configuration["VapidDetails:PublicKey"];
        HomeIndexViewModel model = new
        (
            UserId: _userManager.GetUserId(User)!, //if authorized, the id is present, not null, safe !
            ProfilePicture: "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
            Contacts: new List<ContactDTO>() {
                new ContactDTO("Olle Persson", "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png"),
                new ContactDTO("Lilly Torrvik", "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png")
            }
        );
        return View(model);
    }

    





    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
