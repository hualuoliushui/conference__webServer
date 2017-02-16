$(function () {
    $(document).on("click", "#meetingPlace", function () {
        window.location.href = "/MeetingPlace/Index_admin";
    })

    $(document).on("click", "#device", function () {
        window.location.href = "/Device/Index_admin";
    })

    $(document).on("click", "#user", function () {
        window.location.href = "/User/Index_admin";
    })

    $(document).on("click", "#role", function () {
        window.location.href = "/Role/Index_admin";
    })
})