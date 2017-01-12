$(function () {
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

    $(document).on("click", "#newPeople", function () {
        window.location.href = "/User/Add_organizor";
    });

    $(document).on("click", ".cancel", function () {

    });

    $(document).on("click", ".save", function () {
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
                $("#Status").text(respond.Message);
                if (respond.Code == 0) {
                    window.location.href = "/Delegate/Index_organizor?meetingID="+meetingID;
                }
            }
        });
    });
});

