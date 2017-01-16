///
function getMeetingID() {
    return $("#meetingID").val();
}

//===============================

//加载界面和动态创建
$(function () {
    //进入会议项页面
    $(document).on("click", ".returnIndex", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Meeting/Show_organizor?meetingID=" + meetingID;
    });
    //进入新建参会人员页面
    $(document).on("click", ".new", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Delegate/Add_organizor?meetingID=" + meetingID;
    });

    //======================================

    function device_role_change(the) {
        var meetingID = getMeetingID();

        var tempDelegate = the.parent().parent();

        var delegateID = tempDelegate.children(".delegateID").val();

        var deviceID = tempDelegate.find(".deviceID").val();

        var userMeetingRole = tempDelegate.find(".userMeetingRole").val();

        var obj = {
            delegateID: delegateID,
            meetingID: meetingID,
            deviceID: deviceID,
            userMeetingRole: userMeetingRole
        };

        var str = JSON.stringify(obj);

        $.ajax({
            type: "POST",
            url: "/Delegate/Edit_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                setStatus(respond);
                console.log(respond);
            }
        });
    }

    $(".deviceID").change(function(){
        device_role_change($(this));
    });

    $(".userMeetingRole").change(function () {
        device_role_change($(this));
    });

    //=======================================

    //保存并退出
    $(document).on("click", ".confirmDelete", function () {
        $(this).attr("disabled", true);
        var cur = $(this);

        //修改后台数据库，删除$deleteItem和$deletedList这两个里面对应的li
        var str = JSON.stringify(getIDList());

        $.ajax({
            type: "POST",
            url: "/Delegate/Delete_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                console.log(respond);
                $("#Status").text(respond.Message);
                if (respond.Code == 0) {
                    $deletedList = new Array();
                    IDlist = new Array();
                    setConfirmDeleteBtnStatus()
                }
            }
        }).done(function () {
            cur.removeAttr("disabled");
        });

    });
});