// Get the current URL
var currentUrl = window.location.href;

// Remove the 'returnUrl' parameter from the URL
var updatedUrl = currentUrl.split('?')[0];

// Update the URL without the 'returnUrl' parameter
window.history.replaceState({}, document.title, updatedUrl);

$(document).ready(function () {
    BindDeliverySubType(0);
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
    GetDeliveryTypeList();
    $(".m-modal").modal('show');
}
function BindDeliverySubType(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetDeliverySubTypeList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response, function (index, item) {
                rows += '<tr><td>' + item.DelivName + '</td><td>' + item.DelivOptName + '</td><td class="">' + (item.IsActive == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditDeliverySubType(this, ' + item.DelivOptId + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;<span title="Delete" class="btn btn-danger icon-del" onclick="DeleteDeliverySubType(' + item.DelivOptId + ')" ><i class="fa fa-trash " aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblDeliverySubType').html(rows);
            createDataTables();
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
}
function GetDeliveryTypeList() {
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetDeliveryTypeList',
        data: {},
        async: false,
        success: function (response) {
            var datalist = response.data;
            var selectDropdown = $('#DeliveryType'); // Select the dropdown by its ID

            selectDropdown.empty(); // Clear existing options

            selectDropdown.append('<option value="0">-- Select Delivery Type --</option>');

            for (var i = 0; i < datalist.length; i++) {
                selectDropdown.append('<option value="' + datalist[i].DelivId + '">' + datalist[i].DelivName + '</option>');
            }
        },
        error: function (response) {
            // Handle errors if needed
        }
    });
}
function SaveDeliverySubType() {
    
    var deliveryType = $('#DeliveryType').val();
    $('#DeliveryType').next('span').html('');
    if (deliveryType == undefined || deliveryType == '' || deliveryType == null || deliveryType == 0) {
        $('#DeliveryType').next('span').html('Please select delivery type');
        return false;
    }
    var deliverySubType = $('#DeliverySubType').val();
    $('#DeliverySubType').next('span').html('');
    if (deliverySubType == undefined || deliverySubType == '' || deliverySubType == null ) {
        $('#DeliverySubType').next('span').html('Please enter delivery subtype');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.DelivOptId = $('#DelivOptId').val();
    model.DelivId = $('#DeliveryType').val();
    model.DelivOptName = $('#DeliverySubType').val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveDeliverySubType/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                BindDeliverySubType(1);
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

function EditDeliverySubType(Ctrl, Id) {
    OpenPrintModel();
    $('#DeliveryHiddenType').val('Local');
    $('#DelivOptId').val(Id);
    var rows = $(Ctrl).parent('td').parent('tr');
    var selectedDeliveryType = rows.find('td:eq(0)').text();
    $('#DeliveryHiddenType').val(selectedDeliveryType);
    $('#DeliverySubType').val(rows.find('td:eq(1)').text());
    var activeText = rows.find('td:eq(2)').text();
    if (activeText == "Active") {
        $('#IsActive').prop('checked', true);
    } else {
        $('#IsActive').prop('checked', false);
    }

    // Now select the appropriate option in the DeliveryType dropdown
    $('#DeliveryType option').each(function () {
        if ($(this).text() === selectedDeliveryType) {
            $(this).prop('selected', true);
        }
    });
}

function DeleteDeliverySubType(Id) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteDeliverySubType/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                BindDeliverySubType(1);
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
