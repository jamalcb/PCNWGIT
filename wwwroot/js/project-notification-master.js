
$(document).ready(function () {
    BindProjNotification(0);
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
function BindProjNotification(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetProjNotificationList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.Email + '</td><td><span title="Delete" class="btn btn-danger icon-del" onclick="DeleteProjNotification(' + item.Id + ')" ><i class="fa fa-trash" aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblNotification').html(rows);
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
function SaveProjNotification() {
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
        url: '/GlobalMaster/SaveProjNotification/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindProjectSubType(1);
                alert(response.statusMessage);
            }
            else {
                //$('#message').fadeIn();
                //$('#message').text(response.statusMessage).fadeOut(5000);
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
function DeleteProjNotification(Id) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteProjNotification/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                console.log(response);
                BindProjNotification(1);
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