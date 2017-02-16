$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

$(function () {
    $(document).on("click", "#cancel1", function () {
        window.location.href = "/MeetingPlace/Index_admin";
    });
    $(document).on("click", "#cancel2", function () {
        window.location.href = "/MeetingPlace/Index_admin";
    });
});

//$(function(){
//   $("#keep").click(function(){
//      alert("保存成功!");
//   }); 
//});

$(function(){
    $("#keep").click(function () {
        var meetingPlaceCapacity = $("#input2").val();
        if (meetingPlaceCapacity < 0) {
            $("#Status").text("会场容量不小于0");
            return;
        }
      $.post("/MeetingPlace/CreateMeetingPlace",{
          meetingPlaceName : $("#input1").val(),
          meetingPlaceCapacity : $("#input2").val()
      }, function (data, textStatus) {
          setStatus(data);
          if (data.Code == 0) {
              window.location.href = "/MeetingPlace/Index_admin";
          }
      },"json"); 
   }); 
});