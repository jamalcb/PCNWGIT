
$(document).ready(function () {
    BindInComleteMember(0);
});
function createDataTables() {
    setTimeout(function () {
        $('.mTables').DataTable();
    }, 1000);
}
function reinitialLizedDataTables() {
    $('.mTables').dataTable().fnClearTable();
    $('.mTables').dataTable().fnDestroy();
}
function BindInComleteMember(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Report/GetInCompleteMemberList',
        data: { },
        async: false,
        success: function (response) {
            console.log(response)
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.Company + '</td><td>' + item.CompanyPhone + '</td><td>' + item.FirstName + '</td><td>' + item.ContactEmail + '</td><td>' + item.ContactPhone + '</td><td>' + item.MailAddress + '</td></tr>';
            });
            $('#tblInCompleteMember').html(rows);
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
