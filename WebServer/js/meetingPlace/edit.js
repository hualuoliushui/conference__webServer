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
    $.get("/MeetingPlace/GetMeetingPlaceForUpdate?MeetingPlaceID=" + meetingPlaceID, function (data, textStatus) {
        $("#Status").text(data.Message);
       var meetingPlaceID = data.Result.meetingPlaceID;
       var meetingPlaceName = data.Result.meetingPlaceName;
       var meetingPlaceCapacity = data.Result.meetingPlaceCapacity;
       var seatType = data.Result.seatType;
       
       $("#input1").attr("meetingPlaceID",meetingPlaceID);
       $("#input1").val(meetingPlaceName);
       $("#input2").val(meetingPlaceCapacity);
    }, "json");
});


$(function(){
    $("#keep").click(function () {
        var meetingPlaceCapacity = $("#input2").val();
        if (meetingPlaceCapacity < 0) {
            $("#Status").text("会场容量不小于0");
            return;
        }
        $.post("/MeetingPlace/UpdateMeetingPlace", {
            meetingPlaceID: $("#input1").attr("meetingPlaceID"),
            meetingPlaceName: $("#input1").val(),
            meetingPlaceCapacity: $("#input2").val(),
        }, function (data, textStatus) {
            setStatus(data);
            if (data.Code == 0) {
                console.log(data);
                window.location.href = "/MeetingPlace/Index_admin"
            }
        }, "json");
    });
});