$("#timeSelect1").datetimepicker();
$("#timeSelect2").datetimepicker();

$(function () {
    $(":input").focus(function () {
        $(this).addClass("focus");
    }).blur(function () {
        $(this).removeClass("focus");
    });
});

function getMeetingID() {
    return $("#meetingID").val();
}

$(function () {
    $(document).on("click", ".cancel", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Meeting/Show_organizor?meetingID=" + meetingID;
    });

    $(document).on("click", ".keep", function () {

        $(this).attr("disabled", true);
        var cur = $(this);

        var meetingID = getMeetingID();
        var meetingName = $(".meetingName").val();
        var meetingPlaceID = $(".meetingPlaceID").val();
        var meetingToStartTime = $(".meetingToStartTime").val();
        var meetingStartedTime = $(".meetingStartedTime").val();
        var meetingSummary = $(".meetingSummary").val();

        var obj = {
            meetingID: meetingID,
            meetingName: meetingName,
            meetingPlaceID: meetingPlaceID,
            meetingToStartTime: meetingToStartTime,
            meetingStartedTime: meetingStartedTime,
            meetingSummary: meetingSummary
        };
        var str = JSON.stringify(obj);

        $.ajax({
            type: "POST",
            url: "/Meeting/Edit_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                setStatus(respond);
                if (respond.Code == 0) {
                    window.location.href = "/Meeting/Show_organizor?meetingID="+meetingID;
                }
            }
        }).done(function () {
            cur.removeAttr("disabled");
        });
    });
 });
