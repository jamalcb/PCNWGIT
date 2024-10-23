
$(document).ready(function () {
    BindDailySubscriptions(0);
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
function BindDailySubscriptions(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Report/GetDailySubscriptionsList',
        data: { },
        async: false,
        success: function (response) {
            console.log(response)
            var rows = '';
            $.each(response.data, function (index, item) {
                var UnSubscribeDate = new Date(item.UnSubscribeDate);
                var formattedDate = [UnSubscribeDate.getMonth() + 1, UnSubscribeDate.getDate(), UnSubscribeDate.getFullYear()].join('/');
                item.UnSubscribeDate = formattedDate;
                rows += '<tr><td>' + item.UserEmail + '</td><td>' + item.UnSubscribeDate + '</td></tr>';
            });
            $('#tblDailySubscriptions').html(rows);
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
