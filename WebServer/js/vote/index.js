$(function () {

    function getMeetingID() {
        return $("#meetingID").val();
    }

    function getAgendaID(currentObj) {
        return $("#agendaID").val();
    }

    function getVoteID(currentObj) {
        var voteID = currentObj.parent().siblings(".voteID").val();
        return voteID;
    }

    $(document).on("click", ".returnIndex", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Agenda/Index_organizor?meetingID=" + meetingID;
    });

    //进入新建页面
    $(document).on("click", ".new", function () {
        var agendaID = getAgendaID();
        window.location.href = "/Vote/Add_organizor?agendaID=" + agendaID;
    });

    //进入编辑议程页面
    $(document).on("click", ".edit", function () {
        var voteID = getVoteID($(this));
        window.location.href = "/Vote/Edit_organizor?voteID=" + voteID;
    });

    //==========================================

    //保存并退出
    $(document).on("click", ".confirmDelete", function () {
        $(this).attr("disabled", true);
        var cur = $(this);

        //修改后台数据库，删除$deleteItem和$deletedList这两个里面对应的li
        var str = JSON.stringify(getIDList());

        $.ajax({
            type: "POST",
            url: "/Vote/Delete_organizor",
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


