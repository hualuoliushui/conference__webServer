$(function () {
    setConfirmDeleteBtnStatus()

    function getMeetingID() {
        return $("#meetingID").val();
    }

    function getAgendaID(currentObj) {
        var agendaID = currentObj.parent().parent().find(".ID").val();
        return agendaID;
    }
    //进入附件首页
    $(document).on("click", ".document", function () {
        var agendaID = getAgendaID($(this));
        window.location.href = "/Document/Index_organizor?agendaID=" + agendaID;
    });
    //进入表决首页
    $(document).on("click", ".vote", function () {
        var agendaID = getAgendaID($(this));
        window.location.href = "/Vote/Index_organizor?agendaID=" + agendaID;
    });
    //进入会议项页面
    $(document).on("click", ".returnIndex", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Meeting/Show_organizor?meetingID=" + meetingID;
    });
    //进入新建议程页面
    $(document).on("click", ".new", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Agenda/Add_organizor?meetingID=" + meetingID;
    });
    //进入编辑议程页面
    $(document).on("click", ".edit", function () {
        var agendaID = getAgendaID($(this));
        window.location.href = "/Agenda/Edit_organizor?agendaID=" + agendaID;
    });
    //保存并退出
    $(document).on("click", ".confirmDelete", function () {
        $(this).attr("disabled", true);;
        var cur = $(this);

        //修改后台数据库，删除$deleteItem和$deletedList这两个里面对应的li
        var str = JSON.stringify(getIDList());

        $.ajax({
            type: "POST",
            url: "/Agenda/Delete_organizor",
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


