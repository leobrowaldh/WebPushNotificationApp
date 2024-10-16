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

function addMessageToChat(message) {
    let isCurrentUserMessage = message.userName === username;

    let container = document.createElement('div');
    container.className = "chat-container"; 

    let messageContainer = document.createElement('div');
    messageContainer.className = isCurrentUserMessage ? "sender" : "receiver";
    let sender = document.createElement('p');
    sender.className = "sender-name"; 
    sender.innerHTML = message.userName;

   
    let text = document.createElement('p');
    text.className = "message-text"; 
    text.innerHTML = message.text;

    let when = document.createElement('span');
    when.className = "message-time"; 
    var currentdate = new Date();

    
    let formattedDate = currentdate.getFullYear() + '-' +
        String(currentdate.getMonth() + 1).padStart(2, '0') + '-' +
        String(currentdate.getDate()).padStart(2, '0');

   
    let hours = String(currentdate.getHours()).padStart(2, '0');
    let minutes = String(currentdate.getMinutes()).padStart(2, '0');
    let seconds = String(currentdate.getSeconds()).padStart(2, '0');

    
    when.innerHTML = formattedDate + ' ' + hours + ':' + minutes + ':' + seconds;

    messageContainer.appendChild(sender);
    messageContainer.appendChild(text);
    messageContainer.appendChild(when);

    container.appendChild(messageContainer);
    document.getElementById('chat').appendChild(container);

    const chats = document.getElementById('chat');
    chats.scrollTop = chats.scrollHeight;
}
