/// <reference path="../lib/jquery/dist/jquery.js" />
function ChangePublish(i) {

    jQuery.ajax({
        url: '/StaffAccount/ChangePublish/',
        data: { "Change": true, "ProjId": i },
        type: "POST",
        async: false,
        success: function (response) {
            location.reload();
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
            url: '/Report/ProjectStatsSelect/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                console.log(response);
                $('#AKCount').html(response.AKCount);
                $('td#BActiveCount').text(response.BActiveCount);
                $('td#BArchiveCount').text(response.BArchiveCount);
                $('#CACount').html(response.CACount);
                $('#IDCount').html(response.IDCount);
                $('#MTCount').html(response.MTCount);
                $('#ORCount').html(response.ORCount);
                $('td#RActiveCount').text(response.RActiveCount);
                $('td#RArchiveCount').text(response.RArchiveCount);
                $('td#UActiveCount').text(response.UActiveCount);
                $('td#UArchiveCount').text(response.UArchiveCount);
                $('td#VActiveCount').text(response.VActiveCount);
                $('td#VArchiveCount').text(response.VArchiveCount);
                $('#WACount').html(response.WACount);
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
        var value = $('input[id = RdWeek]:checked').val();
        console.log(value);
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/ProjectStatsSelect/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                console.log(response);  
                $('#AKCount').html(response.AKCount);
                $('td#BActiveCount').text(response.BActiveCount);
                $('td#BArchiveCount').text(response.BArchiveCount);
                $('#CACount').html(response.CACount);
                $('#IDCount').html(response.IDCount);
                $('#MTCount').html(response.MTCount);
                $('#ORCount').html(response.ORCount);
                $('td#RActiveCount').text(response.RActiveCount);
                $('td#RArchiveCount').text(response.RArchiveCount);
                $('td#UActiveCount').text(response.UActiveCount);
                $('td#UArchiveCount').text(response.UArchiveCount);
                $('td#VActiveCount').text(response.VActiveCount);
                $('td#VArchiveCount').text(response.VArchiveCount);
                $('#WACount').html(response.WACount);
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
            url: '/Report/ProjectStatsSelect/',
            data: { 'value': value },
            async: false,
            success: function (response) {
                console.log(response);
                $('#AKCount').html(response.AKCount);
                $('td#BActiveCount').text(response.BActiveCount);
                $('td#BArchiveCount').text(response.BArchiveCount);
                $('#CACount').html(response.CACount);
                $('#IDCount').html(response.IDCount);
                $('#MTCount').html(response.MTCount);
                $('#ORCount').html(response.ORCount);
                $('td#RActiveCount').text(response.RActiveCount);
                $('td#RArchiveCount').text(response.RArchiveCount);
                $('td#UActiveCount').text(response.UActiveCount);
                $('td#UArchiveCount').text(response.UArchiveCount);
                $('td#VActiveCount').text(response.VActiveCount);
                $('td#VArchiveCount').text(response.VArchiveCount);
                $('#WACount').html(response.WACount);
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
            url: '/Report/ProjectStatsSelect/',
            data: { 'value': value, },
            async: false,
            success: function (response) {
                $('#AKCount').html(response.AKCount);
                $('td#BActiveCount').text(response.BActiveCount);
                $('td#BArchiveCount').text(response.BArchiveCount);
                $('#CACount').html(response.CACount);
                $('#IDCount').html(response.IDCount);
                $('#MTCount').html(response.MTCount);
                $('#ORCount').html(response.ORCount);
                $('td#RActiveCount').text(response.RActiveCount);
                $('td#RArchiveCount').text(response.RArchiveCount);
                $('td#UActiveCount').text(response.UActiveCount);
                $('td#UArchiveCount').text(response.UArchiveCount);
                $('td#VActiveCount').text(response.VActiveCount);
                $('td#VArchiveCount').text(response.VArchiveCount);
                $('#WACount').html(response.WACount);
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
    else
    {
        var startDate = $('#startPicker').val();
        var endDate = $('#endPicker').val();
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Report/ProjectStatsRange/',
            data: { 'startDate': startDate, 'endDate': endDate },
            async: false,
            success: function (response) {
                $('#AKCount').html(response.AKCount);
                $('td#BActiveCount').text(response.BActiveCount);
                $('td#BArchiveCount').text(response.BArchiveCount);
                $('#CACount').html(response.CACount);
                $('#IDCount').html(response.IDCount);
                $('#MTCount').html(response.MTCount);
                $('#ORCount').html(response.ORCount);
                $('td#RActiveCount').text(response.RActiveCount);
                $('td#RArchiveCount').text(response.RArchiveCount);
                $('td#UActiveCount').text(response.UActiveCount);
                $('td#UArchiveCount').text(response.UArchiveCount);
                $('td#VActiveCount').text(response.VActiveCount);
                $('td#VArchiveCount').text(response.VArchiveCount);
                $('#WACount').html(response.WACount);
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
//window.onload = function () {
//    setTimeout(
//        function () {
//            document.getElementById('dateError').style.display = 'none';
//        }, 3000);
//}