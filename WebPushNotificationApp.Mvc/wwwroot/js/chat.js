class Message {
    constructor(username, text, when) {
        this.userName = username;
        this.text = text;
        this.when = when;
    }
}

// userName is declared in razor page.
const username = userName;
const textInput = document.getElementById('messageText');
const whenInput = document.getElementById('when');
const chat = document.getElementById('chat');
const messagesQueue = [];

const chats = document.getElementById('chat');
chats.scrollTop = chats.scrollHeight;

document.getElementById('submitButton').addEventListener('click', () => {
    var currentdate = new Date();
    currentdate.innerHTML =
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })

    //Sending notifications to all other users:
    fetch('/Notifications/NotifyAll', { method: 'POST' })
        .then(response => response.text())
        .catch(error => console.error('Error:', error));
});

function clearInputField() {
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessage() {
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;
    
    let when = new Date();
    let message = new Message(username, text);
    sendMessageToHub(message);
}

function formatDate(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-indexed
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

function addMessageToChat(message) {
    let isCurrentUserMessage = message.userName === username;

    let container = document.createElement('div');
    container.className = "chat-container";

    let messageContainer = document.createElement('div');
    messageContainer.className = isCurrentUserMessage ? "sender" : "receiver";

    // Create the username and formatted timestamp span
    let span = document.createElement('span');

    // Use the custom formatDate function
    let formattedDate = formatDate(new Date(message.when)); // Ensure message.when is a Date object

    span.innerHTML = `<strong>${message.userName}</strong> - ${formattedDate}`;

    // Append the span to the messageContainer
    messageContainer.appendChild(span);

    // Append other message elements (if needed)
    let text = document.createElement('p');
    text.className = "message-text";
    text.innerHTML = message.text;

    messageContainer.appendChild(text); // Add text after username and timestamp

    container.appendChild(messageContainer);
    document.getElementById('chat').appendChild(container);

    const chats = document.getElementById('chat');
    chats.scrollTop = chats.scrollHeight; // Scroll to the bottom
}

