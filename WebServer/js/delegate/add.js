//////
function getMeetingID() {
    return $("#meetingID").val();
}

function getUserID() {
    return $(".userID").val();
}

function getDeviceID() {
    return $(".deviceID").val();
}
function getMeetingRole() {
    return $(".meetingRole").val();
}



//===============================

$(function () {
    

    $(document).on("click", "#newPeople", function () {
        window.location.href = "/User/Add_organizor";
    });

    $(document).on("click", ".cancel", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Delegate/Index_organizor?meetingID=" + meetingID;
    });

    $(document).on("click", ".save", function () {
        $(this).attr("disabled", true);
        var cur = $(this);

        var userID = getUserID();
        var meetingID = getMeetingID();
        var deviceID = getDeviceID();
        var userMeetingRole = getMeetingRole();

        var obj = {
            userID: userID,
            meetingID: meetingID,
            deviceID: deviceID,
            userMeetingRole: userMeetingRole
        };

        var str = JSON.stringify(obj);

        $.ajax({
            type: "POST",
            url: "/Delegate/Add_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                setStatus(respond);
                if (respond.Code == 0) {
                    window.location.href = "/Delegate/Index_organizor?meetingID="+meetingID;
                }
            }
        }).done(function () {
            cur.removeAttr("disabled");
        });
    });
});

