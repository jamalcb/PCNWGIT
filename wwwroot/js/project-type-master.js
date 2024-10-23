$(document).ready(function () {
    BindProjectType(0);
    // Get the current URL
    var currentUrl = window.location.href;

    // Remove the 'returnUrl' parameter from the URL
    var updatedUrl = currentUrl.split('?')[0];

    // Update the URL without the 'returnUrl' parameter
    window.history.replaceState({}, document.title, updatedUrl);
});

function createDataTables()
{
    setTimeout(function () {
        $('.pTables').DataTable();
    }, 1000);
}

function reinitialLizedDataTables()
{
   // $('.pTables').DataTable().fnClearTable();
   // $('.pTables').DataTable().fnDestroy();
}

function OpenPrintModel() {
	$("#prev-btn").click();
	$("#frmPrintForm").trigger('reset');
	$('#frmPrintForm').find("input[type=text], textarea").val('');
	$('span.loginError').html('');
	$(".m-modal").modal('show');
}

function BindProjectType(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/GlobalMaster/GetProjectTypeList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.ProjType + '</td><td class="">' + (item.IsActive == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditProjectType(this, ' + item.ProjTypeId + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;</td></tr>';
            });
            $('#tblBody').html(rows);
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

function SaveProjectType() {
    // Add validation here
    var Ptype = $('#ProjType').val();
    $('#ProjType').next('span').html('');
    if (Ptype == undefined || Ptype == '' || Ptype == null) {
        $('#ProjType').next('span').html('Please enter your project type.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.ProjTypeId = $('#ProjTypeId').val();
    model.ProjType = $('#ProjType').val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveProjectType/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            debugger;
            if (response.success) {
                console.log(response);
                BindProjectType(1);
                console.log(response.statusMessage);
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

function EditProjectType(Ctrl, ProjTypeId) {
    OpenPrintModel();
    $('#ProjTypeId').val(ProjTypeId);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#ProjType').val(rows.find('td:eq(0)').text());
    var activeText = rows.find('td:eq(1)').text();
    if (activeText == "Active")
        $('#IsActive').prop('checked', true);
    else
        $('#IsActive').prop('checked', false);
}

function DeleteProjectType(ProjTypeId) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            //contentType: "application/json; charset=utf-8",
            url: '/GlobalMaster/DeleteProjectType/',
            data: { 'ProjTypeId': ProjTypeId },
            async: false,
            success: function (response) {
                console.log(response);
                BindProjectType(1);
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









