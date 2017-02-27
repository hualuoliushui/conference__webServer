$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

$(function () {
    $(document).on("click", "#cancel1", function () {
        window.location.href = "/User/Index_admin";
    });
    $(document).on("click", "#cancel2", function () {
        window.location.href = "/User/Index_admin";
    });
});

$(function () {
    $.get("/Role/GetRolesForUser", function (data, textStatus) {
        {
            $("#Status").text(data.Message);
            if(data.Code==0){
                var roleSelect = $("#select");
                //循环遍历 下拉框绑定
                for (var i = 0 ; i < data.Result.length; i++) {
                    //第一种方法
                    var opt = "<option value='"+data.Result[i].roleID+"' > "+data.Result[i].roleName + "</option>";
                    roleSelect.append(opt);
                }
                getUserForUpdate();
            }
        }
    });
});

function getUserForUpdate() {
    var url = location.search;
    var Request = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1)　//去掉?号
        var strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            Request[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    var userID = Request["userID"];
    $.get("/User/GetUserForUpdate?userID=" + userID, function (data, textStatus) {
        $("#Status").text=data.Message;
        var userID = data.Result.userID;
        var userName = data.Result.userName;
        var userDepartment = data.Result.userDepartment;
        var userJob = data.Result.userJob;
        var roleID = data.Result.roleID;
        var userLevel = data.Result.userLevel;
        var userDescription = data.Result.userDescription;

        $("#input1").attr("userID", userID);
        $("#input1").val(userName);
        $("#input2").val(userDepartment);
        $("#input3").val(userJob);
        $("#description").val(userDescription);

        $("#select option").each(function (i) {
            if (this.value == roleID) {
                this.selected = true;
            }
        });
        $("#selectLevel option").each(function (i) {
            if (this.value == userLevel) {
                this.selected = true;
            }
        });

    }, "json");
}

$(function(){
    $("#keep").click(function () {
        var roleID;
        $("#select option").each(function (i) {
            if ( this.selected == true) {
                roleID = this.value;
            }
        });
        var userLevel;
        $("#selectLevel option").each(function (i) {
            if (this.selected == true) {
                userLevel = this.value;
            }
        });
       $.post("/User/UpdateUser", {
          userID : $("#input1").attr("userID"),
          userName : $("#input1").val(),
          userDepartment : $("#input2").val(),
          userJob : $("#input3").val(),
          userDescription: $("#description").val(),
          roleID: roleID,
          userLevel:userLevel
       }, function (data, textStatus) {
           setStatus(data);
           if (data.Code == 0)
               window.location.href = "/User/Index_admin";
       },"json");
    });
}); 
    


function RemoveOption() {
    $("#select option").remove();
}

function AppendOption(value, text) {
    $("#select").append("<option value='" + value + "'>" + text + "</option>");
}