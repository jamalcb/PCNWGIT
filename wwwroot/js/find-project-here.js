$(window).on("load", function () {
    var loader = $("#Membloader");
    loader.hide();
});
var appliedmodel = "";
jQuery(document).ready(function () {
    setTimeout(() => {
        jQuery("#table-id-active").wrap("<div class='tbl-wrap'></div>");
        jQuery("#table-id-pending").wrap("<div class='tbl-wrap'></div>")
        jQuery("#table-id-past").wrap("<div class='tbl-wrap'></div>")
    }, 2000)

    var activetab = localStorage.getItem('activetab');
    if (activetab) {
        var activetabRadioButton = document.getElementById(activetab);

        activetabRadioButton.checked = true;
        localStorage.removeItem('activetab');
    }



    //logged in User State Selection........


    $.ajax({
        type: "GET",
        url: "/Member/userMemberStates",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                var userstatesArray = Array.isArray(data) ? data : [data];

                // Store the "Select State" option
                var selectStateOption = $("#inpState option[value='0']").detach();

                $("#inpState option[value!='0']").prop("disabled", true).css("background-color", "#dddddd");

                $.each(userstatesArray, function (index, state) {
                    // Create a span element with the desired ID
                    var spanId = state + "Count";
                    var spanElement = $("<span>").attr("id", spanId);

                    // Add the span to the option
                    $("#inpState option:contains('" + state + "')").append(spanElement);

                    // Enable and reset background color for the option
                    $("#inpState option:contains('" + state + "')").prop("disabled", false).css("background-color", "");
                });

                var enabledOptions = $("#inpState option:enabled[value!='0']");
                var disabledOptions = $("#inpState option:disabled[value!='0']");

                $("#inpState").empty().append(enabledOptions).append(disabledOptions);

                // Append the "Select State" option back to the dropdown
                $("#inpState").prepend(selectStateOption);

                $("#inpState").trigger("change");
            }
        }
    });


    $.ajax({
        type: "Get",
        url: "/Member/GetFilterCount",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                // Assuming 'data' structure is like { Type: [...], SubType: [...], ... }

                // Update ProjectType
                updateFilterData("ProjectType", data["Type"], "ProjectTypeIds");

                if (data["SubType"].length > 0) {
                    $('#btnsubtype').removeClass('disabled');
                    updateFilterData("ProjectSubType", data["SubType"], "ProjectSubTypeIds");
                }
                // Update SubType

                updateFilterData("ProjectScope", data["Scope"], "ProjectScopes");

                updateFilterData("ProjectestCost", data["estCost"], "ProjectestCosts");
                updateFilterData("ProjectState", data["State"], "ProjectStates");

                updatePrevailingWagesCheckbox(data["Wages"]);

                // Update other elements as needed
                getAppliedFilters();
                $("#searchform").on("change", ":checkbox", function () {

                    prepareForm();
                });
            }
        }
    });
    function updatePrevailingWagesCheckbox(wagesData) {
        var wagesElement = $("#wages");

        // Clear existing content
        wagesElement.empty();

        if (wagesData && wagesData.length > 0) {
            var checkboxElement = $("<input>")
                .attr("type", "checkbox")
                .attr("name", "PrevailingWageFlag")
                .attr("id", "PrevailingCheck")
                .val("false") // Set the value to "true"
                .click(function () {
                    checkboxElement.prop("checked", !checkboxElement.prop("checked"));
                    this.closest('li').addClass('selected');
                    prepareForm();
                }); // Hide the checkbox

            var labelElement = $("<label>")
                .text("Prevailing Wages");

            var countElement = $("<span>")
                .text((wagesData[0]?.ProjectCount || 0))
                .addClass("count");
            var div = $('<div class="wageCheck">')
                .append(checkboxElement)
                .append(labelElement);
            var listItem = $("<li>")
                .append(div)
                .append(countElement)
                .click(function () {
                    // Toggle checkbox on click
                    checkboxElement.prop("checked", !checkboxElement.prop("checked"));

                    // Toggle background color
                    listItem.toggleClass("selected");
                    prepareForm();
                });

            wagesElement.append(listItem);
        }
    }

    function updateFilterData(filterId, filterData, name) {
        var filterElement = $("#" + filterId);

        // Clear existing content
        filterElement.empty();

        // Append new data
        $.each(filterData, function (index, item) {
            var checkboxElement = $("<input>")
                .attr("type", "checkbox")
                .attr("name", name)
                .val(item.TypeId)
                .hide(); // Hide the checkbox

            var labelElement = $("<label>")
                .text(item.Type);

            var countElement = $("<span>")
                .text(item.ProjectCount)
                .addClass("count");

            var listItem = $("<li>")
                .append(checkboxElement)
                .append(labelElement)
                .append(countElement)
                .click(function () {
                    checkboxElement.prop("checked", !checkboxElement.prop("checked")).trigger("change");
                    listItem.toggleClass("selected");
                });

            filterElement.append(listItem);
        });
    }





    function getAppliedFilters() {
        $.ajax({
            type: "Get",
            url: "/Member/AppliedFilters",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    appliedmodel = JSON.parse(data);
                    if (appliedmodel) {
                        if (appliedmodel.SearchText) {
                            $('#txtSearchProj').val(appliedmodel.SearchText);
                        }
                        if (appliedmodel.Distance) {
                            $('#inpDistance').val(appliedmodel.Distance);
                        }
                        if (appliedmodel.City) {
                            $('#inpCity').val(appliedmodel.City);
                        }
                        //if (appliedmodel.ProjectTypeId != null) {
                        //    $('#ProjectTypeId').val(appliedmodel.ProjectTypeId);
                        //    var subTypeHtml = '';
                        //    jQuery.ajax({
                        //        url: '/Project/GetProjectSubType/',
                        //        data: { "prefix": "", "ProjTypeId": appliedmodel.ProjectTypeId },
                        //        type: "POST",
                        //        success: function (response) {
                        //            var model = response;
                        //            jQuery('#ProjectSubTypeId').html('');
                        //            subTypeHtml += '<option value="0">Project Sub Type</option>';
                        //            $.each(model, function (index, item) {
                        //                subTypeHtml += '<option value="' + item.val + '">' + item.label + '</option>';
                        //            });
                        //            jQuery('#ProjectSubTypeId').html(subTypeHtml);

                        //            // Find and select the option based on appliedmodel.ProjectSubTypeId
                        //            if (appliedmodel.ProjectSubTypeId) {
                        //                $('#ProjectSubTypeId').val(appliedmodel.ProjectSubTypeId);
                        //            }
                        //        }
                        //    });
                        //}

                        if (appliedmodel.EstCost != null) {
                            $('#ddlCost').val(appliedmodel.EstCost);
                        }
                        //if (appliedmodel.PrevailingWageFlag != null) {
                        //    var isPrevailingWage = appliedmodel.PrevailingWageFlag === true;
                        //    //$('input[name="PrevailingWageFlag"]').filter('[value="' + isPrevailingWage + '"]').prop('checked', true);
                        //    $('input[name="PrevailingWageFlag"]').val(isPrevailingWage).prop('checked', isPrevailingWage);

                        //}
                        if (appliedmodel.strBidDateFrom) {
                            $('#strBidDateFrom').val(appliedmodel.strBidDateFrom);
                            $('#calenderdropdownMenuLink').css({
                                'color': '#0056b3',
                                'text-decoration': 'underline'
                            });
                        }
                        if (appliedmodel.strBidDateTo) {
                            $('#strBidDateTo').val(appliedmodel.strBidDateTo);
                            $('#calenderdropdownMenuLink').css({
                                'color': '#0056b3',
                                'text-decoration': 'underline'
                            });
                        }
                        //if (appliedmodel.StateList && appliedmodel.StateList.length > 0) {
                        //    // Iterate through the array
                        //    $(appliedmodel.StateList).each(function (index, value) {
                        //        // Find the checkbox with the corresponding value
                        //        var checkbox = $('input[name="StateList"][value="' + value + '"]');

                        //        // Check the checkbox if found
                        //        if (checkbox.length > 0) {
                        //            checkbox.prop('checked', true);
                        //        }
                        //    });
                        //}
                        if (appliedmodel.ProjectTypeIds && appliedmodel.ProjectTypeIds.length > 0) {

                            $(appliedmodel.ProjectTypeIds).each(function (index, value) {
                                // Find the hidden checkbox with the corresponding value
                                var checkbox = $('input[name="ProjectTypeIds"]:hidden').filter(function () {
                                    return $(this).val() == value;
                                });

                                // Check the checkbox if found
                                if (checkbox.length > 0) {
                                    checkbox.prop('checked', true);
                                    // Add the 'selected' class to the parent li
                                    checkbox.closest('li').addClass('selected');
                                }
                            });
                            var firstCheckedCheckbox = $('input[name="ProjectTypeIds"]:hidden:checked').first();
                            var card = firstCheckedCheckbox.closest('.card');
                            card.find('button[data-toggle="collapse"]').trigger('click');
                        }
                        if (appliedmodel.ProjectSubTypeIds && appliedmodel.ProjectSubTypeIds.length > 0) {

                            $(appliedmodel.ProjectSubTypeIds).each(function (index, value) {
                                // Find the hidden checkbox with the corresponding value
                                var checkbox = $('input[name="ProjectSubTypeIds"]:hidden').filter(function () {
                                    return $(this).val() == value;
                                });

                                // Check the checkbox if found
                                if (checkbox.length > 0) {
                                    checkbox.prop('checked', true);
                                    // Add the 'selected' class to the parent li
                                    checkbox.closest('li').addClass('selected');
                                }
                            });
                            var firstCheckedCheckbox = $('input[name="ProjectSubTypeIds"]:hidden:checked').first();
                            var card = firstCheckedCheckbox.closest('.card');
                            card.find('button[data-toggle="collapse"]').trigger('click');
                        }
                        if (appliedmodel.ProjectScopes && appliedmodel.ProjectScopes.length > 0) {

                            $(appliedmodel.ProjectScopes).each(function (index, value) {
                                // Find the hidden checkbox with the corresponding value
                                var checkbox = $('input[name="ProjectScopes"]:hidden').filter(function () {
                                    return $(this).val() == value;
                                });

                                // Check the checkbox if found
                                if (checkbox.length > 0) {
                                    checkbox.prop('checked', true);
                                    // Add the 'selected' class to the parent li
                                    checkbox.closest('li').addClass('selected');
                                }
                            });
                            var firstCheckedCheckbox = $('input[name="ProjectScopes"]:hidden:checked').first();
                            var card = firstCheckedCheckbox.closest('.card');
                            card.find('button[data-toggle="collapse"]').trigger('click');
                        }
                        if (appliedmodel.ProjectestCosts && appliedmodel.ProjectestCosts.length > 0) {

                            $(appliedmodel.ProjectestCosts).each(function (index, value) {
                                // Find the hidden checkbox with the corresponding value
                                var checkbox = $('input[name="ProjectestCosts"]:hidden').filter(function () {
                                    return $(this).val() == value;
                                });

                                // Check the checkbox if found
                                if (checkbox.length > 0) {
                                    checkbox.prop('checked', true);
                                    // Add the 'selected' class to the parent li
                                    checkbox.closest('li').addClass('selected');
                                }
                            });
                            var firstCheckedCheckbox = $('input[name="ProjectestCosts"]:hidden:checked').first();
                            var card = firstCheckedCheckbox.closest('.card');
                            card.find('button[data-toggle="collapse"]').trigger('click');
                        }
                        if (appliedmodel.ProjectStates && appliedmodel.ProjectStates.length > 0) {

                            $(appliedmodel.ProjectStates).each(function (index, value) {
                                // Find the hidden checkbox with the corresponding value
                                var checkbox = $('input[name="ProjectStates"]:hidden').filter(function () {
                                    return $(this).val() == value;
                                });

                                // Check the checkbox if found
                                if (checkbox.length > 0) {
                                    checkbox.prop('checked', true);
                                    // Add the 'selected' class to the parent li
                                    checkbox.closest('li').addClass('selected');
                                }
                            });
                            var firstCheckedCheckbox = $('input[name="ProjectStates"]:hidden:checked').first();
                            var card = firstCheckedCheckbox.closest('.card');
                            card.find('button[data-toggle="collapse"]').trigger('click');
                        }

                        if (appliedmodel.PrevailingWageFlag != null) {
                            var isPrevailingWage = appliedmodel.PrevailingWageFlag === true;
                            var checkbox = $('input[name="PrevailingWageFlag"]');

                            // Check the checkbox if found
                            if (checkbox.length > 0 && isPrevailingWage) {
                                checkbox.prop('checked', isPrevailingWage);
                                // Add the 'selected' class to the parent li
                                checkbox.closest('li').addClass('selected');
                            }
                        }

                        if (appliedmodel.State) {
                            $('#inpState').val(appliedmodel.State);
                        }

                    }

                }
            }
        });
    }
});




