$(function () {
    $(":input").focus(function () {
        $(this).addClass("focus");
    }).blur(function () {
        $(this).removeClass("focus");
    });
});

$(function () {
    $("#keep").click(function () {
        $.post("", {
            userID: $("#input1").attr("userID"),
            userName: $("#input1").val(),
            userDepartment: $("#input2").val(),
            userJob: $("#input3").val(),
            userDescription: $("#description").val()
        }, function (data, textStatus) {
            alert(JSON.stringify(data));
        }, "json");
    });
});