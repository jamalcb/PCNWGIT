$('#addnewSearch').click(function () {
    showModal();
    Savedbindfilter();
    setTimeout(function () {
        GettingFieldsAddSavedSearch();
    },500);
});


function showModal() {    
    jQuery("#frmEditSaveSearch").trigger('reset');
    jQuery('#frmEditSaveSearch').find("input[type=text], input[type=hidden],input[type=radio] textarea").not('#EditinpDistance').val('');
    jQuery('#frmEditSaveSearch').find("select").not('#EditinpDistance').html('');
    $('.es-modal').modal('show');
}

function EditSaveSearch(id) {
    storedProjectSubTypeIds = [];
    loader.show();
    Savedbindfilter();
    jQuery("#frmEditSaveSearch").trigger('reset');
    jQuery('#frmEditSaveSearch').find("input[type=text], input[type=hidden],input[type=radio] textarea").not('#EditinpDistance').val('');
    jQuery('#frmEditSaveSearch').find("select").not('#EditinpDistance').html('');

    $.ajax({
        url: '/Member/EditSavedSearch/',
        type: 'POST',
        data: { id: id },
        dataType: 'json',
        async: false,
        success: function (response) {
            // Fill the input fields with values from the response
            subTypeHtml = "";
            $('#EditSearchState option').prop('selected', false);
            $('#hiddenSearchId').val(id);
            $('#EditProjectTypeId').val(response.ProjectTypeId);
            //$("#EditProjectSubTypeId option:selected").text(response.ProjectSubTypeId);

            $('#EdittxtSearchProj').val(response.SearchText);
            if (response.PrevailingWageFlag != null) {
                var isPrevailingWage = response.PrevailingWageFlag === true;
                $('input[name="EditPrevailingWageFlag"]').val(isPrevailingWage).prop('checked', isPrevailingWage);

            }

            if (response.State) {
                $('#EditSearchState option[value="' + response.State + '"]').prop('selected', true);
            } 
            $('#EditinpCity').val(response.City);
            $('#EditinpDistance').val(response.Distance);
            $('#EditddlCost').val(response.EstCost != null ? response.EstCost : 0);
            $('#EditNameSearch').val(response.Name);
            // Extract the date part from the response.strBidDateFrom
            var dateStr = response.strBidDateFrom.split(' ')[0]; // This assumes the date part always appears before the time part
            if (dateStr !== null && dateStr !== "" && dateStr !== undefined) {
                // Create a JavaScript Date object from the date string
                var date = new Date(dateStr);
                // Format the date as "YYYY-MM-DD" (required for <input type="date>)
                var formattedDate = date.toISOString().split('T')[0];
                // Set the value of the input element
                $('#EditstrBidDateFrom').val(formattedDate);
            }
            var dateStrTo = response.strBidDateFrom.split(' ')[0];
            if (dateStrTo != null && dateStrTo != "" && dateStrTo != undefined) {
                var dateto = new Date(dateStrTo);
                var formattedDateTo = dateto.toISOString().split('T')[0];
                $('#EditstrBidDateTo').val(formattedDateTo);
            }

            setTimeout(() => {
                

                if (Array.isArray(response.ProjectTypeIds)) {
                    response.ProjectTypeIds.forEach(function (value) {
                        $('#EditProjectTypeIds option[value="' + value + '"]').prop('selected', true);
                        EditProjSubType(value);
                    });
                }
                if (Array.isArray(response.ProjectSubTypeIds)) {
                    storedProjectSubTypeIds = response.ProjectSubTypeIds;
                    setSelectedSubTypes();
                }
                if (Array.isArray(response.ProjectestCosts)) {
                    response.ProjectestCosts.forEach(function (value) {
                        $('#EditProjectestCosts option[value="' + value + '"]').prop('selected', true);
                    });
                }
                if (Array.isArray(response.ProjectScopes)) {
                    response.ProjectScopes.forEach(function (value) {
                        $('#EditProjectScopes option[value="' + value + '"]').prop('selected', true);
                    });
                }
                if (Array.isArray(response.ProjectStates)) {
                    // Assuming your 'StateList' is represented by checkboxes 
                    response.ProjectStates.forEach(function (state) {
                        $('#EditSearchState option[value="' + state + '"]').prop('selected', true);
                    });
                }
                $('#EditProjectState option[value="' + response.State + '"]').prop('selected', true);
                //$('#EditProjectSubTypeId').val(response.ProjectSubTypeId);
                loader.hide();
            }, 1000);
            
            

            $('.es-modal').modal('show');

        }
    });
}


