
const chatUrl = 'https://webpushchatapp-e7d5dac2fjdyfxaa.northeurope-01.azurewebsites.net/';
self.addEventListener('push', async function (event) {
    console.log('Push event received in service worker:');
    let data;

    try {
        // Attempt to parse the received data as JSON
        data = event.data.json();
    } catch (e) {
        // If parsing fails, treat it as plain text
        data = { title: 'Notification', message: event.data.text() };
    }

    const options = {
        body: data.message,
        icon: data.icon,
        badge: data.badge,
        //extracts the notification ID that was sent
        data: { notificationId: data.notificationId },
        vibrate: [1000]
    };
    //retrieve browser tabs, or clients:
    const clientList = await self.clients.matchAll({
        type: 'window',
        includeUncontrolled: true
    });

    // Check if a client with the chat is already focused
    const isChatOpen = clientList.some(client => client.url === chatUrl && client.focused);

    // Only show notification if the chat tab is not open and focused
    if (!isChatOpen) {
        await self.registration.showNotification(data.title, options);
    }

    //calls the method to set ServiceWorkerReveicved prop to true
    //this will only happen if the service gets a push from the server
    fetch(`/Notifications/ServiceWorkerReceivedPush/${data.notificationId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        }
    }).then(response => {
        if (response.ok) {
            console.log('Acknowledgment sent for notification:', data.notificationId);
        } else {
            console.error('Failed to send acknowledgment:', response.status);
        }
    }).catch(error => {
        console.error('Error sending acknowledgment:', error);
    });
});

self.addEventListener('notificationclick', (event) => {
    console.log('User clicked on notification.');
    event.notification.close(); 

    const notificationId = event.notification.data.notificationId;
    // Make a fetch request to our method in our controllet to update the database and set InteractWith prop to true
    fetch(`/Notifications/Interact?notificationId=${notificationId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
    }).then(response => {
        if (response.ok) {
            console.log('Interaction logged successfully');
        } else {
            console.error('Failed to log interaction');
        }
    }).catch(error => {
        console.error('Error logging interaction:', error);
    });

    event.waitUntil(
        clients.matchAll({ type: 'window' }).then( (clientList) => {
            // Check if the app is already open in a tab
            for (var i = 0; i < clientList.length; i++) {
                var client = clientList[i];
                if (client.url === chatUrl && 'focus' in client) {
                    return client.focus();
                }
            }
            // If not, open a new window or tab
            if (clients.openWindow) {
                return clients.openWindow(chatUrl);
            }
        })
    );
});


self.addEventListener('notificationclose', (event) => {
    console.log('User dismissed the notification.');

    const notificationId = event.notification.data.notificationId;

    // Make a fetch request to our method in our controllet to update the database and set InteractWith prop to true
    fetch(`/Notifications/Interact?notificationId=${notificationId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
    }).then(response => {
        if (response.ok) {
            console.log('Dismissal logged successfully');
        } else {
            console.error('Failed to log dismissal');
        }
    }).catch(error => {
        console.error('Error logging dismissal:', error);
    });
});