/// <reference path="../lib/jquery/dist/jquery.js" />
function ManageStateText(Ctrl, Id) {
    var model = {}
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea").val('');
    $('#frmPrintForm2').find("label.idPackageName").html('');
    $('span.loginError').html('');
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#idPackage').val(Id);
    $('#idPackageName').html(rows.find('td:eq(0)').text());
    $('#CountyText').val(rows.find('td:eq(1)').text());
    $(".m-modal").modal('show');
};
function SaveStateText() {

    var CountyText = $('#CountyText').val();
    $('#CountyText').next('span').html('');
    if (CountyText == undefined || CountyText == '' || CountyText == null) {
        $('#YPrice').next('span').html('This Field can not be blank.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.Id = $('#idPackage').val();
    model.RegionText = $('#CountyText').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveStateText/',
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
