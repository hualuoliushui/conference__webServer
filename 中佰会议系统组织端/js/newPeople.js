$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

//$(function(){
//   $("#keep").click(function(){
//      alert("保存成功!");
//   }); 
//});

$(function () {
var result = [
        {
            "roleID": 1,
            "roleName": "Admin"
},
        {
            "roleID": 2,
            "roleName": "Organizor"
},
        {
            "roleID": 3,
            "roleName": "Member"
}
];
$.ajax({
    url: "../addPeople.json",
    type: "GET",
    async: true,
    dataType: "json",
    // contentType:"application/json",
    data: result,
    success: function succFunction(tt) {   
                $("#select").empty();
                var str="";
                $.each(result, function () {
                    str=str+'<option value="'+this.roleName+'">'+this.roleName+'</option>';
                });
                $("#select").append(str);
    }, //成功执行方法
    error: function errFunction() {
                alert("error");
    }  //错误执行方法   
     
    });
});


$(function(){
   $("#keep").click(function(){
       
    //    alert($("#select").val());
    //    return false;
      $.ajax({
          url:"",
          type:"POST",
          async:true,
          dataType:"json",
          data:{
              Code:0,
              Message:$("#select").val()
          },
//          beforeSend:function(XMLHttpRequest){
//              console.log(this);
//              $("#select").val("正在获取数据……");
//          },
          success:function(data){
              
              
            
          },
//          complete:function(XMLHttpRequest,textStatus){
//              if(textStatus=="timeout"){
//                  var xmlhttp=window.XMLHttpRequest?new window.XMLHttpRequest():new ActiveXObject("Microsoft.XMLHTTP");
//                  xmlhttp.abort();
//                  $("#mainContentRight").html("网络超时！");
//                  alert("请刷新页面！");
//              }
//          },
          error:function(XMLHttpRequest,textStatus){
              console.log(XMLHttpRequest);
              console.log(textStatus);
              $("#mainContentRight").html("服务器错误！");
          }
      });
   }); 
});



$(function(){
   $("#keep").click(function(){
      $.ajax({
          url:"",
          type:"POST",
//          timeout:5000,
          async:true,
          dataType:"json",
          contentType:"application/json",
          data:{
                "userName": "小A",
                "userDepartment": "经理",
                "userJob": "华工",
                "userDescription": "一个大老板",
                "roleID": 2
               },
//          beforeSend:function(XMLHttpRequest){
//              console.log(this);
//              $("#input1").val("正在获取数据……");
//              $("#input2").val("正在获取数据……");
//              $("#input3").val("正在获取数据……");
//              $("#description").val("正在获取数据……");
//          },
          success:function(data){
              console.log(data);
              $("#input1").attr("userID",data.Result.userID);
              $("#input1").val("userName");
              $("#input2").val("userDepartment");
              $("#input3").val("userJob");
              $("#description").val("userDescription");
          },
//          complete:function(XMLHttpRequest,textStatus){
//              if(textStatus=="timeout"){
//                  var xmlhttp=window.XMLHttpRequest?new window.XMLHttpRequest():new ActiveXObject("Microsoft.XMLHTTP");
//                  xmlhttp.abort();
//                  $("#mainContentRight").html("网络超时！");
//                  alert("请刷新页面！");
//              }
//          },
          error:function(XMLHttpRequest,textStatus){
              console.log(XMLHttpRequest);
              console.log(textStatus);
              $("#mainContentRight").html("服务器错误！");
          }
      });
   }); 
});
//$(function () {
//    $("#btnGet").click(function () {
//        $.ajax({
//            url: "",
//            type: "Post",
//            contentType: "application/json",
//            dataType: "json",
//            success: function (data) {
//                var ddl = $("#ddlDatas");
//
//                //删除节点
//                RemoveOption();
//
//                //方法1：添加默认节点 
//                ddl.append("<option value='-1'>--请选择--</option>");
//
//                //方法2：添加默认节点
//                //ddl[0].options.add(new Option("--请选择--", "-1"));
//
//                //转成Json对象
//                var result = eval(data);
//
//                //循环遍历 下拉框绑定
//                $(result).each(function (key) {
//                    //第一种方法
//                    var opt = $("<option></option>").text(result[key].ProName).val(result[key].ProID);
//                    ddl.append(opt);
//
//                    //第二种方法
//                    // var proid = result[key].ProID;
//                    // var proname = result[key].ProName;
//                    //调用自定义方法
//                    //AppendOption(proid, proname);
//                });
//
//                //第三种方法
//                //$.each(result, function (key, value) {
//                //alert("dd");
//                //var op = new Option(value.ProName, value.ProID);
//                // ddl[0].options.add(op);
//                // });
//            },
//            error: function (data) {
//                alert("Error");
//            }
//        });
//    });
//
//
//});

//function RemoveOption() {
//    $("#select option").remove();
//}
//
//function AppendOption(value, text) {
//    $("#select").append("<option value='" + value + "'>" + text + "</option>");
//}


//$(function(){
//   $("#keep").click(function(){
//      $.post("",{
//          userID : $("#input1").attr("userID"),
//          userName : $("#input1").val(),
//          userDepartment : $("#input2").val(),
//          userJob : $("#input3").val(),
//          userDescription : $("#description").val()
//      },function(data,textStatus){
//           
//         
//      },"json"); 
//   }); 
//});