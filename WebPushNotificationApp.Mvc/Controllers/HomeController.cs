using Database.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace WebPushNotificationApp.Mvc.Controllers;

public class HomeController(
    IConfiguration _configuration,
    IMessageRepository _messageRepository,
    UserManager<AplicationUser> _userManager) : Controller
{
    [Authorize]
    public async Task<IActionResult> Index()
    {
        //gets the current user
        var currentUser = await _userManager.GetUserAsync(User);
        ViewBag.PublicKey = _configuration["VapidDetails:PublicKey"];
        var messageList = await _messageRepository.GetAllMessagesAsync();
        if (User.Identity is not null && User.Identity.IsAuthenticated && currentUser is not null)
        {
            ViewBag.CurrentUser = currentUser;
        }
        foreach (var message in messageList)
        {
            _messageRepository.ReloadEntity(message);
        }

        HomeIndexViewModel model = new
        (
            Contacts: await _userManager.Users
                .Select(u => new ContactDTO ( (u.UserName ?? u.Email) ?? "Unamed User", u.ProfilePicture ?? "" ))
                .ToListAsync(),
            Messages: messageList,
            UserId: currentUser!.Id,
            ProfilePicture: currentUser.ProfilePicture ?? ""
        );
        return View(model);
    }

    public async Task<IActionResult> Create(Message message)
    {
        //sends the chat message to our database
        message.UserName = User.Identity.Name;
        var sender = await _userManager.GetUserAsync(User);
        message.UserId = sender.Id;
        await _messageRepository.AddMessageAsync(message);
        return Ok();

    }

    public IActionResult AboutUs()
    {
        return View();
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
    public async Task<IActionResult> Homepage()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index");
        }
        return View();
    }
}