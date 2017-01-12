$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

$(function () {
    $(document).on("click", "cancel", function () {
        //需要从服务器上获取
        //var meetingID = $(".meetingID").val();
        //window.location.href = "/Agenda/Index_organizor?meetingID=" + meetingID;
    });

    $(document).on("click", ".keep", function () {
        var agendaID = $(".agendaID").val();
        var agendaName = $(".agendaName").val();
        var agendaDuration = $(".agendaDuration").val();
        var userID = $(".userID option:selected").val();
        var test = "";
        $.ajax({
            type: "POST",
            url: "/Agenda/Edit_organizor",
            data: {
                agendaID: agendaID,
                agendaName: agendaName,
                agendaDuration: agendaDuration,
                userID: userID
            },
            dataType: "json",
            success: function (respond) {
                $("#Status").text(respond.Message);
            }
        });
    });
});


