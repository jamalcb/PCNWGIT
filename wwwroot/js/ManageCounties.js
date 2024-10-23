/// <reference path="../lib/jquery/dist/jquery.js" />
$(document).ready(function () {
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetPackage/',
        async: false,
        success: function (response) {
            var model = new Array();
            model = response;
            $.each(model, function (index, item) {
                optionText = item.PackageName;
                optionValue = item.MemberType;
                $('#PackageSelect').append('<option value="' + item.MemberType + '">' + item.PackageName + '</option>');
            });
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
    var stateHtml = ''
    $('.classCounty').html('')
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetCounties/',
        async: false,
        success: function (response) {
            var model = new Array();
            model = response;
            $.each(model, function (index, item) {
                stateHtml += '<div class="col-md-2">'
                    + '<label><input type="checkbox" class="filled-in" value="' + item.CountyId+'">'
                    + '<span>' + item.County + '</span></label></div>'
            });
            $('.classCounty').html(stateHtml);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
});
function OnPackageSelect() {
    var MemberType = $('#PackageSelect').find(":selected").val();
    if (MemberType != 0) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/OnPackageSelect/',
            data: { 'MemberType': MemberType },
            async: false,
            success: function (response) {
                var model = new Array();
                model = response;
                $('input[type=checkbox]').prop('checked', false);
                $.each(model, function (index, item) {
                    $(':checkbox[value='+item+']').prop("checked", "true");
                });
               
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
};
function UpdateCounty()
{
    var obj = [];
    var MemberType = $('#PackageSelect').find(":selected").val();
    var Package = $('#PackageSelect').find(":selected").text();
    $("input:checkbox").each(function () {
        var model = {};
        model.CountyId = $(this).val();
        model.isActive = $(this).prop('checked');
        model.MemberTypeId = MemberType;
        obj.push(model);
    });
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/UpdateCounty/',
        data: { 'obj': obj, 'Package': Package },
        async: false,
        success: function (response) {
            alert(response.statusMessage);
            OnPackageSelect();

        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}

