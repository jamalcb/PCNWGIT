
$(document).ready(function () {
    BindCommunication(0);
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
function BindCommunication(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    var value = 'All';
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Report/AllCommunication',
        data: { 'value': value, },
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.Company + '</td><td>' + item.Email + '</td></tr>';
            });
            $('#tblCommunication').html(rows);
            createDataTables();
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
}
$('input[name = RdRange]').change(function () {
    if ($('input[id = All]').is(':checked')) {
        var value = $('input[id = All]:checked').val();
       
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/AllCommunication/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                
                var rows = '';
                $.each(response.data, function (index, item) {
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.Email + '</td></tr>';
                });
                $('#tblCommunication').html(rows);
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
    else if ($('input[id = OR]').is(':checked')) {
        debugger;
        var value = $('input[id = OR]:checked').val();
        
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/CommunicationList/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                
                var rows = '';
                $.each(response.data, function (index, item) {
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.Email + '</td></tr>';
                });
                $('#tblCommunication').html(rows);
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
    else if ($('input[id = WA]').is(':checked')) {
        var value = $('input[id = WA]:checked').val();
        
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/CommunicationList/',
            data: { 'value': value },
            async: false,
            success: function (response) {
                var rows = '';
                $.each(response.data, function (index, item) {
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.Email + '</td></tr>';
                });
                $('#tblCommunication').html(rows);
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
    else if ($('input[id = Expiring]').is(':checked')) {
        var value = $('input[id = Expiring]:checked').val();
        
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/ExpiringCommunicationList/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                var rows = '';
                $.each(response.data, function (index, item) {
                    rows += '<tr><td>' + item.Company + '</td><td>' + item.Email + '</td></tr>';
                });
                $('#tblCommunication').html(rows);
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

$(document).ready(function () {
    $('#exampless').DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copyHtml5',
            'excelHtml5',
            'csvHtml5',
            'pdfHtml5'
        ]
    });
});
