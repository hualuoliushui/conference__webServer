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
function isEmptyObject(e) {
    var t;
    for (t in e)
        return !1;
    return !0;
}
function setStatus(respond) {
    $("#Status").text(respond.Message);
    if (respond.Code) {
        if (typeof (respond.Result) == undefined) {
            return;
        }
        $("#Status").append("<br/>");
        $.each(respond.Result, function () {
            var str = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + JSON.stringify($(this)[0]) + "<br/>";
            $("#Status").append(str);
        });
    }
}
