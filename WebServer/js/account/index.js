$(function () {
    $(document).on("click", "#loginBtn", function () {
        var userName = $(".userName").val();
        var password = $(".password").val();

        var system = $(".system:checked").val();

        var obj = {
            userName: userName,
            password: password
        };

        var str = JSON.stringify(obj);

        $.ajax({
            type: "POST",
            url: "/Account/Login",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                console.log(respond);
                if (respond.Code == 0) {
                    if (system) {
                        window.location.href = "/Account/" + system;
                    }
                } else {
                    alert(respond.Message);
                }
            }
        });

    })
});