///
function getMeetingID() {
    return $("#meetingID").val();
}

//===============================

//存储参会人员信息
var delegates = new Array();
var newDelegates = new Array();

function setDelegates() {
    $(".delegate").each(function () {
        var delegate = $(this);

        var delegateID_node = delegate.find(".ID");
        var delegateID = delegateID_node.val();

        var userMeetingRole_node = delegate.find(".userMeetingRole");
        var userMeetingRole = userMeetingRole_node.val();

        var deviceID_node = delegate.find(".deviceID");
        var deviceID = deviceID_node.val();

        delegates.push({
            delegateID: delegateID,
            userMeetingRole: userMeetingRole,
            deviceID_node : deviceID_node,
            deviceID: deviceID
        });
    })
    return delegates;
}

//获取全部设备
var devices = new Array();
function getDevices() {
    $(".deviceID option").each(function () {
        var deviceID = $(this).val();
        var deviceIndex = parseInt($(this).text());
        devices.push({
            deviceID: deviceID,
            deviceIndex: deviceIndex
        });
    });

    $(".leftOption").each(function () {
        var deviceID = $(this).val();
        var deviceIndex = $(this).attr("index");
        var checkSame = 0;
        for (var i = 0;i<devices.length; i++) {
            if (devices[i].deviceID == deviceID) {
                checkSame = 1;
                break;
            }
        }
        if (checkSame == 1) {
            return true;
        }
        devices.push({
            deviceID: deviceID,
            deviceIndex: deviceIndex
        });
    });
    console.log(devices);
}

function appOption() {
    console.log("app Option:");
    $(".deviceID").each(function () {
        for (var i = 0 ; i < devices.length; i++) {
            if (parseInt($(this).text()) != devices[i].deviceIndex) {

                var str = "<option value='" + devices[i].deviceID + "' >" + devices[i].deviceIndex + "</option>";
                //console.log(str);
                $(this).append(str);
            }
        }
    });
}

//改变设备或角色
function device_role_change(delegateID,deviceID,userMeetingRole) {
    var meetingID = getMeetingID();

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
            if (respond.Code == 0) {
                for (var i = 0 ; i < delegates.length; i++) {
                    if (delegates[i].delegateID == delegateID) {
                        delegates[i].userMeetingRole = userMeetingRole;
                        delegates[i].deviceID = deviceID;
                        delegates[i].deviceID_node.val(deviceID);
                    }
                }
            }
        }
    });
}

//加载界面和动态创建
$(function () {
    getDevices();
    appOption();

    setDelegates();
    console.log(delegates);

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

    //进入排座界面
    $(document).on("click", ".seatArrange", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Delegate/SeatArrange_organizor?meetingID=" + meetingID;
    });

    //======================================

   
    $(".deviceID").change(function () {
        var deviceID_node = $(this);

        var newDeviceID = parseInt($(this).val());

        var delegateID = parseInt(deviceID_node.attr("delegateID"));

        var oldDeviceID = undefined;

        console.log("第零次");
        for (var i = 0 ; i < delegates.length; i++) {
            console.log(delegates[i].deviceID);
        }

        console.log("new:" + newDeviceID);
        for (var i = 0 ; i < delegates.length; i++) {
            if (delegates[i].delegateID == delegateID) {
                console.log("i:" + i);

                oldDeviceID = delegates[i].deviceID;

                device_role_change(delegateID, newDeviceID, delegates[i].userMeetingRole);

                console.log("第一次");
                for (var i = 0 ; i < delegates.length; i++) {
                    console.log(delegates[i].deviceID);
                }
            }
        }
        
        if (oldDeviceID) {
            for (var i = 0 ; i < delegates.length; i++) {
                if (delegates[i].delegateID != delegateID && delegates[i].deviceID == newDeviceID) {
                    device_role_change(delegates[i].delegateID, oldDeviceID, delegates[i].userMeetingRole);
                    console.log("第二次");
                    for (var i = 0 ; i < delegates.length; i++) {
                        console.log(delegates[i].deviceIndex);
                    }
                }
            }
        }
    });

    $(".userMeetingRole").change(function () {
        var userMeetingRole_node = $(this);
        var userMeetingRole = userMeetingRole_node.val();
        var delegateID = userMeetingRole_node.attr("delegateID");
        var deviceID;
        for (var i = 0 ; i < delegates.length; i++) {
            if (delegates[i].delegateID == delegateID) {
                deviceID = delegates[i].deviceID;
            }
        }

        device_role_change(delegateID, deviceID, userMeetingRole);
    });

    //=======================================

    //确认删除
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
