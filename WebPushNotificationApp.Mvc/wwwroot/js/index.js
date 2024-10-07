
console.log('Public Key:', publicKey);//DEBUG


// Subscription:
document.getElementById('subscribe-button').addEventListener('click', async function () {
    console.log('subscribe button clicked'); 

    registerServiceWorker();

    // Wait for the service worker to be ready
    const registration = await navigator.serviceWorker.ready;
    console.log('Service Worker is ready:', registration);

    // Check if the user is already subscribed
    let existingSubscription = null;
    try {
        existingSubscription = await registration.pushManager.getSubscription();
        if (!existingSubscription) {
            console.log('No subscription found');
        } else {
            console.log('Subscription exists:', existingSubscription);
        }
    } catch (error) {
        console.error('Error getting subscription:', error);
    }
 

    if (!existingSubscription) {
        console.log('Subscribing user...'); 

        // Ask user for permission through the browsers NotificationAPI:
        let permission = await Notification.requestPermission();
        if (permission === 'granted') {
            console.log('Push Notifications - permission accepted');

            //subscribing the service worker:
            const newSubscription = await registration.pushManager.subscribe({
                userVisibleOnly: true, // visible notifications for the user
                applicationServerKey: urlBase64ToUint8Array(publicKey) // Convert public key to the expected format.
            });
            console.log('New subscription:', JSON.stringify(newSubscription)); 

            
            // **************Sending subscription to the server**************


            // Stringifying the subscription object, to access its hidden properties
            const stringifiedSubscription = JSON.stringify(newSubscription);

            // Parse it back into an object, to acces its properties
            const parsedSubscription = JSON.parse(stringifiedSubscription);

            // Constructing the object to send to the server, as Asp.NetCore.WebPush expects it:
            const subscriptionToSend = {
                endpoint: parsedSubscription.endpoint,
                p256dh: parsedSubscription.keys.p256dh,
                auth: parsedSubscription.keys.auth
            };

            console.log('Sending POST request to server with subscription data...'); 
            const response = await fetch('/Notifications/SavingSubscriptionToDb', {
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
                console.log('User ID:', data.id);
                localStorage.setItem('userId', data.id); // Store userId locally
            } else {
                console.error('Failed to subscribe:', response.statusText);
            }

        } else if (permission === 'denied') {
            console.log('Push Notifications - permission denied');
            return; 
        } else {
            console.log('Push Notifications - permission dismissed or default');
            return; 
        }
    } else {
        console.log('User is already subscribed.'); 
    }

});

// Notification:
document.getElementById('push-button').addEventListener('click', async function () {
    console.log('Push button clicked'); 
    // Wait for the service worker to be ready
    const registration = await navigator.serviceWorker.ready;
    console.log('Service Worker is ready:', registration);

    const userId = localStorage.getItem('userId'); // Retrieve userId from local storage
    
    if (userId) {
        console.log('Sending notification...'); 
        //sending the id via form data to the server:
        const formData = new FormData();
        formData.append('userId', userId);
        const notificationResponse = await fetch('/Notifications/SendNotification', {
            method: 'POST',
            body: formData
        });
        console.log('Notification response:', notificationResponse); 
    } else {
        console.log('No userId available to send notification.');
    }
});


//************************************ Helper Funcions ************************************************** */

// If not present, the function registers the service worker.
//If allready registered in the browser, it uses the existing one.
//It also updates the service worker if there are changes in the code.
function registerServiceWorker() {
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
}

// Function to convert VAPID public key from base64 URL format to Uint8Array
function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    return new Uint8Array([...rawData].map(char => char.charCodeAt(0)));
}