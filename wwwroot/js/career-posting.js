$(document).ready(function () {
    BindCareerPosting(0);
});

function createDataTables()
{
    setTimeout(function () {
        $('.pTables').DataTable();
    }, 1000);
}

function reinitialLizedDataTables()
{
    //$('.pTables').DataTable().fnClearTable();
    //$('.pTables').DataTable().fnDestroy();
}

function OpenPrintModel() {
	$("#prev-btn").click();
	$("#frmPrintForm").trigger('reset');
	$('#frmPrintForm').find("input[type=text], textarea").val('');
	$('span.loginError').html('');
	$(".m-modal").modal('show');
}

function BindCareerPosting(isReinitialized) {
    debugger;
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetCareerPostingList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.PositionName + '</td><td>' + item.OpaningNo + '</td><td>' + item.Experience + '</td><td>' + item.Qualification + '</td><td>' + item.ContactPerson + '</td><td>' + item.ContactNumber + '</td><td>' + item.JobDescription + '</td><td class="d-flex"><span title="Edit" class="btn btn-primary icon-edit" onclick="EditCareerPosting(this, ' + item.Id + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;<span title="Delete" class="btn btn-danger icon-del" onclick="DeleteCareerPosting(' + item.Id + ')" ><i class="fa fa-trash " aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblCareerPosting').html(rows);
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

function SaveCareerPosting() {
    var model = {};
    $('#message').val('');
    model.Id = $('#Id').val();
    model.PositionName = $('#PositionName').val();
    model.OpaningNo = $('#OpaningNo').val();
    model.Experience = $('#Experience').val();
    model.Qualification = $('#Qualification').val();
    model.ContactPerson = $('#ContactPerson').val();
    model.ContactNumber = $('#ContactNumber').val();
    model.JobDescription = $('#JobDescription').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveCareerPosting/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindCareerPosting(1);
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

function EditCareerPosting(Ctrl, Id) {
    OpenPrintModel();
    $('#Id').val(Id);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#PositionName').val(rows.find('td:eq(0)').text());
    $('#OpaningNo').val(rows.find('td:eq(1)').text());
    $('#Experience').val(rows.find('td:eq(2)').text());
    $('#Qualification').val(rows.find('td:eq(3)').text());
    $('#ContactPerson').val(rows.find('td:eq(4)').text());
    $('#ContactNumber').val(rows.find('td:eq(5)').text());
    $('#JobDescription').val(rows.find('td:eq(6)').text());
}

function DeleteCareerPosting(Id) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteCareerPosting/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                console.log(response);
                BindCareerPosting(1);
                alert(response.statusMessage);
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









