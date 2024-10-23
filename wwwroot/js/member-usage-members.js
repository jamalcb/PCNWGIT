
$(document).ready(function () {
    BindMemberUsage(0);
});
function createDataTables() {
    setTimeout(function () {
        $('.muTables').DataTable();
    }, 1000);
}
function reinitialLizedDataTables() {
    $('.muTables').dataTable().fnClearTable();
    $('.muTables').dataTable().fnDestroy();
}
function BindMemberUsage(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Report/GetMemberUsageList',
        data: { },
        async: false,
        success: function (response) {
            console.log(response)
            var rows = '';
            $.each(response.data, function (index, item) {
                var LoginTime = new Date(item.LoginTime);
                var formattedDate = [LoginTime.getMonth() + 1, LoginTime.getDate(), LoginTime.getFullYear()].join('/');
                item.LoginTime = formattedDate;
                rows += '<tr><td>' + item.UserName + '</td><td>' + item.LoginTime + '</td></tr>';
            });
            $('#tblMemberUsage').html(rows);
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
