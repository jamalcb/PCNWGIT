$(document).ready(function () {
    BindEntityType(0);
});
function createDataTables() {
    setTimeout(function () {
        $('.eTables').datatables();
    }, 1000);
}
function reinitialLizedDataTables() {
    $('.eTables').dataTable().fnClearTable();
    $('.eTables').dataTable().fnDestroy();
}
function OpenPrintModel2() {
    $("#prev-btn").click();
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea").val('');
    $('span.loginError').html('');
    $(".m-modal").modal('show');
}
function BindEntityType(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetEntityTypeList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.EntityType + '</td><td class="">' + (item.IsActive == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditEntityType(this, ' + item.EntityID + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblEntityBody').html(rows);
            createDataTables();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}
function SaveEntityType() {
    var Etype = $('#EntityType').val();
    $('#EntityType').next('span').html('');
    if (Etype == undefined || Etype == '' || Etype == null) {
        $('#EntityType').next('span').html('Please enter your entity type.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.EntityID = $('#EntityID').val();
    model.EntityType = $('#EntityType').val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveEntityType/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindEntityType(1);
                alert(response.statusMessage);
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
function EditEntityType(Ctrl, EntityID) {
    OpenPrintModel2();
    $('#EntityID').val(EntityID);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#EntityType').val(rows.find('td:eq(0)').text());
    var activeText = rows.find('td:eq(1)').text();
    if (activeText == "Active")
        $('#IsActive').prop('checked', true);
    else
        $('#IsActive').prop('checked', false);
}
function DeleteEntityType(EntityID) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteEntityType/',
            data: { 'EntityID': EntityID },
            async: false,
            success: function (response) {
                console.log(response);
                BindEntityType(1);
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
    return false;
}