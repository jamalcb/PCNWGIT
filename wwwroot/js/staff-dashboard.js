var searchText = "";
var ActivesearchText = "";
var PastSearchText = "";
jQuery(document).ready(function () {
    TempDataValue = jQuery('#tmpHiddenMsg').val();
    var ActiveTab = jQuery('#hdnTab').val();
    var searchIText = jQuery('#hdnSearchText').val();
    var ActivesearchIText = jQuery('#hdnActSearchText').val();
    var PastsearchIText = jQuery('#hdnPastSearchText').val();
    if (ActiveTab != '') {
        if (ActiveTab == 'about') {
            $('#about').attr('checked', true);
            $('#home').attr('checked', false);
            $('#contact').attr('checked', false);
        }
        else if (ActiveTab == 'home') {
            $('#home').attr('checked', true);
            $('#about').attr('checked', false);
            $('#contact').attr('checked', false);
        }
        else {
            $('#contact').attr('checked', true);
            $('#about').attr('checked', false);
            $('#home').attr('checked', false);
        }
    }
    if (searchIText != '') {
        searchText = searchIText;
    }
    if (ActivesearchIText != '') {
        ActivesearchText = ActivesearchIText;
    }
    if (PastsearchIText != '') {
        PastSearchText = PastsearchIText;
    }

    if (TempDataValue != '') {
        alert(TempDataValue)
    }

    jQuery('#hdnTab').val(ActiveTab);
    jQuery('#hdnSearchText').val('');
    jQuery('#hdnActSearchText').val('');
    jQuery('#hdnPastSearchText').val('');
});
jQuery(document).ready(function () {
    setTimeout(() => {
        jQuery('#table-id-active').wrap('<div class="heit-test"></div>');
        jQuery('#table-id-pending').wrap('<div class="heit-test"></div>');
        jQuery('#table-id-past').wrap('<div class="heit-test"></div>');
    }, 2000)
});

function ActiveTextValue() {
    var chkPrevTab = $('#hdnTab').val();
    if (chkPrevTab == "" || chkPrevTab == 'home') {
        var pandingSearch = $('#table-id-pending_filter').find('input').val();
        $('#table-id-active_filter').find('input').val(pandingSearch);
        sTable.search(pandingSearch).draw();
    }
    if (chkPrevTab == 'contact') {
        var pastSearch = $('#table-id-past_filter').find('input').val();
        $('#table-id-active_filter').find('input').val(pastSearch);
        sTable.search(pastSearch).draw();
    }
    $('#hdnTab').val('about');
}
function PandingTextValue() {
    var chkPrevTab = $('#hdnTab').val();
    if (chkPrevTab == 'about') {
        var activeSearch = $('#table-id-active_filter').find('input').val();
        $('#table-id-pending_filter').find('input').val(activeSearch);
        pTable.search(activeSearch).draw();
    }
    if (chkPrevTab == 'contact') {
        var pastSearch = $('#table-id-past_filter').find('input').val();
        $('#table-id-pending_filter').find('input').val(pastSearch);
        pTable.search(pastSearch).draw();
    }
    $('#hdnTab').val('home');
}
function PastSearchTextValue() {
    var chkPrevTab = $('#hdnTab').val();
    if (chkPrevTab == 'home') {
        var pandingSearch = $('#table-id-pending_filter').find('input').val();
        $('#table-id-past_filter').find('input').val(pandingSearch);
        tTable.search(pandingSearch).draw();
    }
    if (chkPrevTab == 'about') {
        var activeSearch = $('#table-id-active_filter').find('input').val();
        $('#table-id-past_filter').find('input').val(activeSearch);
        tTable.search(activeSearch).draw();
    }
    $('#hdnTab').val('contact');
}

function searchtext(ctrl) {
    var search = $('#table-id-pending_filter').find('input').val();
    var SearchText = ctrl.nextElementSibling;
    var ActiveElement = ctrl.nextElementSibling.nextElementSibling;
    SearchText.value = search;
    ActiveElement.value = 'home';
    return true;

}

function TitleSearchText(ctrl) {
    var search = $('#table-id-pending_filter').find('input').val();
    var SearchText = ctrl.nextElementSibling;
    var form = $(ctrl).parent();

    var ActiveElement = ctrl.nextElementSibling.nextElementSibling;
    SearchText.value = search;
    ActiveElement.value = 'home';
    form.submit();

}

function searchtextActive(ctrl) {
    var search = $('#table-id-active_filter').find('input').val();
    var ActiveSearchText = ctrl.nextElementSibling;
    var ActiveElement = ctrl.nextElementSibling.nextElementSibling;
    ActiveSearchText.value = search;
    ActiveElement.value = 'about';
    return true;
}

function TitleActiveText(ctrl) {
    var search = $('#table-id-active_filter').find('input').val();
    var ActiveSearchText = ctrl.nextElementSibling;
    var form = $(ctrl).parent();

    var ActiveElement = ctrl.nextElementSibling.nextElementSibling;
    ActiveSearchText.value = search;
    ActiveElement.value = 'about';
    form.submit();
}

function PastTextValue(ctrl) {
    var search = $('#table-id-past_filter').find('input').val();
    var PastSearchText = ctrl.nextElementSibling;
    var ActiveElement = ctrl.nextElementSibling.nextElementSibling;
    PastSearchText.value = search;
    ActiveElement.value = 'contact';
    return true;
}

function TitlePastText(ctrl) {
    var search = $('#table-id-past_filter').find('input').val();
    var PastSearchText = ctrl.nextElementSibling;
    var form = $(ctrl).parent();

    var ActiveElement = ctrl.nextElementSibling.nextElementSibling;
    PastSearchText.value = search;
    ActiveElement.value = 'contact';
    form.submit();
}

