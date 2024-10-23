/// <reference path="../lib/jquery/dist/jquery.js" />

function ManageBulletPoints(Ctrl, Id) {
    var model = {}
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea").val('');
    $('#frmPrintForm2').find("label.SubPlanName").html('');
    $('span.loginError').html('');
    var rows = $(Ctrl).parent('td').parent('tr');
    $('input#idBullet').val(Id);
    $('label#idSubPlanName').html(rows.find('td:eq(0)').text());
    $('input#RegionHead').val(rows.find('td:eq(2)').text());
    $('input#UserText').val(rows.find('td:eq(1)').text());
    $(".m-modal").modal('show');
}
function SaveBulletPoints() {

    var RegionHead = $('#RegionHead').val();
    $('#RegionHead').next('span').html('');
    if (RegionHead == undefined || RegionHead == '' || RegionHead == null) {
        $('#RegionHead').next('span').html('This Field can not be blank.');
        return false;
    }
    var UserText = $('#UserText').val();
    $('#UserText').next('span').html('');
    if (UserText == undefined || UserText == '' || UserText == null) {
        $('#UserText').next('span').html('This Field can not be blank.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.Id = $('#idBullet').val();
    model.UserText = UserText
    model.RegionHead = RegionHead;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveBulletPoints/',
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
