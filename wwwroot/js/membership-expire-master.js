
$(document).ready(function () {
    BindMembershipExpire(0);
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
function BindMembershipExpire(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetMembershipExpireList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.Email + '</td><td>' + item.Timer + '</td><td><span title="Delete" class="btn btn-danger icon-del" onclick="DeleteMembershipExpire(' + item.Id + ')" ><i class="fa fa-trash " aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblMembershipExpire').html(rows);
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
function SaveMembershipExpire() {
    var email = $('#Email').val();
    $('#Email').next('span').html('');
    if (email == undefined || email == '' || email == null) {
        $('#Email').next('span').html('Please enter your email.');
        return false;
    }
    var timer = $('#Timer').val();
    $('#Timer').next('span').html('');
    if (timer == undefined || timer == '' || timer == null) {
        $('#Timer').next('span').html('Please enter your membership expire days.');
        return false;
    }
    var timers = $('#Timer').val();
    $('#Timer').next('span').html('');
    if (timers > 365) {
        $('#Timer').next('span').html('Please enter max or equal to 365 days.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.Id = $('#Id').val();
    model.Email = $('#Email').val();
    model.Timer = $('#Timer').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveMembershipExpire/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindMembershipExpire(1);
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
function DeleteMembershipExpire(Id) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteMembershipExpire/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                console.log(response);
                BindMembershipExpire(1);
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