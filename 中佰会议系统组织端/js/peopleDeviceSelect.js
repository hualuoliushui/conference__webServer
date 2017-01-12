$(function(){
   $(":input").focus(function(){
     $(this).addClass("focus");  
   }).blur(function(){
      $(this).removeClass("focus"); 
   });
});

// $(function(){
//    $("#keep").click(function(){
//       alert("保存成功!");
//    }); 
// });

$("#timeSelect").datetimepicker();

