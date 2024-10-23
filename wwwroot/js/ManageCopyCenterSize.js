/// <reference path="../lib/jquery/dist/jquery.js" />
$(document).ready(function () {
    BindCopySize();
    // Get the current URL
    var currentUrl = window.location.href;

    // Remove the 'returnUrl' parameter from the URL
    var updatedUrl = currentUrl.split('?')[0];

    // Update the URL without the 'returnUrl' parameter
    window.history.replaceState({}, document.title, updatedUrl);
});
function OpenPrintModel() {
    $("#frmPrintForm").trigger('reset');
    $('#frmPrintForm').find("input[type=text], textarea").val('');
    $('span.loginError').html('');
    $(".m-modal").modal('show');
}
function EditSize(Ctrl, id) {
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea").val('');
    $('#frmPrintForm2').find("label#SizeName").html('');
    $("#frmPrintForm2").find("span.loginError").html('');
    $('#idSze').val(id);
    var rows = $(Ctrl).parent('td').parent('tr');
    var size = rows.find('td:eq(1)').text();
    $('label#SizeName').html(rows.find('td:eq(0)').text());
    $('input#EdSize').val(rows.find('td:eq(1)').text());
    var activeText = rows.find('td:eq(2)').text();
    if (activeText == 'Active')
        $('#IsActive').prop('checked', true);
    else
        $('#IsActive').prop('checked', false);
    $(".p-modal").modal('show');
};
function EditPageSize() {
    $('#EdSize').next('span').html('');
    var Size = $('#EdSize').val();
    $('#EdSize').next('span').html('');
    if (Size == undefined || Size == '' || Size == null) {
        $('#EdSize').next('span').html('Please enter your Size.');
        return false;
    }
    var model = {};
    if ($('#IsActive').is(":checked"))
        model.isActive = "Active";
    else
        model.isActive = "InActive";
    model.Id = $('#idSze').val();
    model.Size = Size;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/EditPageSize/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                alert('Value updated successfully');
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
};
function SavePageSize() {
    var SizeName = $('#SizeName').val();
    $('#SizeName').next('span').html('');
    if (SizeName == undefined || SizeName == '' || SizeName == null) {
        $('#SizeName').next('span').html('Please enter your Size Name.');
        return false;
    }
    var Size = $('#Size').val();
    $('#Size').next('span').html('');
    if (Size == undefined || Size == '' || Size == null) {
        $('#Size').next('span').html('Please enter your Size.');
        return false;
    }
    var MemberPrice = $('#MemberPrice').val();
    $('#MemberPrice').next('span').html('');
    if (MemberPrice == undefined || MemberPrice == '' || MemberPrice == null) {
        $('#MemberPrice').next('span').html('Please enter member charge.');
        return false;
    }
    var MemberPrice = $('#NonMemberPrice').val();
    $('#NonMemberPrice').next('span').html('');
    if (NonMemberPrice == undefined || NonMemberPrice == '' || NonMemberPrice == null) {
        $('#NonMemberPrice').next('span').html('Please enter your non member charge.');
        return false;
    }
    var model = {};
    $('#message').val('');
    if ($('#IsActiveSave').is(":checked"))
        model.isActive = "Active";
    else
        model.isActive = "InActive";
    model.SizeName = $('#SizeName').val();
    model.Size = $('#Size').val();
    model.MemberPrice = $('#MemberPrice').val();
    model.NonMemberPrice = $('#NonMemberPrice').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SavePageSize/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                alert('Value added successfully');
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

function GetActiveData() {
    // Get a reference to the checkbox element by its ID
    var checkbox = document.getElementById("switch");
    // Check if the checkbox is checked
    if (checkbox.checked) {
        $.ajax({
            type: "GET",
            dataType: 'json',
            //contentType: "application/json; charset=utf-8",
            url: '/GlobalMaster/GetActiveSizeList',
            data: {},
            async: false,
            success: function (response) {
                var rows = '';
                $('#tblBody').html('');
                $.each(response.data, function (index, item) {
                    rows += '<tr><td>' + item.SizeName + '</td><td>' + item.Size + '</td><td class="">' + item.isActive + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditSize(this, ' + item.Id + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;</td></tr>';
                });
                $('#tblBody').html(rows);
                $('#lblActive').text('Get all data');
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    }
    else {
        $.ajax({
            type: "GET",
            dataType: 'json',
            //contentType: "application/json; charset=utf-8",
            url: '/GlobalMaster/GetAllCopyCenterSizeList',
            data: {},
            async: false,
            success: function (response) {
                var rows = '';
                $('#tblBody').html('');
                $.each(response.data, function (index, item) {
                    rows += '<tr><td>' + item.SizeName + '</td><td>' + item.Size + '</td><td class="">' + item.isActive + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditSize(this, ' + item.Id + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;</td></tr>';
                });
                $('#tblBody').html(rows);
                $('#lblActive').text('Get only active data');
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    }
}

function BindCopySize() {
    $.ajax({
        type: "GET",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/GlobalMaster/GetAllCopyCenterSizeList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $('#tblBody').html('');
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.SizeName + '</td><td>' + item.Size + '</td><td class="">' + item.isActive + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditSize(this, ' + item.Id + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;</td></tr>';
            });
            $('#tblBody').html(rows);
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}