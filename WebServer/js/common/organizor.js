$(function () {
    $(":input").focus(function () {
        $(this).addClass("focus");
    }).blur(function () {
        $(this).removeClass("focus");
    });
    $(".confirmDelete").hide();
});

$(function () {
    $(document).on("click", ".logout", function () {
        $.ajax({
            type: "GET",
            url: "/Account/Logout",
            dataType: "json",
            success: function (respond) {
                setStatus(respond);
                if (respond.Code == 0) {
                    window.location.href = "/Account/Index";
                }
            }
        });
    });
});

//=================================
//设置“确认删除”按钮显示
function setConfirmDeleteBtnStatus() {
    if (IDlist.length == 0) {
        $(".confirmDelete").hide();
        $(".undoDelete").attr("disabled", true);
    } else {
        $(".confirmDelete").show();
        $(".undoDelete").attr("disabled", false);
    }
}


////////////////////////////////////////
//状态设置
function isEmptyObject(e) {
    var t;
    for (t in e)
        return !1;
    return !0;
}
function setStatus(respond) {
    $("#Status").empty().append("<b>"+respond.Message+"</b>");
    if (respond.Code == 5) {
        if (typeof (respond.Result) == undefined) {
            return;
        }
        $("#Status").append("<br/>");
        $.each(respond.Result, function () {
            var str = "<b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + JSON.stringify($(this)[0]) + "<br/><b>";
            $("#Status").append(str);
        });
    }
}

//================================================
var a;

var IDlist = new Array();//返回给后台的需要删除的议程的agendaID的数组
var $deletedList = new Array(); //存储删除的行，后面用于恢复删除

//===========================
//批量删除
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

//返回给后台包含需要删除的议程的agendaID的数组
function getIDList() {
    console.log("获取ID列表");
    console.log("IDlist:");
    console.log(IDlist);
    return IDlist;
}

$(function () {
    //多选和反选
    $(document).on("click", "#selectAll", function () {
        CheckAll();
    });

    //批量删除
    $(document).on("click", ".deleteMutiple", function () {
        console.log("批量删除");
        var item;
        console.log("list_index");
        console.log($(".list_index"));
        $(".list_index").each(function () {
            console.log($(this).find("input:checkbox").attr("value"));
            if ($(this).find("input:checkbox").is(':checked')) {
                item = $(this).find(".ID").val();
                console.log("item" + item);
                $(this).remove();
                $deletedList.push($(this));
                IDlist.push(item);
            }
        });
        console.log("IDlist:");
        console.log(IDlist);

        setConfirmDeleteBtnStatus()

    });

    //单个删除
    $(document).on("click", ".delete", function () {
        console.log("单个删除");
        var item;
        var list_index = $(this).parent().parent();
        item = list_index.children(".ID").val();
        list_index.remove();
        $deletedList.push(list_index);
        IDlist.push(item);

        console.log("IDlist:");
        console.log(IDlist);

        setConfirmDeleteBtnStatus()
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

        console.log("IDlist:");
        console.log(IDlist);

        setConfirmDeleteBtnStatus()
    });
});

//========================================================
//限制为整数
function isInteger(obj) {
    reg = /^[-+]?\d+$/;
    if (!reg.test(obj)) {
        $("#Status").html("<b>请输入整数</b>");
        return false;
    } else {
        $("#Status").html("");
        return true;
    }
}


//========================================================
//在列表中查询
var searchlist = new Array();
var first = 1;
function search(searchStr) {
    searchStr= $.trim(searchStr);
    console.log("搜索词:");
    console.log(searchStr);
    if (searchStr.length == 0) {
        searchlist.each(function () {
            $(this).show();
        });
        return;
    }
    searchlist.each(function () {
        var item = $(this);
        console.log(item.find(".searchStr").text());
        var name = item.find(".searchStr").text(); 
        var position = name.indexOf(searchStr);
        if (position >= 0) {
            item.show();
        } else {
            item.hide();
        }
    });
   
}

$(function () {
    $(document).on("click", "#searchBtn", function () {
        var searchStr = $("#searchInput").val();
        if (first) {
            searchlist = $(".list_index");
            first = 0;
        }
        search(searchStr);
    });
});
