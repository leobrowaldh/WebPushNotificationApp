
//Register the service worker
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('sw.js')
        .then(function (registration) {
            console.log('Service Worker registered with scope:', registration.scope);
        }).catch(function (error) {
            console.log('Service Worker registration failed:', error);
        });
} else {
    console.error("Service workers are not supported.");
}

console.log('Public Key:', publicKey);//DEBUG

// Subscription button logic
document.getElementById('push-button').addEventListener('click', async function () {
    console.log('Button clicked'); // DEBUG
    
    let userId = null;

    // Wait for the service worker to be ready
    const registration = await navigator.serviceWorker.ready;
    console.log('Service Worker is ready:', registration); // DEBUG

    // Check if the user is already subscribed
    const existingSubscription = await registration.pushManager.getSubscription();
    console.log('Existing subscription:', existingSubscription); // DEBUG

    if (!existingSubscription) {
        console.log('No subscription found, subscribing user...'); // DEBUG

        // Ask user for permission to send push notifications
        let permission = await Notification.requestPermission();
        if (permission === 'granted') {
            console.log('Push Notifications - permission accepted');

            // **********Subscribe the user to push notifications***************

            //subscribing the service worker:
            const newSubscription = await registration.pushManager.subscribe({
                userVisibleOnly: true, // visible notifications for the user
                applicationServerKey: urlBase64ToUint8Array(publicKey) // Convert public key here
            });
            console.log('New subscription:', JSON.stringify(newSubscription)); // DEBUG
            /* It looks like this:
            New subscription: {"endpoint":"https://fcm.googleapis.com/fcm/send/dbeXbZ-G9yc:APA91bHnsBWfCc9UnJCreoXGPa4WJlcLr8HQtnn-d-XKevE4xvtZDv1ATHiHqw6P5Kt8oEF0ZFC5PRFvUddXuFfWRsPVmX-FDLWZS0Nxnpy6QxeJEol487xgbSjRr6gLznocHJZjvwvn",
            "expirationTime":null,
            "keys":{"p256dh":"BMpHpen87XTEOWYGub3Q-KXGxD7DbouDRfmIYVioeT_G2AOG5M5hnukNgK_2qE6SUL4MwRdfFFmpA8krDswEBkA","auth":"2G9581mqIGY8jnokL3YWAQ"}}
            */

            // **************Send subscription to the server**************

            // Accomodating data to fit into Asp.NetCore.WebPush:
            // Stringify the subscription object
            const stringifiedSubscription = JSON.stringify(newSubscription);

            // Parse it back into an object so it matches the expected structure
            const parsedSubscription = JSON.parse(stringifiedSubscription);

            // Now construct the object to send to the server
            const subscriptionToSend = {
                endpoint: parsedSubscription.endpoint,
                p256dh: parsedSubscription.keys.p256dh,
                auth: parsedSubscription.keys.auth
            };

            console.log('Sending POST request to server with subscription data...'); // DEBUG
            const response = await fetch('/Home/SavingSubscriptionToDb', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(subscriptionToSend)
            });
            if (response.ok) {
                // Extracting JSON from the response to retrieve the userId
                const data = await response.json();
                console.log('Response data:', data);
                userId = data.id; // Getting the userId from the parsed JSON
                console.log('User ID:', userId); // DEBUG
            } else {
                console.error('Failed to subscribe:', response.statusText);
            }

        } else if (permission === 'denied') {
            console.log('Push Notifications - permission denied');
            return; // Stop further execution if permission is denied
        } else {
            console.log('Push Notifications - permission dismissed or default');
            return; // Stop further execution for default or dismissed
        }
    } else {
        console.log('User is already subscribed.'); // DEBUG

    }

    // ****************Send notification***************
    if (userId) {
        console.log('Sending notification...'); // DEBUG
        //sending the id via form data to the server:
        const formData = new FormData();
        formData.append('userId', userId);
        const notificationResponse = await fetch('/Home/SendNotification', {
            method: 'POST',
            body: formData
        });
        console.log('Notification response:', notificationResponse); // DEBUG
    } else {
        console.log('No userId available to send notification.'); // DEBUG
    }
});

// Function to convert VAPID public key from base64 URL format to Uint8Array
function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    return new Uint8Array([...rawData].map(char => char.charCodeAt(0)));
}

//document.getElementById("test-local-notification").addEventListener("click", function spam() {
//    new Notification("This is a local test notification");
//    console.log("Sending local Notification");
//});

//document.getElementById("test-local-notification-persistent").addEventListener("click", function spam() {
//    navigator.serviceWorker.getRegistration().then((reg) => reg.showNotification("Test persistent notification"))
//    console.log("Sending local Notification");
//});