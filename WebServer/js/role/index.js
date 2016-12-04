$(function () {
    $.ajax({
        type: "get",
        url: "/Role/GetPermissions",
        datatype: "json",
        success: function (data) {
            $("#Status").text(data.Message);
            if (data.Code == 0)
            {
                //添加角色表头
                var roleTitle = "<div class='role role_head'>角色</div>";
                $("#roleTable").append(roleTitle);
                //添加权限列表
                for (var i = 0 ; i < data.Result.length; i++) {
                    addPermission(data.Result[i].permissionID, data.Result[i].permissionName);
                }
                getRoles();
            } 
        }
    });
});

$(function () {
    $(":input").focus(function () {
        $(this).addClass("focus");
    }).blur(function () {
        $(this).removeClass("focus");
    });
});

//在权限列表中添加权限选择框,并且在角色列表表头添加权限
function addPermission(permissionID, permissionName) {
    var permission = "<div class='create_role_permission_list'>" +
        "<label class='check-inline'>" +
        "<input type='checkbox' name='permission' value='" +
        permissionID + "' />" + permissionName +
        "</label>" +
        "</div>";
    $("#createRole").append(permission);
    //添加权限表头
    addPermissionForRoleTable(permissionID,permissionName);
}

function addPermissionForRoleTable(permissionID,permissionName){
    var permissions_th = "<div class='permission_head' value=" + permissionID + ">" +
        permissionName+
        "</div>";
    $("#permissionTable").append(permissions_th);
}

//根据权限及角色，在角色列表添加角色
function getRoles() {
    $.get("/Role/GetRoles", function (data, Status) {
        $("#Status").text(data.Message);
        if (data.Code == 0) {
            var roles = data.Result.roles;
            for (var i = 0 ; i < roles.length; i++) {
                var roleStr = "<div class='role_name_list' roleID=" + roles[i].roleID + ">" +
                   roles[i].roleName + "</div>";
                $("#roleTable").append(roleStr);

                var permissions = roles[i].hasPermission;
                var permissionStr = "<div>";
                for (var j = 0; j < permissions.length; j++) {
                    var content = "";
                    if (permissions[j] == 1) {
                        content = "<img src='/images/yes.png'/>";
                    } else {
                        content = "";
                    }
                    
                    permissionStr += "<div class='role_permission_list'>" +  content + "</div>";
                }
                permissionStr += "</div>";
                $("#permissionTable").append(permissionStr);
            }
        } else {

        }
    });
}



$(function(){
    $("#create").click(function () {
        var permissions = [];
        var index = 0;
       $("input:checkbox[name='permission'][checked]").each(function () {
           {
               console.log($(this).val());
               permissions[index] = ($(this).val());
               index++;
           }
       });
       console.log(JSON.stringify(permissions));
       var permissionsStr = JSON.stringify(permissions);
       var roleName = $("#input1").val();
       $.ajax({
           type: "POST",
           url: "/Role/CreateRole",
           data: {
               roleName: roleName,
               permissionIDs: permissions
           },
           traditional: true,
           dataType: "json",
           success: function (response) {
               $("#Status").text(response.Message);
               if (response.Code == "0") {
                   window.location.href = "/Role/Index_admin";
               }
           }
       });
   }); 
});

