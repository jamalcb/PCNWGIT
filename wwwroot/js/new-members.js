
$(document).ready(function () {
    BindNewMember(0);
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
function BindNewMember(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    var value = 'Daily';
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Report/NewMemberDaily',
        data: { 'value': value, },
        async: false,
        success: function (response) {
            console.log(response)
            var rows = '';
            $.each(response.data, function (index, item) {
                var InsertDate = new Date(item.InsertDate);
                var formattedDate = [InsertDate.getMonth() + 1, InsertDate.getDate(), InsertDate.getFullYear()].join('/');
                item.InsertDate = formattedDate;
                rows += '<tr><td>' + item.Company + '</td><td>' + item.MailCity + '</td><td>' + item.MailState + '</td><td>' + item.Discipline + '</td><td>' + item.InsertDate + '</td></tr>';
            });
            $('#tblNewMember').html(rows);
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
$('input[name = RdRange]').change(function () {
    if ($('input[id = RdDaily]').is(':checked')) {
        var value = $('input[id = RdDaily]:checked').val();
        console.log(value);
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/NewMemberDaily/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                console.log(response)
                var rows = '';
                $('#tblNewMember').html('');
                $.each(response.data, function (index, item) {
                    var InsertDate = new Date(item.InsertDate);
                    var formattedDate = [InsertDate.getMonth() + 1, InsertDate.getDate(), InsertDate.getFullYear()].join('/');
                    item.InsertDate = formattedDate;
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.MailCity + '</td><td>' + item.MailState + '</td><td>' + item.Discipline + '</td><td>' + item.InsertDate + '</td></tr>';
                });
                $('#tblNewMember').html(rows);
                createDataTables();
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
    else if ($('input[id = RdWeek]').is(':checked')) {
        debugger;
        var value = $('input[id = RdWeek]:checked').val();
        console.log(value);
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/NewMemberList/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                console.log(response)
                var rows = '';
                $('#tblNewMember').html('');
                $.each(response.data, function (index, item) {
                    var InsertDate = new Date(item.InsertDate);
                    var formattedDate = [InsertDate.getMonth() + 1, InsertDate.getDate(), InsertDate.getFullYear()].join('/');
                    item.InsertDate = formattedDate;
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.MailCity + '</td><td>' + item.MailState + '</td><td>' + item.Discipline + '</td><td>' + item.InsertDate + '</td></tr>';
                });
                $('#tblNewMember').html(rows);
                createDataTables();
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
    else if ($('input[id = RdMonth]').is(':checked')) {
        var value = $('input[id = RdMonth]:checked').val();
        console.log(value);
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/NewMemberList/',
            data: { 'value': value },
            async: false,
            success: function (response) {
                var rows = '';
                $('#tblNewMember').html('');
                $.each(response.data, function (index, item) {
                    var InsertDate = new Date(item.InsertDate);
                    var formattedDate = [InsertDate.getMonth() + 1, InsertDate.getDate(), InsertDate.getFullYear()].join('/');
                    item.InsertDate = formattedDate;
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.MailCity + '</td><td>' + item.MailState + '</td><td>' + item.Discipline + '</td><td>' + item.InsertDate + '</td></tr>';
                });
                $('#tblNewMember').html(rows);
                createDataTables();
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
    else if ($('input[id = RdYear]').is(':checked')) {
        var value = $('input[id = RdYear]:checked').val();
        console.log(value);
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/NewMemberList/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                var rows = '';
                $('#tblNewMember').html('');
                $.each(response.data, function (index, item) {
                    var InsertDate = new Date(item.InsertDate);
                    var formattedDate = [InsertDate.getMonth() + 1, InsertDate.getDate(), InsertDate.getFullYear()].join('/');
                    item.InsertDate = formattedDate;
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.MailCity + '</td><td>' + item.MailState + '</td><td>' + item.Discipline + '</td><td>' + item.InsertDate + '</td></tr>';
                });
                $('#tblNewMember').html(rows);
                createDataTables();
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
    

});
function checkDate() {
    var startDate = new Date($('#startPicker').val());
    var endDate = new Date($('#endPicker').val());

    if (startDate > endDate) {
        $('#dateError').html('Start Date should be less than end date').css('color', 'red');
        $('#dateError').delay(2000).fadeOut();
        return false;// Do something
    }
    else {
        var startDate = $('#startPicker').val();
        var endDate = $('#endPicker').val();
        //$('input[name="RdRange"]').checked = false;
        $('input[name = RdRange]').attr('checked', false);
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/NewMemberRange/',
            data: { 'startDate': startDate, 'endDate': endDate },
            async: false,
            success: function (response) {
                var rows = '';
                $.each(response.data, function (index, item) {
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.MailCity + '</td><td>' + item.MailState + '</td><td>' + item.Discipline + '</td><td>' + item.InsertDate + '</td></tr>';
                });
                $('#tblNewMember').html(rows);
                createDataTables();
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
}