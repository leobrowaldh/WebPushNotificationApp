using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebPush;
using WebPushNotificationApp.Mvc.Models;
using WebPushNotificationsApp.PushService;

namespace WebPushNotificationApp.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PushService _pushService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, PushService pushService, IConfiguration configuration)
        {
            _logger = logger;
            _pushService = pushService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.PublicKey = _configuration["VapidDetails:PublicKey"];
            return View();
        }

        [HttpPost]
        public IActionResult Subscribe([FromBody] PushSubscription subscription)
        {
            // Save the subscription object to database.
            // This object contains the endpoint and keys to send push notifications.
            return Ok();
        }

        public async Task<IActionResult> SendNotification()
        {
            //here we should retrieve the subscription fron the database
            var subscription = new PushSubscription
            {
                Endpoint = "https://fcm.googleapis.com/fcm/send/example_subscription"
            };
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
