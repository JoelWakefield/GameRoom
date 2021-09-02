"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
var username = "";

var sendButton = document.getElementById('sendButton');
var messageInput = document.getElementById('message');


//  set username upon landing
function SetUsername() {
    //  verify username
    var usernameinput = document.getElementById('username-display').innerText;
    if (usernameinput === "")
        return console.log('there is no username');

    username = usernameinput;
    console.log('enter user: ', username);

    //  Alert everyone of the new user
    connection.invoke("ConnectUser", username);

    //  hide username html and show the message panel
    document.getElementById('message-container').style.display = 'block';
    document.getElementById('new-message').style.display = 'block';
}

function postMessage(user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var formattedMsg = `${user} > ${msg}`;
    postElement(formattedMsg);
}


//  functions to post html
function postElement(text) {
    var li = document.createElement('li');
    li.textContent = text;
    document.getElementById('message-list').appendChild(li);
}

function addUserToSelection(user) {
    var op = document.createElement('option');
    op.value = user;
    op.textContent = user;
    document.getElementById('user-selections').appendChild(op);
}


//  establish the receive message function
connection.on("ReceiveMessage", (user, message) => postMessage(user, message));

//  set events for entering and leaving the chat
connection.on("LoadChatRoom", function (users, messages) {
    messages.forEach(m => postMessage(m.senderName, m.text));
    users.forEach(user => addUserToSelection(user));
});

connection.on("UserEntered", function (user) {
    postElement(` - - ${user} has entered the chat - - `);

    if (user !== username)
        addUserToSelection(user);
});

connection.on("UserLeft", function (user) {
    postElement(` - - ${user} has left the chat - - `);

    if (user !== username)
        $(`#user-selections option[value='${user}']`).remove();
});

//  enable connection upon page load?
connection.start().then(function () {
    console.log('connected');
    SetUsername();
}).catch(function (err) {
    return console.error(err.toString());
});


//  add event for the send button
sendButton.addEventListener("click", function (e) {
    var message = messageInput.value;

    if (message === "")
        return alert("please enter some kind of message");

    var isprivate = $('#isPrivate:checked').val();
    console.log(`${username} > [${isprivate}]: ${message}`);

    if (isprivate) {
        var who = $("#user-selections :selected").text();
        var msg = ` -private- ${message}`;

        connection.invoke("SendPrivateMessage", who, username, msg).then(function () {
            messageInput.value = "";
        }).catch(function (err) {
            return console.error(err.toString());
        });
    } else {
        connection.invoke("SendPublicMessage", username, message).then(function () {
            messageInput.value = "";
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }

    e.preventDefault();
});