$(function () {
    function getMeetingID() {
        return $("#meetingID").val();
    }

    function getUserName() {
        return $("#userName").val();
    }

    function getUserDepartment() {
        return $("#userDepartment").val();
    }

    function getUserJob() {
        return $("#userJob").val();
    }

    function getUserDescription() {
        return $("#userDescription").val();
    }

    //==================================

    $(document).on("click", ".cancel", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Delegate/Add_organizor?meetingID=" + meetingID;
    });
    //=====================================

    $(document).on("click", ".save", function () {
        var userName = getUserName();
        var userDepartment = getUserDepartment();
        var userJob = getUserJob();
        var userDescription = getUserDescription();

        var obj = {
            userName: userName,
            userDepartment: userDepartment,
            userJob: userJob,
            userDescription: userDescription
        };

        var str = JSON.stringify(obj);

        $.ajax({
            type: "POST",
            url: "/User/Add_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                setStatus(respond);
                if (respond.Code == 0) {
                    var meetingID = getMeetingID();
                    window.location.href = "/Delegate/Index_organizor?meetingID=" + meetingID;
                }
            }
        });
    })




    ///////////////////////////////////////======================================
    //上传文件事件
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
        xhr.open("POST", "/User/Import", true);

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
                var meetingID = getMeetingID();
                window.location.href = "/Delegate/Index_organizor?meetingID=" + meetingID;
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
});