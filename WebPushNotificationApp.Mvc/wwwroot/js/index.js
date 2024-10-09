
//If the user is logged in, only then do we proceed to check for subscriptions:
if (userId) {
    console.log('User logged in with userId = ', userId);
    ManagingSubscriptionState();
}



//checks the current state of webPush subscriptions in this browser and acts accordingly
async function ManagingSubscriptionState() {
    // Preparing service worker
    await registerServiceWorker();
    let registration = await navigator.serviceWorker.ready;
    console.log('Service Worker is ready:', registration);

    // Check the state of subscription in the worker
    let existingSubscription = null;
    try {
        existingSubscription = await registration.pushManager.getSubscription();
        if (!existingSubscription) {
            console.log('No subscription found');
            // Show subscription request, display subscription button
            document.getElementById('notification-overlay').classList.remove('d-none');
        } else {
            console.log('Subscription exists:', existingSubscription);
            //formating subscription for server:
            let subscriptionToSend = formatSubscriptionForServer(existingSubscription);
            // Check that subscription corresponds to the logged-in user
            const isUserSub = await isUserSubscription(subscriptionToSend);
            if (isUserSub) {
                console.log('The subscription belongs to the logged-in user.');
            } else if (isUserSub === false) {
                console.log('The subscription does not belong to this user, unsubscribing...');
                // Unsubscribe the old subscription
                await removeOldSubscription(existingSubscription);
                // Show subscription request, display subscription button
                document.getElementById('notification-overlay').classList.remove('d-none');
            } else {
                console.log('Could not verify subscription due to an error.');
                // Optionally, handle this scenario with a retry or logging
            }
        }
    } catch (error) {
        console.error('Error getting subscription:', error);
        // Show subscription request, display subscription button
        document.getElementById('notification-overlay').classList.remove('d-none');
    }
}


//accept-notifications button listener:
document.getElementById('subscribe-button').addEventListener('click', async function () {
    //remove subscription request and hide subscription button
    document.getElementById('notification-overlay').classList.add('d-none');

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
        let subscriptionToSend = formatSubscriptionForServer(newSubscription);

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
            console.log('Subscription ID:', data.id);
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

});

//deny-notifications button listener:
document.getElementById('no-thanks').addEventListener('click', function () {
    console.log('Notifications rejected by user.');

    //remove subscription request and hide subscription button
    document.getElementById('notification-overlay').classList.add('d-none');
});

//Converts the subscription to server expected format:
function formatSubscriptionForServer(newSubscription) {
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
    return subscriptionToSend;
}

//Checks if the current subscription is actually the logged in users subscription:
async function isUserSubscription(subscription) {
    try {
        const response = await fetch('/Notifications/CheckUserSubscriptionAsync', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(subscription)
        });

        if (response.ok) {
            const data = await response.json();
            return data.isUserSubscription; // true or false based on server response
        } else {
            console.error('Failed to check user subscription:', response.statusText);
            return null; // Indeterminate state (error occurred)
        }
    } catch (error) {
        console.error('Error verifying subscription:', error);
        return null; // Indeterminate state (error occurred)
    }
}


//unsubscribing and removing subscription from db:

async function removeOldSubscription(existingSubscription) {
    try {
        const unsubscribeResult = await existingSubscription.unsubscribe();
        if (unsubscribeResult) {
            console.log('Successfully unsubscribed.');

            // Remove the subscription from the database
            const response = await fetch('/Notifications/RemoveSubscriptionAsync', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(existingSubscription)
            });

            if (response.ok) {
                console.log('Subscription removed from the database.');
            } else {
                console.error('Failed to remove subscription from the database.');
            }
        } else {
            console.error('Failed to unsubscribe from PushAPI.');
        }
    } catch (error) {
        console.error('Error unsubscribing:', error);
    }
}



// send-notification button-listener:
document.getElementById('push-button').addEventListener('click', async function () {
    console.log('Push button clicked'); 
    // Wait for the service worker to be ready
    const registration = await navigator.serviceWorker.ready;
    console.log('Service Worker is ready:', registration);
    
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


// If not present, the function registers the service worker.
//If allready registered in the browser, it uses the existing one.
//It also updates the service worker if there are changes in the code.
async function registerServiceWorker() {
    if ('serviceWorker' in navigator) {
        try {
            registration = await navigator.serviceWorker.register('sw.js');
            console.log('Service Worker registered with scope:', registration.scope);
        } catch (error) {
            console.error('Service Worker registration failed:', error);
        }
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

