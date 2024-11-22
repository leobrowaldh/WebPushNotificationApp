using Database;
using Database.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPush;
using WebPushNotificationsApp.PushService;

namespace WebPushNotificationApp.Mvc.Controllers;

[Route("Notifications")]
public class NotificationsController(
    ILogger<HomeController> _logger,
    IPushService _pushService,
    ISubscriptionRepository _subscriptionRepository,
    IMessageRepository _messageRepository,
	IConfiguration _configuration,
	UserManager<AplicationUser> _userManager,
    WebPushAppContext webPushAppContext) : Controller
{

    [HttpPost("SavingSubscriptionToDb")]
    public async Task<IActionResult> SavingSubscriptionToDb([FromBody] PushSubscriptionDto subscriptionDto)
    {
        // Restructuring the subscription
        var dbSubscription = new WebPush.PushSubscription
        {
            Endpoint = subscriptionDto.Endpoint,
            P256DH = subscriptionDto.Keys.P256DH, // Flattening 'keys'
            Auth = subscriptionDto.Keys.Auth
        };

        // Save the subscription object to database.
        // This object contains the endpoint and keys to send push notifications.
        _logger.LogInformation("Received subscription: Endpoint = {Endpoint}, P256dh = {P256dh}, Auth = {Auth}",
        dbSubscription.Endpoint, dbSubscription.P256DH, dbSubscription.Auth);

        // Ensure keys are not null
        if (string.IsNullOrEmpty(dbSubscription.P256DH) || string.IsNullOrEmpty(dbSubscription.Auth))
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

        // Get the logged-in user's ID
        string? userId = null;
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            userId = _userManager.GetUserId(User);
        }
        //try to save to db.
        int subscriptionId = 0;
        if (userId != null)
        {
            subscriptionId = await _subscriptionRepository.SaveSubscriptionAsync(JsonConvert.SerializeObject(dbSubscription), userId);
        }
        if (subscriptionId > 0)
        {
            _logger.LogInformation("Successfully saved the subscription for: {Endpoint}", dbSubscription.Endpoint);
            //this c# anonymous object will be automatically serialized into JSON by asp.net core:
            return Ok(new { message = "Subscription saved to database.", id = subscriptionId });
        }
        else if (subscriptionId == -1)
        {
            _logger.LogWarning("Subscription allready exists in database, no additional subscription added.");
            return Ok("Subscription already exists in database");
        }
        else
        {
            _logger.LogError("Failed to save the subscription for: {Endpoint}", dbSubscription.Endpoint);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save the subscription.");
        }
    }

    [HttpPost("SendNotification")]
    public async Task<IActionResult> SendNotification(string userId)
    {
        // Retrieve subscriptions from database
        List<Subscription> subscriptions = await _subscriptionRepository.GetUserSubscriptionsAsync(userId);
        
        if (subscriptions.Count == 0)
        {
            return BadRequest("No subscription found in database.");
        }

        foreach (Subscription subscription in subscriptions)
        {
            if (string.IsNullOrEmpty(subscription.SubscriptionJson))
            {
                _logger.LogWarning("SubscriptionJson is null or empty for subscription with ID {SubscriptionId}.", subscription.Id);
                continue; // Skipping this subscription if SubscriptionJson is invalid
            }
            var subscriptionToPushTo = JsonConvert.DeserializeObject<PushSubscription>(subscription.SubscriptionJson);

            if (subscriptionToPushTo == null)
            {
                _logger.LogWarning("Failed to deserialize SubscriptionJson for subscription with ID {SubscriptionId}.", subscription.Id);
                continue; // Skipping this subscription if deserialization fails
            }

            //Loggs some data to database if the notification is sent
            var notification = new Notification
            {
                Created = DateTime.Now,
                SenderId = userId,
                ReceiverId = subscription.UserId,
                Why = "Clicked the 'Test Notification' button.",
                Sent = true,
                InteractedWith = false,
                SubscriptionId = subscription.Id
               
            };
            
            webPushAppContext.Notifications.Add(notification);
            await webPushAppContext.SaveChangesAsync();

            var payload = JsonConvert.SerializeObject(new
            {
                notificationId = notification.Id,
                title = "Test Notification",
                message = "This is a test Notification",
                icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/85/Smiley.svg/1024px-Smiley.svg.png",
                badge = "https://static-00.iconduck.com/assets.00/message-icon-1023x1024-7pbl8unr.png",
            });

            //removes a sub from database if it's faulty
            bool removeSubscription = await _pushService.SendNotificationAsync(subscriptionToPushTo, payload);
            if (removeSubscription)
            {
                bool removed = await _subscriptionRepository.RemoveSubscriptionAsync(subscription.SubscriptionJson);
                if (removed) { _logger.LogInformation("faulty subscription id={subscriptionId} removed from database", subscription.Id); }
                else { _logger.LogInformation("faulty subscription id={subscriptionId} not correctly removed from database", subscription.Id); }
            }
        }
        return Ok("Notification Sent");
    }

    [HttpPost("NotifyAll")]
    public async Task<IActionResult> NotifyAllExceptMe()
    {
        AplicationUser? sender = await _userManager.GetUserAsync(User);
        if (sender == null)
        {
            return BadRequest("No logged in user was found.");
        }

        // Retrieve subscriptions from database
        List<Subscription> subscriptions = await _subscriptionRepository.GetAllNonSenderSubscriptionsAsync(sender.Id);
        if (subscriptions.Count == 0)
        {
            return BadRequest("No subscription found in database.");
        }

        foreach (Subscription subscription in subscriptions)
        {
            if (string.IsNullOrEmpty(subscription.SubscriptionJson))
            {
                _logger.LogWarning("SubscriptionJson is null or empty for subscription with ID {SubscriptionId}.", subscription.Id);
                continue; // Skipping this subscription if SubscriptionJson is invalid
            }
            var subscriptionToPushTo = JsonConvert.DeserializeObject<PushSubscription>(subscription.SubscriptionJson);

            if (subscriptionToPushTo == null)
            {
                _logger.LogWarning("Failed to deserialize SubscriptionJson for subscription with ID {SubscriptionId}.", subscription.Id);
                continue; // Skipping this subscription if deserialization fails
            }

            //Loggs some data to database if the notification is sent

            var notification = new Notification
            {
                Created = DateTime.Now,
                SenderId = sender.Id,
                ReceiverId = subscription.UserId,
                Why = "Sent message to chat.",
                Sent = true,
                InteractedWith = false,
                SubscriptionId = subscription.Id

            };
            webPushAppContext.Notifications.Add(notification);
            await webPushAppContext.SaveChangesAsync();

            var payload = JsonConvert.SerializeObject(new
            {
                //sends the ID of the created notification in the payload so we can extract it later. 
                notificationId = notification.Id,
                //pictures need to be urls to accessible pictures in the correct format for icon and badge.
                title = "New Message from " + sender.UserName,
                message = _messageRepository.GetLastMessageAsync().Result,
                icon = sender.ProfilePicture,
                badge = "https://static-00.iconduck.com/assets.00/message-icon-1023x1024-7pbl8unr.png", //the app logo
            });

            bool removeSubscription = await _pushService.SendNotificationAsync(subscriptionToPushTo, payload);
            if (removeSubscription)
            {
                bool removed = await _subscriptionRepository.RemoveSubscriptionAsync(subscription.SubscriptionJson);
                if (removed) { _logger.LogInformation("faulty subscription id={subscriptionId} removed from database", subscription.Id); }
                else { _logger.LogInformation("faulty subscription id={subscriptionId} not correctly removed from database", subscription.Id); }
            }


        }
        return Ok( "Notifications Sent");
    }

    //sets InteractedWith prop in our database to true so we know if the user have interacted with out notification
    [HttpPost("Interact")]
    public async Task<IActionResult> MarkAsInteracted([FromQuery] int notificationId)
    {
        // Find the notification by ID
        var notification = await webPushAppContext.Notifications.FindAsync(notificationId);
        if (notification == null)
        {
            return NotFound("Notification not found.");
        }

        // Update the InteractedWith property
        notification.InteractedWith = true;
        // Save changes to the database
        await webPushAppContext.SaveChangesAsync();

        return Ok("Notification marked as interacted.");
    }

    //sets ServiceWorkerReceived prop to true if the targeted Service Workerd recieved the push from Endpoint server.
    [HttpPost("ServiceWorkerReceivedPush/{notificationId}")]
    public async Task<IActionResult> AcknowledgeNotification(int notificationId)
    {
        // Find the notification by ID
        var notification = await webPushAppContext.Notifications.FindAsync(notificationId);
        if (notification == null)
        {
            return NotFound("Notification not found.");
        }

        // Update the ServiceWorkerReceived property
        notification.ServiceWorkerReceived = true;

        // Save changes to the database
        await webPushAppContext.SaveChangesAsync();

        // Log the acknowledgment
        _logger.LogInformation($"Notification {notificationId} recieved by service worker.");

        return Ok("Service Worker received the push.");
    }

    [HttpPost("CheckUserSubscriptionAsync")]
    public async Task<IActionResult> CheckUserSubscriptionAsync([FromBody] PushSubscriptionDto subscriptionDto)
    {
        // Restructuring the subscription
        var dbSubscription = new WebPush.PushSubscription
        {
            Endpoint = subscriptionDto.Endpoint,
            P256DH = subscriptionDto.Keys.P256DH,
            Auth = subscriptionDto.Keys.Auth
        };

        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId is null)
        {
            _logger.LogWarning("User not logged in, redirecting to login page.");
            return Redirect($"~/Identity/Account/Login?ReturnUrl={Url.Action("Index", "Home")}");
        }

        var subscriptionJson = JsonConvert.SerializeObject(dbSubscription);
        _logger.LogInformation("Serialized subscription from client: {subscriptionJson}", subscriptionJson);

        try
        {
            bool isUserSubscription = await _subscriptionRepository.IsUserSubscriptionAsync(subscriptionJson, currentUserId);
            _logger.LogInformation("IsUserSubscriptionAsync result: {isUserSubscription}", isUserSubscription);

            return Ok(new { isUserSubscription });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking user subscription.");
            return StatusCode(500, "An error occurred while checking the subscription.");
        }
    }


    [HttpPost("RemoveSubscriptionAsync")]
    public async Task<IActionResult> RemoveSubscriptionAsync([FromBody] PushSubscriptionDto subscriptionDto)
    {
        // Restructuring the subscription
        var dbSubscription = new WebPush.PushSubscription
        {
            Endpoint = subscriptionDto.Endpoint,
            P256DH = subscriptionDto.Keys.P256DH,
            Auth = subscriptionDto.Keys.Auth
        };

        bool success = await _subscriptionRepository.RemoveSubscriptionAsync(JsonConvert.SerializeObject(dbSubscription));
        if (success)
        {
            return Ok("subscription successfully removed from database.");
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to remove the subscription from database.");
        }
    }
}