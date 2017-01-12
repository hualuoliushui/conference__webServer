$(function () {
    function getMeetingID() {
        return $("#meetingID").val();
    }

    function getUserName() {
        return $("#userName").val();
    }

    function getUserDepartment() {
        return $("#userDepartment").val();
    }

    function getUserJob() {
        return $("#userJob").val();
    }

    function getUserDescription() {
        return $("#userDescription").val();
    }

    //==================================

    $(document).on("click", ".cancel", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Delegate/Add_organizor?meetingID=" + meetingID;
    });

    //$(document).on("click", ".view_delegate_index", function () {
    //    var meetingID = getMeetingID();
    //    window.location.href = "/Delegate/Index_organizor?meetingID=" + meetingID;
    //});
    //$(document).on("click", ".view_meeting_show", function () {
    //    var meetingID = getMeetingID();
    //    window.location.href = "/Meeting/Show_organizor?meetingID=" + meetingID;
    //});
    //$(document).on("click", ".returnIndex", function () {
    //    window.location.href = "/Account/Organizor";
    //});

    //=====================================

    $(document).on("click", ".save", function () {
        var userName = getUserName();
        var userDepartment = getUserDepartment();
        var userJob = getUserJob();
        var userDescription = getUserDescription();

        var obj = {
            userName: userName,
            userDepartment: userDepartment,
            userJob: userJob,
            userDescription: userDescription
        };

        var str = JSON.stringify(obj);

        $.ajax({
            type: "POST",
            url: "/User/Add_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                $("#Status").text(respond.Message);
                if (respond.Code == 0) {
                    var meetingID = getMeetingID();
                    window.location.href = "/Delegate/Index_organizor?meetingID="+meetingID;
                }
            }
        });
    })
});