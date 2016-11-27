$(function () {
    $(":input").focus(function () {
        $(this).addClass("focus");
    }).blur(function () {
        $(this).removeClass("focus");
    });
});

$(function () {
    $(document).on("click", "#cancel1", function () {
        window.location.href = "/User/Index_admin";
    });
    $(document).on("click", "#cancel2", function () {
        window.location.href = "/User/Index_admin";
    });
});

$(function () {
    $("#keep").click(function () {
        var roleID;
        $("#select option").each(function (i) {
            if (this.selected == true) {
                roleID = this.value;
            }
        });
        $.post("/User/CreateUser", {
            userName: $("#input1").val(),
            userDepartment: $("#input2").val(),
            userJob: $("#input3").val(),
            userDescription: $("#description").val(),
            roleID:roleID
        }, function (data, textStatus) {
            $("#Status").text(data.Message);
            if (data.Code == 0) {
                window.location.href = "/User/Index_admin"
            }
        }, "json");
    });
});

$(function () {
    $.get("/Role/GetRolesForUser", function (data, textStatus) {
        {
            $("#Status").text(data.Message);
            if (data.Code == 0) {
                var roleSelect = $("#select");
                //循环遍历 下拉框绑定
                for (var i = 0 ; i < data.Result.length; i++) {
                    //第一种方法
                    var opt = "<option value='" + data.Result[i].roleID + "' > " + data.Result[i].roleName + "</option>";
                    roleSelect.append(opt);
                }
            }
        }
    });
});
