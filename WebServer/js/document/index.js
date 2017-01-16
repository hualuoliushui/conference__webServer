﻿//获取议程ID
function getAgendaID() {
    return $("#agendaID").val();
}

//获取会议ID
function getMeetingID() {
    return $("#meetingID").val();
}

//右侧按钮功能
$(function () {
    $(document).on("click", ".returnIndex", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Agenda/Index_organizor?meetingID=" + meetingID;
    });
    $(document).on("click", ".new", function () {
        var agendaID = getAgendaID();
        window.location.href = "/Document/Add_organizor?agendaID=" + agendaID;
    });


    //==========================================
    //保存并退出
    $(document).on("click", ".confirmDelete", function () {
        $(this).attr("disabled", true);
        var cur = $(this);

        //修改后台数据库，删除$deleteItem和$deletedList这两个里面对应的li
        var str = JSON.stringify(getIDList());
        console.log(getIDList());
        $.ajax({
            type: "POST",
            url: "/Document/Delete_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                console.log(respond);
                setStatus(respond);
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
