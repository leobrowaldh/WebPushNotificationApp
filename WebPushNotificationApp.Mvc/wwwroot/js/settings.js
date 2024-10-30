import { removeSubscriptionFromDatabase, isUserSubscription, registerServiceWorker, urlBase64ToUint8Array } from './notifications.js'; 

//Checks if the service workers subscription exists and correspond to this user, 
//and updates the notification slider and the test notification button accordingly.
async function ManagingSubscriptionState() {

    const pushButton = document.getElementById('push-button');
    const registration = await navigator.serviceWorker.ready;
    const existingSubscription = await registration.pushManager.getSubscription();

    if (existingSubscription) {
        console.log('Subscription exists');
        const isUserSub = await isUserSubscription(existingSubscription);
        if (isUserSub) {
            pushButton.disabled = false; 
            document.getElementById('notification-switch').checked = true;
            pushButton.classList.remove('disabled'); 
            document.getElementById('status-message').textContent = 'Notifications are enabled.';
        } else {
            await registerServiceWorker();
            console.log('The subscription does not belong to this user, ask them to subscribe');
        }
    } else {
        await registerServiceWorker();
        console.log('No subscription found');
        pushButton.disabled = true; 
        document.getElementById('notification-switch').checked = false;
        pushButton.classList.add('disabled');
        document.getElementById('status-message').textContent = 'Notifications are disabled.';
    }
}


async function subscribeUser() {

    let permission = await Notification.requestPermission();

    if (permission === 'granted') {
        await registerServiceWorker();
        const registration = await navigator.serviceWorker.ready;

        const newSubscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: urlBase64ToUint8Array(publicKey)
        });

        const response = await fetch('/Notifications/SavingSubscriptionToDb', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newSubscription)
        });

        if (response.ok) {
            console.log('subscription correctly saved to db.');
            document.getElementById('status-message').textContent = 'Notifications are enabled.';
            ManagingSubscriptionState();
        } else {
            console.error('Failed to subscribe:', response.statusText);
            document.getElementById('notification-switch').checked = false; // Revert switch on failure
        }
    } else {
        console.log('Push Notifications - permission denied');
        document.getElementById('notification-switch').checked = false; // Revert switch if denied
    }

}


async function unsubscribeUser() {

    const registration = await navigator.serviceWorker.ready;
    const existingSubscription = await registration.pushManager.getSubscription();
    if (existingSubscription) {
        const unsubscribeResult = await existingSubscription.unsubscribe();
        if (unsubscribeResult) {
            console.log('Successfully unsubscribed from browser.');
            await removeSubscriptionFromDatabase(existingSubscription);
            document.getElementById('status-message').textContent = 'Notifications are disabled.';
        }
    }

}

// send-notification button-listener:
document.getElementById('push-button').addEventListener('click', async function () {


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

        if (notificationResponse.ok) {
            const data = await notificationResponse.json();
            const notificationId = data.notificationId; // Get the notification ID from the response

            // Here you can also trigger the actual push notification if needed
            console.log('Notification sent with ID:', notificationId);
            console.log('Notification response:', notificationResponse);
        }
        } else {
            console.log('No userId available to send notification.');
        }
    });


document.getElementById('notification-switch').addEventListener('change', async function () {
    this.disabled = true;

    const pushButton = document.getElementById('push-button');
    if (this.checked) {
        pushButton.disabled = false; // Enable the button
        pushButton.classList.remove('disabled'); // Remove Bootstrap's disabled class
        // Call the subscribeUser function if you want to subscribe immediately

        await subscribeUser();

    } else {
        pushButton.disabled = true; // Disable the button
        pushButton.classList.add('disabled'); // Add Bootstrap's disabled class
        // Call the unsubscribeUser function if you want to unsubscribe immediately
        await unsubscribeUser();
    }
    this.disabled = false;
});

ManagingSubscriptionState()

let installPrompt = null;
const installButton = document.querySelector("#install");

window.addEventListener("beforeinstallprompt", (event) => {
    event.preventDefault();
    installPrompt = event;
    installButton.removeAttribute("hidden");
});

installButton.addEventListener("click", async () => {
    if (!installPrompt) {
        return;
    }
    const result = await installPrompt.prompt();
    console.log(`Install prompt was: ${result.outcome}`);
    disableInAppInstallPrompt();
});

function disableInAppInstallPrompt() {
    installPrompt = null;
    installButton.setAttribute("hidden", "");
}
window.addEventListener("appinstalled", () => {
    disableInAppInstallPrompt();
});
