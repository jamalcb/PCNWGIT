$(document).ready(function () {
    BindPHLType(0);
});
function createDataTables() {
    setTimeout(function () {
        $('.phTables').datatables();
    }, 1000);
}
function reinitialLizedDataTables() {
    $('.phTables').dataTable().fnClearTable();
    $('.phTables').dataTable().fnDestroy();
}
function OpenPrintModel3() {
    $("#prev-btn").click();
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea").val('');
    $('span.loginError').html('');
    $(".m-modal").modal('show');
}
function BindPHLType(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetPHLTypeList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.PHLType + '</td><td class="">' + (item.IsActive == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditPHLType(this, ' + item.PHLID + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblPHLBody').html(rows);
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
function SavePHLType() {
    var phltype = $('#PHLType').val();
    $('#PHLType').next('span').html('');
    if (phltype == undefined || phltype == '' || phltype == null) {
        $('#PHLType').next('span').html('Please enter your phl type.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.PHLID = $('#PHLID').val();
    model.PHLType = $('#PHLType').val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SavePHLType/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindPHLType(1);
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
function EditPHLType(Ctrl, PHLID) {
    OpenPrintModel3();
    $('#PHLID').val(PHLID);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#PHLType').val(rows.find('td:eq(0)').text());
    var activeText = rows.find('td:eq(1)').text();
    if (activeText == "Active")
        $('#IsActive').prop('checked', true);
    else
        $('#IsActive').prop('checked', false);
}
function DeletePHLType(PHLID) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeletePHLType/',
            data: { 'PHLID': PHLID },
            async: false,
            success: function (response) {
                console.log(response);
                BindPHLType(1);
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