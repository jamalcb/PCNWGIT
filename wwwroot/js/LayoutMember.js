/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/lodash.js/lodash.js" />

jQuery(document).ready(function () {
    var Email = jQuery('#hdnLogId').val();
    jQuery.ajax({
        type: 'POST',
        url: "/Home/GetUserInfo",
        data: { "Email": Email },
        success: function (data) {
            jQuery('#nameloader').hide();
            jQuery('#hdnConId').val(data.ConId);
            jQuery('#hdnId').val(data.Id);
            jQuery('#hdnName').val(data.Name);
            jQuery('#hdnEmail').val(data.Email);
            jQuery('#hdnCompany').val(data.Company);
            jQuery('#hdnPhone').val(data.hdnPhone);
            jQuery('#lblLogin').text(data.Name);
            jQuery('#hdnUid').val(data.Uid);
        }
    });
    var wrapper = document.getElementById("btnFree");
    jQuery.ajax({
        type: 'GET',
        url: "/Home/GetTabData",
        success: function (response) {
            if (!response.data.SetTab) {
                jQuery('#btnFreeTrail').css('display', 'none');
            }

        }
    });
});
function ShowProfile() {
    var id = jQuery('#hdnId').val();
    jQuery.ajax({
        type: 'POST',
        url: "/Member/MemberProfile",
        data: { "id": id },
        success: function (data) {
            jQuery('#ProfileChk').val('Y');
            location.href = "/Member/MemberProfile/" + id;
        }
    });
}

function AddDashboardProject(i, e, arg) {
    jQuery.ajax({
        url: '/Member/AddDashboard/',
        data: { "Change": i, "ProjId": e },
        type: "POST",
        async: false,
        success: function (response) {
            location.reload();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}