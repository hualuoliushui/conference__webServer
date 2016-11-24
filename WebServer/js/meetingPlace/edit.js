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
    var meetingPlaceID = Request["meetingPlaceID"];
   $.get("/MeetingPlace/GetMeetingPlaceForUpdate?MeetingPlaceID="+meetingPlaceID,function(data,textStatus){
       var meetingPlaceID = data.Result.meetingPlaceID;
       var meetingPlaceName = data.Result.meetingPlaceName;
       var meetingPlaceCapacity = data.Result.meetingPlaceCapacity;
       
       $("#input1").attr("meetingPlaceID",meetingPlaceID);
       $("#input1").val(meetingPlaceName);
       $("#input2").val(meetingPlaceCapacity);
   },"json"); 
});

$(function(){
    $("#keep").click(function(){
       $.post("/MeetingPlace/UpdateMeetingPlace",{
           meetingPlaceID : $("#input1").attr("meetingPlaceID"),
           meetingPlaceName : $("#input1").val(),
           meetingPlaceCapacity : $("#input2").val()
       },function(data,textStatus){
           alert(JSON.stringify(data));
           if (data.Code == 0) {
               window.location.href="/MeetingPlace/Index_admin"
           }
       },"json"); 
    });
});