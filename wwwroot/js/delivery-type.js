// Get the current URL
var currentUrl = window.location.href;

// Remove the 'returnUrl' parameter from the URL
var updatedUrl = currentUrl.split('?')[0];

// Update the URL without the 'returnUrl' parameter
window.history.replaceState({}, document.title, updatedUrl);

$(document).ready(function () {
    BindSpecialMsg(0);
});

function createDataTables() {
    setTimeout(function () {
        $('.pnTables').DataTable();
    }, 1000);
}
function reinitialLizedDataTables() {
    //$('.pnTables').dataTable().fnClearTable();
   // $('.pnTables').dataTable().fnDestroy();
}
function OpenPrintModel() {
    $("#prev-btn").click();
    $("#frmPrintForm").trigger('reset');
    $('#frmPrintForm').find("input[type=text], textarea").val('');
    $('span.loginError').html('');
    $(".m-modal").modal('show');
}
function BindSpecialMsg(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetDeliveryTypeList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.DelivName + '</td><td class="">' + (item.IsActive == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditDeliveryType(this, ' + item.DelivId + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;<span title="Delete" class="btn btn-danger icon-del" onclick="DeleteDeliveryType(' + item.DelivId + ')" ><i class="fa fa-trash " aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblDeliveryType').html(rows);
            createDataTables();
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
}
function SaveDeliveryType() {
    
    var deliveryType = $('#DeliveryType').val();
    $('#DeliveryType').next('span').html('');
    if (deliveryType == undefined || deliveryType == '' || deliveryType == null) {
        $('#DeliveryType').next('span').html('Please enter your delivery type');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.DelivId = $('#DelivId').val();
    model.DelivName = $('#DeliveryType').val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveDeliveryType/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                BindSpecialMsg(1);
                alert(response.statusMessage);
            }
            else {
                alert(response.statusMessage);
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
function EditDeliveryType(Ctrl, Id) {
    OpenPrintModel();
    $('#DelivId').val(Id);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#DeliveryType').val(rows.find('td:eq(0)').text());
    var activeText = rows.find('td:eq(1)').text();
    if (activeText == "Active")
        $('#IsActive').prop('checked', true);
    else
        $('#IsActive').prop('checked', false);
}
function DeleteDeliveryType(Id) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteDeliveryType/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                BindSpecialMsg(1);
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
