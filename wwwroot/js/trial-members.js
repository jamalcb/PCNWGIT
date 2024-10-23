
$(document).ready(function () {
    BindTrialMember(0);
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
function BindTrialMember(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Report/GetTrialMembersList',
        data: { },
        async: false,
        success: function (response) {
            console.log(response)
            var rows = '';
            $.each(response.data, function (index, item) {
                var RenewalDate = new Date(item.RenewalDate);
                var formattedDate = [RenewalDate.getMonth() + 1, RenewalDate.getDate(), RenewalDate.getFullYear()].join('/');
                item.RenewalDate = formattedDate;
                var Discipline = new Date(item.Discipline);
                var formattedDate = [Discipline.getMonth() + 1, Discipline.getDate(), Discipline.getFullYear()].join('/');
                item.Discipline = formattedDate;
                rows += '<tr><td>' + item.Company + '</td><td>' + item.MailCity + '</td><td>' + item.MailState + '</td><td>' + item.RenewalDate + '</td><td>' + item.Discipline + '</td></tr>';
            });
            $('#tblTrialMembers').html(rows);
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
