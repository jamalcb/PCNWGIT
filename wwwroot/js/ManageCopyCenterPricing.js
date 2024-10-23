/// <reference path="../lib/jquery/dist/jquery.js" />

// Get the current URL
var currentUrl = window.location.href;

// Remove the 'returnUrl' parameter from the URL
var updatedUrl = currentUrl.split('?')[0];

// Update the URL without the 'returnUrl' parameter
window.history.replaceState({}, document.title, updatedUrl);

function OpenForm(Ctrl, id) {
    $("#frmPrintForm").trigger('reset');
    $('#frmPrintForm').find("input[type=text], textarea").val('');
    $('#frmPrintForm').find("label#SizeName").html('');
    $("#frmPrintForm").find("span.loginError").html('');
    $('#idSze').val(id);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('label#Size').html(rows.find('td:eq(0)').text());
    $('input#MemberPrice').val(rows.find('td:eq(1)').text());
    $('input#NonMemberPrice').val(rows.find('td:eq(2)').text());
    $('input#ColorMemberPrice').val(rows.find('td:eq(3)').text());
    $('input#ColorNonMemberPrice').val(rows.find('td:eq(4)').text());
    var activeText = rows.find('td:eq(5)').text();
    if (activeText == 'Active')
        $('#IsActiveUpdate').prop('checked', true);
    else
        $('#IsActiveUpdate').prop('checked', false);
    $(".m-modal").modal('show');
};
function SavePagePrice() {
    var MemberPrice = $('#MemberPrice').val();
    $('#MemberPrice').next('span').html('');
    if (MemberPrice == undefined || MemberPrice == '' || MemberPrice == null) {
        $('#MemberPrice').next('span').html('Please enter your Size.');
        return false;
    }
    var NonMemberPrice = $('#NonMemberPrice').val();
    $('#NonMemberPrice').next('span').html('');
    if (NonMemberPrice == undefined || NonMemberPrice == '' || NonMemberPrice == null) {
        $('#NonMemberPrice').next('span').html('Please enter your Size.');
        return false;
    }
    var ColorMemberPrice = $('#ColorMemberPrice').val();
    $('#ColorMemberPrice').next('span').html('');
    if (ColorMemberPrice == undefined || ColorMemberPrice == '' || ColorMemberPrice == null) {
        $('#ColorMemberPrice').next('span').html('Please enter your Size.');
        return false;
    }
    var ColorNonMemberPrice = $('#ColorNonMemberPrice').val();
    $('#ColorNonMemberPrice').next('span').html('');
    if (ColorNonMemberPrice == undefined || ColorNonMemberPrice == '' || ColorNonMemberPrice == null) {
        $('#ColorNonMemberPrice').next('span').html('Please enter your Size.');
        return false;
    }
    var model = {};
    if ($('#IsActiveUpdate').is(":checked"))
        model.isActive = "Active";
    else
        model.isActive = "InActive";
   
    model.Id = $('#idSze').val();
    model.MemberPrice = MemberPrice;
    model.NonMemberPrice = NonMemberPrice;
    model.ColorMemberPrice = ColorMemberPrice;
    model.ColorNonMemberPrice = ColorNonMemberPrice;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/EditPagePrice/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                alert('Value updated successfully');
            }
            else {
                alert(response.statusMessage);
            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
};