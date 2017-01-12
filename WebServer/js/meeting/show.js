$(function () {

    function getMeetingID() {
        return $("#meetingID").val();
    }

    function getAgendaID(currentObj) {
        var agendaID = currentObj.parent().siblings(".agendaID").val();
        return agendaID;
    }

    $(document).on("click", ".meeting", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Meeting/Edit_organizor?meetingID=" + meetingID;
    });

    $(document).on("click", ".agenda", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Agenda/Index_organizor?meetingID=" + meetingID;
    });

    $(document).on("click", ".document", function () {
        var agendaID = getAgendaID($(this));
        window.location.href = "/Document/Index_organizor?agendaID=" + agendaID;
    });

    $(document).on("click", ".vote", function () {
        var agendaID = getAgendaID($(this));
        window.location.href = "/Vote/Index_organizor?agendaID=" + agendaID;
    });

    $(document).on("click", ".tempDelegate", function () {
        var meetingID = getMeetingID();
        window.location.href = "/Delegate/Index_organizor?meetingID=" + meetingID;
    });

    $(document).on("click", ".returnIndex", function () {
        window.location.href = "/Account/Organizor";
    });
});