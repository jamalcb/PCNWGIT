$(document).ready(function () {
    BindFile(0);
});

function reinitialLizedDataTables() {
    $('.fTables').dataTable().fnClearTable();
    $('.fTables').dataTable().fnDestroy();
}

function BindFile(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetFileList',
        data: {},
        async: false,
        success: function (response) {
            $('#Id').val(response.data[0].Id);
            $('#File').val(response.data[0].FileStorage);
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}
function SaveFile() {
    var file = $('#File').val();
    $('#File').next('span').html('');
    if (file == undefined || file == '' || file == 0) {
        $('#File').next('span').html('Please select File storage');
        return false;
    }
    
    var model = {};
    $('#message').val('');
    model.Id = $('#Id').val();
    model.FileStorage = $('#File').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveFile/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindFile(1);
                alert(response.statusMessage);
            }
            else {
                $('#message').fadeIn();
                $('#message').text(response.statusMessage).fadeOut(5000);

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
function EditFile(Ctrl, Id) {
    OpenPrintModel();
    $('#Id').val(Id);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#File').val(rows.find('td:eq(0)').text());
}
