
$(document).ready(function () {
    BindLoginReport(0);
});
function createDataTables() {
    setTimeout(function () {
        $('.pTables').DataTable();
    }, 1000);
}
function reinitialLizedDataTables() {
    $('.pTables').dataTable().fnClearTable();
    $('.pTables').dataTable().fnDestroy();
}
function BindLoginReport(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetLoginReportList',
        data: { },
        async: false,
        success: function (response) {
            console.log(response)
            var rows = '';
            $.each(response.data, function (index, item) {
                var LoginTime = new Date(item.LoginTime);
                var formattedDate = [LoginTime.getMonth() + 1, LoginTime.getDate(), LoginTime.getFullYear()].join('/');
                item.LoginTime = formattedDate;
                if (item.LastActivity != null || item.LastActivity == "") {
                    var LastActivity = new Date(item.LastActivity);
                    var formattedDate = [LastActivity.getMonth() + 1, LastActivity.getDate(), LastActivity.getFullYear()].join('/');
                    item.LastActivity = formattedDate;
                }
                else
                {
                    item.LastActivity = "";
                }
                rows += '<tr><td>' + item.UserName + '</td><td>' + item.LoginTime + '</td><td>' + item.LastActivity + '</td><td>' + (item.IsAutoLogout == true?"Yes":"No") + '</td></tr>';
            });
            $('#tblLoginReport').html(rows);
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
