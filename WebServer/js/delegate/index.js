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

    var a;
    function CheckAll() {
        var len = $('.check').length;
        var checkbox = $('.check');
        if (a == 1) {
            for (var i = 0; i < len; i++) {
                var e = checkbox[i];
                e.checked = false;
            }
            a = 0;
        }
        else {
            for (var i = 0; i < len; i++) {
                var e = checkbox[i];
                e.checked = true;
            }
            a = 1;
        }
    }
    //多选和反选
    $(document).on("click", "#selectAll", function () {
        CheckAll();
    });

    var IDlist = new Array();//返回给后台的需要删除的议程的agendaID的数组
    var $deletedList = new Array(); //存储删除的行，后面用于恢复删除

    //批量删除
    $(document).on("click", ".deleteMutiple", function () {
        var item;
        $(".list_index").each(function () {
            console.log($(this).children("input:checkbox").attr("checked"));
            if ($(this).children("input:checkbox").attr("checked")) {
                item = $(this).children(".ID").val();
                $(this).remove();
                $deletedList.push($(this));
                IDlist.push(item);
            }
        });
    });

    //单个删除
    $(document).on("click", ".delete", function () {
        var item;
        var list_index = $(this).parent().parent();
        item = list_index.children(".ID").val();
        list_index.remove();
        $deletedList.push(list_index);
        IDlist.push(item);
    });

    //恢复删除
    $(document).on("click", ".undoDelete", function (data) {
        //删除恢复
        for (i in $deletedList) {
            $(".contentRightListGroupOutWindowList").append($deletedList[i]);
        }
        //每次恢复删除后，将Array置为空，防止内容重复
        //alert($deletedList.length);
        $deletedList = new Array();
        IDlist = new Array();
    });
    //返回给后台包含需要删除的议程的agendaID的数组
    function getIDList() {
        // for (i in agendaIDlist) {
        //     alert(agendaIDlist[i]);
        // }
        return IDlist;
    }

    //保存并退出
    $(document).on("click", ".confirmDelete", function () {
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
                }
            }
        });

    });
});