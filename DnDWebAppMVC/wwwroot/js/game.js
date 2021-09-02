(async () => {
    const body = document.getElementById('new-message');

    body.addEventListener("click", (e) => {
        console.log('click');

        if ($(e.target).hasClass('btn-send'))
            sendMessage();
    });

    function sendMessage() {
        console.log('sending...');
        console.log($("#form").serialize());

        $.ajax({
            async: true,
            data: $("#form").serialize(),
            type: "POST",
            url: "/GameRooms/Send/",
            success: function (partialView) {
                console.log('sent.');
                $('#message-feed').html(partialView);
            }
        });

        $('#text-input').val('').focus();
    }

    function refreshMessages() {
        console.log('refreshing...');

        $.ajax({
            async: true,
            data: $("#form").serialize(),
            type: "POST",
            url: "/GameRooms/Refresh/",
            success: function (partialView) {
                console.log('refreshed.');
                $('#message-feed').html(partialView);
            }
        });
    }

    var seconds = 15;
    window.setInterval(refreshMessages, seconds * 1000);
})();