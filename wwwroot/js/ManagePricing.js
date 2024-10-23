/// <reference path="../lib/jquery/dist/jquery.js" />

function ManagePricing(Ctrl, SubMemberShipPlanId) {
    var model = {}
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea").val('');
    $('#frmPrintForm2').find("label.SubPlanName").html('');
    $('span.loginError').html('');
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#idSubPlan').val(SubMemberShipPlanId);
    $('#idSubPlanName').html(rows.find('td:eq(1)').text());
    $('#YPrice').val(rows.find('td:eq(2)').text());
    $('#QPrice').val(rows.find('td:eq(3)').text());
    $('#MPrice').val(rows.find('td:eq(4)').text());
    $(".m-modal").modal('show');
}
function SavePackageData() {
    
    var YPrice = $('#YPrice').val();
    $('#YPrice').next('span').html('');
    if (YPrice == undefined || YPrice == '' || YPrice == null) {
        $('#YPrice').next('span').html('This Field can not be blank.');
        return false;
    }
    var QPrice = $('#QPrice').val();
    $('#QPrice').next('span').html('');
    if (QPrice == undefined || QPrice == '' || QPrice == null) {
        $('#QPrice').next('span').html('This Field can not be blank.');
        return false;
    }
    var MPrice = $('#MPrice').val();
    $('#MPrice').next('span').html('');
    if (MPrice == undefined || MPrice == '' || MPrice == null) {
        $('#MPrice').next('span').html('This Field can not be blank.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.SubMemberShipPlanId = $('#idSubPlan').val();
    model.YearlyPrice = $('#YPrice').val();
    model.QuarterlyPrice = $('#QPrice').val();
    model.MonthlyPrice = $('#MPrice').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SavePackageData/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                $('#message').text('Value saved successfully').fadeOut(2000);
                $(".m-modal").modal('hide');
                location.reload();
            }
            else {
                $('#message').fadeIn();
                $('#message').text(response.statusMessage).fadeOut(5000);
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

