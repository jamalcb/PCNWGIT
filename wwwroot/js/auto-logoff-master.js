jQuery(document).ready(function () {
    BindLogOff(0);
});

function reinitialLizedDataTables() {
    jQuery('.pnTables').dataTable().fnClearTable();
    jQuery('.pnTables').dataTable().fnDestroy();
}
function BindLogOff(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetLogOffList',
        data: {},
        async: false,
        success: function (response) {
            jQuery('#hdnId').val(response.data[0].Id);
            jQuery('#inpLogOff').val(response.data[0].LogOff); 
            jQuery('#autoLogOff').val(response.data[0].LogOff); 
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}
function SaveLogOff(Id) {
    
    var timer = jQuery('#inpLogOff').val();
    jQuery('#inpLogOff').next('span').html('');
    if (timer == undefined || timer == '' || timer == null) {
        jQuery('#inpLogOff').next('span').html('Please enter your log off time in minutes.');
        return false;
    }
    var timers = jQuery('#inpLogOff').val();
    jQuery('#inpLogOff').next('span').html('');
    if (timers > 10) {
        jQuery('#inpLogOff').next('span').html('Please enter max or equal to time in 10 minutes.');
        return false;
    }
   
    var model = {};
    jQuery('#message').val('');
    model.Id = jQuery('#hdnId').val();
    model.LogOff = jQuery('#inpLogOff').val();
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveLogOff/',
        data: { 'model': model, },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                alert(response.statusMessage);
                jQuery('#inpLogOff').val(response.data);
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