function GettingFieldsAddSavedSearch() {
    $.ajax({
        type: "Get",
        url: "/Member/AppliedFilters",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                appliedmodel = JSON.parse(data);
                if (appliedmodel) {
                    if (appliedmodel.SearchText) {
                        $('#EdittxtSearchProj').val(appliedmodel.SearchText);
                    }
                    if (appliedmodel.Distance) {
                        $('#EditinpDistance').val(appliedmodel.Distance);
                    }
                    if (appliedmodel.City) {
                        $('#EditinpCity').val(appliedmodel.City);
                    }                    

                    if (appliedmodel.EstCost != null) {
                        $('#EditddlCost').val(appliedmodel.EstCost);
                    }
                    if (appliedmodel.strBidDateFrom) {
                        $('#EditstrBidDateFrom').val(appliedmodel.strBidDateFrom);
                    }
                    if (appliedmodel.strBidDateTo) {
                        $('#EditstrBidDateTo').val(appliedmodel.strBidDateTo);                        
                    }                   
                    if (appliedmodel.ProjectTypeIds && appliedmodel.ProjectTypeIds.length > 0) {

                        $(appliedmodel.ProjectTypeIds).each(function (index, value) {
                            $('#EditProjectTypeIds option[value="' + value + '"]').prop('selected', true);
                            EditProjSubType(value);
                        });                        
                    }
                    if (appliedmodel.ProjectSubTypeIds && appliedmodel.ProjectSubTypeIds.length > 0)
                    {
                        storedProjectSubTypeIds = appliedmodel.ProjectSubTypeIds;
                        setSelectedSubTypes();
                    }
                    if (appliedmodel.ProjectScopes && appliedmodel.ProjectScopes.length > 0) {

                        $(appliedmodel.ProjectScopes).each(function (index, value) {
                            $('#EditProjectScopes option[value="' + value + '"]').prop('selected', true);
                        });
                    }
                    if (appliedmodel.ProjectestCosts && appliedmodel.ProjectestCosts.length > 0) {
                        $(appliedmodel.ProjectestCosts).each(function (index, value) {
                            $('#EditProjectestCosts option[value="' + value + '"]').prop('selected', true);
                        });
                    }
                    if (appliedmodel.ProjectStates && appliedmodel.ProjectStates.length > 0) {
                        $(appliedmodel.ProjectStates).each(function (index, value) {
                            $('#EditSearchState option[value="' + value + '"]').prop('selected', true);
                        });
                       
                    }

                    if (appliedmodel.PrevailingWageFlag != null) {
                        var isPrevailingWage = appliedmodel.PrevailingWageFlag === true;
                        $('input[name="EditPrevailingWageFlag"]').val(isPrevailingWage).prop('checked', isPrevailingWage);

                    }

                    if (appliedmodel.State) {
                        $('#EditProjectState option[value="' + appliedmodel.State + '"]').prop('selected', true);
                    }

                }

            }
        }
    });
}




