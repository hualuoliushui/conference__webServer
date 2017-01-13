$(function () {
    $(document).on("click", ".logout", function () {
        $.ajax({
            type: "GET",
            url: "/Account/Logout",
            dataType: "json",
            success: function (respond) {
                setStatus(respond);
                if (respond.Code == 0) {
                    window.location.href = "/Account/Index";
                }
            }
        });
    });
});

//=================================
function setStatus(respond) {
    $("#Status").text(respond.Message);
    if (respond.Code == 5) {
        $("#Status").append(JSON.stringify(respond.Result));
    }
}