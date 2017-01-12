//获取议程ID
function getAgendaID() {
    return $("#agendaID").val();
}

//返回文件首页
$(function () {
    $(document).on("click", ".returnIndex", function () {
        var agendaID = getAgendaID();
        window.location.href = "/Document/Index_organizor?agendaID=" + agendaID;
    });
});

//提交文件转换请求
function startConvert() {
    var agendaID = getAgendaID();
    $.ajax({
        type: "GET",
        url: "/Document/StartConvert?agendaID=" + agendaID,
        dataType: "json",
        success: function (respond) {
            console.log("转换:");
            console.log(respond);
            $("#Status").text(respond.Message);
        }
    });
}

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

function createXmlHttpRequest(){
    if(window.ActiveXObject){
        //IE
        return new ActiveXObject("Microsoft.XMLHTTP");
    }else if(window.XMLHttpRequest){
        return new XMLHttpRequest();
    }
}

//上传文件
function upload() {
    xhr = createXmlHttpRequest();

    var fd = new FormData();

    var agendaID = getAgendaID();

    fd.append("file", file);
    fd.append("agendaID", agendaID);

    //监听事件
    xhr.upload.addEventListener("progress", uploadProgress, false);

    //设置回调函数
    xhr.onreadystatechange = uploadReady;
    //发送文件和表单自定义参数
    xhr.open("POST", "/Document/Upload", true);

    xhr.send(fd);
}

function uploadReady() {
    if (xhr.readyState == 4 && xhr.status == 200) {
        isUploading = false;
        var respond = JSON.parse(xhr.response);
        console.log("上传:");
        console.log(respond);
        $("#Status").text(respond.Message);
        if (respond.Code == 0) {
            startConvert();
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