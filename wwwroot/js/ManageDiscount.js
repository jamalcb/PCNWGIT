/// <reference path="../lib/jquery/dist/jquery.js" />

// Get the current URL
var currentUrl = window.location.href;

// Remove the 'returnUrl' parameter from the URL
var updatedUrl = currentUrl.split('?')[0];

// Update the URL without the 'returnUrl' parameter
window.history.replaceState({}, document.title, updatedUrl);

$(function () {
    $(".datepicker").datepicker();
});
function OpenPrintModel() {
    $("#frmPrintForm").trigger('reset');
    $('#frmPrintForm').find("input[type=text], textarea").val('');
    $("#frmPrintForm").find("span.loginError").html('');
    $(".m-modal").modal('show');
}
function EditDiscount(Ctrl, id) {
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea").val('');
    $('#frmPrintForm2').find("label#SizeName").html('');
    $("#frmPrintForm2").find("span.loginError").html('');
    $('#idSze').val(id);
    var rows = $(Ctrl).parent('td').parent('tr');
    var stDate = rows.find('td:eq(1)').text();
    var etDate = rows.find('td:eq(2)').text();
    $('input#UStartDate').val(stDate);
    $('input#UEndDate').val(etDate);
    $('input#UDiscountRate').val(rows.find('td:eq(0)').text());
    $('textarea#UDescription').val(rows.find('td:eq(3)').text());
    var activeText = rows.find('td:eq(4)').text();
    if (activeText == 'Active')
        $('#IsActiveUpdate').prop('checked', true);
    else
        $('#IsActiveUpdate').prop('checked', false);
    $(".p-modal").modal('show');
};
function UpdateDiscount() {
    var DiscountId = $('#idSze').val();
    var DiscountRate = $('#UDiscountRate').val();
    $('#UDiscountRate').next('span').html('');
    if (DiscountRate == undefined || DiscountRate == '' || DiscountRate == null) {
        $('#UDiscountRate').next('span').html('Please enter discount rate.');
        return false;
    } 
    if ($.isNumeric(DiscountRate) == false) {
        $('#UDiscountRate').next('span').html('Please enter a positive integer value.');
        return false;
    }
    var StartDate = $('#UStartDate').val();
    $('#UStartDate').next('span').html('');
    if (StartDate == undefined || StartDate == '' || StartDate == null || StartDate == 0) {
        $('#UStartDate').next('span').html('Please select start date.');
        return false;
    }
    var EndDate = $('#UEndDate').val();
    $('#UEndDate').next('span').html('');
    if (EndDate == undefined || EndDate == '' || EndDate == null || StartDate == 0) {
        $('#UEndDate').next('span').html('Please enter end date.');
        return false;
    }
    var stVal = new Date($('#UStartDate').val());
    var etVal = new Date($('#UEndDate').val());
    if (stVal > etVal) {
        $('#UEndDate').next('span').html('Start Date should be less than end date.');
        $('#UEndDate').next('span').delay(2000).fadeOut();
        return false;// Do something
    }
    var Description = $('#UDescription').val();
    var model = {};
    $('#message').val('');
    if ($('#IsActiveUpdate').is(":checked"))
        model.isActive = "Active";
    else
        model.isActive = "InActive";
    model.DiscountId = DiscountId;
    model.DiscountRate = DiscountRate;
    model.StartDate = StartDate;
    model.EndDate = EndDate;
    model.Description = Description;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/UpdateDiscount/',
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
function SaveDiscount() {
    var DiscountRate = $('#DiscountRate').val();
    $('#DiscountRate').next('span').html('');
    if (DiscountRate == undefined || DiscountRate == '' || DiscountRate == null) {
        $('#DiscountRate').next('span').html('Please enter discount rate.');
        return false;
    }
    if ($.isNumeric(DiscountRate) == false || DiscountRate <= 0) {
        $('#DiscountRate').next('span').html('Please enter a positive integer value.');
        return false;
    }
    var StartDate = $('#StartDate').val();
    $('#StartDate').next('span').html('');
    if (StartDate == undefined || StartDate == '' || StartDate == null || StartDate==0) {
        $('#Size').next('span').html('Please select start date.');
        return false;
    }
    var EndDate = $('#EndDate').val();
    $('#EndDate').next('span').html('');
    if (EndDate == undefined || EndDate == '' || EndDate == null || StartDate == 0) {
        $('#EndDate').next('span').html('Please enter end date.');
        return false;
    }
    var stVal = new Date($('#StartDate').val());
    var etVal = new Date($('#EndDate').val());
    if (stVal > etVal) {
        $('#EndDate').next('span').html('Start Date should be less than end date.');
        $('#EndDate').next('span').delay(2000).fadeOut();
        return false;// Do something
    }
    var Description = $('#Description').val();
    var model = {};
    $('#message').val('');
    if ($('#IsActiveSave').is(":checked"))
        model.isActive = "Active";
    else
        model.isActive = "InActive";
    model.DiscountRate = DiscountRate;
    model.StartDate = StartDate;
    model.EndDate = EndDate;
    model.Description = Description;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveDiscount/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                alert('Value added successfully');
                location.reload();
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
}