$(document).ready(function () {
    BindHolidaySetting(0);
    $("#BtnReset").css('display', 'none');
    $("#update").css('display', 'none');
});
$(function () {
    $(".datepicker").datepicker();
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
function BindHolidaySetting(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetHolidaySettingList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                var tempDate = new Date(item.HolidayDt);
                var formattedDate = [tempDate.getMonth() + 1, tempDate.getDate(), tempDate.getFullYear()].join('/');
                item.HolidayDt = formattedDate;
                rows += '<tr><td>' + item.Holiday + '</td><td>' + item.HolidayDt + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditHolidaySetting(this, ' + item.DiholidayId + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;<span title="Delete" class="btn btn-danger icon-del" onclick="DeleteHolidaySetting(' + item.DiholidayId + ')" ><i class="fa fa-trash " aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblHolidaySetting').html(rows);
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
function Reset() {
    document.getElementById("DiholidayId").value = "";
    document.getElementById("Holiday").value = "";
    document.getElementById("HolidayDt").value = "";
    $("#next-btn").css('display', 'block');
    $("#BtnReset").css('display', 'none');
    $("#update").css('display', 'none');
    $(".loginError").css('display', 'none');
}
function SaveHolidaySetting() {
    debugger;
    var Holiday = $('#Holiday').val();
    $('#Holiday').next('span').html('');
    if (Holiday == undefined || Holiday == '' || Holiday == null) {
        $('#Holiday').next('span').html('Please enter holiday name.');
        $(".loginError").css('display', 'block');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.DiholidayId = $('#DiholidayId').val();
    model.Holiday = $('#Holiday').val();
    model.HolidayDt = $('#HolidayDt').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveHolidaySetting/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindHolidaySetting(1);
                console.log(response.statusMessage);
                alert(response.statusMessage);
                Reset();
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
function EditHolidaySetting(Ctrl, DiholidayId) {
    $(".loginError").css('display', 'none');
    $('#DiholidayId').val(DiholidayId);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#Holiday').val(rows.find('td:eq(0)').text());
    $('#HolidayDt').val(rows.find('td:eq(1)').text());
    $("#BtnReset").css('display', 'block');
    $("#update").css('display', 'block');
    $("#next-btn").css('display', 'none');
}
function DeleteHolidaySetting(Id) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteHolidaySetting/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                console.log(response);
                BindHolidaySetting(1);
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









