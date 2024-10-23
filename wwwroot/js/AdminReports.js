$(document).ready(function () {
    var d = new Date();

    n = d.getMonth();
    y = d.getFullYear();
    $('#monthSelect option:eq(' + n + ')').prop('selected', true);
});
function RenewalProjectionMonth() {
    var d = new Date();

    n = d.getMonth();
    y = d.getFullYear();
    var year = y;
    var month = $('#monthSelect option:selected').val();
    $.ajax({
        type: "GET",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/GlobalMaster/RenewalProjectionMonth',
        data: { 'month': month, 'year': year },
        async: false,
        success: function (response) {
            console.log(response);
            if (response != "" || response != null) {
                var model = new Array();
                model = response.data;
                $("#activeTblBody").html('');
                var activeRows = "";
                $.each(model, function (index, item) {
                    activeRows +=
                        '<tr><td>' +
                        item.Company +
                        '</td><td>' +
                        item.RenewalDate +
                        '</td><td>' +
                        item.StartDate +
                        '</td><td>' +
                        item.Term +
                        '</td><td>' +
                        item.Status +
                        '</td><td>' +
                        item.Option +
                        '</td><td>' +
                        item.MemberShip +
                        "</td></tr>";
                });
                $("#activeTblBody").html(activeRows);
                $('#spnRev').html(response.statusMessage)
            } else {
                alert("No project. Please try again.");
            }
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}
