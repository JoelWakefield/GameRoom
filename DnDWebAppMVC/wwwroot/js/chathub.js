"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
var username = "";
var roomOwnerId;
var roomOwnerName;
var message = {};
var player = {};

var sendButton = document.getElementById('sendButton');
var messageInput = document.getElementById('message');


//  set username upon landing
function SetUsername() {
    //  verify username
    var usernameinput = document.getElementById('username-display').innerText;
    if (usernameinput === "")
        return console.log('there is no username');

    username = usernameinput;

    roomOwnerId = $('#Room_OwnerId').val();
    roomOwnerName = $('#Room_OwnerName').text().replace("'s Room", "");

    player = JSON.parse($('#PlayerCharacter').val());
    message = JSON.parse($('#CurrentMessage').val());

    //  Alert everyone of the new user
    connection.invoke("ConnectUser", player, roomOwnerId);
}

function formatDateTime(date) {
    var dt = new Date(Date.parse(date));
    var hours = dt.getHours();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    return `${dt.getMonth() + 1}/${dt.getUTCDate()}/${dt.getFullYear()}  ${hours}:${dt.getMinutes()} ${ampm}`;
}

function postMessage(message) {
    console.log('posting: ', message);

    //  Create elements
    var card = document.createElement('div');
    card.classList.add('message', 'col-lg-12');

    var header = document.createElement('div');
    header.classList.add('justify-content-between', 'row', 'mb-1');

    var sender = document.createElement('div');
    sender.classList.add('col-lg-8', 'text-lg-left', 'font-weight-bold');
    var lbl = (message.isPrivate === true ? `[private-${message.receiverName}]>` : `>`);

    sender.innerText = `${message.senderName}${lbl}`;
    var time = document.createElement('div');
    time.classList.add('col-lg-4', 'text-md-right');
    time.innerText = formatDateTime(message.sentOn);

    var body = document.createElement('div');
    body.classList.add('col-lg-12', 'text-left','text-wrap', 'pb-1');
    body.innerText = message.text;

    //  Combine elements
    header.appendChild(sender);
    header.appendChild(time);

    card.appendChild(header);
    card.appendChild(body);

    postListItem(card);
}


//  functions to post html
function postListItem(item) {
    var li = document.createElement('li');
    li.appendChild(item);
    document.getElementById('message-list').appendChild(li);
}

function postText(text) {
    var li = document.createElement('li');
    li.textContent = text;
    document.getElementById('message-list').appendChild(li);
}

function addUserToSelection(user) {
    var op = document.createElement('option');
    op.value = user.ownerId;
    op.textContent = user.name;
    document.getElementById('user-selections').appendChild(op);
}


//  establish the receive message function
connection.on("ReceiveMessage", (message) => postMessage(message));

//  set events for entering and leaving the chat
connection.on("LoadChatRoom", function (users, messages) {
    console.log('loading:', users, messages);
    messages.forEach(m => postMessage(m));
    users.forEach(user => addUserToSelection(user));
});

connection.on("UserEntered", function (user) {
    postText(` - - ${user.name} has entered the chat - - `);

    if (user.name !== username)
        addUserToSelection(user);
});

connection.on("UserLeft", function (user) {
    postText(` - - ${user} has left the chat - - `);

    if (user !== username)
        $(`#user-selections option[value='${user}']`).remove();
});

//  enable connection upon page load?
connection.start().then(function () {
    SetUsername();
    console.log('connected');
}).catch(function (err) {
    return console.error(err.toString());
});


//  add event for the send button
sendButton.addEventListener("click", function (e) {
    var text = messageInput.value;

    if (text === "")
        return alert("please enter some kind of message");

    var isprivate = ($('#isPrivate:checked').val() === undefined ? false : true);
    message.isPrivate = isprivate;
    console.log(`${username} > [${isprivate}]: ${text}`);

    message.text = text;

    if (isprivate) {
        var toOwner;

        if (message.senderId === roomOwnerId) {
            toOwner = false;
            message.receiverId = $("#user-selections").val();
            message.receiverName = $("#user-selections :selected").text();
        }
        else {
            toOwner = true;
            message.receiverId = roomOwnerId;
            message.receiverName = roomOwnerName;
        }

        console.log(message);

        connection.invoke("SendPrivateMessage", message).then(function () {
            messageInput.value = "";
        }).catch(function (err) {
            return console.error(err.toString());
        });
    } else {
        connection.invoke("SendPublicMessage", message).then(function () {
            messageInput.value = "";
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }

    e.preventDefault();
});