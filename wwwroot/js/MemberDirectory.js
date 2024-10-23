/// <reference path="../lib/jquery/dist/jquery.js" />
var jq = $.noConflict();
jQuery(window).on("load", function () {
    var loader = jQuery("#Membloader");
    loader.hide();
});
jQuery(document).ready(function () {
    setTimeout(() => {
        jQuery('#table-id').wrap('<div class="heit-test"></div>');
    }, 2000)
});

function loadDataTables() {
    setTimeout(function () {
        jq('.pTables').DataTable({
            "pageLength": 100
        });
        //jq('#table-id_length').attr("style", "display:none;");
    }, 1000);
}

jq(document).ready(function () {
    loadDataTables();
});
function ShowCard(Id) {
    jq("#frmPrintForm").trigger('reset');
    jq('#frmPrintForm').find("input[type=text], textarea").val('');
    jq('#frmPrintForm').find("label").html('');
    jq("#frmPrintForm").find("span#message").html('');
    jq.ajax({
        type: "POST",
        dataType: 'json',
        url: '/Member/ShowCard/',
        data: { 'Id': Id },
        async: false,
        success: function (response) {
            if (response.success) {
                jq('#lblCompany').html(response.data.Company);
                jq('#lblMail').html(response.data.ConsMailAddress);
                jq('#lblBill').html(response.data.ConsBillAddress);
                jq('#lblDBA').html(response.data.Dba);
                jq('#lblPC').html(response.data.ContactName);
                jq('#lblEmail').html(response.data.Email);
                jq('#lblPhone').html(response.data.CompanyPhone);
                jq('#lblPB').html(response.data.Div);
                jq('#lblSP').html(response.data.Discipline);
                jq('#lblLic').html(response.data.License);
                console.log(response.data);
            }
            else {
                jq('#message').fadeIn();
                jq('#message').text(response.statusMessage).fadeOut(5000);
            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
    jq('.li-modal').modal('show');
}
function initializeMap(address) {
    // Open a new window with the map URL based on the provided address
    var mapUrl = 'https://maps.google.com/maps?q=' + encodeURIComponent(address);
    window.open(mapUrl, '_blank');
}