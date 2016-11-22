$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

$(function(){
   $("#keep").click(function(){
      alert("保存成功!");
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
    var userID =  Request["userID"];
    $.get("/User/GetUserForUpdate?userID="+userID, function (data, textStatus) {
          var userID = data.Result.userID;
          var userName = data.Result.userName;
          var userDepartment = data.Result.userDepartment;
          var userJob = data.Result.userJob; 
          var userRole = data.Result.userRole;
          var userDescription = data.Result.userDescription;
          
          $("#input1").attr("userID",userID);
          $("#input1").val(userName);
          $("#input2").val(userDepartment);
          $("#input3").val(userJob);
          $("#description").val(userDescription);
          
          
//           $.each(data.Result, function (i,item) {
//               alert(item.roleID);
//           });
          
      },"json");
});

$(function(){
   $("#keep").click(function(){
       $.post("/User/UpdateUser", {
          userID : $("#input1").attr("userID"),
          userName : $("#input1").val(),
          userDepartment : $("#input2").val(),
          userJob : $("#input3").val(),
          userDescription : $("#description").val()
       }, function (data, textStatus) {
          
          var userName = data.userName;
          var userDepartment = data.userDepartment;
          var userJob = data.userJob;
          var userDescription = data.userDescription;
      },"json"); 
   }); 
});
    