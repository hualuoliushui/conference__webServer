function getMeetingID() {
    return $("#meetingID").val();
}

//**************************************************//

$(function () {
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

$(function () {
    $(document).on("click", ".cancel", function () {
        //需要从服务器上获取
        var meetingID = getMeetingID();
        window.location.href = "/Agenda/Index_organizor?meetingID=" + meetingID;
    });

    $(document).on("click", ".keep", function () {
        $(this).attr("disabled", true);
        var cur = $(this);

        var agendaID = $(".agendaID").val();
        var agendaName = $(".agendaName").val();
        var agendaDuration = parseInt($(".agendaDuration").val());
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
                setStatus(respond);
                if (respond.Code == 0) {
                    window.location.href = "/Agenda/Index_organizor?meetingID=" + getMeetingID();
                }
            }
        }).done(function () {
            cur.removeAttr("disabled");
        });
    });
});


