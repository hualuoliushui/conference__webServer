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
        var userLevel;
        var userLevel = parseInt($("#selectLevel").val());
        $.post("/User/CreateUser", {
            userName: $("#input1").val(),
            userDepartment: $("#input2").val(),
            userJob: $("#input3").val(),
            userDescription: $("#description").val(),
            roleID: roleID,
            userLevel: userLevel
        }, function (data, textStatus) {
            setStatus(data);
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

var file = null;
var isUploading = false;

$(function () {
    $(".upload").click(function () {
        if (isUploading) {
            return;
        }
        upload();
        isUploading = true;
    });
});


///////////////////////////////////////======================================
//上传文件

var input = document.getElementById("file_upload");

//文件域选择文件时, 执行readFile函数
input.addEventListener('change', readFile, false);

function readFile() {
    file = this.files[0];
}

/*******************************************/

var xhr;

function createXmlHttpRequest() {
    if (window.ActiveXObject) {
        //IE
        return new ActiveXObject("Microsoft.XMLHTTP");
    } else if (window.XMLHttpRequest) {
        return new XMLHttpRequest();
    }
}

//上传文件
function upload() {
    xhr = createXmlHttpRequest();

    var fd = new FormData();

    fd.append("file", file);

    //监听事件
    xhr.upload.addEventListener("progress", uploadProgress, false);

    //设置回调函数
    xhr.onreadystatechange = uploadReady;
    //发送文件和表单自定义参数
    xhr.open("POST","/User/Import", true);

    $("#Status").text("导入中。。。");

    xhr.send(fd);
}

function uploadReady() {
    if (xhr.readyState == 4 && xhr.status == 200) {
        isUploading = false;
        var respond = JSON.parse(xhr.response);
        console.log("上传:");
        console.log(respond);
        setStatus(respond);
        if (respond.Code == 0) {
            window.location.href = "/User/Index_admin";
        }
    }
}

function uploadProgress(evt) {
    if (evt.lengthComputable) {
        //evt.loaded：文件上传的大小 evt.total：文件总的大小
        var percentComplete = Math.round((evt.loaded) * 100 / evt.total);
        //加载进度条，同时显示信息
        $("#percent").html(percentComplete + '%')
        $("#progressNumber").css("width", "" + percentComplete + "px");
    }
}