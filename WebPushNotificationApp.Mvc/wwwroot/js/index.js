
//Register the service worker
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/sw.js')
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

            // Subscribe the user to push notifications
            const newSubscription = await registration.pushManager.subscribe({
                userVisibleOnly: true, // visible notifications for the user
                applicationServerKey: publicKey // private key is only known by the server
            });
            console.log('New subscription:', newSubscription); // DEBUG

            // Send subscription to the server
            console.log('Sending POST request to server with subscription data...'); // DEBUG
            const response = await fetch('/Home/Subscribe', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(newSubscription)
            });
            console.log('Server response:', response); // DEBUG
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

    // Send notification
    console.log('Sending notification...'); // DEBUG
    const notificationResponse = await fetch('/Home/SendNotification');
    console.log('Notification response:', notificationResponse); // DEBUG
});
