$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

//$(function(){
//   $("#keep").click(function(){
//      alert("保存成功!");
//   }); 
//});

$(function () {
    $(document).on("click", "#cancel1", function () {
        window.location.href = "/Device/Index_admin";
    });
    $(document).on("click", "#cancel2", function () {
        window.location.href = "/Device/Index_admin";
    });
});


$(function(){
   $("#keep").click(function(){
      $.post("/Device/CreateDevice",{
          deviceID : $("#input1").attr("deviceID"),
          deviceIndex : $("#input1").val(),
          IMEI : $("#input2").val()
      }, function (data, textStatus) {
          setStatus(data);
          if (data.Code == 0) {
              window.location.href = "/Device/Index_admin";
          }
      },"json"); 
   }); 
});

//$(function(){
//   $("#keep").click(function(){
//      $.ajax({
          
//      });
//   }); 
//});