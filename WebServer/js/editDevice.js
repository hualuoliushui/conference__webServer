$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

$(function(){
   $("#keep").click(function(){
      alert("保存成功!");
   }); 
});

$(function(){
    $.get("/Device/GetDeviceForUpdate?deviceID=862823023300546", function (data, textStatus) {
        var deviceID = data.Result.deviceID;
        var IMEI = data.Result.IMEI;
       var deviceIndex = data.Result.deviceIndex;
       
       $("#input1").attr("deviceID", deviceID);
       $("#input1").val(deviceIndex);
       $("#input2").val(IMEI);
   },"json"); 
});


$(function(){
   $("#keep").click(function(){
       $.post("/Device/UpdateDevice", {
           deviceID: $("#input1").attr("deviceID"),
           IMEI: $("#input2").val(),
           deviceIndex : $("#input1").val()
      },function(data,textStatus){
          var code = data.code;
          var message = data.message;

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