jQuery(document).ready(function () {
    jQuery('#table-id-pending_wrapper').scroll(function () {
        alert('asdf');
        var scrollTop = jQuery(this).scrollTop();
        jQuery('.fix-thead').css('transform', 'translateY(' + scrollTop + '10px)');
    });
});
var pTable; var sTable; var tTable;
function loadDataTables() {
    setTimeout(function () {
        pTable = $('.pTables').DataTable({
            "pageLength": 100,
            "search": {
                "search": searchText
            },
            columnDefs: [
                {
                    targets: [5],
                    orderData: [6, 6]
                },
                {
                    target: 6,
                    visible: false,
                    searchable: false
                },
                {
                    targets: [10],
                    orderData: [11, 11]
                },
                {
                    target: 11,
                    visible: false,
                    searchable: false
                },
                {
                    targets: [12],
                    orderData: [12, 12]
                },
                {
                    targets: [12],
                    visible: false,
                    searchable: true
                },
            ],
            lengthMenu: [
                [25, 50, 100, -1],
                [25, 50, 100, 'All']
            ],
            order: [[0, 'desc']]
        });
        tTable = $('.tTables').DataTable({
            "pageLength": 100,
            "search": {
                "search": PastSearchText
            },
            columnDefs: [
                {
                    targets: [5],
                    orderData: [6, 6]
                },
                {
                    target: 6,
                    visible: false,
                    searchable: false
                },
                {
                    targets: [8],
                    orderData: [8, 8]
                },
                {
                    targets: [8],
                    visible: false,
                    searchable: true
                },
            ],
            lengthMenu: [
                [25, 50, 100, -1],
                [25, 50, 100, 'All']
            ],
            order: [[0, 'desc']]
        });
        sTable = $('.sTables').DataTable({
            "pageLength": 100,
            "search": {
                "search": ActivesearchText
            },
            columnDefs: [
                {
                    targets: [5],
                    orderData: [6, 6]
                },
                {
                    target: 6,
                    visible: false,
                    searchable: false
                },
                {
                    targets: [8],
                    orderData: [9, 9]
                },
                {
                    target: 9,
                    visible: false,
                    searchable: false
                },
                {
                    targets: [10],
                    orderData: [10, 10]
                },
                {
                    targets: [10],
                    visible: false,
                    searchable: true
                },
            ],
            lengthMenu: [
                [25, 50, 100, -1],
                [25, 50, 100, 'All']
            ],
            order: [[0, 'desc']]
        });
    }, 1000);
}
$(document).ready(function () {
    loadDataTables();
});

function ChangeSpecsOnPlans(i, e, arg) {
    var myString = arg;
    var lastChar = myString[myString.length - 1];
    var a = jQuery("#" + arg).parent('td').siblings().children('input[id^=chkSpc_]').is(':checked');
    if (i == true && a == true)
        jQuery("#" + arg).parent('td').siblings().children('input[id^=chkPub]').removeAttr("disabled");
    else {
        jQuery("#" + arg).parent('td').siblings().children('input[id^=chkPub]').attr("disabled", "disabled");
    }


}
function ChangeSpecsOrPlans(i, arg) {
    var myString = arg;
    var lastChar = myString[myString.length - 1];

    console.log("#" + arg)
    if (i == true)
        jQuery("#" + arg).parent('td').siblings().children('input[id^=chkPub]').removeAttr("disabled");
    else {
        jQuery("#" + arg).parent('td').siblings().children('input[id^=chkPub]').attr("disabled", "disabled");
    }
}

function ChangeSpc(i, e, arg) {
    var myString = arg;
    var lastChar = myString[myString.length - 1];
    var a = jQuery("#" + arg).parent('td').siblings().children('input[id^=chkPlan_]').is(':checked');
    if (i == true && a == true)
        jQuery("#" + arg).parent('td').siblings().children('input[id^=chkPub]').removeAttr("disabled");
    else {
        jQuery("#" + arg).parent('td').siblings().children('input[id^=chkPub]').attr("disabled", "disabled");
    }

}
function ChangePublish(i, e, arg, ctrl) {
    jQuery.ajax({
        url: '/StaffAccount/ChangePublish/',
        data: { "Change": i, "ProjId": e },
        type: "POST",
        async: false,
        success: function (response) {
            //$(ctrl).parents("tr").remove();
            window.location.reload();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
    return false;
}

function ChangeUnPublish(i, e, arg, ctrl) {
    jQuery.ajax({
        url: '/StaffAccount/ChangePublish/',
        data: { "Change": i, "ProjId": e },
        type: "POST",
        async: false,
        success: function (response) {
            //$(ctrl).parents("tr").remove();
            window.location.reload();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
    return false;
}
onclick = "return confirm('')"
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});
function DeleteProject(Id, ctrl) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/StaffAccount/DeleteProject/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                alert('Project Deleted');
                $(ctrl).parents("tr").remove();
                //$(this).closest('tr').remove();
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

function BRChecked(ProjId,ctrl) {
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        url: '/StaffAccount/BidResult/',
        data: { 'ProjId': ProjId },
        async: false,
        success: function (response) {
            if (response.success) {
                jQuery(ctrl).prop("checked", true);
                jQuery(ctrl).prop("disabled", true);
            }
        },
        error: function (response) {
           
        },
        failure: function (response) {
           
        }
    });
}
jQuery(document).ready(function () {
    var table = jQuery(".cusTable");
    // Add an event listener to the table to detect when the user scrolls it
    setTimeout(function () {
        table.find("thead").addClass("fix-top");
    }, 1000);
});
jQuery(window).on("load", function () {
    var loader = jQuery("#Memloader");
    loader.hide();
});