/// <reference path="../lib/$/dist/$.js" />

var valProjTypeId = 0;
var currentDate = new Date();
var hours = currentDate.getHours();
var minutes = currentDate.getMinutes();
var seconds = currentDate.getSeconds();

// Convert single-digit hours, minutes, and seconds to double digits
hours = (hours < 10) ? '0' + hours : String(hours);
minutes = (minutes < 10) ? '0' + minutes : String(minutes);
seconds = (seconds < 10) ? '0' + seconds : String(seconds);

// Concatenate hours, minutes, and seconds into a single string
var currentTimeString = hours + minutes + seconds;
function autofill() {
    var term = $("#inpProjTypeId").val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/autofill/',
        data: { 'term': term },
        async: false,
        success: function (response) {
            console.log(response[0].label);
            $("#inpProjTypeId").val(response[0].label);
            $("#ProjTypeId").val(response[0].val);

        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
// script for adding input on server files icon
$("#d-input").click(() => {
    $("#show-input").show() && $("#d-input").hide()
})
// ======================================================================================================================================
// script for adding new row on project team section.

$("#cost_row_add").click(function () {
    var i = $('.EntityNameClass').length;
    newRowAdd =
        '<div id="new-addedRow" class="row ">' +
        '<div class="col-md-12 col-lg-12">' +
        '<input type="text" class="form-control pr-0 EntityNameClass" id="EntityName_' + i + '" name="EIList[' + i + '].EntityName" placeholder="Entity Name" />' +
        '</div>' +
        '<div class="col-8 col-md-8 col-lg-8 py-2">' +
        '<input autocomplete="off" class="form-control" id="EntityType_' + i + '" type="text" value="" name="EIList[' + i + '].EntityType" placeholder="Entity Type" />' +
        '</div>' +
        '<div class="col-4 col-lg-4 col-xl-4 d-flex align-items-center">' +
        '<input type="checkbox" class="" value="" />' +
        //'</div>' +
        //'<div class="col-lg-2 col-xl-1">' +
        '<div class="remove_prebid_row "><span class="action-del mx-2" id="del-addedRow"><i class="fa fa-trash"></i></span></div>' +
        '</div>' +
        '</div>';

    $('#membr-costInput').append(newRowAdd);
    InitializedAutoComplete();
});
// deleting the row on del-icon click
$("body").on("click", "#membr-del-costRow", function () {
    $("#remove-cost").remove();
    //$(this).parents("#project-team-row").remove();
})

$("body").on("click", "#del-addedRow", () => {
    $("#new-addedRow").remove();
})
// ==============================================================================================================================
// script to add pre-bid info row

$("#bidDate_add").click(() => {
    var biddateconveniancecount = $("div[class*='biddateconveniancecount']").length + 2;
    bidDateRow =
        '<div class="row pb-3 biddateconveniancecount align-items-center" id="remove-biddate">' +
        '<div class="col-md-6 col-lg-6 ">' +
        '<input type="date" id="BidDt' + biddateconveniancecount + '" name="BidDt' + biddateconveniancecount + '" class="form-control" placeholder = "select date" onchange="getDateIndex((this).id,event)"/> ' +
        '</div>' +
        '<div class=" col-10 col-4 col-sm-4 col-md-4 col-lg-4 cus-mt">' +
        '<input type="text" class="form-control" placeholder="PST" id="strBidDt' + biddateconveniancecount + '" name="strBidDt' + biddateconveniancecount + '" />' +
        '</div>' +
        '<div class="col-md-2 col-lg-2 col-sm-2 col-2 remove_prebid_row">' +
        '<span id="del-biddateRow" class="action-del"><i class="fa fa-trash" aria-hidden="true"></i></span>' +
        '</div>' +
        '</div>';
    if (biddateconveniancecount < 6)
        $("#bidDate_input").append(bidDateRow);
    else {
        bidDateRow = '<p class="text-danger err-message" id="TempMessage">You can only have upto 5 bidDate</p>';
        $("#bidDate_input").append(bidDateRow);
        $('.err-message').delay(1000).fadeOut();
    }
});

$("body").on("click", "#del-biddateRow", function () {
    $("#remove-biddate").remove();
})
$(document).ready(function () {
    $('.toggle-option').click(function () {
        $('.cst-optional').slideToggle(300);
        $('.cst-optional').css('max-height', 'fit-content');
        $(this).toggleClass('rotate');
    });
});
$(document).ready(function () {
    var currentUrl = window.location.href;
    var updatedUrl = currentUrl.split('?')[0];
    window.history.replaceState({}, document.title, updatedUrl);
    if ($('#ErrMsg').val() == 'Y')
        alert('Something went wrong');
    if ($('#ErrMsg').val() == 'OK') {
        alert('Project Submitted Successfully. A member of our staff will be in touch to finalize the details prior to posting.');
        $('#ErrMsg').val('N');
    }
    InitializeEntName();
    InitializeEntType();
    InitializePhlName();
    InitializePhlCon();
    PopulateModal();
});
// ==============================================================================================================================
// script for adding addenda row section

function selFiles(ctrl, event) {
    var files = event.target.files;
    var IdClass = ctrl.id;
    jQuery('#uploadError').html('');
    var delCount = jQuery("div[class*='drag-file']").length + 1;
    var checkName = document.querySelectorAll('[class*="file__value--text"]');
    var chkFile = new Array();
    if (checkName.length > 0) {
        for (var i = 0; i < checkName.length; i++) {
            chkFile.push(checkName[i].innerHTML);
        }
    }

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var fileHT = file.name;
        var innerhtml = "";

        if (chkFile.includes(fileHT)) {
            if (confirm('Already selected' + ' ' + fileHT + ' ' + 'name of the file, do you want to replace it?')) {
                chkFile = chkFile.filter(function (value) {
                    return value !== fileHT;
                });
                removeFile(IdClass, fileHT);
            } else {
                // Handle the case when the user chooses not to replace
            }
        } else {
            innerhtml = "<div class='file__value' tempid='" + IdClass + "'><div class='file__value--text'>" +
                fileHT +
                "</div></div>";
        }
        jQuery('#file_input').append(innerhtml);
    }

    $(ctrl).parents('div.drag-file').css('display', 'none');
    uploadHtml = '<div class="drag-file" >' +
        '<div class="file__input" id ="file__input" style = "height: 100px; border: 3px dotted;" >' +
        '<input class="file__input--file" id="fileUpload' + delCount + '" tabindex="9" name="pdfFile" type="file" multiple="multiple" onchange="selFiles(this,event)">' +
        '<label class="file__input--label" for="fileUpload' + delCount + '" data-text-btn="Upload"> Drag and Drop a file: </label>' +
        '</div></div>';
    $(ctrl).parents('div.drag-append').append(uploadHtml);

    UploadPdfFile();
}
function removeFile(tempid, filename) {
    var fileElement = jQuery('[tempid="' + tempid + '"] [class*="file__value--text"]').filter(function () {
        return jQuery(this).text() === filename;
    }).closest('.file__value');
    fileElement.click();
}
$(document).ready(function () {
    // ------------  File upload BEGIN ------------

    var fileHtml = ''
    jQuery(".file__input--file").on("change", function (event) {
        selFiles(this, event);
    });

    //Click to remove item
    jQuery("body").on("click", ".file__value", function () {
        var filename = jQuery(this).find('div.file__value--text').text();

        var id = jQuery(this).attr('tempid');
        var inputElement = jQuery('#' + id)[0]; // Replace 'your-input-element-id' with the actual ID of your input element

        var dataTransfer = new DataTransfer();

        // Iterate through the files in the input element
        for (var i = 0; i < inputElement.files.length; i++) {
            var file = inputElement.files[i];

            // Add the file to the new DataTransfer object if it doesn't match the filename
            if (file.name !== filename) {
                dataTransfer.items.add(file);
            }
        }
        // Replace the files in the input element with the new files
        inputElement.files = dataTransfer.files;
        if (inputElement.files.length === 0) {
            jQuery("#uploadCheck").val('N');
            jQuery("#selectCheck").val('N');            
        }
        jQuery(this).remove();
        UploadPdfFile();
    });
    // ------------ File upload END ------------
});
function InitializedAutoComplete() {
    $('[id^=EntityName]').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Project/GetCompanyName/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {

        },
        minLength: 2
    }).focus(function () {
        $(this).autocomplete("search");
    });

    $('[id^=EntityType]').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Project/GetEntityType/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
        },
        minLength: 2
    }).focus(function () {
        $(this).autocomplete("search");
    });
}
$(function () {
    InitializedAutoComplete()
});[]
$(function () {
    $("#inpProjTypeId").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Project/GetProjectType/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            console.log(i.item.val);
            $('input[id = ProjTypeId]').val(i.item.val);
            valProjTypeId = i.item.val;

            //$("#ProjTypeId").val();
        },
        minLength: -1
    }).focus(function () {
        $(this).autocomplete("search");
    });
});
$(function () {
    var ProjTypeId = $('input[id = ProjTypeId]').val();
    $("#inpProjSubTypeId").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Project/GetProjectSubType/',
                data: { "prefix": request.term, "ProjTypeId": valProjTypeId },
                type: "POST",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $("#ProjSubTypeId").val(i.item.val);
        },
        minLength: -1
    }).focus(function () {
        $(this).autocomplete("search");
    });
});
GetProjCode();
function GetProjCode() {
    $.ajax({
        url: '/Home/GetProjectCode/',
        data: {},
        type: "POST",
        success: function (response) {
            $('#ProjNumber').val(response.data.Result);
        },

        failure: function (response) {
            alert(response.statusMessage);
        }
    });
}
//Lowering city
const upperCaseFirstLetter = string =>
    `${string.slice(0, 1).toUpperCase()}${string.slice(1)}`;

const lowerCaseAllWordsExceptFirstLetters = string =>
    string.replaceAll(/\S*/g, word =>
        `${word.slice(0, 1)}${word.slice(1).toLowerCase()}`
    );
