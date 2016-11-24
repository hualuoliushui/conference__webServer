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

$(function(){
   $("#keep").click(function(){
      $.post(" ",{
          roleName : $("#input1").val()
      },function(data,textStatus){
          var roleName = data.roleName;
      },"json"); 
   }); 
});

$(function(){
   $("#create").click(function(){
      $.post(" ",{
          createMeetingAgenda : $("#inlineCheckbox1").val(),
          viewMeetingInfo : $("#inlineCheckbox2").val(),
          uploadAttachment : $("#inlineCheckbox3").val(),
          downloadAttachment : $("#inlineCheckbox4").val(),
          editVoteResult : $("#inlineCheckbox5").val(),
          editPeopleDevice : $("#inlineCheckbox6").val(),
          viewPeopleDevice : $("#inlineCheckbox7").val()
      },function(data,textStatus){
          var createMeetingAgenda = data.createMeetingAgenda;
          var viewMeetingInfo = data.viewMeetingInfo;
          var uploadAttachment = data.uploadAttachment;
          var downloadAttachment = data.downloadAttachment;
          var editVoteResult = data.editVoteResult;
          var editPeopleDevice = data.editPeopleDevice;
          var viewPeopleDevice = data.viewPeopleDevice;
          
          var txtHtml=""
      },"json"); 
   }); 
});