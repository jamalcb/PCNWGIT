$(document).ready(function () {
    BindProjectSubType(0);
});
function createDataTables() {
    setTimeout(function () {
        $('.pTables').datatables();
    }, 1000);
}
function reinitialLizedDataTables() {
    $('.pTables').dataTable().fnClearTable();
    $('.pTables').dataTable().fnDestroy();
}
function OpenPrintModel1() {
    $("#prev-btn").click();
    $("#frmPrintForm1").trigger('reset');
    $('#frmPrintForm1').find("input[type=text], textarea").val('');
    $('span.loginError').html('');
    $(".m-modal").modal('show');
}
function BindProjectSubType(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetProjectSubTypeList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.ProjSubType + '</td><td class="">' + item.ProjTypeID + '</td><td class="">' + (item.IsActive == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditProjectSubType(this, ' + item.ProjSubTypeID + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblSubBody').html(rows);
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
function SaveProjectSubType() {
    var Ptype = $('#ProjSubType').val();
    $('#ProjSubType').next('span').html('');
    if (Ptype == undefined || Ptype == '' || Ptype == null) {
        $('#ProjSubType').next('span').html('Please enter your project sub type.');
        return false;
    }
    var Sorder = $('#ProjTypeID').val();
    $('#ProjTypeID').next('span').html('');
    if (Sorder == undefined || Sorder == '' || Sorder == null) {
        $('#ProjTypeID').next('span').html('Please enter your project type id.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.ProjSubTypeID = $('#ProjSubTypeID').val();
    model.ProjSubType = $('#ProjSubType').val();
    model.ProjTypeID = $('#ProjTypeID').val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveProjectSubType/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindProjectSubType(1);
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
function EditProjectSubType(Ctrl, ProjSubTypeID) {
    OpenPrintModel1();
    $('#ProjSubTypeID').val(ProjSubTypeID);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#ProjSubType').val(rows.find('td:eq(0)').text());
    $('#ProjTypeID').val(rows.find('td:eq(1)').text());
    var activeText = rows.find('td:eq(2)').text();
    if (activeText == "Active")
        $('#IsActive').prop('checked', true);
    else
        $('#IsActive').prop('checked', false);
}
function DeleteProjectSubType(ProjSubTypeID) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteProjectSubType/',
            data: { 'ProjSubTypeID': ProjSubTypeID },
            async: false,
            success: function (response) {
                console.log(response);
                BindProjectSubType(1);
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