$(document).ready(function () {
    //var zipCode = $(this).val();
    $('#LocZip').keyup(function () {
        var zip = $(this).val();
        $('#LocCity').val('');
        $('#LocState').val('');
        $('#LocAddr2').val('');
        $('#CountyId').val('');
        var xmldata = '<CityStateLookupRequest USERID="405PLANC0113"><ZipCode><Zip5>' + zip + '</Zip5></ZipCode></CityStateLookupRequest>';
        //var apiKey = 'YOUR_API_KEY_HERE';
        var apiUrl = 'https://production.shippingapis.com/ShippingAPI.dll?API=CityStateLookup&XML=' + xmldata;
        if (zip.length == 5) {
            $.ajax({
                url: apiUrl,
                type: 'GET',
                dataType: 'xml',
                success: function (data) {
                    console.log(data);
                    $(data).find('ZipCode').each(function () {
                        var City = $(this).find('City').text();
                        var State = $(this).find('State').text();
                        if (City != null && State != null) {
                            //console.log('City : ' + City + ' State : ' + State);
                            $.ajax({
                                type: "POST",
                                dataType: 'json',
                                url: '/Project/CheckCounty/',
                                data: { 'City': City, 'State': State },
                                async: false,
                                success: function (response) {
                                    console.log(response);
                                    var county = '';
                                    var countyId = 0;
                                    if (response.data != null) {
                                        //debugger;
                                        county = response.data.County;
                                        countyId = response.data.CountyId;
                                    }
                                    var formattedCity = upperCaseFirstLetter(lowerCaseAllWordsExceptFirstLetters(City));
                                    $('#LocCity').val(formattedCity);
                                    $('#LocState').val(State);
                                    $('#LocAddr2').val(county);
                                    $('#CountyId').val(countyId);
                                    $('#Longitude').val(response.data.Longitude);
                                    $('#Latitude').val(response.data.Latitude);

                                },
                                error: function (response) {
                                    alert(response.responseText);
                                },
                                failure: function (response) {
                                    alert(response.responseText);
                                }
                            });
                        }
                        else {
                            $('#LocCity').val('');
                            $('#LocState').val('');
                            $('#LocAddr2').val('');
                            $('#CountyId').val('');
                        }
                    });
                },
                error: function ($XHR, textStatus, errorThrown) {
                    //console.log(textStatus + ': ' + errorThrown);
                }
            });
        }
    });
});
function changeScope(id) {
    //alert(id);
    var strManipulated = '';
    var strProjectScope = $('#ProjScope').val();
    var chkValue = '';
    if ($('#' + id).is(':checked')) {
        chkValue = $('#' + id).val();
        //console.log(chkValue);
    }
    var isExists = 'N';
    if (strProjectScope != '') {
        var myArray = strProjectScope.split(",");
        let strValue = "";
        for (let x in myArray) {
            strValue = myArray[x];
            if (strValue != "") {
                let result = strValue.substring(0, 3);
                if (result != ' # ') {
                    if (strValue == chkValue)
                        isExists = 'Y';
                    else
                        strManipulated += strValue + ",";
                }
                else
                    strManipulated += strValue + ",";
            }
        }
    }
    if (isExists == 'N')
        strManipulated += chkValue + ",";
    $('#ProjScope').val(strManipulated);
    console.log($('#ProjScope').val());
}
function checkUpload() {
    //var chk = $('#chkUploaded').val();
    //if (chk == 'N') {
    //    alert('Please upload document(s)');
    //    return false;
    //}
    var title = $('#Title').val();
    $('#Title').next('span').html('');
    if (title == undefined || title == '' || title == null) {
        $('#Title').next('span').html('Please enter Project Name.');
        $('#Title').focus();
        return false;
    }
    var projdesc = $('#projdesc').val();
    $('#projdesc').next('span').html('');
    if (projdesc == undefined || projdesc == '' || projdesc == null) {
        $('#projdesc').next('span').html('Please enter Project Description.');
        $('#projdesc').focus();
        return false;
    }
    var ContactName = $('#ContactName').val();
    $('#ContactName').next('span').html('');
    if (ContactName == undefined || ContactName == '' || ContactName == null) {
        $('#ContactName').next('span').html('Please enter Contact Name.');
        $('#ContactName').focus();
        return false;
    }
    var ContactMember = $('#ContactMember').val();
    $('#ContactMember').next('span').html('');
    if (ContactMember == undefined || ContactMember == '' || ContactMember == null) {
        $('#ContactMember').next('span').html('Please enter Company Name.');
        $('#ContactMember').focus();
        return false;
    }
    var ConEmail = $('#ConEmail').val();
    $('#ConEmail').next('span').html('');
    if (ConEmail == undefined || ConEmail == '' || ConEmail == null) {
        $('#ConEmail').next('span').html('Please enter Conatct Email.');
        $('#ConEmail').focus();
        return false;
    }
    var ContactPhone = $('#ContactPhone').val();
    $('#ContactPhone').next('span').html('');
    if (ContactPhone == undefined || ContactPhone == '' || ContactPhone == null) {
        $('#ContactPhone').next('span').html('Please enter Contact Phone.');
        $('#ContactPhone').focus();
        return false;
    }
    var address = $('#LocAddr1').val();
    $('#LocAddr1').next('span').html('');
    if (address == undefined || address == '' || address == null) {
        $('.cst-optional').css({
            'display': 'block',
            'max-height': 'fit-content'
        });
        $('.toggle-option').addClass('rotate');

        $('#LocAddr1').next('span').html('Please Enter Address.');
        $('#LocAddr1').focus();
        return false;
    }
    //var state = $('#LocState').val();
    //$('#LocState').next('span').html('');
    //if (state == undefined || state == '' || state == null) {
    //    $('#LocState').next('span').html('Please enter state');
    //    $('#LocState').focus();
    //    return false;
    //}
    //var city = $('#LocCity').val();
    //$('#LocCity').next('span').html('');
    //if (city == undefined || city == '' || city == null) {
    //    $('#LocCity').next('span').html('Please enter city');
    //    return false;
    //}
    //var county = $('#LocAddr2').val();
    //$('#LocAddr2').next('span').html('');
    //if (county == undefined || county == '' || county == null) {
    //    $('#LocAddr2').next('span').html('Please enter county');
    //    $('#LocAddr2').focus();
    //    return false;
    //}
    //var zip = $('#LocZip').val();
    //$('#LocZip').next('span').html('');
    //if (zip == undefined || zip == '' || zip == null) {
    //    $('#LocZip').next('span').html('Please enter zip code');
    //    $('#LocZip').focus();
    //    return false;
    //}
    var bidDt = $('#BidDt').val();
    var biddateundecided = $('#biddateundecided').prop('checked');

    $('#BidDt').next('span').html('');
    if ((bidDt == undefined || bidDt == '' || bidDt == null) && !biddateundecided) {
        $('.cst-optional').css({
            'display': 'block',
            'max-height': 'fit-content'
        });
        $('.toggle-option').addClass('rotate');
        $('#BidDt').next('span').html('Please Enter Bid Date/Time.');
        $('#BidDt').focus();
        return false;
    }
    $('#loader-overlay').show();
}
$('#BidDt').change(function () {
    $('#BidDt').next('span').html('');
});
$('#LocAddr1').on('input', function () {
    $('#LocAddr1').next('span').html('');
});
$('#Title').on('input', function () {
    $('#Title').next('span').html('');
});

