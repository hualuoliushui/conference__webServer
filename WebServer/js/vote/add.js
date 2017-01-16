$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

$(function () {
    var optionNumberCur = 0;
    $(document).on("change", ".optionNum", function () {
        var optionNumber = Number($(".optionNum").val());
        if (optionNumber > optionNumberCur) {
            var optionText = "";
            for (var i = 1 ; i <= optionNumber - optionNumberCur ; i++) {
                optionText += "<p>" + (i + optionNumberCur) + ". <input type='text' index='" + (i + optionNumberCur) + "' class='voteOption' /></p>";
            }
            $("#inputGroup").append(optionText);
        } else if (optionNumber < optionNumberCur) {
           $(".voteOption").each(function () {
                if (Number($(this).attr("index")) > optionNumber) {
                    $(this).parent().remove();
                }
            });
        }
        optionNumberCur = optionNumber;
    })
})

$(function () {
    function getAgendaID() {
        return $("#agendaID").val();
    }

    $(document).on("click", ".cancel", function () {
        var agendaID = getAgendaID();
        window.location.href = "/Vote/Index_organizor?agendaID=" + agendaID;
    })

    $(document).on("click", ".keep", function () {
        $(this).attr("disabled", true);
        var cur = $(this);

        var agendaID = getAgendaID();
        var voteName = $(".voteName").val();
        var voteDescription = $(".voteDescription").val();
        var voteType = $(".voteType option:selected").val();
        var voteOptions = new Array();

        $(".voteOption").each(function () {
            voteOptions.push($(this).val());
        });

        var obj = {
            agendaID: agendaID,
            voteName: voteName,
            voteDescription: voteDescription,
            voteType: voteType,
            voteOptions: voteOptions
        };

        var str = JSON.stringify(obj);

        $.ajax({
            type: "POST",
            url: "/Vote/Add_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                setStatus(respond);
                console.log(JSON.stringify(respond));
                if (respond.Code == 0) {
                    window.location.href = "/Vote/Index_organizor?agendaID=" + agendaID;
                }
            }
        }).done(function () {
            cur.removeAttr("disabled");
        });
    });
});