class Message {
    constructor(username, text, when) {
        this.userName = username;
        this.text = text;
        this.when = when;
    }
}

//Press enter to send message and shift+enter to make a new line in the textbox.
document.addEventListener("DOMContentLoaded", function () {
    const messageTextArea = document.getElementById('messageText');
    const messageForm = document.getElementById('message-form');
    const submitButton = document.getElementById('submitButton');


    messageTextArea.addEventListener('keydown', function (event) {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            submitForm();
        }
    });

    messageTextArea.addEventListener('keydown', function (event) {
        if (event.key === 'Enter' && event.shiftKey) {
            return;
        }
    });

    function submitForm() {
        $(messageForm).submit();
    }
    });

// userName is declared on the page
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

//Just adds date and time
function formatDate(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
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
    let formattedDate = formatDate(new Date(message.when));

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
    chats.scrollTop = chats.scrollHeight; // Scroll to the bottom for a better user experiance
}

