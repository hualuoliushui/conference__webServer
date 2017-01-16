function getMeetingID(obj) {
    return obj.attr("value");
}
$(function () {
    //会议名称的显示
    $('.meetingGroup').hover(function () {
        $(this).popover('show');
    }, function () {
        $(this).popover('hide');
    });
    

    $(document).on("click", ".show", function () {
        var meetingID = getMeetingID($(this).parent());
        window.location.href = "/Meeting/Show_organizor?meetingID=" + meetingID;
    })

    $(document).on("click", ".new", function () {
        window.location.href = "/Meeting/Add_organizor";
    })

    $('.m_delete').click(function () {
        var meeting = $(this).parent();
        $.confirm({
            title: '删除提示信息',
            content: '确认删除此会议吗？',
            buttons: {
                确定: function () {
                    confirmDelete(meeting);
                },
                取消: function () {
                    return;
                }
            }
        })
        // $(this).parent().fadeToggle();
    });

    function confirmDelete(meeting) {
        var meetingID = getMeetingID(meeting);
        $.ajax({
            type: "GET",
            url: "/Meeting/Delete_organizor?meetingID=" + meetingID,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                console.log(respond);
                setStatus(respond);
                if (respond.Code == 0) {
                    meeting.remove();
                }
            },
            failed: function (respond) {
                console.log("失败");
                console.log(respond);
            }
        });
    }

    //会议分类
    //全部
    $(document).on("click", ".all_meeting", function () {
        $(this).addClass("active");
        $(this).siblings().removeClass("active");
        $(".meetingGroup").each(function () {
            $(this).show();
        });
        first = 1;
    });
    //未开始
    $(document).on("click", ".unopen_meeting", function () {
        $(this).addClass("active");
        $(this).siblings().removeClass("active");
        $(".meetingGroup").each(function () {
            console.log($(this).attr("meetingStatus"));
            if ($(this).attr("meetingStatus") != 1) {
                $(this).hide();
            } else {
                $(this).show();
            }
        });
        first = 1;
    });
    //正在进行
    $(document).on("click", ".opening_meeting", function () {
        $(this).addClass("active");
        $(this).siblings().removeClass("active");
        $(".meetingGroup").each(function () {
            if ($(this).attr("meetingStatus") != 2) {
                $(this).hide();
            } else {
                $(this).show();
            }
        });
        first = 1;
    });
    //已结束
    $(document).on("click", ".opened_meeting", function () {
        $(this).addClass("active");
        $(this).siblings().removeClass("active");
        $(".meetingGroup").each(function () {
            if ($(this).attr("meetingStatus") != 16) {
                $(this).hide();
            } else {
                $(this).show();
            }
        });
        first = 1;
    });
});