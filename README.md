# WebPushNotificationApp

You will need the Webpush nuget package for the backend webpush notifications.

To generate the vapid keys we will use in appsettings.json, you can use: 
1) node.js:
   
 ```npc web-push generate-vapid-keys```
 
2) Webpush:
```   
using WebPush;

public class VapidHelper
{
    public static void GenerateVapidKeys()
    {
        var keys = VapidHelper.GenerateVapidKeys();
        Console.WriteLine($"Public Key: {keys.PublicKey}");
        Console.WriteLine($"Private Key: {keys.PrivateKey}");
    }
}

The keys will be saved in appsetting.json (you will have to introduce them manually since appsetting.json is not tracked by git for security reasons.)
like this:

{
  "VapidDetails": {
    "PublicKey": "your-public-key-here",
    "PrivateKey": "your-private-key-here",
    "Subject": "mailto:youremail@example.com"
  }
}
```
