$(document).ready(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.start().then(function () {
        viewModel.roomList();
        viewModel.userList();
    }).catch(function (err) {
        return console.error(err);
    });

    connection.on("newMessage", function (messageView) {
        var isMine = messageView.user === viewModel.myName();
        var message = new ChatMessage(messageView.id, messageView.message, messageView.timestamp, messageView.user, isMine);
        viewModel.chatMessages.push(message);
        $(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);
    });

    connection.on("getProfileInfo", function (displayName) {
        viewModel.myName(displayName);
        viewModel.isLoading(false);
    });

    connection.on("addUser", function (user) {
        viewModel.userAdded(new ChatUser(user.username, user.fullName, user.currentRoom, user.device));
    });

    connection.on("removeUser", function (user) {
        viewModel.userRemoved(user.username);
    });

    connection.on("addChatRoom", function (room) {
        viewModel.roomAdded(new ChatRoom(room.id, room.name));
    });

    connection.on("updateChatRoom", function (room) {
        viewModel.roomUpdated(new ChatRoom(room.id, room.name));
    });

    connection.on("removeChatRoom", function (id) {
        viewModel.roomDeleted(id);
    });

    connection.on("removeChatMessage", function (id) {
        viewModel.messageDeleted(id);
    });

    connection.on("onError", function (message) {
        viewModel.serverInfoMessage(message);
        $("#errorAlert").removeClass("d-none").show().delay(5000).fadeOut(500);
    });

    connection.on("onInfoMessage", function (message) {
        viewModel.serverInfoMessage(message);
        $("#errorAlert").removeClass("d-none").show().delay(5000).fadeOut(500);
    });

    connection.on("onRoomDeleted", function (message) {
        viewModel.serverInfoMessage(message);
        $("#errorAlert").removeClass("d-none").show().delay(5000).fadeOut(500);

        if (viewModel.chatRooms().length == 0) {
            viewModel.joinedRoom("");
        }
        else {
            setTimeout(function () {
                $("#rooms-list li a")[0].click();
            }, 50);
        }
    });

    function AppViewModel() {
        var self = this;

        self.message = ko.observable("");
        self.chatRooms = ko.observableArray([]);
        self.chatUsers = ko.observableArray([]);
        self.chatMessages = ko.observableArray([]);
        self.joinedRoom = ko.observable("");
        self.joinedRoomId = ko.observable("");
        self.serverInfoMessage = ko.observable("");
        self.myName = ko.observable("");
        self.isLoading = ko.observable(true);

        self.onEnter = function (d, e) {
            if (e.keyCode === 13) {
                self.sendNewMessage();
            }
            return true;
        }
        self.filter = ko.observable("");
        self.filteredChatUsers = ko.computed(function () {
            if (!self.filter()) {
                return self.chatUsers();
            } else {
                return ko.utils.arrayFilter(self.chatUsers(), function (user) {
                    var displayName = user.displayName().toLowerCase();
                    return displayName.includes(self.filter().toLowerCase());
                });
            }
        });

        self.sendNewMessage = function () {
            self.sendToRoom(self.joinedRoom(), self.message());

            self.message("");
        }

        self.sendToRoom = function (roomName, message) {
            if (roomName.length > 0 && message.length > 0) {
                fetch('/Message', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ room: roomName, message: message })
                });
            }
        }

        self.joinRoom = function (room) {
            connection.invoke("Join", room.name()).then(function () {
                self.joinedRoom(room.name());
                self.joinedRoomId(room.id());
                self.userList();
                self.messageHistory();
            });
        }

        self.roomList = function () {
            fetch('/Room')
                .then(response => response.json())
                .then(data => {
                    self.chatRooms.removeAll();
                    for (var i = 0; i < data.length; i++) {
                        self.chatRooms.push(new ChatRoom(data[i].id, data[i].name));
                    }

                    if (self.chatRooms().length > 0)
                        self.joinRoom(self.chatRooms()[0]);
                });
        }

        self.userList = function () {
            connection.invoke("GetUsers", self.joinedRoom()).then(function (result) {
                self.chatUsers.removeAll();
                for (var i = 0; i < result.length; i++) {
                    self.chatUsers.push(new ChatUser(result[i].email,
                        result[i].currentRoom))
                }
            });
        }

        self.createRoom = function () {
            var roomName = $("#roomName").val();
            fetch('/Room', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name: roomName })
            });
        }
        
        self.deleteRoom = function () {
            fetch('/Room/' + self.joinedRoomId(), {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: self.joinedRoomId() })
            });
        }

        self.deleteMessage = function () {
            var messageId = $("#itemToDelete").val();

            fetch('/Message/' + messageId, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: messageId })
            });
        }

        self.messageHistory = function () {
            fetch('/Message/Room/' + viewModel.joinedRoom())
                .then(response => response.json())
                .then(data => {
                    self.chatMessages.removeAll();
                    for (var i = 0; i < data.length; i++) {
                        var isMine = data[i].user == self.myName();
                        self.chatMessages.push(new ChatMessage(data[i].id,
                            data[i].message,
                            data[i].timestamp,
                            data[i].user,
                            isMine))
                    }

                    $(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);
                });
        }

        self.roomAdded = function (room) {
            self.chatRooms.push(room);
        }

        self.roomUpdated = function (updatedRoom) {
            var room = ko.utils.arrayFirst(self.chatRooms(), function (item) {
                return updatedRoom.id() == item.id();
            });

            room.name(updatedRoom.name());

            if (self.joinedRoomId() == room.id()) {
                self.joinRoom(room);
            }
        }

        self.roomDeleted = function (id) {
            var temp;
            ko.utils.arrayForEach(self.chatRooms(), function (room) {
                if (room.id() == id)
                    temp = room;
            });
            self.chatRooms.remove(temp);
        }
    }

    function ChatRoom(id, name) {
        var self = this;
        self.id = ko.observable(id);
        self.name = ko.observable(name);
    }

    function ChatUser(email, currentRoom) {
        var self = this;
        self.userName = ko.observable(email);
        self.displayName = ko.observable(email);
        self.currentRoom = ko.observable(currentRoom);
    }

    function ChatMessage(id, message, timestamp, user, isMine) {
        var self = this;
        self.id = ko.observable(id);
        self.content = ko.observable(message);
        self.timestamp = ko.observable(timestamp);
        self.timestampRelative = ko.pureComputed(function () {
            var date = new Date(self.timestamp());
            var now = new Date();
            var diff = Math.round((date.getTime() - now.getTime()) / (1000 * 3600 * 24));
            
            var { dateOnly, timeOnly } = formatDate(date);
            
            if (diff == 0)
                return `Today, ${timeOnly}`;
            if (diff == -1)
                return `Yesterday, ${timeOnly}`;

            return `${dateOnly}`;
        });
        self.timestampFull = ko.pureComputed(function () {
            var date = new Date(self.timestamp());
            var { fullDateTime } = formatDate(date);
            return fullDateTime;
        });
        self.from = ko.observable(user);
        self.isMine = ko.observable(isMine);
    }

    function formatDate(date) {
        // Get fields
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var d = hours >= 12 ? "PM" : "AM";

        // Correction
        if (hours > 12)
            hours = hours % 12;

        if (minutes < 10)
            minutes = "0" + minutes;

        // Result
        var dateOnly = `${day}/${month}/${year}`;
        var timeOnly = `${hours}:${minutes} ${d}`;
        var fullDateTime = `${dateOnly} ${timeOnly}`;

        return { dateOnly, timeOnly, fullDateTime };
    }

    var viewModel = new AppViewModel();
    ko.applyBindings(viewModel);
});
