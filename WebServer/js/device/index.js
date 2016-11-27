//加载界面和动态创建
$(function () {
    $.ajax({
        type: "GET",
        url: "/Device/GetDevices",
        dataType: "json",
        success: function (response) {
            $("#Status").text(response.Message);
            var appendListHtml = "";
            var itemCount = 0;
            var FreezeStateBtn = ["btn-default", ""];
            var FreezeStateString = ["冻结", "已冻结"];
            $.each(response.Result, function (i, item) {
                itemCount = i; //i循环是从0开始
                appendListHtml +=
                    "<li href='#' class='list-group-item container-fluid' deviceID=" + item.deviceID + ">" +
                    "<label class='col-md-4 list-If'>" + item.IMEI + "</label>" +
                    "<label class='col-md-4 list-If'>" + item.deviceIndex + "</label>" +
                    "<button class='btn btn-default col-md-1 col-md-offset-1 edit' type='button'>编辑</button>" +
                    "<button class='" + FreezeStateBtn[item.deviceFreezeState] + " " + "btn col-md-1 col-md-offset-1 freeze' buttonType=" + item.deviceFreezeState + " type='button'>" + FreezeStateString[item.deviceFreezeState] + "</button>" +
                    "</li>"; //漏了一个空格
            });
            //alert(appendHtml);//测试完成
            //alert(itemCount);

            $(".contentRightListGroupOutWindowList").append(appendListHtml);

            //关于分页按钮生成
            (function () {
                var maxListContain = 5;
                var appendPageHtml = "";
                var page = Math.ceil((itemCount + 1) / maxListContain); //向上取正小数
                //alert(page);
                //如果小于1个栏目那么分页隐藏
                if (page <= 1) {
                    $("paginationOutWindow").hide();
                } else {
                    appendPageHtml += "<li><a href='#'>&laquo;</a></li>";
                    for (var i = 0; i < page; i++) {
                        appendPageHtml += "<li><a href='#' paginationNumber=" + (i + 1) + ">" + (i + 1) + "</a></li>";
                    }
                    appendPageHtml += "<li><a href='#'>&raquo;</a></li>";
                    $(".pagination").append(appendPageHtml);
                }
            })();

            //人员状态的生成
            (function () {

            })();
        }
    });
});

//关于项目列表分页的实现
$(function () {
    var currentPage = 1;
    var maxListContain = 5; //默认议题展示区域最多放置5个
    $(document).on("click", ".pagination li:first", function () {
        var $cr = $(".contentRightListGroupOutWindowList"); //这里内容包括所有的列表项 这是内框
        var $br = $(".contentRightListGroupOutWindow"); //这里固定住展示页面的大小 这是外框
        var len = $cr.find("li").length; //找到有多少个列表在这里
        var page_count = Math.ceil(len / maxListContain); //最后一页的页数
        var height = $br.height(); //让每一次都翻页为5个就是为外框的高度
        if (!$cr.is(":animated")) { //判断议题展示区域是否正处于动画
            if (currentPage == 1) { //如果显示完最后一个议题，则倒回来从上往下重新显示
                $cr.animate({
                    top: '-=' + height * (page_count - 1)
                }, "fast");
                currentPage = page_count;
            } else {
                $cr.animate({
                    top: "+=" + height
                }, "fast"); //向上翻滚查看议题
                currentPage--;
            }
        }
        return false;
    });
    $(document).on("click", ".pagination li:last", function () {
        var $cr = $(".contentRightListGroupOutWindowList"); //这里内容包括所有的列表项 这是内框
        var $br = $(".contentRightListGroupOutWindow"); //这里固定住展示页面的大小 这是外框
        var len = $cr.find("li").length; //找到有多少个列表在这里
        var page_count = Math.ceil(len / maxListContain); //最后一页的页数
        var height = $br.height(); //让每一次都翻页为5个就是为外框的高度
        if (!$cr.is(":animated")) { //判断议题展示区域是否正处于动画
            if (currentPage == page_count) { //如果显示完最后一个议题，则倒回来从上往下重新显示
                $cr.animate({
                    top: "0px"
                }, "fast");
                currentPage = 1;
            } else {
                $cr.animate({
                    top: "-=" + height
                }, "fast"); //向上翻滚查看议题
                currentPage++;
            }
        }
        return false;
    });
    $(document).on("click", ".pagination li", function () {
        var number = $(this).children().attr("paginationNumber");
        if (number) {
            var $cr = $(".contentRightListGroupOutWindowList"); //这里内容包括所有的列表项 这是内框
            var $br = $(".contentRightListGroupOutWindow"); //这里固定住展示页面的大小 这是外框
            var len = $cr.find("li").length; //找到有多少个列表在这里
            var page_count = Math.ceil(len / maxListContain); //最后一页的页数
            var height = $br.height(); //让每一次都翻页为5个就是为外框的高度
            if (!$cr.is(":animated")) { //判断议题展示区域是否正处于动画
                $cr.animate({
                    top: -(height * (number - 1))
                }, "fast"); //向上翻滚查看议题
                currentPage = number;
            }
            return false;
        }
    });
});