var storedProjectSubTypeIds = [];
function setSelectedSubTypes() {
    if (Array.isArray(storedProjectSubTypeIds)) {
        storedProjectSubTypeIds.forEach(function (value) {
            $('#EditProjectSubTypeId option[value="' + value + '"]').prop('selected', true);
        });
    }
}
var loader = $("#Membloader");
function DeleteSaveSearch(id) {
    if (confirm("Are you sure you want to delete ?")) {
        loader.show();
        $.ajax({
            type: "POST",
            dataType: 'json',
            //contentType: "application/json; charset=utf-8",
            url: '/Member/DeleteSavedSearch/',
            data: { 'id': id },
            async: false,
            success: function (response) {
                alert(response.statusMessage);
                $('#SavedSerachList li:not(:first-child)').remove();
                GetSavedSearch();
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
        loader.hide();
    }
    return false;
}
function UpdateSaveSearches() {
    loader.show();
    setCheckboxHiddenValue('EditPrevailingCheck', 'EditPrevailingcheckhidden');
    var Id = jQuery('#hiddenSearchId').val();
    var NameSearch = jQuery('#EditNameSearch').val();
    if (NameSearch == null || NameSearch == '' || NameSearch == undefined) {
        alert('Please enter search name');
        return false;
    }
    var lstState = [];
    $("#EditSearchState option:selected").each(function () {
        lstState.push($(this).val());
    });
    var lstprojIds = [];
    $("#EditProjectTypeIds option:selected").each(function () {
        lstprojIds.push($(this).val());
    });
    var lstprojsubIds = [];
    $("#EditProjectSubTypeId option:selected").each(function () {
        lstprojsubIds.push($(this).val());
    });
    var lstprojestcosts = [];
    $("#EditProjectestCosts option:selected").each(function () {
        lstprojestcosts.push($(this).val());
    });
    var lstprojscopes = [];
    $("#EditProjectScopes option:selected").each(function () {
        lstprojscopes.push($(this).val());
    });
    var model = {};
    model.ProjectTypeIds = lstprojIds;
    model.ProjectStates = lstState;
    model.ProjectestCosts = lstprojestcosts;
    model.ProjectScopes = lstprojscopes;
    model.ProjectSubTypeIds = lstprojsubIds;
    model.Id = Id;
    model.SearchText = $('#EdittxtSearchProj').val();
    model.ProjectTypeId = jQuery('#EditProjectTypeIds').find(":selected").val();
    model.ProjectSubTypeId = jQuery('#EditProjectSubTypeIds').find(":selected").val();
    model.strBidDateFrom = jQuery('#EditstrBidDateFrom').val();
    model.strBidDateTo = jQuery('#EditstrBidDateTo').val();
    //var data = $('input[name = EditPrevailingWageFlag]').is(":checked");
    var data = jQuery('input[name = EditPrevailingWageFlag]').val();
    model.PrevailingWageFlag = false;
    if (data == 'true')
        model.PrevailingWageFlag = true;
    else
        model.PrevailingWageFlag = false;
    model.City = jQuery('#EditinpCity').val();
    model.Name = NameSearch;
    model.Distance = jQuery('#EditinpDistance').val();
    model.EstCost = jQuery('#EditddlCost').val();
    model.State = jQuery('#EditProjectState').val();
    if (isSearchNameAlreadySaved(NameSearch, Id)) {
        alert('Search name is already saved. Please choose a different name.');
        loader.hide();
        return false;
    }
    $.ajax({
        url: '/Member/SaveSearch/',
        type: 'POST',
        data: { model: model },
        dataType: 'json',
        success: function (response) {
            alert(response.statusMessage);
            $('.es-modal').modal('hide');
            $('#SavedSerachList li:not(:first-child)').remove();
            GetSavedSearch();
        }
    });
    loader.hide();

}
function isSearchNameAlreadySaved(searchName,id) {
    var isSaved = false;

    $.ajax({
        url: '/Member/IsSearchNameAlreadySaved',
        type: 'POST',
        data: { searchName: searchName, id: id },
        async: false, // Make the call synchronous to simplify the logic
        success: function (response) {
            isSaved = response.isSaved;
        }
    });

    return isSaved;
}

function GetSavedSearch() {
    $.ajax({
        type: "POST",
        url: "/Member/GetSavedSearch",
        data: '{}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (r) {
            var model = new Array();
            model = r;
            var table = $('#SavedSerachList');
            var SearchHtml = '';
            $.each(model, function (index, item) {
                table.append($('<li></li>').append($('<a>' + item.Name + '</a>').attr({
                    'href': '#',
                    'id': item.id,
                    'onclick': 'GotoSavedSearch(' + item.Id + ')'
                })))
                table.append($('<li class="text-right"></li>').append(
                    $('<i data-toggle="tooltip" title="Edit" class="fas fa-edit"></i>').attr('onclick', 'EditSaveSearch(' + item.Id + ')')
                ).append(
                    $('<i data-toggle="tooltip" title="Delete" class="fas fa-trash"></i>').attr('onclick', 'DeleteSaveSearch(' + item.Id + ')')
                ));
            });
        }
    });
}

function GotoSavedSearch(id) {
    loader.show();
    $.ajax({
        url: '/Member/EditSavedSearch/',
        type: 'POST',
        data: { id: id },
        dataType: 'json',
        success: function (response) {
            if (response) {
                $.ajax({
                    url: '/Member/FindProjectHereSearch',
                    type: 'POST',
                    data: {
                        model: response,
                        checkvalue: 0
                    },
                    dataType: 'json',
                    success: function (searchResponse) {
                        console.log('Search response:', searchResponse);
                        
                    },
                    error: function (error) {
                        console.error('Error in FindProjectHereSearch AJAX request:', error);
                    }, complete: function () {
                        // Redirect regardless of success or error
                        window.location.href = "FindProjectHere?status=1";
                    }
                });
            }
        },
        error: function (error) {
            console.error('Error in EditSavedSearch AJAX request:', error);
        }
    });


    //$.ajax({
    //    url: '/Member/GoToSavedSearch/',
    //    type: 'POST',
    //    data: { id: id },
    //    dataType: 'json',
    //    success: function (response) {
    //        ClearDataTables();
    //        if (response != "" || response != null) {
    //            var model = new Array();
    //            model = response;
    //            $("#pendingTblBody").html('');
    //            $("#activeTblBody").html('');
    //            $("#pastTblBody").html('');
    //            var pendingRows = '';
    //            var activeRows = '';
    //            var pastRows = '';

    //            $.each(model.ActiveProjs, function (index, item) {
    //                var primeHtml = '';
    //                if (item.MBDCheck == "Y")
    //                    primeHtml = '<p class="text-danger">Note: Primes requests bids at various dates and times.</p>';
    //                activeRows += "<tr><td>" + item.ProjTypeIdString + "</td><td><a name='titleId' href= '/Member/Preview/" + item.ProjId + "'>" + item.Title + primeHtml + "</a></td><td>" + item.LocAddr1 + "</td><td>" + item.strBidDt5 + "</td><td class='text-center'><input id='chkDashboard' type='checkbox' checked='" + item.MemberTrack + "' onchange='AddDashboard(this.checked, this.value, this.id)' value='" + item.ProjId + "'></td><td class='newPro-btn'><a href='' class='table-btn' data-toggle='modal' onclick='ListPdfAddenda(" + item.ProjId + ")'><i class='fa fa-eye' aria-hidden='true'></i>" +
    //                    "</a><span> ," + item.AddendaCount + "</span><td>" + item.TrackCount + "</td></tr>";
    //            });
    //            $.each(model.FutureProjs, function (index, item) {
    //                var primeHtml = '';
    //                if (item.MBDCheck == "Y")
    //                    primeHtml = '<p class="text-danger">Note: Primes requests bids at various dates and times.</p>'
    //                pendingRows += "<tr><td>" + item.ProjTypeIdString + "</td><td><a name='titleId' href= '/Member/Preview/" + item.ProjId + "'>" + item.Title + primeHtml + "</a></td><td>" + item.LocAddr1 + "</td><td>" + item.strBidDt5 + "</td><td class='text-center'><input id='chkDashboard' type='checkbox' checked='" + item.MemberTrack + "' onchange='AddDashboard(this.checked, this.value, this.id)' value='" + item.ProjId + "'></td><td class='newPro-btn'><a href='' class='table-btn' data-toggle='modal' onclick='ListPdfAddenda(" + item.ProjId + ")'><i class='fa fa-eye' aria-hidden='true'></i>" +
    //                    "</a><span> ," + item.AddendaCount + "</span><td>" + item.TrackCount + "</td></tr>";
    //            });
    //            $.each(model.PrevProjs, function (index, item) {
    //                var primeHtml = '';
    //                if (item.MBDCheck == "Y")
    //                    primeHtml = '<p class="text-danger">Note: Primes requests bids at various dates and times.</p>'
    //                pastRows += "<tr><td>" + item.ProjTypeIdString + "</td><td><a name='titleId' href= '/Member/Preview/" + item.ProjId + "'>" + item.Title + primeHtml + "</a></td><td>" + item.LocAddr1 + "</td><td>" + item.strBidDt5 + "</td><td class='text-center'><input id='chkDashboard' type='checkbox' checked='" + item.MemberTrack + "' onchange='AddDashboard(this.checked, this.value, this.id)' value='" + item.ProjId + "'></td><td class='newPro-btn'><a href='' class='table-btn' data-toggle='modal' onclick='ListPdfAddenda(" + item.ProjId + ")'><i class='fa fa-eye' aria-hidden='true'></i>" +
    //                    "</a><span> ," + item.AddendaCount + "</span><td>" + item.TrackCount + "</td></tr>";
    //            });
    //            $("#pendingTblBody").html(pendingRows);
    //            $("#activeTblBody").html(activeRows);
    //            $("#pastTblBody").html(pastRows);
    //        }
    //        else {
    //            alert("No project. Please try again.");
    //            $("#txtSearchProj").focus();
    //        }
    //        loadDataTables();
    //    }
    //});
};

function Savedbindfilter() {
    $.ajax({
        type: "Get",
        url: "/Member/GetFilterCount",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                // Assuming 'data' structure is like { Type: [...], SubType: [...], ... }

                // Update ProjectType
                updatSavedSearchFilterData("EditProjectTypeIds", data["Type"], "EditProjectTypeIds");

                if (data["SubType"].length > 0) {
                    updatSavedSearchFilterData("EditProjectSubTypeId", data["SubType"], "EditProjectSubTypeIds");
                }
                // Update SubType

                updatSavedSearchFilterData("EditProjectScopes", data["Scope"], "EditProjectScopes");

                updatSavedSearchFilterData("EditProjectestCosts", data["estCost"], "EditProjectestCosts");
                updatSavedSearchFilterData("EditSearchState", data["State"], "State");
                jQuery('#EditProjectState').html('<option value="0">Project State</option>');
                updatSavedSearchFilterData("EditProjectState", data["State"], "EditProjectState");

                //updatePrevailingWagesCheckbox(data["Wages"]);

                // Update other elements as needed
                //getAppliedFilters();
                $("#searchform").on("change", ":checkbox", function () {

                    //prepareForm();
                });
            }
        }
    });
}
function updatSavedSearchFilterData(filterId, filterData, name) {
    var filterElement = $("#" + filterId);

    // Clear existing options
    //filterElement.find('option').not(':first').remove();

    // Append new options
    $.each(filterData, function (index, item) {
        var optionElement = $("<option>")
            .attr("value", item.TypeId)
            .text(item.Type);

        filterElement.append(optionElement);
    });

}
