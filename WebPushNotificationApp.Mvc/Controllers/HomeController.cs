//DTO's Would be more appropiate, we use Db entities for now.
using Database.EntityModels;

using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebPush;
using WebPushNotificationApp.Mvc.DTOs;
using WebPushNotificationApp.Mvc.Models;
using WebPushNotificationsApp.PushService;
using static System.Net.WebRequestMethods;

namespace WebPushNotificationApp.Mvc.Controllers
{
    public class HomeController(ILogger<HomeController> _logger, IPushService _pushService, IConfiguration _configuration, IUserRepository _userRepository) : Controller
    {
        public IActionResult Index()
        {
            ViewBag.PublicKey = _configuration["VapidDetails:PublicKey"];
            HomeIndexViewModel model = new
            (
                UserId: 1, //get by authenticatiuon of user.
                ProfilePicture: "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
                Contacts: new List<ContactDTO>() { 
                    new ContactDTO("Olle Persson", "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png"),
                    new ContactDTO("Lilly Torrvik", "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png")
                }
            );
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SavingSubscriptionToDb([FromBody] PushSubscription subscription)
        {
            _logger.LogInformation("Received subscription: Endpoint = {Endpoint}, P256dh = {P256dh}, Auth = {Auth}",
            subscription.Endpoint, subscription.P256DH, subscription.Auth);

            // Save the subscription object to database.
            // This object contains the endpoint and keys to send push notifications.

            // Ensure keys are not null
            if (string.IsNullOrEmpty(subscription.P256DH) || string.IsNullOrEmpty(subscription.Auth))
            {
                _logger.LogWarning("Subscription keys are missing or invalid.");
                return BadRequest("Subscription must include auth and p256dh keys.");
            }

            //check modelstate of subscription object
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid subscription model state: {@ModelState}", ModelState);
                return BadRequest("Invalid subscription data.");
            }
            //try to save to db.
            int subscriptionId = await _userRepository.SaveSubscriptionAsync(JsonConvert.SerializeObject(subscription), "c39c78c3-3d5b-44ab-90ec-a5a7f5ddff9b"); //get userId from user authentication. (this is a test userID)
            if (subscriptionId != 0)
            {
                _logger.LogInformation("Successfully saved the subscription for: {Endpoint}", subscription.Endpoint);
                //this c# anonymous object will be automatically serialized into JSON by asp.net core:
                return Ok(new { message = "Subscription saved to database.", id = subscriptionId });
            }
            else 
            {
                _logger.LogError("Failed to save the subscription for: {Endpoint}", subscription.Endpoint);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save the subscription.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(string userId)
        {
            // Retrieve subscription from database
            string? subscriptionJSON = await _userRepository.GetUserSubscriptionsAsync(userId);

            if (subscriptionJSON is null)
            {
                return BadRequest("No subscription found in database.");
            }

            var subscription = JsonConvert.DeserializeObject<PushSubscription>(subscriptionJSON);
            var payload = JsonConvert.SerializeObject(new
            {
                title = "Test Notification",
                message = "This is a notification for you!",
                icon = "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png", 
                badge = "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png", 
            });

            await _pushService.SendNotificationAsync(subscription, payload);
            return Ok("Notification Sent");
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
}
