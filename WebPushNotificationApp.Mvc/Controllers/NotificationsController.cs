using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPush;
using WebPushNotificationsApp.PushService;

namespace WebPushNotificationApp.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly PushService _pushService;

        public NotificationsController(PushService pushService)
        {
            _pushService = pushService;
        }
        
        //Handels the subscription
        [HttpPost("surbscribe")]
        public IActionResult Subscribe([FromBody] PushSubscription subscription)
        {
            // Store the subscritpion for later use
            return Ok();
        }

        // Sends the notification
        [HttpPost("notify")]
        public async Task<IActionResult> Notify()
        {
            var subscription = new PushSubscription
            {
                Endpoint = "https://fcm.googleapis.com/fcm/send/example_subscription"
            };
            var payload = JsonConvert.SerializeObject(new
            {
                title = "Test Notification",
                message = "This is a test message"
            });

            await _pushService.SendNotificationAsync(subscription, payload);
            return Ok();
        }
    }
}