async function UploadPdfFile(e) {
    var FileControls = document.querySelectorAll(".file__input--file");
    var projNum = $('#ProjNumber').val();
    $('#ProjNum').val(projNum);
    jQuery('#uploadError').html('')

    for (i = 0; i < FileControls.length; i++) {
        var element = FileControls[i];
        var elementId = element.getAttribute("id");
        var files = jQuery("#" + elementId).get(0).files;
        if (files.length > 0) {
            for (var k = 0; k < files.length; k++) {
                var file = files[k];
                var fileName = file.name;
                var extension = fileName.replace(/^.*\./, '');
                if (extension != 'pdf' && extension != 'docx' && extension != 'jpg' && extension != 'tif') {
                    jQuery('#uploadError').text(fileName + ' is not a valid file please upload only pdf/docx/jpg/tif file(s)');
                    return false;
                }
            }
        }
    }
    var limit = FileControls.length;
    var allFiles = [];

    for (i = 0; i < limit; i++) {

        var element = FileControls[i];
        var elementId = element.getAttribute("id");
        var files = jQuery("#" + elementId).get(0).files;

        if (files.length > 0) {
            jQuery('#chkUploaded').val('Y')
            allFiles = allFiles.concat(Array.from(files));
        }
    }
    var fileMap = new Map();

    // Iterate through allFiles in reverse order
    for (let i = allFiles.length - 1; i >= 0; i--) {
        let file = allFiles[i];
        if (!fileMap.has(file.name)) {
            fileMap.set(file.name, file);
        }
    }

    // Extract the unique files from the map
    allFiles = Array.from(fileMap.values());
    var ext = jQuery('#chkUploaded').val();
    if (ext != 'Y') {
        jQuery('#uploadError').html('Please select file.').css('color', 'red');
        return false;
    }
    var progressContainer = document.getElementById("progress-container");
    var progressBar = document.getElementById("progress-bar");
    var progressText = document.getElementById("progress-text");
    var filename = document.getElementById("filenameid");
    var totalFiles = 0;
    var successfulUploads = 0;
    for (i = 0; i < allFiles.length; i++) {
        totalFiles = allFiles.length;

        jQuery('#btnfileUpload1').prop('disabled', true);
        progressContainer.style.display = "block";
        progressBar.style.width = "0%";
        progressText.textContent = "0%";
        filename.innerText = "";

        var formData = new FormData();
        formData.append("pdfFile", allFiles[i]);
        formData.append("projNum", projNum)
        await new Promise((resolve, reject) => {
            jQuery.ajax({
                type: 'POST',
                url: '/Member/UploadPdf',
                data: formData,
                processData: false,
                contentType: false,
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();

                    xhr.upload.addEventListener("progress", function (e) {
                        if (e.lengthComputable) {
                            var percent = Math.round((e.loaded / e.total) * 100);
                            progressBar.style.width = percent + "%";
                            progressText.textContent = percent + "%";
                            if (allFiles[i].name) {
                                filename.innerText = "Uploading " + allFiles[i].name;
                            }
                        }
                    }, false);

                    return xhr;
                },
                success: function (response) {
                    if (response.Status === "success") {
                        successfulUploads++;
                        resolve(); // Resolve the promise when the AJAX call is successful

                            progressContainer.style.display = "none";
                        if (successfulUploads === totalFiles) {
                            jQuery('#uploadError').html('File Uploaded successfully').css('color', 'Blue')
                            $('#chkUploaded').val('Y');
                        }
                    } else {
                        reject(new Error("Upload failed")); // Reject the promise if the AJAX call is not successful

                    }
                },
                error: function (error) {
                    reject(error); // Reject the promise in case of an error
                }
            });
        });


    }
}
function UploadFile(e) {
   
    var formData = new FormData();
    var files = $("#fileUpload1").get(0).files;
    if (files.length > 0) {
        for (var i = 0; i < files.length; i++) {
            formData.append("pdfFile", files[i]);
        }
        formData.append("projNum", projNum)
        $.ajax({
            type: 'POST',
            url: '/Member/UploadPdf',
            data: formData,
            processData: false,
            contentType: false
        }).done(function (response) {
            if (response.Status === "success") {

                $('#chkUploaded').val('Y');
                Uploadhtml = '';
                $('#ftbody').html('');
                for (var i = 0; i < files.length; i++)
                    Uploadhtml += '<tr class="trUpload"><td>' + (i + 1) + '</td><td class="tdUpoadFile">' + files[i].name + '</td></tr>'
                $('#ftbody').html(Uploadhtml);
                $('.f-modal').modal('show');
            }
        });
    }
    else alert('Please select file(s) to upload');
    //   e.preventDefault();
};
$(function () {
    $("#ProjTypeIdString").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Project/GetProjectType/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }));
                    const input = $('[id*=ProjTypeIdString]')[0];
                    if (data.length > 0) {
                        const selectedValue = data[0].label; // Choose the first suggestion
                        $("#ProjTypeIdString").val(selectedValue);
                        $('input[id = ProjTypeId]').val(data[0].val);
                        valProjTypeId = data[0].val;
                        const startIndex = request.term.length;
                        const endIndex = selectedValue.length;
                        // Set the cursor position to overwrite mode
                        input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        $('input[id = ProjTypeId]').val(0);
                        valProjTypeId = 0;
                    }

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $('input[id = ProjTypeId]').val(i.item.val);
            valProjTypeId = i.item.val;

            //$("#ProjTypeId").val();
        },
        change: function (e, i) {
            var chkVal = $(this).val();
            if (chkVal == '') {
                valProjTypeId = 0;
                $('input[id = ProjTypeId]').val(0);
                $('input[id = ProjSubTypeId]').val(0);
                $('input[id = ProjSubTypeIdString]').val('');
            }
            //$("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        minLength: 1
    }).focus(function () {
        $(this).autocomplete("search");
    });
});
$(function () {
    $("#ProjSubTypeIdString").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Project/GetProjectSubType/',
                data: { "prefix": request.term, "ProjTypeId": valProjTypeId },
                type: "POST",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                    //        subTypeArr = data;
                    //           allowBackspace(subTypeArr[0].val, subTypeArr[0].label);
                    const input = $('[id*=ProjSubTypeIdString]')[0];
                    if (data.length > 0) {
                        $("#ProjSubTypeId").val(data[0].val);
                        $("#ProjSubTypeIdString").val(data[0].label);

                        const selectedValue = data[0].label; // Choose the first suggestion
                        const startIndex = request.term.length;
                        const endIndex = selectedValue.length;
                        // Set the cursor position to overwrite mode
                        input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        $('input[id = ProjSubTypeId]').val(0);
                    }
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $("#ProjSubTypeId").val(i.item.val);
        },
        change: function (e, i) {
            var chkVal = $(this).val();
            if (chkVal == '') {
                $('input[id = ProjSubTypeId]').val(0);
            }
            //$("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        minLength: 0
    }).focus(function () {
        $(this).autocomplete("search");
    });
});
function ChangeBidDt(id) {

    var chk = validateDateControl(id);
    if (chk != false) {
        var dateStr = document.getElementById(id).value;
        const currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);
        const [month, day, year] = dateStr.split("/");
        const dateObject = new Date(year, month - 1, day); // Month is 0-based in JavaScript Date constructor

        if (dateObject <= currentDate) {
            alert("The Date must be greater to today's date")
            document.getElementById(id).value = "";
            $('#' + id).focus();
            return false;
        }
        var pub = $('#Publish').val();
        if (pub == 'True' || pub == 'true') {
            $('#BidDt2').val($('#HiddenBidDt').val())
            $('#strBidDt4').val($('#hdnLastBidDt').val());
            $('#strBidDt2').val($('#strBidDt2').val());
            $('#divPrevBD').css('display', 'block');
        }
    }
};
function validateDateControl(id) {
    var dateStr = document.getElementById(id).value;
    const inputElement = document.getElementById(id);
    const isValid = /^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])\/(?:\d{2}|\d{4})?$|^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])$/.test(dateStr);
    // /^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])\/(?:\d{2}|\d{4})?$/

    if (dateStr != '') {
        if (!isValid) {
            alert("Invalid date/date format. Please use MM/dd/ or MM/dd/yyyy format or check the month date value.");
            document.getElementById(id).value = "";
            $('#' + id).focus();
            return false;
        }
        else {
            var parts = dateStr.split("/");

            var formattedValue = (parts[0].length === 1 ? "0" + parts[0] : parts[0]) + "/" +
                (parts[1].length === 1 ? "0" + parts[1] : parts[1]) + "/";
            if (parts[2]) {
                if (parts[2].length === 2) {
                    if (parts[2] == "00") {
                        alert('Year component cannot be zero')
                        $('#' + id).focus();
                        document.getElementById(id).value = "";
                        return false;
                    }
                    else { formattedValue += "20" + parts[2]; }

                } else {
                    formattedValue += parts[2];
                }
            }
            else {
                var currentYear = new Date().getFullYear();
                var currentYearLastTwoDigits = currentYear.toString();

                if (parts[0] <= new Date().getMonth() + 1) {
                    if (parts[0] == new Date().getMonth() + 1) {
                        if (parts[1] < new Date().getDate())
                            currentYearLastTwoDigits = (currentYear + 1).toString();
                    }
                    else {
                        currentYearLastTwoDigits = (currentYear + 1).toString();
                    }
                }

                formattedValue += currentYearLastTwoDigits;
            }
            document.getElementById(id).value = formattedValue;
            dateStr = formattedValue;
        }
        var validArr = dateStr.split('/');
        var monthPart = parseInt(validArr[0]);
        var dayPart = parseInt(validArr[1]);
        var yearPart = parseInt(validArr[2]);
        if (monthPart == 4 || monthPart == 9 || monthPart == 11 || monthPart == 6) {
            if (dayPart > 30) {
                alert('You can not enter date more than 30 for month of April, June, September and November');
                $('#' + id).focus();
                document.getElementById(id).value = "";
                return false;
            }
        }
        else if (monthPart == 1 || monthPart == 3 || monthPart == 5 || monthPart == 7 || monthPart == 8 || monthPart == 10 || monthPart == 12) {
            if (dayPart > 31) {
                alert('You can not enter date more than 31 for month of January, March, May, July, August, October and December');
                $('#' + id).focus();
                document.getElementById(id).value = "";
                return false;
            }
        }
        else if (monthPart == 2) {
            const leap = checkLeapYear(yearPart);
            if (leap) {
                if (dayPart > 29) {
                    alert('You can not enter date more than 29 for month of February.');
                    $('#' + id).focus();
                    document.getElementById(id).value = "";
                    return false;
                }
            }
            else {
                if (dayPart > 28) {
                    alert('You can not enter date more than 28 for month of February.');
                    $('#' + id).focus();
                    document.getElementById(id).value = "";
                    return false;
                }
            }

            //if ((year % 4 == 0) && (year % 100 != 0) || (year % 400 == 0)) {
            //    if (dayPart > 29) {
            //        alert('You can not enter date more than 29 for month of February.');
            //    }
            //}
        }
        else {
            alert('Date is not correct Please check date.');
            $('#' + id).focus();
            document.getElementById(id).value = "";
            return false;
        }
    }
};
function checkLeapYear(year) {

    const leap = new Date(year, 1, 29).getDate() === 29;
    if (leap) {
        return true;
    } else {
        return false;
    }
};
function UndecidedPre(ctrl) {
    var Assign = ctrl.checked;
    var TBD = "TBD";

    var parentDiv = $(ctrl).closest('.prebidconveniancecount');

    if (Assign) {
        $('#strPreBidDt').val(TBD);
        // Disable all inputs within the parent div
        parentDiv.find(':input:not([id*=Id])').prop('disabled', true);
        parentDiv.find(':input:not([id*=Id])').val('');
        parentDiv.find(':input[type="checkbox"]').prop('disabled', true);
        //jQuery("span#bidInfo-row-add").css("pointer-events", "none");

    } else {
        // Enable all inputs within the parent div
        parentDiv.find(':input').prop('disabled', false);
        parentDiv.find(':input[id$="PST"]').val('PT');
        parentDiv.find(':input[id$="PST"]').prop('readonly', true);
        parentDiv.find(':input[type="checkbox"]').prop('disabled', false);
        //jQuery("span#bidInfo-row-add").css("pointer-events", "auto");
        jQuery('[id *= hComp]', parentDiv).val('00');
        jQuery('[id *= tComp]', parentDiv).val('00');
        jQuery('[id *= mValue]', parentDiv).val('AM');
    }
    $(ctrl).prop('disabled', false);
    $('[id *= UndecidedPreBid]').prop('disabled', false);
    $(ctrl).prop('checked', Assign);
    var hiddenInput = ctrl.closest('.form-check').querySelector('input[type="hidden"][id *=  UndecidedPreBid]');

    // Check if a hidden input field was found
    if (hiddenInput) {
        // Set the value of the found hidden input field
        hiddenInput.value = Assign;
    }
};
function ChangeMandatory(checkbox) {
    var hiddenInput = checkbox.nextElementSibling.nextElementSibling; // Get the next sibling element, which is the hidden input
    hiddenInput.value = checkbox.checked ? "true" : "false"; // Assign the value based on the checkbox checked state
};
function ChangeAnd(checkbox) {
    var hiddenInput = checkbox.nextElementSibling.nextElementSibling; // Get the next sibling element, which is the hidden input
    hiddenInput.value = checkbox.checked ? "true" : "false"; // Assign the value based on the checkbox checked state
};

const spanElement = document.getElementById('bidInfo-row-add');

// Add a keydown event listener
spanElement.addEventListener('keydown', function (event) {
    // Check if the Enter key was pressed (key code 13)
    if (event.keyCode === 13 || event.which === 13) {
        // Perform the desired action when Enter is pressed
        // For example, you can trigger a click event
        spanElement.click();
        event.preventDefault();
    }
});

//const phlSpanElement = document.getElementById('planHolder-add');

//// Add a keydown event listener
//phlSpanElement.addEventListener('keydown', function (event) {
//    // Check if the Enter key was pressed (key code 13)
//    if (event.keyCode === 13 || event.which === 13) {
//        // Perform the desired action when Enter is pressed
//        // For example, you can trigger a click event
//        phlSpanElement.click();
//        event.preventDefault();
//    }
//});
const spanElementCost = document.getElementById('cost-row-add');

// Add a keydown event listener
spanElementCost.addEventListener('keydown', function (event) {
    // Check if the Enter key was pressed (key code 13)
    if (event.keyCode === 13 || event.which === 13) {
        // Perform the desired action when Enter is pressed
        // For example, you can trigger a click event
        spanElementCost.click();
        event.preventDefault();
    }
});
const spanElementPro = document.getElementById('project-row-adder');

// Add a keydown event listener
spanElementPro.addEventListener('keydown', function (event) {
    // Check if the Enter key was pressed (key code 13)
    if (event.keyCode === 13 || event.which === 13) {
        // Perform the desired action when Enter is pressed
        // For example, you can trigger a click event
        spanElementPro.click();
        event.preventDefault();
    }
});
const btnEntModel = document.getElementsByClassName('popClass');
for (let i = 0; i < btnEntModel.length; i++) {
    btnEntModel[i].addEventListener('keydown', function (event) {
        // Check if the Enter key was pressed (key code 13)
        if (event.keyCode === 13 || event.which === 13) {
            // Perform the desired action when Enter is pressed
            // For example, you can trigger a click event
            this.click();
            event.preventDefault();
        }
    });
}
function delSapn() {
    const delEntModel = document.getElementsByClassName('popClassDel');
    for (let i = 0; i < delEntModel.length; i++) {
        delEntModel[i].addEventListener('keydown', function (event) {
            // Check if the Enter key was pressed (key code 13)
            if (event.keyCode === 13 || event.which === 13) {
                // Perform the desired action when Enter is pressed
                // For example, you can trigger a click event
                this.click();
                event.preventDefault();
            }
        });
    }

}