//编辑保存按钮的功能
$(function () {
    $(document).on("click", ".edit", function () {
        var deviceID = $(this).parent().attr("deviceID");
        location.href = "/Device/Edit_admin?deviceID=" + deviceID;
    });
    $(document).on("click", ".freeze", function () {
        var deviceID = $(this).parent().attr("deviceID");
        //alert(userID);
        if ($(this).attr("buttonType") == 1) {
            $.ajax({
                type: "POST",
                url: "/Device/UpdateDeviceAvailable",
                data: {
                    "deviceID": deviceID,
                    "state": 0
                },
                dataType: "json",
                success: function (response) {
                    $("#Status").text(data.Message);
                }
            });
            $(this).attr("buttonType", 0);
            $(this).addClass("btn-default");
            $(this).html("冻结");
        } else {
            $.ajax({
                type: "POST",
                url: "/Device/UpdateDeviceAvailable",
                data: {
                    "deviceID": deviceID,
                    "state": 0
                },
                dataType: "json",
                success: function (response) {
                    $("#Status").text(data.Message);
                }
            });
            $(this).attr("buttonType", 1);
            $(this).removeClass("btn-default");
            $(this).html("已冻结");
        };
    });
});

//右侧按钮功能
$(function () {
    $(document).on("click", ".returnIndex", function () {
        window.location.href = "/Account/Admin";
    });
    $(document).on("click", ".new", function () {
        window.location.href = "/Device/Add_admin";
    });
});

$(function () {
    $(document).on("click", "#mainContentRight .input-group .btn", function () {
        //alert($("#mainContentRight .input-group .form-control").val());
    });
});

//状态功能展示
$(function () {
    $("#ContentRightHeadState span");
});


/*功能作废 源代码作废
//删除功能实现
$(function() {
    //普通删除
    var $deleteItem = new Array(); //用一个数字来存储
    $(document).on("click", ".contentRightListGroupOutWindowList .close", function() {
        $deleteItem.push($(this).parent().detach()); //detach可以保持绑定数据和事件
        //alert(deleteItem);测试完成
    });

    $(document).on("click", ".deleteRedo", function(data) {
        for (i in $deleteItem) {
            $(".contentRightListGroupOutWindowList").append($deleteItem[i]);
        }
    });

    //批量删除
    $(document).on("change", ".contentRightListGroupBanderListInput", function() {
        if ($(".contentRightListGroupBanderListInput").prop("checked") == true) {
            $("input[name=items]:checkbox").prop('checked', true);
        }
        if ($(".contentRightListGroupBanderListInput").prop("checked") == false) {
            $("input[name=items]:checkbox").prop('checked', false);
        }
    });
});
*/



/*
 * 
 * 
 * 
 * 
 */