function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}
// Get the current URL
var currentUrl = window.location.href;

// Remove the 'returnUrl' parameter from the URL
var updatedUrl = currentUrl.split('?')[0];

// Update the URL without the 'returnUrl' parameter
window.history.replaceState({}, document.title, updatedUrl);





//getstateList('1');
//function getstateList(SelectedTab) {
//    $.ajax({
//        type: "GET",
//        url: "/Implement/GetDistinctStateOfTab",
//        data: { SelectedTab },
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (r) {
//            var table = $('#StateListUL');
//            $(r.data).each(function (index, item) {
//                var i = parseInt(index) + 1;
//                table.append($('<li></li>')

//                    .append($('<input>').attr({
//                        'id': 'chk' + i,
//                        'type': 'checkbox',
//                        'name': 'StateList',
//                        'value': item.Value
//                    }))
//                    .append($("<label>").attr(
//                        {
//                            'for': 'chk' + i
//                        })
//                        .text(item.Text))
//                );
//            });

//        }
//    }).done(function () {
//        if (appliedmodel.StateList && appliedmodel.StateList.length > 0) {
//            // Iterate through the array
//            $(appliedmodel.StateList).each(function (index, value) {
//                // Find the checkbox with the corresponding value
//                var checkbox = $('input[name="StateList"][value="' + value + '"]');

//                // Check the checkbox if found
//                if (checkbox.length > 0) {
//                    checkbox.prop('checked', true);
//                }
//            });
//        }
//    });
//}

$(function () {
    //$.ajax({
    //    type: "POST",
    //    url: "/Implement/GetDistinctState",
    //    data: '{}',
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    success: function (r) {
    //        var table = $('#StateListUL');
    //        $(r.data).each(function (index, item) {
    //            var i = parseInt(index) + 1;
    //            table.append($('<li></li>')

    //                .append($('<input>').attr({
    //                    'id': 'chk' + i,
    //                    'type': 'checkbox',
    //                    'name': 'StateList',
    //                    'value': item.Value
    //                }))
    //                .append($("<label>").attr(
    //                    {
    //                        'for': 'chk' + i
    //                    })
    //                    .text(item.Text))
    //            );
    //        });

    //    }
    //});
    //$('#prevproj').on('click', function () {
    //    $('#StateListUL').empty();
    //    getstateList('0');       
    //});
    //$('#futureproj').on('click', function () {
    //    $('#StateListUL').empty();
    //    getstateList('2');        
    //});
    //$('#activeproj').on('click', function () {
    //    $('#StateListUL').empty();
    //    getstateList('1');        
    //});
    $('#btnFilterByState').on('click', function () {
        var states = "";
        var stateString = "";
        $('input[name=statelist]').each(function (index, item) {
            if ($(this).is(":checked")) {
                var id = $(this).val();
                var state = $(this).closest('li').find('label').html();
                //states += "Id : " + id + "    State : " + state + "\n";
                states += "" + state + ",";
                stateString = states.substring(0, states.length - 1);
                //states = state;
            }
        });
        //start
        var a = 0;

        if (states != "") {
            $.ajax({
                url: "/Member/FindProjectSortByState/",
                type: "POST",
                data: { searchText: stateString },
                dataType: "json",
                //contentType: 'application/json',
                success: function (response) {
                    if (response != "" || response != null) {
                        var model = new Array();
                        model = response;
                        $("#pendingTblBody").html('');
                        $("#activeTblBody").html('');
                        $("#pastTblBody").html('');
                        var pendingRows = "";
                        var activeRows = "";
                        var pastRows = "";

                        $.each(model, function (index, item) {
                            if (item.ProjNumber == null) {
                                item.ProjNumber = "";
                            }

                            if (item.LocCity == null) {
                                item.LocCity = "";
                            }
                            if (item.MemberTrack == null) {
                                item.MemberTrack = false;
                            }
                            if (item.TrackCount == null) {
                                item.TrackCount = "";
                            }
                            if (item.Title == null) {
                                item.Title = "";
                            }
                            if (item.LocState == null) {
                                item.LocState = "";
                            }

                            if (item.ProjTypeId == null) {
                                item.ProjTypeId = "";
                            }
                            if (item.BidDt == null) {
                                item.BidDt = "";
                            }
                            if (item.LastBidDt == null) {
                                item.LastBidDt = "";
                            }
                            if (item.strBidDt == null) {
                                item.strBidDt = "";
                            }
                            if (item.StrAddenda == null) {
                                item.StrAddenda = "";
                            } else {
                                var date = new Date(item.BidDt);
                                var year = date.getFullYear();

                                var month = (1 + date.getMonth()).toString();
                                month = month.length > 1 ? month : "0" + month;

                                var day = date.getDate().toString();
                                day = day.length > 1 ? day : "0" + day;
                                item.BidDt = month + "/" + day + "/" + year;
                            }
                            if (item.ArrivalDt == null) {
                                item.ArrivalDt = "";
                            } else {
                                var date = new Date(item.ArrivalDt);
                                var year = date.getFullYear();

                                var month = (1 + date.getMonth()).toString();
                                month = month.length > 1 ? month : "0" + month;

                                var day = date.getDate().toString();
                                day = day.length > 1 ? day : "0" + day;
                                item.ArrivalDt = month + "/" + day + "/" + year;
                            }

                            if (item.Publish == null) {
                                item.Publish = false;
                            }
                            if (item.SpecsOnPlans == null) {
                                item.SpecsOnPlans = false;
                            }
                            if (item.SpcChk == null) {
                                item.SpcChk = false;
                            }
                            if (!item.Publish) {
                                pendingRows +=
                                    "<tr> <td>" +
                                    item.ProjTypeIdString +
                                    "</td> <td><a name='titleId' href= '/Member/Preview/" +
                                    item.ProjId +
                                    "'>" +
                                    item.Title +
                                    "</a></td><td>" +
                                    item.LocState +
                                    "</td><td>" +
                                    item.strBidDt +
                                    "</td><td><input id='chkDashboard' type='checkbox' checked=" +
                                    item.MemberTrack +
                                    " value=" +
                                    item.ProjId +
                                    "></td><td>" +
                                    item.StrAddenda +
                                    "</td><td>" +
                                    item.TrackCount +
                                    "</td><td></td></tr>";
                            }
                            if (item.Publish) {
                                activeRows +=
                                    "<tr> <td>" +
                                    item.ProjTypeIdString +
                                    "</td> <td><a name='titleId' href= '/Member/Preview/" +
                                    item.ProjId +
                                    "'>" +
                                    item.Title +
                                    "</a></td><td>" +
                                    item.LocState +
                                    "</td><td>" +
                                    item.strBidDt +
                                    "</td><td><input id='chkDashboard' type='checkbox' checked=" +
                                    item.MemberTrack +
                                    " value=" +
                                    item.ProjId +
                                    "></td><td>" +
                                    item.StrAddenda +
                                    "</td><td>" +
                                    item.TrackCount +
                                    "</td><td></td></tr>";
                            }
                            pastRows +=
                                "<tr> <td>" +
                                item.ProjTypeIdString +
                                "</td> <td><a name='titleId' href= '/Member/Preview/" +
                                item.ProjId +
                                "'>" +
                                item.Title +
                                "</a></td><td>" +
                                item.LocState +
                                "</td><td>" +
                                item.strBidDt +
                                "</td><td><input id='chkDashboard' type='checkbox' checked=" +
                                item.MemberTrack +
                                " value=" +
                                item.ProjId +
                                "></td><td>" +
                                item.StrAddenda +
                                "</td><td>" +
                                item.TrackCount +
                                "</td><td></td></tr>";
                        });
                        $("#pendingTblBody").html(pendingRows);
                        $("#activeTblBody").html(activeRows);
                        $("#pastTblBody").html(pastRows);
                    } else {
                        alert("No project. Please try again.");
                        $("#txtSearchProj").focus();
                    }
                },
            });
        }

        //End

    });
});
function loadDataTables() {
    setTimeout(function () {
        $('.mTables').DataTable({
            "pageLength": 100,
            "search": {
                "search": activeText
            },
            lengthMenu: [
                [25, 50, 100, -1],
                [25, 50, 100, 'All']
            ],
        });

        $('.tTables').DataTable({
            "pageLength": 100,
            "search": {
                "search": preText
            },
            lengthMenu: [
                [25, 50, 100, -1],
                [25, 50, 100, 'All']
            ],
        });

        $('.lTables').DataTable({
            "pageLength": 100,
            "search": {
                "search": futureText
            },
            lengthMenu: [
                [25, 50, 100, -1],
                [25, 50, 100, 'All']
            ],
        });
    }, 1000);
}
function ClearDataTables() {
    $('.mTables').dataTable().fnClearTable();
    $('.mTables').dataTable().fnDestroy();
    $('.tTables').dataTable().fnClearTable();
    $('.tTables').dataTable().fnDestroy();
    $('.lTables').dataTable().fnClearTable();
    $('.lTables').dataTable().fnDestroy();
}
$(document).ready(function () {
    loadDataTables();
    GetSavedSearch();
    $('#inpDistance').keypress(function (event) {
        // Get the current input value
        var inputValue = $(this).val();

        // Allow digits, backspace, and delete keys
        if (
            (event.which >= 48 && event.which <= 57) || // Digits
            event.which === 8 || // Backspace
            event.which === 46 // Decimal point
        ) {
            // Check if the input is already a decimal number
            if (inputValue.indexOf('.') !== -1 && event.which === 46) {
                event.preventDefault(); // Prevent another decimal point
            } else if (inputValue === '0' && event.which !== 8 && event.which !== 46) {
                event.preventDefault(); // Prevent leading zeros
            }
        } else {
            event.preventDefault(); // Prevent non-numeric input
        }
    });



});
function CommonSearch() {
    var lstState = [];
    $("input:checkbox[name=StateList]:checked").each(function () {
        lstState.push($(this).next('label').text());
    });
    var model = {};
    model.SearchText = $('#txtSearchProj').val();
    model.ProjectTypeId = jQuery('#ProjectTypeId').find(":selected").val();
    model.ProjectSubTypeId = jQuery('#ProjectSubTypeId').find(":selected").val();
    model.strBidDateFrom = jQuery('#strBidDateFrom').val();
    model.strBidDateTo = jQuery('#strBidDateTo').val();
    var data = jQuery('input[name = example]:checked').val();
    model.PrevailingWageFlag = false;
    if (data == 'true')
        model.PrevailingWageFlag = true;
    else
        model.PrevailingWageFlag = false;
    model.StateList = lstState;
    model.State = jQuery('#inpState').val();
    model.City = jQuery('#inpCity').val();
    model.Distance = jQuery('#inpDistance').val();
    $.ajax({
        url: '/Member/FindProjectHere/',
        type: 'POST',
        data: { model: model },
        dataType: 'json',
        success: function (response) {
            ClearDataTables();
            if (response != "" || response != null) {
                var model = new Array();
                model = response;
                $("#pendingTblBody").html('');
                $("#activeTblBody").html('');
                $("#pastTblBody").html('');
                var pendingRows = '';
                var activeRows = '';
                var pastRows = '';

                $.each(model, function (index, item) {
                    const Dt = new Date();
                    var Chkbiddate = addDays(Dt, 30);
                    var ChkImpdate = addDays(Dt, -3);
                    if (item.ProjNumber == null) {
                        item.ProjNumber = '';
                    }

                    if (item.LocCity == null) {
                        item.LocCity = '';
                    }
                    if (item.MemberTrack == null) {
                        item.MemberTrack = false;
                    }
                    if (item.TrackCount == null) {
                        item.TrackCount = '';
                    }
                    if (item.Title == null) {
                        item.Title = '';
                    }
                    if (item.LocState == null) {
                        item.LocState = '';
                    }

                    if (item.ProjTypeId == null) {
                        item.ProjTypeId = '';
                    }
                    if (item.BidDt == null) {
                        item.BidDt = '';
                    }
                    if (item.LastBidDt == null) {
                        item.LastBidDt = '';
                    }
                    if (item.strBidDt == null) {
                        item.strBidDt = '';
                    }
                    if (item.StrAddenda == null) {
                        item.StrAddenda = '';
                    }
                    else {
                        var date = new Date(item.BidDt);
                        var year = date.getFullYear();

                        var month = (1 + date.getMonth()).toString();
                        month = month.length > 1 ? month : '0' + month;

                        var day = date.getDate().toString();
                        day = day.length > 1 ? day : '0' + day;
                        item.BidDt = month + '/' + day + '/' + year;
                    }
                    if (item.ArrivalDt == null) {
                        item.ArrivalDt = '';
                    }
                    else {
                        var date = new Date(item.ArrivalDt);
                        var year = date.getFullYear();

                        var month = (1 + date.getMonth()).toString();
                        month = month.length > 1 ? month : '0' + month;

                        var day = date.getDate().toString();
                        day = day.length > 1 ? day : '0' + day;
                        item.ArrivalDt = month + '/' + day + '/' + year;
                    }

                    if (item.Publish == null) {
                        item.Publish = false;
                    }
                    if (item.SpecsOnPlans == null) {
                        item.SpecsOnPlans = false;
                    }
                    if (item.SpcChk == null) {
                        item.SpcChk = false;
                    }

                    activeRows += "<tr><td>" + item.ProjTypeIdString + "</td><td><a name='titleId' href= '/Member/Preview/" + item.ProjId + "'>" + item.Title + "</a></td><td>" + item.LocState + "</td><td>" + item.strBidDt + "</td><td><input id='chkDashboard' type='checkbox' checked=" + item.MemberTrack + " value=" + item.ProjId + "></td><td class='newPro-btn'><a href='' class='table-btn' data-toggle='modal' onclick='ListPdfAddenda(" + item.ProjId + ")'><i class='fa fa-eye'></i>" +
                        "</a><td>" + item.TrackCount + "</td></tr>";


                    if (item.BidDt != '') {
                        if (item.BidDt >= Chkbiddate) {
                            pendingRows += "<tr><td>" + item.ProjTypeIdString + "</td><td><a name='titleId' href= '/Member/Preview/" + item.ProjId + "'>" + item.Title + "</a></td><td>" + item.LocState + "</td><td>" + item.strBidDt + "</td><td><input id='chkDashboard' type='checkbox' checked=" + item.MemberTrack + " value=" + item.ProjId + "></td><td class='newPro-btn'><a href='' class='table-btn' data-toggle='modal' onclick='ListPdfAddenda(" + item.ProjId + ")'><i class='fa fa-eye'></i>" +
                                "</a><td>" + item.TrackCount + "</td></tr>";
                        }
                    }
                    if (item.ImportDt != null) {
                        if (item.ImportDt >= ChkImpdate) {
                            pastRows += pendingRows += "<tr><td>" + item.ProjTypeIdString + "</td><td><a name='titleId' href= '/Member/Preview/" + item.ProjId + "'>" + item.Title + "</a></td><td>" + item.LocState + "</td><td>" + item.strBidDt + "</td><td><input id='chkDashboard' type='checkbox' checked=" + item.MemberTrack + " value=" + item.ProjId + "></td><td class='newPro-btn'><a href='' class='table-btn' data-toggle='modal' onclick='ListPdfAddenda(" + item.ProjId + ")'><i class='fa fa-eye'></i>" +
                                "</a><td>" + item.TrackCount + "</td></tr>";
                        }
                    }
                });
                $("#pendingTblBody").html(pendingRows);
                $("#activeTblBody").html(activeRows);
                $("#pastTblBody").html(pastRows);
            }
            else {
                alert("No project. Please try again.");
                $("#txtSearchProj").focus();
            }
            loadDataTables();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
};
function setCheckboxHiddenValue(checkboxId, hiddenId) {
    var checkbox = document.getElementById(checkboxId);
    var hidden = document.getElementById(hiddenId);
    hidden.value = checkbox.checked;
}
function prepareForm() {
    var loader = $("#Membloader");
    loader.show();
    setCheckboxHiddenValue('PrevailingCheck', 'Prevailingcheckhidden');
    //setCheckboxHiddenValue('NewConstructioncheck', 'NewConstructionhidden');
    //setCheckboxHiddenValue('Remodelcheck', 'Remodelhidden');
    //setCheckboxHiddenValue('Additioncheck', 'Additionhidden');
    //setCheckboxHiddenValue('FutureWorkcheck', 'FutureWorkhidden');

    var checkedRadioBtn = document.querySelector('input[name="nav"]:checked');

    if (checkedRadioBtn) {
        localStorage.setItem('activetab', checkedRadioBtn.id);
    }

    document.getElementById('searchform').submit();
}


function SaveSearch() {
    var NameSearch = jQuery('#NameSearch').val()
    if (NameSearch == null || NameSearch == '' || NameSearch == undefined) {
        alert('Please enter search name');
        return false;
    }
    var lstState = [];
    $("input:checkbox[name=StateList]:checked").each(function () {
        lstState.push($(this).val());
    });
    var model = {};
    model.SearchText = $('#txtSearchProj').val();
    model.ProjectTypeId = jQuery('#ProjectTypeId').find(":selected").val();
    model.ProjectSubTypeId = jQuery('#ProjectSubTypeId').find(":selected").val();
    model.strBidDateFrom = jQuery('#strBidDateFrom').val();
    model.strBidDateTo = jQuery('#strBidDateTo').val();
    var data = jQuery('input[name = example]:checked').val();
    model.PrevailingWageFlag = false;
    if (data == 'true')
        model.PrevailingWageFlag = true;
    else
        model.PrevailingWageFlag = false;
    model.StateList = lstState;
    model.State = jQuery('#inpState').val();
    model.City = jQuery('#inpCity').val();
    model.Name = jQuery('#NameSearch').val()
    model.Distance = jQuery('#inpDistance').val();
    model.EstCost = jQuery('#ddlCost').val();
    ; $.ajax({
        url: '/Member/SaveSearch/',
        type: 'POST',
        data: { model: model },
        dataType: 'json',
        success: function (response) {
            sucMessage(response.statusMessage);
            $('#SavedSerachList').html('');
            GetSavedSearch();
        }
    });

    //  console.log(a)

}
function sucMessage(msg) {
    jQuery('#sucMsg').html(msg);
    setTimeout(
        function () {
            document.getElementById('sucMsg').style.display = 'none';
        }, 5000);
}
window.onload = function () {
    setTimeout(
        function () {
            document.getElementById('spnMessage').style.display = 'none';
        }, 3000);
}
function GetProjSubType() {
    var ProjectTypeId = jQuery('#ProjectTypeId').find(":selected").val();
    var subTypeHtml = '';
    if (ProjectTypeId == 0) {
        jQuery('#ProjectSubTypeId').children('option').remove();
        jQuery('#ProjectSubTypeId').append('<option value="0">Project Sub Type</option>');
    }
    else {
        jQuery.ajax({
            url: '/Project/GetProjectSubType/',
            data: { "prefix": "", "ProjTypeId": ProjectTypeId },
            type: "POST",
            success: function (response) {
                var model = new Array();
                model = response;
                jQuery('#ProjectSubTypeId').html('');
                subTypeHtml += '<option value="0">Project Sub Type</option>';
                $.each(model, function (index, item) {
                    subTypeHtml += '<option value="' + item.val + '">' + item.label + '</option>';
                });
                jQuery('#ProjectSubTypeId').html(subTypeHtml);

                //console.log(data);
            }
        });
    }
}
function EditProjSubType() {
    var projectTypeIds = jQuery('#EditProjectTypeIds').val();

    // Check if any project type is selected
    if (projectTypeIds && projectTypeIds.length > 0) {
        var subTypeHtml = '';

        // Reset the dropdown
        //jQuery('#EditProjectSubTypeId').html('<option value="0">Project Sub Type</option>');
        jQuery('#EditProjectSubTypeId').html('');

        // Iterate through each selected project type
        jQuery.each(projectTypeIds, function (index, projectId) {
            jQuery.ajax({
                url: '/Project/GetProjectSubType/',
                data: { "prefix": "", "ProjTypeId": projectId },
                type: "POST",
                async: false, // Ensure synchronous execution to preserve order
                success: function (response) {
                    var model = response;
                    $.each(model, function (index, item) {
                        subTypeHtml += '<option value="' + item.val + '">' + item.label + '</option>';
                    });
                }
            });
        });

        // Append the subtypes to the dropdown
        jQuery('#EditProjectSubTypeId').append(subTypeHtml);
        setSelectedSubTypes();
    } else {
        // No project type selected, reset the dropdown
        jQuery('#EditProjectSubTypeId').html('');
    }
}

$("body").on("click", "#PrintAddenda", function () {
    html2canvas($('#Addendapdf')[0], {
        onrendered: function (canvas) {
            var data = canvas.toDataURL();
            var docDefinition = {
                content: [{
                    image: data,
                    width: 500
                }]
            };
            pdfMake.createPdf(docDefinition).download("AddendaPdfFile.pdf");
        }
    });
});
function ListPdfAddenda(ProjId) {
    if (ProjId != '') {
        $.ajax({
            url: '/Member/AddendaListPdfContent/',
            type: 'POST',
            dataType: 'json',
            data: { 'ProjId': ProjId },
            success: function (response) {
                $("#tblAddendaFile").html('');
                if (response != "" || response != null) {
                    var model = new Array();
                    model = response;
                    var ComHtml = '';
                    if (model.length == 0) {
                        $("#tblAddendaFile").html('<p class="text-danger">No data available</p>');
                        $('.m-modal').modal('show');
                    }
                    else {
                        for (i = 0; i < model.length; i++) {
                            ComHtml += '<tr><td>' + model[i] + '</td></tr>'
                        }
                        $("#tblAddendaFile").html(ComHtml);
                        $('.m-modal').modal('show');
                    }
                }
                else {
                    $("#tblAddendaFile").html('<p class="text-danger">No data available</p>');
                    $('.m-modal').modal('show');
                }

            }
        })
    }
}
getPagination('#table-id');

function getPagination(table) {
    var lastPage = 1;
    $('#maxRows')
        .on('change', function (evt) {

            lastPage = 1;
            $('.pagination')
                .find('li')
                .slice(1, -1)
                .remove();
            var trnum = 0;
            var maxRows = parseInt($(this).val());
            var maxRows = parseInt($(this).val());

            if (maxRows == 200) {
                $('.pagination').hide();
            } else {
                $('.pagination').show();
            }

            var totalRows = $(table + ' tbody tr').length;
            $(table + ' tr:gt(0)').each(function () {

                trnum++;
                if (trnum > maxRows) {
                    // if tr number gt maxRows

                    $(this).hide(); // fade it out
                }
                if (trnum <= maxRows) {
                    $(this).show();
                } // else fade in Important in case if it ..
            }); //  was fade out to fade it in
            if (totalRows > maxRows) {
                // if tr total rows gt max rows option
                var pagenum = Math.ceil(totalRows / maxRows); // ceil total(rows/maxrows) to get ..
                //	numbers of pages
                for (var i = 1; i <= pagenum;) {
                    // for each page append pagination li
                    $('.pagination #prev')
                        .before(
                            '<li data-page="' +
                            i +
                            '">\
                                                                                                                                                                                                                                                  <span class="pagination-span">' +
                            i++ +
                            '<span class="sr-only">(current)</span></span>\
                                                                                                                                                                                                                                                </li>'
                        )
                        .show();
                }
            }
            $('.pagination [data-page="1"]').addClass('active');
            $('.pagination li').on('click', function (evt) {

                evt.stopImmediatePropagation();
                evt.preventDefault();
                var pageNum = $(this).attr('data-page');

                var maxRows = parseInt($('#maxRows').val());

                if (pageNum == 'prev') {
                    if (lastPage == 1) {
                        return;
                    }
                    pageNum = --lastPage;
                }
                if (pageNum == 'next') {
                    if (lastPage == $('.pagination li').length - 2) {
                        return;
                    }
                    pageNum = ++lastPage;
                }

                lastPage = pageNum;
                var trIndex = 0;
                $('.pagination li').removeClass('active');
                $('.pagination [data-page="' + lastPage + '"]').addClass('active');
                limitPagging();
                $(table + ' tr:gt(0)').each(function () {

                    trIndex++;
                    if (
                        trIndex > maxRows * pageNum ||
                        trIndex <= maxRows * pageNum - maxRows
                    ) {
                        $(this).hide();
                    } else {
                        $(this).show();
                    }
                });
            });
            limitPagging();
        })
        .val(100)
        .change();
}

function AddDashboard(i, e, arg) {

    jQuery.ajax({
        url: '/Member/AddDashboard/',
        data: { "Change": i, "ProjId": e },
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



function limitPagging() {
    if ($('.pagination li').length > 7) {
        if ($('.pagination li.active').attr('data-page') <= 3) {
            $('.pagination li:gt(5)').hide();
            $('.pagination li:lt(5)').show();
            $('.pagination [data-page="next"]').show();
        } if ($('.pagination li.active').attr('data-page') > 3) {
            $('.pagination li:gt(0)').hide();
            $('.pagination [data-page="next"]').show();
            for (let i = (parseInt($('.pagination li.active').attr('data-page')) - 2); i <= (parseInt($('.pagination li.active').attr('data-page')) + 2); i++) {
                $('.pagination [data-page="' + i + '"]').show();

            }

        }
    }
}
function ShowSearch() {
    $("#spnMessage").css("display", "block");
}

function ActiveSearchText(ctrl) {
    var search = $('#table-id-active_filter').find('input').val();
    var SearchText = ctrl.nextElementSibling;
    var form = $(ctrl).parent();
    SearchText.value = search;
    form.submit();
}
function FutureSearchText(ctrl) {
    var search = $('#table-id-pending_filter').find('input').val();
    var FutureText = ctrl.nextElementSibling;
    var form = $(ctrl).parent();
    FutureText.value = search;
    form.submit();
}
function PreviousSearchText(ctrl) {
    var search = $('#table-id-past_filter').find('input').val();
    var PreviousText = ctrl.nextElementSibling;
    var form = $(ctrl).parent();
    PreviousText.value = search;
    form.submit();
}

var activeText = "";
var futureText = "";
var preText = "";
jQuery(document).ready(function () {
    var ActiveSearchIText = jQuery('#hdnActiveSearchText').val();
    var FuturesearchIText = jQuery('#hdnFutureSearchText').val();
    var PrevioussearchIText = jQuery('#hdnPreviousSearchText').val();

    if (ActiveSearchIText != '') {
        activeText = ActiveSearchIText;
    }
    if (FuturesearchIText != '') {
        futureText = FuturesearchIText;
    }
    if (PrevioussearchIText != '') {
        preText = PrevioussearchIText;
    }
    jQuery('#hdnActiveSearchText').val('');
    jQuery('#hdnFutureSearchText').val('');
    jQuery('#hdnPreviousSearchText').val('');

});