var suggestions = ["00", "15", "30", "45"];
var Tsuggestions = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"];
var Msuggestions = ["AM", "PM"];
var inputField = $('#hComp');
function autoCompleteTimer() {
    // Autocomplete for tComp input
    $('[id *= tComp]').autocomplete({
        source: function (request, response) {
            response(Tsuggestions);
        },
        minLength: 0,
        change: function (event, ui) {
            var value = parseInt(this.value);
            if (value < 1 || value > 12) {
                this.value = '00';
            }
            else if (value < 10) {
                this.value = '0' + value;
            }
        },
        open: function (event, ui) {
            $(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            $(this).autocomplete("widget").removeClass("show-all");
        },
    }).on("focus", function () {
        $(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.DOWN) {
            event.preventDefault();
            $(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            $(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
        }
    }).on("input", function () {
        var value = $(this).val();
        if (value.length > 2) {
            $(this).val(value.slice(0, 2));
        }
    });
    $('[id *= hComp]').autocomplete({
        source: function (request, response) {
            response(suggestions);
        },
        minLength: 0,
        open: function (event, ui) {
            $(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            $(this).autocomplete("widget").removeClass("show-all");
        },
        change: function (event, ui) {
            var value = parseInt(this.value);
            //if (value < 0 || value > 60) {
            //    this.value = '00';
            //}
            //else if (value >= 0 && value <= 7) {
            //    this.value = '00';
            //}
            //else if (value >= 0 && value <= 15) {
            //    this.value = '15';
            //} else if (value >= 16 && value <= 22) {
            //    this.value = '15';
            //} else if (value >= 23 && value <= 37) {
            //    this.value = '30';
            //} else if (value >= 38 && value <= 52) {
            //    this.value = '45';
            //} else if (value >= 53 && value <= 60) {
            //    this.value = '00';
            //}
            if (isNaN(value)) {
                this.value = '00';
            } else if (value !== 15 && value !== 30 && value !== 0 && value !== 45)
                this.value = '00';
        }
    }).on("focus", function () {
        $(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.DOWN) {
            event.preventDefault();
            $(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            $(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
            //  $.ui.keyCode.ba
        }
    }).on("keyup", function (event) {
        var value = $(this).val();
        if (value.length > 2) {
            $(this).val(value.slice(0, 2));
        }
        var inputKeyCode = event.keyCode;
        console.log("Key Code at input: " + inputKeyCode);
        value = parseInt(this.value);

        if (inputKeyCode != 8) {
            if (value === 0) {
                this.value = '00';
            }
            else if (value === 1) {
                this.value = '15';
            }
            else if (value === 3) {
                this.value = '30';
            }
            else if (value === 4) {
                this.value = '45';
            }
        }

    });
    $('[id *= mValue]').autocomplete({
        source: function (request, response) {
            response(Msuggestions);
        },
        minLength: 0,
        open: function (event, ui) {
            $(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            $(this).autocomplete("widget").removeClass("show-all");
        },
        change: function (event, ui) {
            var value = this.value.toUpperCase();
            if (value == 'A') {
                this.value = 'AM';
            }
            else if (value == 'P') {
                this.value = 'PM';
            }

            else if (value !== 'AM' && value !== 'PM') {
                this.value = 'AM';
            }
        }
    }).on("focus", function () {
        $(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.DOWN) {
            event.preventDefault();
            $(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            $(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
        }
    }).on("input", function () {
        var value = $(this).val();
        if (value.length > 2) {
            $(this).val(value.slice(0, 2));
        }
    });
}

$(function () {
    autoCompleteTimer()
});

$("#bidInfo-row-add").click(() => {
    var delCount = $("div[class*='delPreBidCount']").length;
    var preText = $("div[class*='prebidconveniancecount']").length;
    var prebidconveniancecount = $("div[class*='prebidconveniancecount']").length;
    prebidconveniancecount = prebidconveniancecount + delCount;
    var tabIndex = parseFloat($('#bidInfo-row-add').attr('tabindex'));
    var Fid = 'preBidInfos_' + prebidconveniancecount + '__PreBidDate';
    var EFid = '';
    //if (delCount > 0) {
    //    var checkissue = document.querySelectorAll('[class*="delPreBidCount"]');
    //    var classess = checkissue[0].className
    //    classess = classess.replace('delPreBidCount', 'prebidconveniancecount');
    //    checkissue[0].setAttribute('class', classess);
    //    checkissue[0].setAttribute('style', '');
    //    var inputElement = checkissue[0].getElementsByTagName('input');
    //    for (var i = 0; i < inputElement.length; i++) {
    //        var element = inputElement[i];
    //        var elementType = element.getAttribute("type");
    //        var PST = element.getAttribute("id");
    //        var checkPST = "PST";
    //        var checkDeleted = "IsDeleted"
    //        if (elementType !== null) {
    //            console.log("Element Type: " + elementType + ' ' + element.id + ' ' + element.value);
    //        } else {
    //            console.log("No type attribute found for element: " + element.id);
    //        }
    //        if (elementType == 'text' && !PST.endsWith(checkPST)) {
    //            element.value = '';
    //            if (PST.endsWith('__PreBidDate'))
    //                EFid = PST;
    //        }
    //        else if (elementType == 'time') { element.value = ''; }
    //        else if (elementType == 'checkbox') { element.checked = false; }
    //        else if (elementType == 'hidden' && !PST.endsWith('_Id')) { element.value = 'false'; }
    //        // Check if the element has a type attribute
    //        if (elementType !== null) {
    //            console.log("Element Type: " + elementType + ' ' + element.id + ' ' + element.value);
    //        } else {
    //            console.log("No type attribute found for element: " + element.id);
    //        }
    //    }
    //    $('#' + EFid).focus();
    //}
    //else {
    preBidRow =
        '<div class="row mt-2 pb-2 border-bottom prebidconveniancecount" id="prebid-info-row">' +
        '<div class="col-2 col-sm-2 col-md-2 Pre-Text">' +
        '<label>Meeting ' + (preText + 1) + '</label>' +
        '</div>' +
        //'<div class="col-6 col-sm-6 col-md-6 mb-lg-0 mb-2">' +
        //'<input type="checkbox" class="mx-1" value="">Mandatory ' +
        //'</div>' +
        '<div class="col-10 col-sm-10 col-md-10 mb-lg-0 mb-2 pl-0">' +
        '<div class="row">' +
        '<div class="col-lg-3 col-md-3">' +
        '<input type="hidden" id="preBidInfos_' + prebidconveniancecount + '__Id" name="preBidInfos[' + prebidconveniancecount + '].Id">' +

        '<input type="text" placeholder="Select Date" tabindex="' + (tabIndex + 1) + '" class="form-control datepicker-custom" id="preBidInfos_' + prebidconveniancecount + '__PreBidDate" autocomplete="off" name="preBidInfos[' + prebidconveniancecount + '].PreBidDate" onchange="getPreDateIndex((this).id,event)" />' +
        '<input type="hidden" id="preBidInfos_' + prebidconveniancecount + '__IsDeleted" name="preBidInfos[' + prebidconveniancecount + '].IsDeleted" class="Pre-Del" > ' +
        '</div>' +
        '<div class="col-10 col-lg-3 col-md-3 col-sm-3 ">' +
        '<div id="time_input">' +
        '<label for="hours">' +
        '<input type="number" tabindex="' + (tabIndex + 2) + '" id="preBidInfos_' + prebidconveniancecount + '__tComp" name="preBidInfos[' + prebidconveniancecount + '].tComp" value="00">' +
        '</label>' +
        '<span>:</span>' +
        '<label for="minutes">' +
        '<input type="number" tabindex="' + (tabIndex + 3) + '" id="preBidInfos_' + prebidconveniancecount + '__hComp" name="preBidInfos[' + prebidconveniancecount + '].hComp" value="00">' +
        '</label>' +
        '<span>:</span>' +
        '<label for="seconds">' +
        '<input type="Text" tabindex="' + (tabIndex + 4) + '" id="preBidInfos_' + prebidconveniancecount + '__mValue" name="preBidInfos[' + prebidconveniancecount + '].mValue" value="AM">' +
        '</label>' +
        '</div>' +
        '<input class="form-control time-input" style="display:none;" type="time" placeholder="Time" id="preBidInfos_' + prebidconveniancecount + '__PreBidTime" name="preBidInfos[' + prebidconveniancecount + '].PreBidTime">' +
        '</div>' +
        '<div class="col-2 col-lg-2 col-md-2 col-sm-2 ">' +
        '<input class="form-control" type="text" id="preBidInfos_' + prebidconveniancecount + '__PST" name="preBidInfos[' + prebidconveniancecount + '].PST" value="PT" readonly>' +
        '</div>' +
        //'<div class="col-2 col-lg-1  col-md-1 col-sm-1 ">' +
        //'<div class="form-check form-check-inline">' +
        //'<input class="form-check-input" type="checkbox" tabindex="' + (tabIndex + 6) + '" onchange="ChangeAnd(this)">' +
        //' <label class="form-check-label" for="and">and</label>' +
        //'<input type="hidden" id="preBidInfos_' + prebidconveniancecount + '__PreBidAnd" name="preBidInfos[' + prebidconveniancecount + '].PreBidAnd">' +
        //' </div>' +
        //' </div>' +
        '<div class="col-2 col-lg-2 col-md-2 col-sm-1">' +
        ' <div class="form-check form-check-inline">' +
        '<input class="form-check-input" tabindex="' + (tabIndex + 6) + '" type="checkbox" onchange="ChangeMandatory(this)">' +
        '<label class="form-check-label" for="">Mandatory</label>' +
        '<input type="hidden" id="preBidInfos_' + prebidconveniancecount + '__Mandatory" name="preBidInfos[' + prebidconveniancecount + '].Mandatory" >' +
        '</div>' +
        '</div>' +
        '<div class="col-2 col-lg-1 col-md-1 col-sm-1">' +

        '<div class=" remove_prebid_row "><span id="delPrebid-row" class="p-0 popClassDel"><i class="fa fa-trash"></i></span></div>' +
        '</div>' +
        '<div class="col-9 col-lg-8 col-md-8 col-sm-9 mt-2">' +
        '<input tabindex="' + (tabIndex + 7) + '" class="form-control" type="text" id="preBidInfos_' + prebidconveniancecount + '__Location" name="preBidInfos[' + prebidconveniancecount + '].Location" placeholder="Location">' +
    '</div>' +
    '<div class="col-lg-2">' +
    '<div class="form-check form-check-inline" >' +
    '<input type="hidden" name="preBidInfos[' + prebidconveniancecount + '].UndecidedPreBid" id="preBidInfos_' + prebidconveniancecount + '__UndecidedPreBid" value="false"/>' +
    '<input class="form-check-input tbdcheck" type="checkbox" id="inlineCheckbox1" value="option1" tabindex="' + (tabIndex + 8) + '" onchange="UndecidedPre(this)">' +
    '<label class="form-check-label font-weight-bold" >T B D</label>' +
    '</div>' +
    '</div >' +
        //'<div class="col-2 col-sm-2 col-lg-1 col-xl-1">' +
        //'<div class="remove_prebid_row pt-2"><span id="delPrebid-row"><i class="fa fa-trash"></i></span></div>' +
        //'</div>' +
        '</div>'
    '</div>'
    '</div>';

    if (preText < 5)
        $("#bidInfo-input").append(preBidRow);
    else {
        preBidRow = '<p class="text-danger error-message" id="TempMessage">You can only have upto 5 pre bid date</p>'
        $("#bidInfo-input").append(preBidRow);
        $('.error-message').delay(1000).fadeOut();
    }
    delSapn();
    ReInitializeDatePicker();
    $('#' + Fid).focus();
    $('#bidInfo-row-add').attr('tabindex', tabIndex + 9);
    //}
    autoCompleteTimer();
    dateValidate();

});

$("body").on("click", "#delPrebid-row", function () {
    if (confirm("Are you sure you want to delete prebid info?")) {
        $(this).closest("#prebid-info-row").find('input.Pre-Del').val('true');
        $(this).parents("#prebid-info-row").hide();
        var rootDiv = $(this).parents("#prebid-info-row").children('div.Pre-Text');
        rootDiv.removeClass('Pre-Text');
        let DivRemove = document.querySelectorAll('[class$="Pre-Text"]');
        var x = 1;
        DivRemove.forEach(function (input) {
            input.textContent = '<label>Meeting ' + x + '</label>';
            input.innerHTML = '<label>Meeting ' + x + '</label>';
            x++
        });
        $(this).parents("#prebid-info-row").removeClass("prebidconveniancecount");
        $(this).parents("#prebid-info-row").addClass("delPreBidCount");

    }
});
function getPreDateIndex(id, e) {
    var BidDate = document.getElementById(id).value;
    var chk = validateDateControl(id)
    if (chk != false) {
        var currentDate = new Date();
        var month = currentDate.getMonth() + 1;
        if (month < 10) month = "0" + month;
        var dateOfMonth = currentDate.getDate();
        if (dateOfMonth < 10) dateOfMonth = "0" + dateOfMonth;
        var year = currentDate.getFullYear();
        var formattedDate = month + "/" + dateOfMonth + "/" + year;

        var checklistvalide = true;
        var dateArr = [];
        var date = '';
        $('div.date-index').find('input[class=chkBidDup]').each(function (index, item) {
            if ($(this).attr('id') == id) {
                date = $(this).val();
            }
            if ($(this).attr('id') != id)
                dateArr.push($(this).val());
        });
        for (var idx = 0; idx < dateArr.length; idx++) {
            if (dateArr[idx] === date) {
                alert('date already exists');
                $('#' + id).val('');
                checklistvalide = false;
            }
        }
        return checklistvalide;
    }
};
$(function () {
    $(".datepicker-custom").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    });
});
$('.datepicker-custom').change(function () {
    $(this).focus();
});
function ReInitializeDatePicker() {
    $(".datepicker-custom").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    });
};
function dateValidate() {
    const dateInputs = document.querySelectorAll('[id$="BidDate"]');

    // Get the current date and extract the year
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();

    // Loop through each input element and apply the functionality
    dateInputs.forEach(function (dateInput) {
        //// Set the placeholder text with the current year
        //dateInput.placeholder = `MM/dd/${currentYear}`;

        // Validate the input when the user changes the value
        dateInput.addEventListener("change", function () {
            const value = dateInput.value;
            var callingElementId = event.target.id;
            validateDateControl(callingElementId);
        });
    });
}
$("#cost-row-add").click(function () {
    var costTextCount = $("div[class*='costconveniancecount']").length;
    var costconveniancecount = $("div[class*='costconveniancecount']").length;
    var delCount = $("div[class*='delCostCount']").length;
    costconveniancecount = costconveniancecount + delCount;
    var tabIndex = parseFloat($(this).attr('tabindex'));
    var Fid = 'EstCostDetails_' + costconveniancecount + '__RangeSign';
    var EFid = '';
    //if (delCount > 0) {

    //    var checkissue = document.querySelectorAll('[class*="delCostCount"]');
    //    var classess = checkissue[0].className;
    //    classess = classess.replace('delCostCount', 'costconveniancecount');
    //    checkissue[0].setAttribute('class', classess);
    //    var styles = checkissue[0].getAttribute('style')
    //    styles = styles.replace('display: none;', '');
    //    checkissue[0].setAttribute('style', styles);
    //    var inputElement = checkissue[0].getElementsByTagName('input');
    //    for (var i = 0; i < inputElement.length; i++) {
    //        var element = inputElement[i];
    //        var PST = element.getAttribute("id");
    //        var elementType = element.getAttribute("type");
    //        if (elementType == 'text') {
    //            element.value = '';
    //            if (PST.endsWith('__EstFrom'))
    //                EFid = PST;
    //        }
    //        else if (elementType == 'hidden' && !PST.endsWith('_Removed')) { element.value = 'false'; }
    //        // Check if the element has a type attribute
    //    }
    //    $('#' + EFid).focus();
    //}
    //else {
    newCostRow =
        '<div class="row align-items-center pb-2 costconveniancecount" id="remove-cost">' +
        '<div class="col-12 col-sm-12 col-md-12 col-lg-12 Cost-Text"><label>Estimated Cost ' + (costTextCount + 1) + '</label></div>' +
        '<div class="col-md-2 col-lg-2 pr-0" style="flex:0 0 9%;width:fit-content;">' +
    '<select class="form-control range-sign p-0" tabindex="' + (tabIndex + 1) + '"id="EstCostDetails_' + costconveniancecount + '__RangeSign" name="EstCostDetails[' + costconveniancecount + '].RangeSign" data-index="' + costconveniancecount + '" ><option selected value="0"> </option><option value="1">&lt;</option><option value="2">&gt;</option></select>' +
        '</div > ' +
        '<div class="col-md-5 col-lg-3 text-nowrap ">' +
        '<input type="hidden" id="EstCostDetails_' + costconveniancecount + '__Removed" name="EstCostDetails[' + costconveniancecount + '].Removed" class="Del-CostClass">' +
        '<input type="text" tabindex="' + (tabIndex + 2) + '" class="form-control pr-0 inpRem" id="EstCostDetails_' + costconveniancecount + '__EstFrom" name="EstCostDetails[' + costconveniancecount + '].EstFrom" placeholder="Cost" onkeyup="formatNumberInput(this)"/>' +
        '<input type="hidden" id="EstCostDetails_' + costconveniancecount + '__Id" name="EstCostDetails[' + costconveniancecount + '].Id">' +
        '</div>' +
        '<span class="">TO</span>' +
        '<div class="col-md-5 col-lg-3">' +
        '<input type="text" class="form-control inpRem" tabindex="' + (tabIndex + 3) + '" id="EstCostDetails_' + costconveniancecount + '__EstTo" name="EstCostDetails[' + costconveniancecount + '].EstTo" placeholder="Cost" onkeyup="formatNumberInput(this)"/>' +
        '</div>' +
        '<div class="col-md-5 col-lg-3">' +
        '<input type="text" class="form-control" tabindex="' + (tabIndex + 4) + '" id="EstCostDetails_' + costconveniancecount + '__Description" name="EstCostDetails[' + costconveniancecount + '].Description" placeholder="Description" />' +
        '</div>' +
        '<div class="col-2 col-md-2 col-lg-1 added-icon">' +
        '<span class="action-del popClassDel" id="del-costRow" tabindex="' + (tabIndex + 5) + '"><i class="fa fa-trash" aria-hidden="true"></i></span>' +
        '</div>' +
        '</div>';

    if (costTextCount < 5) {

        $("#costInput").append(newCostRow);
    }
    else {
        costDateRow = '<p class="text-danger error-msg" id="TempMessage">You can only have upto 5 Estimated cost</p>'
        $("#costInput").append(costDateRow);
        $('.error-msg').delay(1000).fadeOut();
    }
    delSapn();
    $(this).attr('tabindex', tabIndex + 5);
    $('#' + Fid).focus();
    //}
});
$(document).on('change', '.range-sign', function () {
    var index = $(this).data('index');
    var rangeid = 'EstCostDetails_' + index + '__EstTo';
    var estToInput = $('#' + rangeid); // Assuming 'inpRem' class is used for EstTo inputs
    //var estToInput = $('.inpRem').eq(index); // Assuming 'inpRem' class is used for EstTo inputs
    var disable = ($(this).val() == '1' || $(this).val() == '2');
    // Enable or disable based on the selected value
    estToInput.val('');
    estToInput.prop('disabled', disable);
});
$('.range-sign').each(handleRangeSelectChange);
//$(document).on('change', '.range-sign', handleRangeSelectChange);
// Function to handle the change event
function handleRangeSelectChange() {
    var selectedIndex = $(this).val();
    var estToField = $(this).closest('.costconveniancecount').find('.toinpRem');

    if (selectedIndex == "1" || selectedIndex == "2") {
        estToField.prop('disabled', true);
    } else {
        estToField.prop('disabled', false);
    }
}
// script to delete cost 2 cost row
$("body").on("click", "#del-costRow", function () {
    if (confirm("Are you sure you want to delete estimated cost info?")) {
        $(this).parents("#remove-cost").hide();
        var rootDiv = $(this).parents("#remove-cost").children('div.Cost-Text');
        rootDiv.removeClass('Cost-Text');
        let DivRemove = document.querySelectorAll('div[class$="Cost-Text"]');
        var x = 1;
        DivRemove.forEach(function (input) {
            input.textContent = 'Estimated Cost ' + x;
            input.innerHTML = '<label>Estimated Cost ' + x + '</label>';
            x++
        });
        $(this).parents("#remove-cost").removeClass("costconveniancecount");
        $(this).parents("#remove-cost").addClass("delCostCount");
        $(this).closest("#remove-cost").find('input.Del-CostClass').val('true');
    }
});
function formatNumberInput(input) {
    // Remove non-numeric characters from the input value
    let numericValue = input.value.replace(/\D/g, '');

    // Format the numeric value with commas
    let formattedValue = numericValue.replace(/\B(?=(\d{3})+(?!\d))/g, ',');

    // Add a dollar sign prefix only if the input is not empty
    formattedValue = numericValue.length === 0 ? '' : '$' + formattedValue;

    // Set the formatted value back to the input
    input.value = formattedValue;
};
var EntSuggestion = new Array();
function InitializeEntName() {
    var term = '';
    var ctrl = '';
    $('[id*=EntityName]').autocomplete({
        source: function (request, response) {
            term = request.term;
            var Entid = $(this.element).prop("id");
            var index = Entid.replace('Entities_', '');
            index = index.replace('__EntityName', '');
            index = parseInt(index);
            $.ajax({
                url: '/Project/GetEntityName/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }));
                    EntSuggestion = data;
                    const input = $('[id*=EntityName]')[index];
                    var nameId = Entid.replace('__EntityName', '__NameId');
                    if (EntSuggestion.length > 0) {
                        const selectedValue = EntSuggestion[0].label; // Choose the first suggestion
                        $('#' + nameId).val(EntSuggestion[0].val);
                        const startIndex = request.term.length;
                        const endIndex = selectedValue.length;
                        $(input).data('autocomplete-fill', true);
                        // Set the cursor position to overwrite mode
                        $(input).val(selectedValue);

                        input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        $(input).data('autocomplete-fill', false);
                        $('#' + nameId).val('');
                    }


                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            event.preventDefault();


            // Replace the input value with the selected value

            // Place the cursor in overwrite mode after the original term

            $(this).next('input').val(i.item.val);

            //$("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        change: function (e, i) {
            var isAutocompleteFill = $(this).data('autocomplete-fill');
            if (isAutocompleteFill != true && isAutocompleteFill != "true" && isAutocompleteFill != "True") {
                var chkVal = $(this).val();
                var chkId = $(this).attr('id');
                var chkReg = $(this).next('input').val();
                if (chkReg == '') {
                    if (confirm("The company you entered does not exist. Do you want to add new company?")) { openMemModal(chkId, chkVal); }
                    else { $(this).val(''); }
                }
            }

        },
        minLength: 1,

    }).focus(function () {
        $(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            ctrl.setSelectionRange(term.length - 1, $(this).val().length);
        }
    });

}
var EntTypeSuggestion = new Array();
function InitializeEntType() {
    var term = '';
    var ctrl = '';
    $('[id*=EntityTypeString]').autocomplete({
        source: function (request, response) {
            term = request.term;
            var Entid = $(this.element).prop("id");
            var index = Entid.replace('Entities_', '');
            index = index.replace('__EntityTypeString', '');
            index = parseInt(index);
            $.ajax({
                url: '/Project/GetEntityType/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }));
                    EntTypeSuggestion = data;
                    const input = $('[id*=EntityTypeString]')[index];
                    ctrl = input;
                    var nameId = Entid.replace('__EntityTypeString', '__EntityType');
                    if (EntTypeSuggestion.length > 0) {
                        const selectedValue = EntTypeSuggestion[0].label; // Choose the first suggestion
                        $('#' + nameId).val(EntTypeSuggestion[0].val);
                        const startIndex = request.term.length;
                        const endIndex = selectedValue.length;
                        $(input).data('autocomplete-fill', true);
                        // Set the cursor position to overwrite mode
                        $(input).val(selectedValue);
                        input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        $(input).data('autocomplete-fill', false);
                        $('#' + nameId).val('');
                    }
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            var nameId = Entid.replace('__EntityTypeString', '__EntityType');
            $('#' + nameId).val(i.item.val);
        },
        change: function (e, i) {
            var isAutocompleteFill = $(this).data('autocomplete-fill');
            if (isAutocompleteFill != true && isAutocompleteFill != "true" && isAutocompleteFill != "True") {
                var chkVal = $(this).val();
                var chkId = $(this).attr('id');
                var chkReg = $(this).next('input').val();
                if (chkReg == '') {
                    if (confirm("The entity type you entered does not exist. Do you want to add new entity type?")) { openEntModal(chkId, chkVal); }
                    else { $(this).val(''); }
                }
            }

        },
        minLength: 1
    }).focus(function () {
        $(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            //         const selObj = window.getSelection();
            ctrl.setSelectionRange(term.length - 1, $(this).val().length);
            //                    alert(selObj);
            //             console.log(selObj);
        }
    });
};

$("#project-row-adder").click(function () {
    i = $("input[class*='EntityNameClass']").length;
    var tabIndex = parseFloat($('#project-row-adder').attr('tabindex'));
    var Fid = 'Entities_' + i + '__EntityTypeString';
    newRowAdd =
        '<div id="project-team-row" class="row align-items-baseline mb-3">' +
        '<div class="col-12  col-md-5 col-lg-5">' +
        '<input type="hidden" id="Entities_' + i + '__EntityID" type="text" value="" name="Entities[' + i + '].EntityID" />' +
        '<input type="hidden" id="Entities_' + i + '__IsActive" name="Entities[' + i + '].IsActive" />' +
        '<input autocomplete="off" tabindex="' + (tabIndex + 1) + '" class="form-control" id="Entities_' + i + '__EntityTypeString" type="text" value="" name="Entities[' + i + '].EntityTypeString" placeholder="Entity Type"/>' +
        '<input type="hidden" id="Entities_' + i + '__EntityType" name="Entities[' + i + '].EntityType" />' +
        '</div>' +
        '<div class="col-12  col-md-5 col-lg-5">' +
        '<input type="text" tabindex="' + (tabIndex + 2) + '" class="form-control  EntityNameClass" id="Entities_' + i + '__EntityName" name="Entities[' + i + '].EntityName" placeholder="Entity Name" />' +
        '<input type="hidden" id="Entities_' + i + '__NameId" name="Entities[' + i + '].NameId" />' +
        '</div>' +
        '<div class="col-1 col-lg-1 col-xl-1 pr-0">' +
        '<input type="hidden" tabindex="' + (tabIndex + 3) + '" id="Entities_' + i + '__chkIssue" name="Entities[' + i + '].chkIssue" class="chk-Issue" />' +
        '<input type="checkbox" class="mx-1 align-middle" tabindex="' + (tabIndex + 3) + '" value="" id="chkBoxIssue" onchange="AssignIssue(this)" />' +
        '</div>' +
        '<div class="col-1 col-lg-1 col-xl-1">' +
        '<div class="remove_prebid_row pb-1"><span tabindex="' + (tabIndex + 4) + '" class="action-del popClassDel" id="DeleteRow"><i class="fa fa-trash"></i></span></div>' +
        '</div>' +
        '</div>';

    $('#newinput').append(newRowAdd);
    InitializeEntName();
    InitializeEntType();
    delSapn();
    $('#project-row-adder').attr('tabindex', tabIndex + 5);
    $('#' + Fid).focus();

});
// deleting the row on del-icon click
$("body").on("click", "#DeleteRow", function () {
    if ($(this).closest("#project-team-row").find('input.chk-Issue').val() == 'True') {
        if (confirm("Are you sure you want to delete project team information? It will also delete the issuing office.")) {
            $(this).closest("#project-team-row").find('input.Del-Ent').val('false');
            $(this).parents("#project-team-row").hide();
            $('#IssuingOffice').val('');
        }
    }
    else {
        if (confirm("Are you sure you want to delete project team information?")) {
            $(this).closest("#project-team-row").find('input.Del-Ent').val('false');
            $(this).parents("#project-team-row").hide();
        }
    }
});

function delSapn() {
    const delEntModel = document.getElementsByClassName('popClassDel');
    for (let i = 0; i < delEntModel.length; i++) {
        delEntModel[i].addEventListener('keydown', function (event) {
            // Check if the Enter key was pressed (key code 13)
            if (event.keyCode === 13 || event.which === 13) {
                // Perform the desired action when Enter is pressed
                // For example, you can trigger a click event
                this.click();
                event.preventDefault();
            }
        });
    }
};

$("#planHolder-add").click((e) => {
    e.preventDefault();
    var ddlHtml = '';
    var BidHtml = ''
    var planPHLecount = $("div[class*='planPHLecount']").length;
    var tbCount = parseFloat(planPHLecount - 1);
    var tabIndex = parseFloat($(".count-class-" + tbCount).attr('tabindex'));
    var Fid = 'phlInfo_' + planPHLecount + '__PHLType';
    $.ajax({
        type: "GET",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/GetPHLType/',
        async: false,
        success: function (response) {
            ddlHtml += '<select tabindex="' + (tabIndex + 1) + '" class="form-control p-0" name="phlInfo[' + planPHLecount + '].PHLType" id="phlInfo_' + planPHLecount + '__PHLType">';
            var selectElement = $('<select>'); // Create a new <select> element

            // Loop through the values and create <option> elements
            $.each(response, function (index, value) {
                ddlHtml += '<option value="' + value.Value + '">' + value.Text + '</option>'
            });
            ddlHtml += '</select>';
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
    $.ajax({
        type: "GET",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/GetBidOption/',
        async: false,
        success: function (response) {
            BidHtml += '<select tabindex="' + (tabIndex + 12) + '" class="form-control p-0" name="phlInfo[' + planPHLecount + '].BidStatus" id="phlInfo_' + planPHLecount + '__BidStatus">';
            BidHtml += '<option value="0">--No Selection--</option>'
            // Loop through the values and create <option> elements
            $.each(response, function (index, value) {
                BidHtml += '<option value="' + value.Value + '">' + value.Text + '</option>'
            });
            BidHtml += '</select>';
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
    planholderRow =
        '<div class="box-space planPHLecount p-1" id="PHL-row">' +
        '<div class="border p-2">' +
        '<div class="row align-items-center">' +
        '<div class="col-lg-8 d-flex flex-wrap">' +
        '<div class="col-lg-3 mb-2">' +
        '<input type="hidden" id="phlInfo_' + planPHLecount + '__IsActive" name="phlInfo[' + planPHLecount + '].IsActive" class="Dec-Class" />' +
        ddlHtml +
        '</div>' +
        '<div class="col-lg-3 mb-2">' +
        '<input class="form-control" id="phlInfo_' + planPHLecount + '__Company" name="phlInfo[' + planPHLecount + '].Company" type="text" tabindex="' + (tabIndex + 2) + '" placeholder="Contractor">' +
        '<input type="hidden" id="phlInfo_' + planPHLecount + '__MemId" name="phlInfo[' + planPHLecount + '].MemId">' +
        '</div>' +
        '<div class="col-lg-3 mb-2">' +
        '<input class="form-control  valContact" id="phlInfo_' + planPHLecount + '__StrContact" name="phlInfo[' + planPHLecount + '].StrContact" type="text" tabindex="' + (tabIndex + 3) + '" placeholder="Contact">' +
        '<input type="hidden" id="phlInfo_' + planPHLecount + '__ConId" name="phlInfo[' + planPHLecount + '].ConId" />' +
        '</div>' +
        '<div class="col-lg-3 mb-2">' +
        '<input class="form-control" id="phlInfo_' + planPHLecount + '__contactPhone" name="phlInfo[' + planPHLecount + '].contactPhone" type="text" tabindex="' + (tabIndex + 4) + '" placeholder="Phone">' +
        '</div>' +
        '<div class="col-lg-9 mb-2">' +
        '<input class="form-control" type="text" id="phlInfo_' + planPHLecount + '__PHLNote" name="phlInfo[' + planPHLecount + '].PHLNote" placeholder="Note" tabindex="' + (tabIndex + 6) + '">' +
        '</div>' +
        '<div class="col-lg-3 mb-2">' +
        '<input class="form-control" id="phlInfo_' + planPHLecount + '__contactEmail" name="phlInfo[' + planPHLecount + '].contactEmail" type="text" placeholder="Email" tabindex="' + (tabIndex + 5) + '">' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-2" style="border-left: 3px solid #5b7576;">' +
        '<div class="col-12 date-img px-2">' +
        '<div>' +
        '<input class="form-control px-2" type="text" id="phlInfo_' + planPHLecount + '__BidDate" name="phlInfo[' + planPHLecount + '].BidDate" placeholder="Select Date" tabindex="' + (tabIndex + 8) + '" />' +
        '<div class="d-flex">' +
        '<div id="time_input" class="my-1">' +
        '<label for="hours">' +
        '<input type="number" style="width:45px" id="phlInfo_' + planPHLecount + '__tComp" name="phlInfo[' + planPHLecount + '].tComp" tabindex="' + (tabIndex + 9) + '" value="00">' +
        '</label>' +
        '<span>:</span>' +
        '<label for="minutes">' +
        '<input type="number" style="width:45px" id="phlInfo_' + planPHLecount + '__hComp" name="phlInfo[' + planPHLecount + '].hComp" tabindex="' + (tabIndex + 10) + '" value="00">' +
        '</label>' +
        '<span>:</span>' +
        '<label for="seconds">' +
        '<input type="Text" style="width:45px" id="phlInfo_' + planPHLecount + '__mValue" name="phlInfo[' + planPHLecount + '].mValue" tabindex="' + (tabIndex + 11) + '" value="AM">' +
        '</label>' +
        '</div>' +
        '<input class="form-control text-center my-1 ml-1" type="text" id="phlInfo_' + planPHLecount + '__PST" name="phlInfo[' + planPHLecount + '].PST" value="PT" readonly>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-2 col-lg-2 d-flex align-items-center" style="border-left: 3px solid #5b7576;min-height: 65px;">' +
        BidHtml +
        '<span id="remove-PHL" class="action-del popClassDel col-2 count-class-' + planPHLecount + '" tabindex="' + (tabIndex + 13) + '"><i class="fa fa-trash" aria-hidden="true"></i></span>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>';

    $("#planholder-input").append(planholderRow);
    delSapn();
    InitializePhlName();
    InitializePhlCon();
    ReInitializeDatePicker();
    autoCompleteTimer();
    //$('#' + Fid).focus();
    $('#' + Fid)[0].focus();
    $("#planHolder-add").attr('tabindex', tabIndex + 7);
    dateValidate();

});

$("body").on("click", "#remove-PHL", function () {
    if (confirm("Are you sure you want to delete plan holder list?")) {
        {
            $(this).closest("#PHL-row").find('input.Dec-Class').val('false');
            $(this).parents("#PHL-row").hide();

        }
    }
})

var PhlSuggestion = new Array();
function InitializePhlName() {
    var term = '';
    var ctrl = '';
    $('input[id$="__Company"]').autocomplete({
        source: function (request, response) {
            console.log(request.term);
            term = request.term;
            var Id = $(this.element).prop("id");
            var index = Id.replace('phlInfo_', '');
            index = index.replace('__Company', '');
            index = parseInt(index);
            $.ajax({
                url: '/Project/GetCompanyName/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }));
                    PhlSuggestion = data;
                    const input = $('[id*=__Company]')[index];
                    ctrl = input;
                    if (PhlSuggestion.length > 0) {
                        //$('#' + Id).val(data[0].label);
                        var CompId = Id.replace("phlInfo_", "");
                        CompId = CompId.replace("__Company", "");
                        var hdnComp = "phlInfo_" + CompId + "__MemId";
                        CompId = "phlInfo_" + CompId + "__Uid";
                        var selectedValue = PhlSuggestion[0].val;
                        var arrVal = selectedValue.split(':');
                        var idVal = arrVal[0];
                        var UidVal = arrVal[1];
                        $('#' + hdnComp).val(idVal);
                        $('#' + CompId).val(UidVal);
                        const selectedText = PhlSuggestion[0].label; // Choose the first suggestion
                        const startIndex = request.term.length;
                        const endIndex = selectedText.length;
                        //$(input).data('autocomplete-fill', true);
                        //$(input).data('id-fill', data[0].val);
                        // Set the cursor position to overwrite mode
                        $(input).val(selectedText);
                        input.setSelectionRange(startIndex, endIndex);

                    }
                    else {
                        Id = Id.replace("phlInfo_", "");
                        Id = Id.replace("__Company", "");
                        var hdnComp = "phlInfo_" + Id + "__MemId";
                        // Find the hidden field relative to the selected text input
                        $('#' + hdnComp).val('');
                        $('#' + Id).val('');
                    }

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (event, ui) {
            // Get the selected value from the autocomplete
            var Id = $(this).attr('id');
            Id = Id.replace("phlInfo_", "");
            Id = Id.replace("__Company", "");
            Id = "phlInfo_" + Id + "__Uid";
            var selectedValue = ui.item.val;
            var arrVal = selectedValue.split(':');
            var idVal = arrVal[0];
            var UidVal = arrVal[1];
            // Find the hidden field relative to the selected text input
            var hiddenField = $(this).next('input[type=hidden]');
            // Assign the selected value to the hidden field
            hiddenField.val(idVal);
            $('#' + Id).val(UidVal);
        },
        change: function (e, i) {
            e.preventDefault();
            var chkReg = $(this).next('input').val();
            var chkId = $(this).attr('id');
            var chkVal = $(this).val();
            if (chkReg == '') {
                if (confirm("The contractor you entered does not exist. Do you want to add new contractor?")) { openMemModal1(chkId, chkVal); }
                else { $(this).val(''); }
            }
            //$("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        blur: function (e) { e.preventDefault(); },
        minLength: 1
    }).focus(function () {
        $(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            //         const selObj = window.getSelection();
            ctrl.setSelectionRange(term.length - 1, $(this).val().length)
            //                    alert(selObj);
            //             console.log(selObj);
        }
    });
    //.on("keyup", function (e) {
    //    if (e.keyCode === 16) { // Check for the Shift key (key code 16)
    //        e.preventDefault();
    //    }

    //});


}
var ConSuggestion = new Array();
function InitializePhlCon() {
    var term = '';
    var ctrl = '';
    $('input[class$="valContact"]').autocomplete({
        source: function (request, response) {
            term = request.term;
            var Id = $(this.element).prop("id");
            var index = Id.replace('phlInfo_', '');
            index = index.replace('__Company', '');
            index = parseInt(index);
            var CompId = Id.replace("phlInfo_", "");
            CompId = CompId.replace("__StrContact", "");
            CompId = "phlInfo_" + CompId + "__MemId";
            var ItemId = $('#' + CompId).val();
            $.ajax({
                url: '/Project/GetContactName/',
                data: { "Id": ItemId, "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {

                    response($.map(data, function (item) {
                        return item;
                    }));
                    ConSuggestion = data;
                    var ConId = Id.replace("__StrContact", "__ConId");
                    const input = $('[id*=__StrContact]')[index];
                    ctrl = input;
                    if (ConSuggestion.length > 0) {
                        //$('#' + Id).val(data[0].label);
                        //var selectedValue = ConSuggestion[0].val;
                        ////$('#' + ConId).val(selectedValue);
                        ////const selectedText = ConSuggestion[0].label; // Choose the first suggestion
                        //const startIndex = request.term.length;
                        //const endIndex = selectedText.length;
                        //$(input).data('autocomplete-fill', true);
                        //$(input).data('id-fill', data[0].val);
                        // Set the cursor position to overwrite mode
                        //$(input).val(selectedText);
                        //input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        // Find the hidden field relative to the selected text input
                        $('#' + ConId).val('');
                    }
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (event, ui) {
            // Get the selected value from the autocomplete
            var selectedValue = ui.item.val;
            var Id = $(this).prop("id");
            var index = Id.replace('phlInfo_', '');
            index = index.replace('__StrContact', '');
            index = "phlInfo_" + index + "__contactEmail"
            var CompId = Id.replace("phlInfo_", "");
            CompId = CompId.replace("__StrContact", "");
            CompId = "phlInfo_" + CompId + "__contactPhone";
            // Find the hidden field relative to the selected text input
            var hiddenField = $(this).next('input[type=hidden]');

            // Assign the selected value to the hidden field
            hiddenField.val(selectedValue);
            $.ajax({
                url: '/Project/GetContactDetail/',
                data: { "ConId": ui.item.val },
                type: "POST",
                async: false,
                success: function (data) {
                    console.log(data)
                    $('#' + CompId).val(data.PhoneNum);
                    $('#' + index).val(data.Email);
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        change: function (e, i) {
            var Id = $(this).prop("id");
            var ConId = Id.replace('phlInfo_', '');
            ConId = ConId.replace('__StrContact', '');
            ConId = "phlInfo_" + ConId + "__ConId";
            var conVal = $('#' + ConId).val();
            var index = Id.replace('phlInfo_', '');
            index = index.replace('__StrContact', '');
            index = "phlInfo_" + index + "__contactEmail"
            var CompId = Id.replace("phlInfo_", "");
            CompId = CompId.replace("__StrContact", "");
            CompId = "phlInfo_" + CompId + "__contactPhone";
            $.ajax({
                url: '/Project/GetContactDetail/',
                data: { "ConId": conVal },
                type: "POST",
                async: false,
                success: function (data) {
                    console.log(data)
                    $('#' + CompId).val(data.PhoneNum);
                    $('#' + index).val(data.Email);

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
            //$("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        minLength: 0
    }).focus(function () {
        $(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            //         const selObj = window.getSelection();
            ctrl.setSelectionRange(term.length - 1, $(this).val().length);
            //                    alert(selObj);
            //             console.log(selObj);
        }
    }).blur(function () {
        var chkReg = $(this).next('input').val();
        var chkId = $(this).attr('id');
        var chkName = $(this).val();
        var CompId = chkId.replace("phlInfo_", "");
        CompId = CompId.replace("__StrContact", "");
        CompId = "phlInfo_" + CompId + "__MemId";
        var CompText = chkId.replace("__StrContact", "__Company");
        var CId = $('#' + CompId).val()
        if (CId == '' || CId == undefined || CId == null) {
            //alert('No contractor is selected. Please select a contractor.')
            $('#' + chkId).val('');
            $('#' + CompText).focus();
            return false;
        }
        if (chkReg == '') {
            if (confirm("The contact for contractor you entered does not exist. Do you want to add new contact?")) { openConModel(chkId, chkName, CId); }
            else { $(this).val(''); }
        }
    });
}

function AssignBidding(checkbox) {
    var hiddenInput = checkbox.nextElementSibling; // Get the next sibling element, which is the hidden input
    hiddenInput.value = checkbox.checked ? "true" : "false"; // Assign the value based on the checkbox checked state
};

$(function () {
    $(".datepicker-custom-Ar").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    }).datepicker('setDate', 'today');
});

function BlockCounties(id) {
    var State = '';
    var checkBox = document.getElementById(id);
    if (id == 'AllOr') {
        if (checkBox.checked == true) {
            $(".classMOcounties").each(function () {

                $(this).prop("checked", true);   // Uncheck the checkbox
                //     $(this).prop("disabled", true);   // Disable the checkbox

            });
        }
        else {
            $(".classMOcounties").each(function () {
                $(this).prop("checked", false);   // Uncheck the checkbox
                //     $(this).prop("disabled", true);   // Disable the checkbox

            });
        }
    }
    //State = 'OR';
    else if (id == 'AllWa') {
        if (checkBox.checked == true) {
            $(".classMWcounties").each(function () {
                var checkboxValue = $(this).val();
                // Check if checkbox value is in the disabledValues array
                $(this).prop("checked", true);   // Uncheck the checkbox
                //     $(this).prop("disabled", true);   // Disable the checkbox

            });
        }
        else {
            $(".classMWcounties").each(function () {
                var checkboxValue = $(this).val();
                // Check if checkbox value is in the disabledValues array
                $(this).prop("checked", false);   // Uncheck the checkbox
                //     $(this).prop("disabled", true);   // Disable the checkbox
            });
        }
    }

};

function AssignMultipleCounties() {
    var cHtml = '';
    var cTextHtml = '';
    var WCounty = document.getElementById('AllWa');
    var OCounty = document.getElementById('AllOr');
    if (OCounty.checked) {
        cHtml += OCounty.value + ', ';
        cTextHtml += OCounty.nextElementSibling.innerText + ', ';
    }
    else {
        $(".classMOcounties").each(function () {
            var checked = $(this).prop('checked');
            if (checked) {
                cHtml += $(this).val() + ', ';
                cTextHtml += $(this).next('label').text() + ', ';
            }
        });
    }
    if (WCounty.checked) {
        cHtml += WCounty.value + ', ';
        cTextHtml += WCounty.nextElementSibling.innerText + ', ';
    }
    else {
        $(".classMWcounties").each(function () {
            var checked = $(this).prop('checked');
            if (checked) {
                cHtml += $(this).val() + ', ';
                cTextHtml += $(this).next('label').text() + ', ';
            }
        });
    }

    $(".classMcounties").each(function () {
        var checked = $(this).prop('checked');
        if (checked) {
            cHtml += $(this).val() + ', ';
            cTextHtml += $(this).next('label').text() + ', ';
        }

    });
    cHtml = cHtml.replace(/,\s*$/, '');
    cTextHtml = cTextHtml.replace(/,\s*$/, '');
    if (typeof cHtml !== 'undefined' && cHtml !== null && cHtml.trim() !== "") {
        $('#Counties').val(cHtml);
    }
    if (typeof cTextHtml !== 'undefined' && cTextHtml !== null && cTextHtml.trim() !== "") {
        $('#divsCounty').html(cTextHtml);
        $('#divCounty').css('display', 'block');
        $('#divsCounty').css('display', 'block');
    }
    $(".m-modal").modal('hide');
    //$('div.modal-backdrop').remove();
};

function PopulateModal() {
    var stateHtml = '';
    var ORhtml = '';
    var WAhtml = '';
    var Ohtml = '';
    var counties = $('#Counties').val();
    var myArray = [];
    if (counties) {
        myArray = counties.split(", ");
    }
    //  $('.classCounty').html('')
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Project/GetCounties/',
        async: false,
        success: function (response) {
            var model = new Array();
            var OModel = new Array();
            var WModel = new Array();
            var idx = 0;
            model = response.ORData;
            OModel = response.OData;
            WModel = response.WAData;
            if (model.length > 0) {

                var AllOr = $.inArray('-1', myArray);
                if (AllOr !== -1) {
                    $.each(model, function (index, item) {
                        ORhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                            + '<input type="checkbox" class="filled-in form-check-input classMOcounties" value="' + item.CountyId + '" checked onchange="OrCheck(this.checked)">'
                            + '<label class="form-check-label">' + item.County + '</label></div>'
                    });
                }
                else {
                    $.each(model, function (index, item) {
                        idx = 0;
                        if (myArray.length > 0) {
                            for (i = 0; i < myArray.length; i++) {
                                if (item.CountyId == myArray[i]) {
                                    idx++;
                                }
                            }
                            if (idx > 0) {
                                ORhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                                    + '<input type="checkbox" class="filled-in form-check-input classMOcounties" value="' + item.CountyId + '" checked onchange="OrCheck(this.checked)">'
                                    + '<label class="form-check-label">' + item.County + '</label></div>'
                            }
                            else {
                                ORhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                                    + '<input type="checkbox" class="filled-in form-check-input classMOcounties" value="' + item.CountyId + '" onchange="OrCheck(this.checked)">'
                                    + '<label class="form-check-label">' + item.County + '</label></div>';
                            }
                        }
                        else {
                            ORhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                                + '<input type="checkbox" class="filled-in form-check-input classMOcounties" value="' + item.CountyId + '" onchange="OrCheck(this.checked)">'
                                + '<label class="form-check-label">' + item.County + '</label></div>';
                        }

                    });
                }
            }
            if (WModel.length > 0) {
                var AllWa = $.inArray('-2', myArray);
                if (AllWa !== -1) {
                    $.each(WModel, function (index, item) {
                        WAhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                            + '<input type="checkbox" class="filled-in form-check-input classMWcounties" value="' + item.CountyId + '" checked onchange="WaCheck(this.checked)">'
                            + '<label class="form-check-label">' + item.County + '</label></div>'
                    });
                }
                else {
                    $.each(WModel, function (index, item) {
                        idx = 0;
                        if (myArray.length > 0) {
                            for (i = 0; i < myArray.length; i++) {
                                if (item.CountyId == myArray[i]) {
                                    idx++;
                                }
                            }
                            if (idx > 0) {
                                WAhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                                    + '<input type="checkbox" class="filled-in form-check-input classMWcounties" value="' + item.CountyId + '" checked onchange="WaCheck(this.checked)">'
                                    + '<label class="form-check-label">' + item.County + '</label></div>'
                            }
                            else {
                                WAhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                                    + '<input type="checkbox" class="filled-in form-check-input classMWcounties" value="' + item.CountyId + '" onchange="WaCheck(this.checked)">'
                                    + '<label class="form-check-label">' + item.County + '</label></div>';
                            }
                        }
                        else {
                            WAhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                                + '<input type="checkbox" class="filled-in form-check-input classMWcounties" value="' + item.CountyId + '" onchange="WaCheck(this.checked)">'
                                + '<label class="form-check-label">' + item.County + '</label></div>';
                        }

                    });
                }
            }
            if (OModel.length > 0) {
                $.each(OModel, function (index, item) {
                    idx = 0;
                    if (myArray.length > 0) {
                        for (i = 0; i < myArray.length; i++) {
                            if (item.CountyId == myArray[i]) {
                                idx++;
                            }
                        }
                        if (idx > 0) {
                            Ohtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                                + '<input type="checkbox" class="filled-in form-check-input classMcounties" value="' + item.CountyId + '" checked>'
                                + '<label class="form-check-label">' + item.County + '</label></div>'
                        }
                        else {
                            Ohtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                                + '<input type="checkbox" class="filled-in form-check-input classMcounties" value="' + item.CountyId + '">'
                                + '<label class="form-check-label">' + item.County + '</label></div>';
                        }
                    }
                    else {
                        Ohtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                            + '<input type="checkbox" class="filled-in form-check-input classMcounties" value="' + item.CountyId + '">'
                            + '<label class="form-check-label">' + item.County + '</label></div>';
                    }

                });
            }
            if (Ohtml != '') {
                Ohtml = '<div class="col-md-12 m-2 form-check form-check-inline d-flex justify-content-center" style="background-color:#E5E5E5;">' +
                    '<div class="d-flex justify-content-center">' +
                    '<div class="form-check px-4">' +
                    '<label class="form-check-label text-primary">' +
                    '<b>Others</b>' +
                    '</label>' +
                    '</div>' +
                    '</div>' +
                    '</div>' + Ohtml;
            };
            if (ORhtml != '') {
                ORhtml = '<div class="col-md-12 m-2 form-check form-check-inline d-flex justify-content-center" style="background-color:#E5E5E5;">' +
                    '<div class="d-flex justify-content-center">' +
                    '<div class="form-check px-4">' +
                    '<label class="form-check-label text-primary">' +
                    '<b>OR</b>' +
                    '</label>' +
                    '</div>' +
                    '</div>' +
                    '</div>' + ORhtml;
            };
            if (WAhtml != '') {
                WAhtml = '<div class="col-md-12 m-2 form-check form-check-inline d-flex justify-content-center" style="background-color:#E5E5E5;">' +
                    '<div class="d-flex justify-content-center">' +
                    '<div class="form-check px-4">' +
                    '<label class="form-check-label text-primary">' +
                    '<b>WA</b>' +
                    '</label>' +
                    '</div>' +
                    '</div>' +
                    '</div>' + WAhtml;
            };
            stateHtml = ORhtml + WAhtml + Ohtml;
            $('.classCounty').html(stateHtml);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
};

function WaCheck(checkbox) {
    var County = document.getElementById('AllWa');

    if (!checkbox) {
        if (County.checked)
            County.checked = false;
    }
    else {
        var allChecked = true;
        var checkboxes = document.querySelectorAll('.classMWcounties');
        for (var i = 0; i < checkboxes.length; i++) {
            if (!checkboxes[i].checked) {
                allChecked = false;
                break;
            }
        }
        if (allChecked)
            County.checked = true;
    }
}
function OrCheck(checkbox) {
    var County = document.getElementById('AllOr');
    if (!checkbox) {
        if (County.checked)
            County.checked = false;
    }
    else {
        var allChecked = true;
        var checkboxes = document.querySelectorAll('.classMOcounties');
        for (var i = 0; i < checkboxes.length; i++) {
            if (!checkboxes[i].checked) {
                allChecked = false;
                break;
            }
        }
        if (allChecked)
            County.checked = true;
    }
};

function AssignIssue(ctrl) {
    if ($(ctrl).is(':checked')) {
        var entityName = $(ctrl).closest('.row').find('input[name*="EntityName"]').val();
        //$('#Entities_1__chkIssue').val(true);
        var checkissue = document.querySelectorAll('[id*="chkIssue"]');
        checkissue.forEach(function (checkbox) {
            // if the checkbox is not the one that was just checked, uncheck it
            checkbox.value = 'False';

        });
        $(ctrl).closest('.row').find('input[type="hidden"][name*="chkIssue"]').val('True');
        $('#IssuingOffice').val(entityName);
        // get all the checkboxes on the page
        var checkboxes = document.querySelectorAll('[id*="chkBoxIssue"]');

        // loop through each checkbox
        checkboxes.forEach(function (checkbox) {
            // if the checkbox is not the one that was just checked, uncheck it
            if (checkbox !== event.target) {
                checkbox.checked = false;
            }
        });
    }
    else {
        $(ctrl).closest('.row').find('input[type="hidden"][name*="chkIssue"]').val('False');
        $('#IssuingOffice').val(entityName);
    }
};