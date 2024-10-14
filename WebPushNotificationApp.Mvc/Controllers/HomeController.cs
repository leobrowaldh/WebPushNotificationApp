using Database;
using Database.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
    WebPushAppContext _webPushAppContext,
    UserManager<AplicationUser> _userManager) : Controller
{
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        ViewBag.PublicKey = _configuration["VapidDetails:PublicKey"];
        var messageList = await _webPushAppContext.Messages.ToListAsync();
        if (User.Identity.IsAuthenticated)
        {
            ViewBag.CurrentUserName = currentUser.UserName;
        }
        foreach(var message in messageList)
        {
            _webPushAppContext.Entry(message).Reload();
        }

        HomeIndexViewModel model = new
        (
            Users: await _userManager.Users.ToListAsync(),
            Messages: messageList,
            UserId: _userManager.GetUserId(User)!, //if authorized, the id is present, not null, safe !
            ProfilePicture: "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
            Contacts: new List<ContactDTO>() {
                new ContactDTO("Arne", "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png")
            }
        );
        return View(model);
    }

    public async Task<IActionResult> Create(Message message) 
    { 

            //message.UserName = User.Identity.Name;
            var sender = await _userManager.GetUserAsync(User);
            //message.UserId = sender.Id;
            await _webPushAppContext.Messages.AddAsync(message);
            await _webPushAppContext.SaveChangesAsync();
            return Ok();

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
