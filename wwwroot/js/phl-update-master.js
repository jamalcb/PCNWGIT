
$(document).ready(function () {
    BindPHLUpdate(0);
});
function createDataTables() {
    setTimeout(function () {
        $('.pnTables').DataTable();
    }, 1000);
}
function reinitialLizedDataTables() {
    $('.pnTables').dataTable().fnClearTable();
    $('.pnTables').dataTable().fnDestroy();
}
function BindPHLUpdate(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetPHLUpdateList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.Email + '</td><td><span title="Delete" class="btn btn-danger  icon-del" onclick="DeletePHLUpdate(' + item.Id + ')" ><i class="fa fa-trash" aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblPHLUpdate').html(rows);
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
function SavePHLUpdate() {
    var email = $('#Email').val();
    $('#Email').next('span').html('');
    if (email == undefined || email == '' || email == null) {
        $('#Email').next('span').html('Please enter your email.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.Id = $('#Id').val();
    model.Email = $('#Email').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SavePHLUpdate/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindPHLUpdate(1);
                alert(response.statusMessage);
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
function DeletePHLUpdate(Id) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeletePHLUpdate/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                console.log(response);
                BindPHLUpdate(1);
                alert(response.statusMessage);
                alert(response.statusMessage);
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