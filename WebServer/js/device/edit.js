$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

$(function () {
    $(document).on("click", "#cancel1", function () {
        window.location.href = "/Device/Index_admin";
    });
    $(document).on("click", "#cancel2", function () {
        window.location.href = "/Device/Index_admin";
    });
});

$(function () {
    var url = location.search;
    var Request = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1)　//去掉?号
        var strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            Request[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    var deviceID = Request["deviceID"];

    $.get("/Device/GetDeviceForUpdate?deviceID=" + deviceID, function (data, textStatus) {
        $("#Status").text(data.Message);
        var deviceID = data.Result.deviceID;
        var IMEI = data.Result.IMEI;
        var deviceIndex = data.Result.deviceIndex;

        $("#input1").attr("deviceID", deviceID);
        $("#input1").val(deviceIndex);
        $("#input2").val(IMEI);
    }, "json");
});
 
$(function(){
   $("#keep").click(function(){
       $.post("/Device/UpdateDevice", {
           deviceID: $("#input1").attr("deviceID"),
           IMEI: $("#input2").val(),
           deviceIndex : $("#input1").val()
       }, function (data, textStatus) {
           $("#Status").text(data.Message);
           if (data.Code == 0)
               window.location.href = "/Device/Index_admin";

      },"json"); 
   }); 
});

//$(function(){
//   $("#keep").click(function(){
//      $.ajax({
//         type : "POST",
//         url : "",
//         dataType : "json",
//         data : {
//             deviceIndex : $("#input1").val(),
//             deviceID : $("#input2").val();
//         }
//      });
//   });
//});