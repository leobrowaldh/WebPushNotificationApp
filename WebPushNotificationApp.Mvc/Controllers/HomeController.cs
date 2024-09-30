using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebPush;
using WebPushNotificationApp.Mvc.Models;
using WebPushNotificationsApp.PushService;

namespace WebPushNotificationApp.Mvc.Controllers
{
    public class HomeController(ILogger<HomeController> logger, PushService pushService, IConfiguration configuration, IUserRepository repository) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly PushService _pushService = pushService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserRepository _userRepository = repository;

        public IActionResult Index()
        {
            ViewBag.PublicKey = _configuration["VapidDetails:PublicKey"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody] PushSubscription subscription)
        {
            // Save the subscription object to database.
            // This object contains the endpoint and keys to send push notifications.

            //check modelstate of subscription object
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid subscription model state: {@ModelState}", ModelState);
                return BadRequest("Invalid subscription data.");
            }
            //try to save to db.
            else if (await _userRepository.SaveSubscription(JsonConvert.SerializeObject(subscription)))
            {
                _logger.LogInformation("Successfully saved the subscription for: {Endpoint}", subscription.Endpoint);
                return Ok("Subscription saved to database.");
            }
            else 
            {
                _logger.LogError("Failed to save the subscription for: {Endpoint}", subscription.Endpoint);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save the subscription.");
            }
        }

        public async Task<IActionResult> SendNotification()
        {
            // Retrieve subscription from session
            var subscriptionJson = HttpContext.Session.GetString("PushSubscription");

            if (string.IsNullOrEmpty(subscriptionJson))
            {
                return BadRequest("No subscription found in session.");
            }

            var subscription = JsonConvert.DeserializeObject<PushSubscription>(subscriptionJson);
            var payload = JsonConvert.SerializeObject(new
            {
                title = "Test Notification",
                message = "This is a notification for you!"
            });

            await _pushService.SendNotificationAsync(subscription, payload);
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
}
