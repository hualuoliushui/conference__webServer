//时间选择器
$("#timeSelect1").datetimepicker();
$("#timeSelect2").datetimepicker();


$(function () {
    //取消创建
    $(document).on("click", ".cancel", function () {
        window.location.href = "/Account/Organizor";
    })
})

$(function () {
    //会议信息和参会人员界面的交换
    $("#delegates").hide();
    $(document).on("click", "#addMeeting_next", function () {
        $("#meeting").hide();
        $("#delegates").show();
    });
    $(document).on("click", ".last", function () {
        $("#meeting").show();
        $("#delegates").hide();
    });
})

$(function () {
    //添加选项到另一个select对象
    function add(options,to){
        var html = "";
        $.each(options, function () {
            html+="<option value='" + $(this).attr("value") + "' >" + $(this).text() + "</option>";
        });
        to.append(html);
    }
    //从一个select对象中移除选项
    function remove(options, from) {
        $.each(options, function () {
            var option = $(this);
            $.each(from.children(), function () {
                if ($(this).val() == option.val()) {
                    $(this).remove();
                }
            });
        });
    }
    //当select2：即参会人员delegate的选项改变时，更新主持人select中的内容
    function delegateChange(select2) {
        $("#host").empty();
        var html = "";
        select2.children().each(function () {
            html +="<option value='" + $(this).attr("value") + "' >" + $(this).text() + "</option>";
        });
        $("#host").append(html);
    }

    $("#right1").click(function () {
        var $options = $("#select1 option:selected");
        add($options, $("#select2"));
        add($options, $("#select3"));
        remove($options, $("#select1"));
        delegateChange($("#select2"));
    });

    $("#left1").click(function () {
        var $options = $("#select2 option:selected");
        add($options, $("#select1"));
        remove($options, $("#select2"));
        remove($options, $("#select3"));
        remove($options, $("#select4"));
        delegateChange($("#select2"));
    });
    $("#allSelect").click(function () {
        var $options = $("#select1 option");
        add($options, $("#select2"));
        add($options, $("#select3"));
        remove($options, $("#select1"));
        delegateChange($("#select2"));
    });
    $("#allUndo").click(function () {
        var $options = $("#select2 option");

        add($options, $("#select1"));
        remove($options, $("#select2"));
        remove($options, $("#select3"));

        var $options = $("#select4 option");
        add($options, $("#select1"));
        remove($options, $("#select4"));

        delegateChange($("#select2"));
    });
    $("#select1").dblclick(function () {
        var $options = $("option:selected", this);
        add($options, $("#select2"));
        add($options, $("#select3"));
        remove($options, $("#select1"));
        delegateChange($("#select2"));
    });
    $("#select2").dblclick(function () {
        var $options = $("option:selected", this);
        add($options, $("#select1"));
        remove($options, $("#select2"));
        remove($options, $("#select3"));
        remove($options, $("#select4"));
        delegateChange($("#select2"));
    });

    $("#right2").click(function () {
        var $options = $("#select3 option:selected");
        add($options, $("#select4"));
        remove($options, $("#select2"));
        remove($options, $("#select3"));
        delegateChange($("#select2"));
    });
    $("#left2").click(function () {
        var $options = $("#select4 option:selected");
        add($options, $("#select2"));
        add($options, $("#select3"));
        remove($options, $("#select4"));
        delegateChange($("#select2"));
    });
    $("#select3").dblclick(function () {
        var $options = $("option:selected", this);
        add($options, $("#select4"));
        remove($options, $("#select2"));
        remove($options, $("#select3"));
        delegateChange($("#select2"));
    });
    $("#select4").dblclick(function () {
        var $options = $("option:selected", this);
        add($options, $("#select2"));
        add($options, $("#select3"));
        remove($options, $("#select4"));
        delegateChange($("#select2"));
    });
});

$(function () {
    $(".add").click(function () {
        $(this).attr("disabled", true);
        var cur = $(this);

        var meetingID = -1;
        var meetingStatus = 1;
        var meetingName = $(".meetingName").val();
        var meetingPlaceID = $(".meetingPlaceID").val();
        var meetingToStartTime = $(".meetingToStartTime").val();
        var meetingStartedTime = $(".meetingStartedTime").val();
        var meetingSummary = $(".meetingSummary").val();

        var hostID = $(".hostID").val();
        var speakerIDs = new Array();
        $(".speakerID").children().each(function () {
            speakerIDs.push($(this).val());
        });
        var otherIDs = new Array();
        $(".otherID").children().each(function () {
            if($(this).val() != hostID ){
                otherIDs.push($(this).val());
            }
        })

        var obj = {
            meeting: {
                meetingID: meetingID,
                meetingName: meetingName,
                meetingPlaceID: meetingPlaceID,
                meetingToStartTime: meetingToStartTime,
                meetingStartedTime: meetingStartedTime,
                meetingSummary: meetingSummary
            },
            delegates: {
                hostID: hostID,
                speakerIDs: speakerIDs,
                otherIDs: otherIDs
            }
        };
        var str = JSON.stringify(obj);

        console.log(obj);
        console.log(str);
        $.ajax({
            type: "POST",
            url: "/Meeting/Add_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            }
        }).done(function (respond) {
            setStatus(respond);
            if (respond.Code == 0) {
                window.location.href = "/Account/Organizor";
            }
        }).done(function () {
            cur.removeAttr("disabled");
        });
        event.stopPropagation();
    });
});
