/// <reference path="../lib/jquery/dist/jquery.js" />
var jq = $.noConflict();
var SelectedDelOption = "";

var currentDate = new Date();

// Get the current time components
var hours = currentDate.getHours();
var minutes = currentDate.getMinutes();
var seconds = currentDate.getSeconds();

// Convert single-digit hours, minutes, and seconds to double digits
hours = (hours < 10) ? '0' + hours : String(hours);
minutes = (minutes < 10) ? '0' + minutes : String(minutes);
seconds = (seconds < 10) ? '0' + seconds : String(seconds);

// Concatenate hours, minutes, and seconds into a single string
var currentTimeString = hours + minutes + seconds;
jQuery(document).ready(function () {
    loadDashboardInfo();
    jQuery('input[name = inlineRadioOptions]').on('change', function () {
        if (jQuery(this).val() == "option1") {
            jQuery('#fileUpload1').removeAttr('disabled');
            jQuery('#btnfileUpload1').removeAttr('disabled');
        }
        else {
            jQuery('#fileUpload1').attr('disabled', 'disabled');
            jQuery('#btnfileUpload1').attr('disabled', 'disabled');
        }
    });
    var currentUrl = window.location.href;
    var updatedUrl = currentUrl.split('?')[0];
    window.history.replaceState({}, document.title, updatedUrl);
    var loadChk = jQuery('#LoadChk').val();
    if (loadChk != 'NoValue') {
        if (loadChk == 'success') {
            jq(".modalthanks").modal('show');
            jQuery('#uploadCheck').val('N');
            jQuery("#hdnFileName").val('');
            jQuery('#LoadChk').val('NoValue');
        }
        else {
            alert(loadChk);
        }
    }
});

function OpenPrintModel() {
    jq('#ftbody').html('');
    jq('.divUpload').addClass("d-none");
    jQuery("#prev-btn").click();
    jQuery("#frmPrintForm").trigger('reset');
    jQuery('#frmPrintForm').find("input[type=text], textarea").val('');
    jQuery('#inlineRadio1').prop('checked', true);
    jQuery('#fileUpload1').removeAttr('disabled');
    jQuery('#btnfileUpload1').removeAttr('disabled');
    jQuery('span.loginError').html('');
    var dragFile = document.querySelectorAll('.drag-file');
    for (i = 0; i < dragFile.length; i++) {
        if (i == 0) {
            var classes = dragFile[i].getAttribute('style');
            if (classes != null) {
                dragFile[i].removeAttribute('style');
            }
        }
        else {
            dragFile[i].remove();
        }
    }
    jQuery('span.loginError').html('');
    jQuery('#file_input').html('');
    jq(".m-modal").modal('show');
    jQuery("#mValue").val('AM');
}

function loadDashboardInfo() {
    jQuery.ajax({
        type: 'POST',
        url: "/Home/GetDashboardInfo",
        data: {},
        async: true,
        success: function (response) {
            console.log(response);
        }
    });
}

var phone = document.getElementById('inpPhone');
var result = document.getElementById('inpPhone');

phone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
jQuery(document).ready(function () {
    //var zipCode = $(this).val();
    jQuery('#inpZip').keyup(function () {
        var zip = jQuery(this).val();
        jQuery('#inpCity').val('');
        jQuery('#inpState').val('');
        var xmldata = '<CityStateLookupRequest USERID="405PLANC0113"><ZipCode><Zip5>' + zip + '</Zip5></ZipCode></CityStateLookupRequest>';
        //var apiKey = 'YOUR_API_KEY_HERE';
        var apiUrl = 'https://production.shippingapis.com/ShippingAPI.dll?API=CityStateLookup&XML=' + xmldata;
        if (zip.length == 5) {
            jQuery.ajax({
                url: apiUrl,
                type: 'GET',
                dataType: 'xml',
                success: function (data) {
                    console.log(data);
                    jQuery(data).find('ZipCode').each(function () {
                        var City = jQuery(this).find('City').text();
                        var State = jQuery(this).find('State').text();
                        if (City != null && State != null) {
                            //console.log('City : ' + City + ' State : ' + State);
                            jQuery.ajax({
                                type: "POST",
                                dataType: 'json',
                                url: '/MemberShip/CheckState/',
                                data: { 'State': State },
                                async: false,
                                success: function (response) {
                                    console.log(response);
                                    var stateId = response.data[0].StateId;
                                    var formattedCity = upperCaseFirstLetter(lowerCaseAllWordsExceptFirstLetters(City));
                                    jQuery('#inpCity').val(formattedCity);
                                    jQuery('#inpState').val(stateId);
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
                            jQuery('#inpCity').val('');
                            jQuery('#inpState').val('');
                            //jq('#LocAddr2').val('');
                            //jq('#CountyId').val('');
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //console.log(textStatus + ': ' + errorThrown);
                }
            });
        }
    });
});
//Lowering city
const upperCaseFirstLetter = string =>
    `${string.slice(0, 1).toUpperCase()}${string.slice(1)}`;

const lowerCaseAllWordsExceptFirstLetters = string =>
    string.replaceAll(/\S*/g, word =>
        `${word.slice(0, 1)}${word.slice(1).toLowerCase()}`
    );
jQuery(document).ready(function () {
    //var zipCode = $(this).val();
    jQuery('#inpDelZip').keyup(function () {
        var zip = jQuery(this).val();
        jQuery('#inpDelCity').val('');
        jQuery('#inpDelState').val('');
        var xmldata = '<CityStateLookupRequest USERID="405PLANC0113"><ZipCode><Zip5>' + zip + '</Zip5></ZipCode></CityStateLookupRequest>';
        //var apiKey = 'YOUR_API_KEY_HERE';
        var apiUrl = 'https://production.shippingapis.com/ShippingAPI.dll?API=CityStateLookup&XML=' + xmldata;
        if (zip.length == 5) {
            jQuery.ajax({
                url: apiUrl,
                type: 'GET',
                dataType: 'xml',
                success: function (data) {
                    console.log(data);
                    jQuery(data).find('ZipCode').each(function () {
                        var City = jQuery(this).find('City').text();
                        var State = jQuery(this).find('State').text();
                        if (City != null && State != null) {
                            //console.log('City : ' + City + ' State : ' + State);
                            jQuery.ajax({
                                type: "POST",
                                dataType: 'json',
                                url: '/MemberShip/CheckState/',
                                data: { 'State': State },
                                async: false,
                                success: function (response) {
                                    console.log(response);
                                    var stateId = response.data[0].StateId;
                                    var formattedCity = upperCaseFirstLetter(lowerCaseAllWordsExceptFirstLetters(City));
                                    jQuery('#inpDelCity').val(formattedCity);
                                    jQuery('#inpDelState').val(stateId);
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
                            jQuery('#inpDelCity').val('');
                            jQuery('#inpDelState').val('');
                            //jq('#LocAddr2').val('');
                            //jq('#CountyId').val('');
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //console.log(textStatus + ': ' + errorThrown);
                }
            });
        }
    });
});
function removeFile(tempid, filename) {
    var fileElement = jQuery('[tempid="' + tempid + '"] [class*="file__value--text"]').filter(function () {
        return jQuery(this).text() === filename;
    }).closest('.file__value');
    fileElement.click();
}
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
    jq(ctrl).parents('div.drag-file').css('display', 'none');
    uploadHtml = '<div class="drag-file" >' +
        '<div class="file__input" id ="file__input" style = "height: 100px; border: 3px dotted;" >' +
        '<input class="file__input--file" id="fileUpload' + delCount + '" tabindex="9" name="pdfFile" type="file" multiple="multiple" onchange="selFiles(this,event)">' +
        '<label class="file__input--label" for="fileUpload' + delCount + '" data-text-btn="Upload"> Drag and Drop a file: </label>' +
        '</div></div>';
    jq(ctrl).parents('div.drag-append').append(uploadHtml);
    // uploadChange();

    UploadPdfFile();
}

jQuery(document).ready(function () {
    // ------------  File upload BEGIN ------------

    var fileHtml = ''
    var uploadHtml = ''
    jQuery(".file__input--file").on("change", function (event) {
        selFiles(this, event);
    });

    //Click to remove item
    jQuery("body").on("click", ".file__value", function () {
        var filename = jQuery(this).find('div.file__value--text').context.innerText;
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
});

jQuery(document).scroll(function () {
    if (jQuery(document).scrollTop() >= 400) {
        jQuery('.mainbar-container').addClass('fixed-header');
        // $('nav div').addClass('visible-title');
    }
    else {
        jQuery('.mainbar-container').removeClass('fixed-header');
        // $('nav div').removeClass('visible-title');
    }
});

function loadDataTables() {
    jq('.pTables').DataTable({
        "pageLength": 100
    });
    jq('.sTables').DataTable({
        "pageLength": 100
    });
    jq('.tTables').DataTable({
        "pageLength": 100
    });
    setTimeout(function () {
        jq('.pTables').DataTable();
        jq('.sTables').DataTable();
        jq('.tTables').DataTable();
    }, 1000);
}
//Enable autocomplete throughout the site
var lstContact = [];
function DelFunction() {
    var checkBox = document.getElementById("chkDel");
    if (checkBox.checked == true) {
        jq("#inpDelAddr").val(jQuery('#inpAddr').val());
        jq("#inpDelCity").val(jQuery('#inpCity').val());
        var a = jQuery("#inpState").children("option").filter(":selected").text()
        jQuery("#inpDelState option:selected").text(jQuery("#inpState").children("option").filter(":selected").text());
        jq("#inpDelZip").val(jQuery('#inpZip').val());
    }
    //validateform();
}

jq(document).ready(function () {
    loadDataTables();
    jq("#member").hide();
    jq("input:text,form").removeAttr("autocomplete");
    jq("#memberid").val(2);
    //jQuery('#loader-overlay2').hide();
});
function submitData(e) {
    var PayMode = jQuery("#inpPayInfo").val();
    if (PayMode == '1') {
        jq(".m-modal").modal('hide');
        jq(".modalPay").modal('show');
    }
    else {
        PayData();
    }
}

function PayData() {
    var model = {};
    var ProjOrderDet = [];
    // loop throght
    jQuery('tr.prc-row').each(function () {
        var ProjOrder = {};
        ProjOrder.FileName = jQuery(this).find("td.prc-name").text();
        ProjOrder.Pages = jQuery(this).find("td.prc-page").text();
        ProjOrder.Copies = jQuery(this).find("td.prc-copy").text();
        ProjOrder.Size = jQuery(this).find("td.prc-size").text();
        ProjOrder.PrintName = jQuery(this).find("td.prc-printsize").text();
        ProjOrder.Price = jQuery(this).find("td.prc-total").text();
        ProjOrderDet.push(ProjOrder);
    });
    var NonMember = true;
    var NonMemberVal = jq("#memberid").val();
    if (NonMemberVal == '1') {
        NonMember = false;
    }
    // assign datalist
    model.GetTblProjs = ProjOrderDet;
    // Main model
    model.Company = jQuery("label[name=Company]").text();
    model.Name = jQuery("label[name=Name]").text();
    model.Addr = jQuery("label[name=Addr]").text();
    model.City = jQuery("label[name=City]").text();
    model.State = jQuery("label[name=State]").text();
    model.Zip = jQuery("label[name=Zip]").text();
    model.Phone = jQuery("label[name=Phone]").text();
    model.Email = jQuery("label[name=Email]").text();
    model.ShipAmt = jQuery("td[id=priceTotal]").text();
    model.ProjId = jQuery("label[name=ProjId]").text();
    model.PO = jQuery("label[name=PO]").text();
    model.Prints = jQuery("label[name=Prints]").text();
    model.DeliveryDt = jQuery("label[name=DeliveryDt]").text();
    model.HowShip = jQuery("input[name=HowShip]").val();
    model.NotifyAddress = jQuery("label[name=NotifyAddress]").text();
    model.Instructions = jQuery("label[name=Instructions]").text();
    model.FileName = jQuery("label[name=FileName]").text();
    model.UID = jQuery("#ModUid").val();
    model.ShipAmt = parseFloat(jQuery("td[id=priceTotal]").text().replace('$', ''));
    model.NonMember = NonMember;
    var DelCity = jQuery("#lblModDelCity").text();
    var DelZip = jQuery('#lblModDelZip').text();
    var DelState = jQuery('#lblModState').text();
    if (DelCity != undefined || DelCity != '' || DelCity != null) {
        model.NotifyAddress = model.NotifyAddress + ' ' + DelCity
    }
    if (DelState != undefined || DelState != '' || DelState != null) {
        model.NotifyAddress = model.NotifyAddress + ' ' + DelState
    }
    if (DelZip != undefined || DelZip != '' || DelZip != null) {
        model.NotifyAddress = model.NotifyAddress + ' ' + DelZip
    }
    var pathChK = jQuery("#hdnFileName").val();
    var PayMode = jQuery("#inpPayInfo").val();
    if (PayMode == '1') {
        model.PaymentMode = 'CC';
    }
    else {
        model.PaymentMode = 'Invoice';
    }
    model.OrderChrgId = jQuery('#hdninpCompany').val();
    jq(".CopyCenterloader").css("display", "block");
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/App/InitiateAuthCopyCenter/',
        data: { 'model': model, 'pathChK': pathChK, 'ContPath': 'StaffAccount', 'ActPath': 'CopyCenter', 'SuccessPath': 'SaveCopyCenterInfo' },
        async: false,
        success: function (response) {
            if (response.Status = 'success') {
                window.location.href = response.Data;
            }
            else {
                alert("There's been issue with payment please try again");
                jQuery(".modalPay").modal('hide');
            }
        },
        error: function (response) {
            alert("There's been issue with payment please try again");
            jQuery(".modalPay").modal('hide');
        },
        failure: function (response) {
            alert("There's been issue with payment please try again");
            jQuery(".modalPay").modal('hide');
        }
    });
}
jq(function () {
    //alert(jq('#ui-id-1').attr("style"));
    jq("#inpCompany").autocomplete({
        source: function (request, response) {
            jq.ajax({
                url: '/Implement/GetCompanyName/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response(jq.map(data, function (item) {
                        return item;
                    }));
                    const input = jq('[id*=inpCompany]')[0];
                    if (data.length > 0) {
                        const selectedValue = data[0].label; // Choose the first suggestion
                        jq("#inpCompany").val(selectedValue);
                        jq("#hdninpCompany").val(data[0].val);
                        const startIndex = request.term.length;
                        const endIndex = selectedValue.length;
                        // Set the cursor position to overwrite mode
                        input.setSelectionRange(startIndex, endIndex);
                        jq("#member").show();
                        jq("#nonmember").hide();
                        jq("#memberid").val(1);

                    }
                    else {
                        jq("#member").hide();
                        jq("#nonmember").show();
                        jq("#memberid").val(2);
                        jq('#inpPayInfo option[value="2"]').remove();
                    }
                    var uicCss = jq('#ui-id-1').attr("style") + " display:block !important;";
                    jq('#ui-id-1').attr("style", uicCss);
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
            jq("#hdninpCompany").val(i.item.val);
            if (jq('#inpContact option').length != 0) {
                jq('#inpContact').children('option').remove();
                jq('#inpContact').append('<option value="0">--Select Contact--</option>');
            }
        },
        minLength: 2
    }).focus(function () {
        jq(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            //         const selObj = window.getSelection();
            ctrl.setSelectionRange(term.length - 1, jq(this).val().length)
            //                    alert(selObj);
            //             console.log(selObj);
        }
    });
    // Onchange events
    jq("#inpCompany").on('change', function () {
        jq.ajax({
            url: '/Implement/GetCompanyAddress',
            data: { "Id": parseFloat(jq('#hdninpCompany').val()) },
            type: "POST",
            async: false,
            success: function (response) {
                lstContact = response.data.ContactList;
                jq('#inpAddr').val(response.data.BillAddress);
                jq('#inpCity').val(response.data.BillCity);
                jq("#inpState option:selected").text(response.data.BillState);
                jq("#inpZip").val(response.data.BillZip);
                jq.each(response.data.ContactList, function (data, value) {
                    jq("#inpContact").append(jq("<option></option>").val(value.ConID).html(value.Contact));
                    validateform(inpAddr);
                    validateform(inpCity);
                    validateform(inpState);
                    validateform(inpZip);
                });
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    });
});
jq("#inpContact").on('change', function () {
    var a = jq(this).val();
    var lstTemp = [];
    lstTemp = lstContact.filter(v => v.ConID == a);
    jq("#inpEmail").val(lstTemp[0].Email);
    jq("#inpPhone").val(lstTemp[0].Phone);
});

function processAnnotations(annotationsData) {
    for (var i = 0; i < annotationsData.length; i++) {
        var data = annotationsData[i];
        if (!data) {
            continue;
        }
        if (data.subtype === 'Link') {
            linkCounter++;
        }
    }
}
var filehtml = '';
var chkhtml = '';
var priceHtml = '';
var priceNHtml = '';
var optionHtml = '';
var ColorpriceHtml = '';
var ColoroptionHtml = '';
var ColorpriceNHtml = '';
function GetCopyCenterPriceDetail() {
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/StaffAccount/GetCopyCenterPriceDetail/',
        data: {},
        async: false,
        success: function (response) {
            if (response.data.length != 0) {
                var model = new Array();
                model = response.data;
                jq.each(model, function (index, item) {
                    optionHtml += '<option value="' + item.Id + '">' + item.Size + '</option>'
                    priceHtml += '<input type="hidden" id="hdnsz_' + item.Id + '" name="' + item.SizeName + '" value="' + item.MemberPrice + '"/>'
                    priceNHtml += '<input type="hidden" id="hdnszn_' + item.Id + '" name="' + item.SizeName + '" value="' + item.NonMemberPrice + '"/>'
                    ColoroptionHtml += '<option value="' + item.Id + '">' + item.Size + '</option>'
                    ColorpriceHtml += '<input type="hidden" id="Colorhdnsz_' + item.Id + '" name="' + item.SizeName + '" value="' + item.ColorMemberPrice + '"/>'
                    ColorpriceNHtml += '<input type="hidden" id="Colorhdnszn_' + item.Id + '" name="' + item.SizeName + '" value="' + item.ColorNonMemberPrice + '"/>'
                })
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

GetCopyCenterPriceDetail();
function readPDFFile(pdf) {
    PDFJS.getDocument({ data: pdf }).then(function (pdf) {
        if (fileArr[loopCounter].indexOf(' ') > 0)
            fileArr[loopCounter] = fileArr[loopCounter].split(' ').join('_');
        filehtml += '<tr class="trUpload"><td class="tdUpoadFile">' + fileArr[loopCounter] + '</td><td class="tdUpload">' + pdf.pdfInfo.numPages + '</td><td><input type="text" class="form-control cus-upload-input"/><span class="loginError"></span></td><td><select class="form-control cus-upload-select"><option value="0" hidden="">-Select-</option>' + optionHtml + '<option value="">Custom</option> </select><span class="loginError"></span></td></tr>';
        loopCounter++;
    });
    //return NoofPages;
    DisplayFiles();
}
var linkCounter;
var fileArr = [];
var loopCounter = 0;
window.onload = function () {
    document.getElementById('fileUpload1').addEventListener('change', function () {

        var file = jQuery('#fileUpload1').val();
        if (file != undefined || file != '' || file != null) {
            jQuery('#selectCheck').val('Y');
            jQuery('#uploadError').html('');
            jq('#btnfileUpload1').prop('disabled', false);
        }
    })
}
function DisplayFiles() {
    setTimeout(function () {
        jq('#ftbody').html('');
        jq('#ftbody').html(filehtml);
        jq('.tblUpload').show();
        jq('.price-body').html('');
        jq('.price-body').html(priceHtml);
        jq('.nprice-body').html('');
        jq('.nprice-body').html(priceNHtml);
        jQuery('.color-price-body').html('');
        jQuery('.color-price-body').html(ColorpriceHtml);
        jQuery('.color-nprice-body').html('');
        jQuery('.color-nprice-body').html(ColorpriceNHtml);
        jq('.divUpload').removeClass("d-none");
        jq('#btnfileUpload1').prop('disabled', false);
        jq('[id *= mValue]').val('AM');
        jQuery('.copynum').val(1);
    }, 1000);
}
//Save page information like size no of copies
var totalPages = 0;
var totalCopy = 0;
var totalPrice = 0;
var obj = [];

function SaveUploadInfo() {
    var isCorrect = PopulatePricTable();
    if (!isCorrect) {
        jq("#" + e + "").removeAttr("data-dismiss")
        jq('.f-modal').modal('show');
    }
    else {
        jq('.m-modal').modal('hide');
        setTimeout(function () {
            jq('.m-modal').modal('show');
        }, 1000);
        jq("#" + e + "").attr("data-dismiss", "modal")
    }

    //            jq('#prcFooter').before(uploadHtml);
}
function PopulatePricTable() {
    totalPages = 0;
    totalCopy = 0;
    totalPrice = 0;
    var uploadHtml = '';
    obj = [];
    var isCorrect = true;
    jq("tr.trUpload").each(function () {
        var item = {};
        var Filename = jq(this).find("td.tdUpoadFile").text();
        item.Filename = Filename;
        var copy = jq(this).find("input.cus-upload-input").val();
        var isNumeric = jq.isNumeric(copy);
        jq(this).find("input.cus-upload-input").next('span').html('');
        if (!isNumeric || copy == "") {
            jq(this).find("input.cus-upload-input").next('span').html('Please enter correct numeric value.');
            isCorrect = false;
        }
        item.copy = copy;
        var pages = jq(this).find("td.tdUpload").text();
        item.pages = pages;
        var size = "";
        var price = "";
        var PrintSelect = "";
        var ColorSelect = jQuery(this).find("select.change-size option:selected").val();
        if (ColorSelect == 1) {
            size = jQuery(this).find("select.black-select option:selected").text();
            item.size = size;
            PrintSelect = 'B & W';
            item.PrintSelect = PrintSelect;
            price = jQuery(this).find("select.black-select option:selected").val();
            jQuery(this).find("select.black-select").next('span').html('');
        }
        else if (ColorSelect == 2) {
            size = jQuery(this).find("select.color-select option:selected").text();
            item.size = size;
            PrintSelect = 'Color';
            item.PrintSelect = PrintSelect;
            price = jQuery(this).find("select.color-select option:selected").val();
            jQuery(this).find("select.color-select").next('span').html('');
        }
        if (price == '0') {
            jq(this).find("select.cus-upload-select").next('span').html('Please select a size.');
            isCorrect = false;
        }
        if (ColorSelect == 1) {
            price = jQuery('#hdnsz_' + price).val();
        }
        else if (ColorSelect == 2) {
            price = jQuery('#Colorhdnsz_' + price).val();
        }
        item.price = price;
        obj.push(item);
        totalPages = totalPages + parseInt(pages);
        totalCopy = totalCopy + parseInt(copy) * parseInt(pages);
        jQuery('#selectCheck').val('Y');

    });
    jq(".prcTbody tr").remove();
    for (var i = 0; i < obj.length; i++) {

        uploadHtml += '<tr class="prc-row" id="prc-row"><td class="">' + (i + 1) + '</td><td class="prc-name" id="prc-name"> ' + obj[i].Filename + '</td><td class="prc-page" id="prc-page">' + obj[i].pages + '</td><td class="prc-copy" id="prc-copy">' + obj[i].copy + '</td><td class="prc-size" id="prc-size">' + obj[i].size + '</td><td class="prc-printsize" id="prc-printsize">' + obj[i].PrintSelect + '</td><td class="prc-price" id="prc-price">' + obj[i].price + '</td><td class="prc-total text-right border" id="prc-total">' + (parseFloat(obj[i].price) * parseFloat(obj[i].copy) * parseFloat(obj[i].pages)).toFixed(2) + '</td></tr > ';
        totalPrice = totalPrice + (parseFloat(obj[i].price) * parseFloat(obj[i].copy) * parseFloat(obj[i].pages));
    }

    jQuery('#hdnTotalPrice').val(totalPrice);
    jq('.prcTbody').html(uploadHtml);
    return isCorrect;
}

function UploadFiles(path) {
    var FileControls = document.querySelectorAll(".file__input--file");
    var formData = new FormData();
    for (i = 0; i < FileControls.length; i++) {
        var element = FileControls[i];
        var elementId = element.getAttribute("id");
        var files = jQuery("#" + elementId).get(0).files;
        if (files.length > 0) {
            for (var j = 0; j < files.length; j++) {
                formData.append("file", files[j]);
            }
        }
    }
    formData.append("FilePath", path);

    jQuery.ajax({
        type: 'POST',
        url: '/StaffAccount/Countdoc',
        data: formData,
        processData: false,
        contentType: false
    }).done(function (response) {
        if (response.Status === "success") {
            if (response.Data != null && response.Data.length > 0) {
                for (i = 0; i < response.Data.length; i++) {
                    filehtml += '<tr class="trUpload"><td class="tdUpoadFile">' + response.Data[i].FileName + '</td><td class="tdUpload">' + response.Data[i].PageNo + '</td><td><input type="text" class="form-control cus-upload-input copynum"/><span class="loginError"></span></td><td><select class="form-control change-size"><option value="1">B & W</option><option value="2">Color</option> </select><span class="loginError"></span></td><td><select onchange="SelectSize(event)" class="form-control cus-upload-select black-select"><option value="0" hidden="">-Select-</option>' + optionHtml + '<option value="">Custom</option> </select><select onchange="SelectSize(event)" class="form-control cus-upload-select color-select"><option value="0" hidden="">-Select-</option>' + ColoroptionHtml + '<option value="">Custom</option> </select><span class="loginError"></span></td><td style="display:flex;justify-content:center;"><button class="trashBtn-table"  onclick="deleteRow(this)"><i class="fa fa-trash"></i></button></td></tr>';
                }
            }
            DisplayFiles();
            jq(".CopyCenterloader").css("display", "none");
        }
    });

}
function deleteRow(button) {
    if (confirm('Do you want to permanently delete this uploaded file.')) {
        var row = jQuery(button).closest('tr');
        var filename = row.find('.tdUpoadFile').text();
        jQuery("body").find('.file__value:contains("' + filename + '")').click();
        var rowIndex = row.index();
        if (rowIndex >= 0) {
            row.remove();
        }

        document.getElementById("btnfileUpload1").click();
        
    }
}
function SelectSize(event) {
    var selectedValue = event.target.value;
    var $span = jQuery(event.target).closest('td').find('span.loginError');

    if (selectedValue > 0 || selectedValue == "") {
        $span.html('');
    } else {
        $span.html('Please select a size.');
    }
}
// Bind the onchange event handler to elements with the 'change-size' class
jQuery(document).on('change', '.change-size', function () {
    // Get the selected value
    var selectedValue = jQuery(this).val();
    jQuery(this).closest('.trUpload').find('.black-select').hide();
    jQuery(this).closest('.trUpload').find('.color-select').hide();
    // Check if the selected value is equal to 1
    if (selectedValue == 1) {
        // Display the 'cus-upload-select' dropdown
        jQuery(this).closest('.trUpload').find('.black-select').show();

    } else if (selectedValue == 2) {
        // Hide the 'cus-upload-select' dropdown if the value is not 1
        jQuery(this).closest('.trUpload').find('.color-select').show();
    }
});

async function UploadPdfFile(e) {
    var FileControls = document.querySelectorAll(".file__input--file");
    


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
            jQuery('#selectCheck').val('Y');
            allFiles = allFiles.concat(Array.from(files));
        }
    }
    var ext = jQuery('#selectCheck').val();
    if (ext != 'Y') {
        jQuery('#uploadError').html('Please select file.').css('color', 'red');
        jQuery('#tblUpload').hide();
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
        formData.append("time", currentTimeString);

        await new Promise((resolve, reject) => {
            jQuery.ajax({
                type: 'POST',
                url: '/StaffAccount/UploadPrintPdf',
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

                        if (successfulUploads === totalFiles) {
                            jQuery(".CopyCenterloader").css("display", "block");
                            jQuery("#uploadCheck").val(response.Flag);
                            jQuery('#uploadCheck').val('Y');
                            progressContainer.style.display = "none";
                            jQuery('#uploadError').html('File Uploaded successfully').css('color', 'Blue')
                            jQuery("#hdnFileName").val(response.Data);
                            fileArr = [];
                            filehtml = "";
                            loopCounter = 0;
                            var NoofPages = 0;
                            UploadFiles(response.FilePath);
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
//function UploadPdfFile() {
//    var FileControls = document.querySelectorAll(".file__input--file");
//    var ext = jQuery('#selectCheck').val();
//    if (ext != 'Y') {
//        jQuery('#uploadError').html('Please select file.');
//        return false;
//    }

//    var formData = new FormData();
//    for (i = 0; i < FileControls.length; i++) {
//        var element = FileControls[i];
//        var elementId = element.getAttribute("id");
//        var files = jQuery("#" + elementId).get(0).files;
//        if (files.length > 0) {
//            for (var j = 0; j < files.length; j++) {
//                formData.append("pdfFile", files[j]);
//            }
//        }
//    }

//    for (i = 0; i < FileControls.length; i++) {
//        var element = FileControls[i];
//        var elementId = element.getAttribute("id");
//        var files = jQuery("#" + elementId).get(0).files;
//        if (files.length > 0) {
//            for (var k = 0; k < files.length; k++) {
//                var file = files[k];
//                var fileName = file.name;
//                var extension = fileName.replace(/^.*\./, '');
//                if (extension != 'pdf' && extension != 'docx' && extension != 'jpg' && extension != 'tif') {
//                    jQuery('#uploadError').text(fileName + ' is not a valid file please upload only pdf/docx/jpg/tif file(s)');
//                    return false;
//                }
//            }
//        }
//    }
//    jq(".CopyCenterloader").css("display", "block");
//    jq.ajax({
//        type: 'POST',
//        url: '/StaffAccount/UploadPrintPdf',
//        data: formData,
//        processData: false,
//        contentType: false
//    }).done(function (response) {
//        if (response.Status === "success") {
//            jQuery("#uploadCheck").val(response.Flag)
//            //jQuery("#lblModPlan").html(response.Message);
//            jQuery('#uploadCheck').val('Y');
//            jQuery('#uploadError').html('File Uploaded successfully').css('color', 'Blue')
//            jQuery("#hdnFileName").val(response.Data);
//            fileArr = [];
//            filehtml = "";
//            loopCounter = 0;
//            var NoofPages = 0;
//            UploadFiles(response.FilePath);

//        }
//    })
//}

jQuery("#next-btn").click(() => {
    var starttComp = jQuery('#tComp').val();
    var starthComp = jQuery('#hComp').val();
    var startmValue = jQuery('#mValue').val();
    var startDateValue = jQuery('#inpDline').val();
    var dateArray = startDateValue.split('/');
    var formattedDate = dateArray[0] + '/' + dateArray[1] + '/' + dateArray[2];
    var StartDate = formattedDate + " " + starttComp + ":" + starthComp + ":00 " + startmValue;
    var startDate = new Date(StartDate);
    var isWeekday = startDate.getDay() >= 1 && startDate.getDay() <= 5;
    var isWorkingHours = startDate.getHours() >= 8 && startDate.getHours() < 17;
    if (!isWeekday || !isWorkingHours) {
        alert("Deadline must be between 8:00 AM to 5:00 PM.");
        jQuery('#tComp').focus()
        return false;
    }
    var res = validateform("next");
    if (res == false)
        return false;

    var Del = jQuery("#inpDelivery").children("option").filter(":selected").val()
    if (Del == 1) {
        jQuery("input[name=HowShip]").val('Pickup');
        jQuery("input[id=DetShip]").css("display", "none")
    }

    if (Del == 2) {
        jQuery("input[name=HowShip]").val('Local Delivery');
        jQuery("input[id=DetShip]").css("display", "block");
    }
    if (Del == 3) {
        jQuery("input[name=HowShip]").val('UPS');
        jQuery("input[id=DetShip]").css("display", "block");
    }
    jQuery('#lblModCompany').text(jQuery('#inpCompany').val());
    var member = jQuery('#memberid').val();
    if (member == '1') {

        jQuery('#lblModContact').text(jQuery("#inpContact").children("option").filter(":selected").text());
        jQuery('#ModUid').val(jQuery("#inpContact").children("option").filter(":selected").val());

    }
    else {
        jQuery('#lblModContact').text(jQuery("#inpnonmember").val());
    }
    jQuery('tr.prc-row').each(function () {
        var Pages = jQuery(this).find("td.prc-page").text();
        var copy = jQuery(this).find("td.prc-copy").text();
        jQuery('#lblModPages').text(Pages);
        jQuery('#lblModCopy').text(copy);
    });
    jQuery('#lblModBillAddr').text(jQuery('#inpAddr').val());
    jQuery('#lblModCity').text(jQuery('#inpCity').val());
    jQuery('#lblModState').text(jQuery("#inpState").children("option").filter(":selected").text());
    jQuery('#lblModDelState').text(jQuery("#inpDelState").children("option").filter(":selected").text());
    jQuery('#lblModDelivery').text(jQuery("#TextHowShip").val());
    jQuery('#lblModZip').text(jQuery('#inpZip').val());
    jQuery('#lblModPhone').text(jQuery('#inpPhone').val());
    jQuery('#lblModEmail').text(jQuery('#inpEmail').val());
    jQuery('#lblModProjNo').text(jQuery('#inpProjNo').val());
    jQuery('#lblModPO').text(jQuery('#inpPO').val());
    var deadlineTime = jQuery('#tComp').val() + ":" + jQuery('#hComp').val() + " " + jQuery('#mValue').val();
    jQuery('#lblDeadlineTime').text(deadlineTime);
    jQuery('#lblModDLine').text(jQuery('#inpDline').val() + " " + deadlineTime);
    jQuery('#lblModDelAddr').text(jQuery('#inpDelAddr').val());
    jQuery('#lblModDelCity').text(jQuery('#inpDelCity').val());
    jQuery('#lblModDelZip').text(jQuery('#inpDelZip').val());
    jQuery('#lblModDelInstruction').text(jQuery('#inpInstruction').val());
    var size = jQuery("#inpCharges option:selected").text();
    var price = jQuery("#inpCharges option:selected").val();
    var Print = jQuery("#inpCopy").val();
    var Count = jQuery("#inpPages").val();
    var total;
    //var TotalPrize;
    if (Del == 2) {
        jQuery('#delivCharges').text('$ 20');
        var fpriceTotal = jQuery('#hdnTotalPrice').val();
        //fpriceTotal = fpriceTotal.replace('$','');
        var tempTotal = parseFloat(fpriceTotal) + 20;
        jQuery('#priceTotal').text('$' + tempTotal.toFixed(2));
    }
    else {
        jQuery('#delivCharges').text('$ 0');
        var fpriceTotal = jQuery('#hdnTotalPrice').val();
        //fpriceTotal = fpriceTotal.replace('$','');
        jQuery('#priceTotal').text('$' + parseFloat(fpriceTotal).toFixed(2));
    }

    jQuery('.order-form').css("display", "none") &&
        jQuery('.review-form').css("display", "block") &&
        jQuery("#next-btn").hide() &&
        jQuery("#orderform-head").css("display", "none") &&
        jQuery("#ordersummary-head").css("display", "block")
});
jQuery("#prev-btn").click((e) => {
    //debugger;
    jQuery('.review-form').css("display", "none") &&
        e.preventDefault();
    jQuery("#next-btn").show() &&
        jQuery('.order-form').css("display", "block") &&
        jQuery("#orderform-head").css("display", "block") &&
        jQuery("#ordersummary-head").css("display", "none")
})

function Reorder(OrderId) {
    var ProjId = jQuery('input[id=ModOrderIdd]').val();
    if (ProjId != null && ProjId != "") {
        OrderId = ProjId;
    }
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/StaffAccount/Reorder/',
        data: { 'OrderId': OrderId },
        async: false,
        success: function (response) {
            if (response.success) {
                alert(response.statusMessage);
                location.reload();
            }
            else {
                alert(response.statusMessage);
                location.reload();
            }
        },
        error: function (response) {
            alert(response.statusMessage);
        },
        failure: function (response) {
            alert(response.statusMessage);
        }
    });
}
function SendNotice(arg) {
    //jQuery('#loader-overlay2').show();
    var myString = arg;
    var lastChar = myString.substring((myString.indexOf('_') + 1), myString.length);
    var ProjId = jQuery('input[id=OrderId_' + lastChar + ']').val();
    jQuery.ajax({
        url: '/StaffAccount/UpdateDoneDt/',
        data: { "ProjId": ProjId },
        type: "POST",
        async: false,
        success: function (data) {
            location.reload(true);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
function SendNoticeModal() {
    var ProjId = jQuery('input[id=ModOrderId]').val();
    jQuery.ajax({
        url: '/StaffAccount/UpdateDoneDt/',
        data: { "ProjId": ProjId },
        type: "POST",
        async: false,
        success: function (data) {
            location.reload(true);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
function ReadyDelivery(arg) {
    var myString = arg;
    var lastChar = myString.substring((myString.indexOf('_') + 1), myString.length);
    var OrderId = jQuery('input[id=OrderIdc_' + lastChar + ']').val();

    jQuery.ajax({
        url: '/StaffAccount/UpdateShipDt/',
        data: { "OrderId": OrderId },
        type: "POST",
        async: false,
        success: function (data) {

            window.location.reload();
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
function ReadyDeliveryModal() {
    var OrderId = jQuery('input[id=ModOrderIdc]').val();
    jQuery.ajax({
        url: '/StaffAccount/UpdateShipDt/',
        data: { "OrderId": OrderId },
        type: "POST",
        async: false,
        success: function (data) {
            location.reload(true);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
function ShowModalC(orderId) {
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/StaffAccount/ViewOrderDoc',
        data: { 'OrderId': orderId },
        async: false,
        success: function (response) {
            var rows = "";
            var totalPrice = 0;
            var item = response.data;
            jQuery('#RlblCompany').text(item.Company);
            jQuery('#ModOrderIdc').val(item.OrderId);
            jQuery('#RlblName').text(item.Name);
            jQuery('#RlblAddr').text(item.Addr);
            jQuery('#RlblPhone').text(item.Phone);
            jQuery('#RlblEmail').text(item.Email);
            jQuery('#RlblProjNo').text((item.ProjId) != null ? item.ProjId : "");
            jQuery('#RlblProjPO').text((item.PO) != null ? item.PO : "");
            var deliveryDate = item.DeliveryDt;
            var initialDate = new Date(deliveryDate);
            // Get the components of the date (day, month, year)
            var day = initialDate.getDate();
            var month = initialDate.getMonth() + 1; // Months are zero-based, so we add 1
            var year = initialDate.getFullYear();
            // Create the formatted date string in the desired format
            var DeliveryDateStr = month + "/" + day + "/" + year;
            jQuery('#RlblDLine').text(DeliveryDateStr);
            jQuery('#RlblHow').text(item.HowShip);
            jQuery('#RlblNotes').text(item.Instruction);
            jQuery('#RviewDocFr').attr('href', '/StaffAccount/ViewDoc?OrderId=' + item.OrderId);
            jQuery.each(response.data.GetTblProjs, function (index, item) {
                rows += '<tr><td>' + item.FileName + '</td><td class="">' + item.Pages + '</td><td class="">' + item.Copies + '</td><td class="">' + (item.PrintName != null ? item.PrintName:"B & W") + '</td><td class="">' + item.Size + '</td><td class="">' + item.Price + '</td></tr>';
                var price = parseFloat(item.Price);
                // Add the price to the total
                totalPrice += price;
            });
            var ShipAmt = item.ShipAmt;
            var totalPriceWithDecimals = totalPrice.toFixed(2);
            if (ShipAmt != null && ShipAmt != 0) {
                jQuery('#RTotalOrderPrice').text(item.ShipAmt);
            }
            else {
                jQuery('#RTotalOrderPrice').text(totalPriceWithDecimals);
            }
            jQuery('#ROrderDetails').html(rows);
            jq('.r-modal').modal('show');
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
}
function ShowModalD(orderId) {
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/StaffAccount/ViewOrderDoc',
        data: { 'OrderId': orderId },
        async: false,
        success: function (response) {
            var rows = "";
            var totalPrice = 0;
            var item = response.data;
            jQuery('#COlblCompany').text(item.Company);
            jQuery('#ModOrderIdd').val(item.OrderId);
            jQuery('#COlblName').text(item.Name);
            jQuery('#COlblAddr').text(item.Addr);
            jQuery('#COlblPhone').text(item.Phone);
            jQuery('#COlblEmail').text(item.Email);
            jQuery('#COlblProjNo').text((item.ProjId) != null ? item.ProjId : "");
            jQuery('#COlblProjPO').text((item.PO) != null ? item.PO : "");
            var deliveryDate = item.DeliveryDt;
            var initialDate = new Date(deliveryDate);
            // Get the components of the date (day, month, year)
            var day = initialDate.getDate();
            var month = initialDate.getMonth() + 1; // Months are zero-based, so we add 1
            var year = initialDate.getFullYear();
            // Create the formatted date string in the desired format
            var DeliveryDateStr = month + "/" + day + "/" + year;
            jQuery('#COlblDLine').text(DeliveryDateStr);
            jQuery('#COlblHow').text(item.HowShip);
            jQuery('#COlblNotes').text(item.Instruction);
            jQuery('#COviewDocFr').attr('href', '/StaffAccount/ViewDoc?OrderId=' + item.OrderId);
            jQuery.each(response.data.GetTblProjs, function (index, item) {
                rows += '<tr><td>' + item.FileName + '</td><td class="">' + item.Pages + '</td><td class="">' + item.Copies + '</td><td>' + (item.PrintName != null ? item.PrintName:"B & W") + '</td><td class="">' + item.Size + '</td><td class="">' + item.Price + '</td></tr>';
                var price = parseFloat(item.Price);
                // Add the price to the total
                totalPrice += price;
            });
            var ShipAmt = item.ShipAmt;
            var totalPriceWithDecimals = totalPrice.toFixed(2);
            if (ShipAmt != null && ShipAmt != 0) {
                jQuery('#COTotalOrderPrice').text(item.ShipAmt);
            }
            else {
                jQuery('#COTotalOrderPrice').text(totalPriceWithDecimals);
            }
            jQuery('#COOrderDetails').html(rows);
            jq('.co-modal').modal('show');
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
}
function ShowModal(arg, orderId) {
    var myString = arg;
    var lastChar = myString.substring((myString.indexOf('_') + 1), myString.length);
    var OrderId = jQuery('#OrderId_' + lastChar).val();
    var Viewed = jQuery('#hidViewed_' + lastChar).val();
    if (!Viewed || Viewed == '' || Viewed == 'False') {
        jQuery.ajax({
            url: '/StaffAccount/UpdateViewed/',
            data: { "OrderId": OrderId },
            type: "POST",
            async: false,
            success: function (response) {
                if (response.Status == "success") {
                    jQuery('#hidViewed_' + lastChar).val('true');
                    jQuery('#ViewCheck_' + lastChar).removeAttr('style');
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
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/StaffAccount/ViewOrderDoc',
        data: { 'OrderId': orderId },
        async: false,
        success: function (response) {
            var rows = "";
            var totalPrice = 0;
            var item = response.data;
            jQuery('#lblCompany').text(item.Company);
            jQuery('#ModOrderId').val(item.OrderId);
            jQuery('#lblName').text(item.Name);
            jQuery('#lblAddr').text(item.Addr);
            jQuery('#lblPhone').text(item.Phone);
            jQuery('#lblEmail').text(item.Email);
            jQuery('#lblProjNo').text((item.ProjId) != null ? item.ProjId : "");
            jQuery('#lblProjPO').text((item.PO) != null ? item.PO : "");
            var deliveryDate = item.DeliveryDt;
            var initialDate = new Date(deliveryDate);
            // Get the components of the date (day, month, year)
            var day = initialDate.getDate();
            var month = initialDate.getMonth() + 1; // Months are zero-based, so we add 1
            var year = initialDate.getFullYear();
            // Create the formatted date string in the desired format
            var DeliveryDateStr = month + "/" + day + "/" + year;
            jQuery('#lblDLine').text(DeliveryDateStr);
            jQuery('#lblHow').text(item.HowShip);
            jQuery('#lblNotes').text(item.Instruction);
            jQuery('#viewDocFr').attr('href', '/StaffAccount/ViewDoc?OrderId=' + item.OrderId);
            jQuery.each(response.data.GetTblProjs, function (index, item) {
                rows += '<tr><td>' + item.FileName + '</td><td class="">' + item.Pages + '</td><td class="">' + item.Copies + '</td><td class="">' + (item.PrintName != null ? item.PrintName:"B & W") + '</td><td class="">' + item.Size + '</td><td class="">' + item.Price + '</td></tr>';
                var price = parseFloat(item.Price);
                // Add the price to the total
                totalPrice += price;
            });
            var ShipAmt = item.ShipAmt;
            var totalPriceWithDecimals = totalPrice.toFixed(2);
            if (ShipAmt != null && ShipAmt != 0) {
                jQuery('#TotalOrderPrice').text(item.ShipAmt);
            }
            else {
                jQuery('#TotalOrderPrice').text(totalPriceWithDecimals);
            }
            jQuery('#OrderDetails').html(rows);
            jq('.v-modal').modal('show');
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
}
function getFloatValue(val) {
    if (val == '' || val == undefined)
        val = 0;
    if (isNaN(val))
        val = 0;
    val = parseFloat(val);
    return val;
}
jQuery(document).ready(function () {

    //var Viewed = jQuery('#hidViewed_' + lastChar).val();
    var numItems = jQuery('.ViewClass').length
    for (var i = 1; i <= numItems; i++) {
        var Viewed = jQuery('#hidViewed_' + i).val();
        if (!Viewed || Viewed == '' || Viewed == 'False')
            jQuery('#ViewCheck_' + i).css("background-color", "#d4fffa");
    }

});

function validateform(arg) {
    if (arg == 'next') {
        var isValide = true;
        var isCorret = PopulatePricTable();
        if (!isCorret)
            isValide = false;
        var Pcompany = jQuery('#inpCompany').val();
        jQuery('#inpCompany').next('span').html('');
        if (Pcompany == undefined || Pcompany == '' || Pcompany == null) {
            jQuery('#inpCompany').next('span').html('Please enter your company name.');
            isValide = false;
        }
        var ext = jQuery('#selectCheck').val();
        if (ext != 'Y') {
            jQuery('#uploadError').html('Please select file.').css('color', 'red');
            jQuery('#tblUpload').hide();
            isValide = false;
        }
        var upChk = jQuery('#uploadCheck').val();
        if (upChk != 'Y') {
            jQuery('#uploadError').html('File is not uploaded.').css('color', 'red');
            jQuery('#tblUpload').hide();
            isValide = false;
        }
        var validatemember = jQuery('#memberid').val();
        if (validatemember == '1') {
            // debugger;
            var pcontact = jQuery('select[id=inpContact] option').filter(':selected').val();
            jQuery('#inpContact').next('span').html('');
            if (pcontact == '0') {
                jQuery('#inpContact').next('span').html('Please select contact.');
                isValide = false;
            }
        }
        else {
            var pcontact = jQuery('#inpnonmember').val();
            jQuery('#inpnonmember').next('span').html('');
            if (pcontact == undefined || pcontact == '' || pcontact == null) {
                jQuery('#inpnonmember').next('span').html('Please select contact.');
                isValide = false;
            }
        }
        var pAdd = jQuery('#inpAddr').val();
        jQuery('#inpAddr').next('span').html('');
        if (pAdd == undefined || pAdd == '' || pAdd == null) {
            jQuery('#inpAddr').next('span').html('Please enter your billing address.');
            isValide = false;
        }
        var pcity = jQuery('#inpCity').val();
        jQuery('#inpCity').next('span').html('');
        if (pcity == undefined || pcity == '' || pcity == null) {
            jQuery('#inpCity').next('span').html('Please enter your billing city.');
            isValide = false;
        }
        var bState = jQuery('select[id=inpState] option').filter(':selected').text();
        jQuery('#inpState').next('span').html('');
        if (bState == '--Select State--') {
            jQuery('#inpState').next('span').html('Please enter your billing state.');
            isValide = false;
        }
        var pzip = jQuery('#inpZip').val();
        jQuery('#inpZip').next('span').html('');
        if (pzip == undefined || pzip == '' || pzip == null) {
            jQuery('#inpZip').next('span').html('Please enter your billing zipcode.');
            isValide = false;
        }
        var phone = jQuery('#inpPhone').val();
        jQuery('#inpPhone').next('span').html('');
        if (phone == undefined || phone == '' || phone == null) {
            jQuery('#inpPhone').next('span').html('Please enter phone number.');
            isValide = false;
        }
        var email = jQuery('#inpEmail').val();
        jQuery('#inpEmail').next('span').html('');
        if (email == undefined || email == '' || email == null) {
            jQuery('#inpEmail').next('span').html('Please enter email.');
            isValide = false;
        }

        var Dline = jQuery('#inpDline').val();
        jQuery('#inpDline').next('span').html('');
        if (Dline == undefined || Dline == '' || Dline == null) {
            jQuery('#inpDline').next('span').html('Please choose deadline datetime.');
            isValide = false;
        }
        var inpDelivery = jQuery('select[id=inpDelivery] option').filter(':selected').val();
        jQuery('#inpDelivery').next('span').html('');
        if (inpDelivery == undefined || inpDelivery == '' || inpDelivery == null) {
            jQuery('#inpDelivery').next('span').html('Please select delivery.');
            isValide = false;
        }
        if (SelectedDelOption != 'Pickup') {
            var pAdd = jQuery('#inpDelAddr').val();
            jQuery('#inpDelAddr').next('span').html('');
            if (pAdd == undefined || pAdd == '' || pAdd == null) {
                jQuery('#inpDelAddr').next('span').html('Please enter your deliver address.');
                isValide = false;
            }
            var pcity = jQuery('#inpDelCity').val();
            jQuery('#inpDelCity').next('span').html('');
            if (pcity == undefined || pcity == '' || pcity == null) {
                jQuery('#inpDelCity').next('span').html('Please enter your deliver city name.');
                isValide = false;
            }
            var bState = jQuery('select[id=inpDelState] option').filter(':selected').text();
            jQuery('#inpDelState').next('span').html('');
            if (bState == '--Select State--') {
                jQuery('#inpDelState').next('span').html('Please enter your deliver state address.');
                isValide = false;
            }
            var pzip = jQuery('#inpDelZip').val();
            jQuery('#inpDelZip').next('span').html('');
            if (pzip == undefined || pzip == '' || pzip == null) {
                jQuery('#inpDelZip').next('span').html('Please enter your deliver zipcode.');
                isValide = false;
            }
        }
        else {
            jQuery(".HideDelivery").css("display", "none");
            jQuery(".modHideShip").css("display", "none");
        }
        var textAreaCtrl = jQuery('#inpInstruction')
        var pInstruction = trimfield(textAreaCtrl.val());
        textAreaCtrl.parent('div').parent('div').find('span').html('');

        return isValide;
    }
    else {
        var isValide = true;
        var isCorret = PopulatePricTable();
        if (!isCorret)
            isValide = false;
        if (arg == 'inpCompany') {
            var Pcompany = jQuery('#inpCompany').val();
            jQuery('#inpCompany').next('span').html('');
            if (Pcompany == undefined || Pcompany == '' || Pcompany == null) {
                jQuery('#inpCompany').next('span').html('Please enter your company name.');
                isValide = false;
            }
        }
        if (arg == 'selectCheck') {
            var ext = jQuery('#selectCheck').val();
            if (ext != 'Y') {
                jQuery('#uploadError').html('Please select file.').css('color', 'red');
                jQuery('#tblUpload').hide();
                isValide = false;
            }
        }
        if (arg == 'uploadCheck') {
            var upChk = jQuery('#uploadCheck').val();
            if (upChk != 'Y') {
                jQuery('#uploadError').html('File is not uploaded.').css('color', 'red');
                jQuery('#tblUpload').hide();
                isValide = false;
            }
        }
        if (arg == 'memberid') {
            var validatemember = jQuery('#memberid').val();
            if (validatemember == '1') {
                // debugger;
                var pcontact = jQuery('select[id=inpContact] option').filter(':selected').val();
                jQuery('#inpContact').next('span').html('');
                if (pcontact == '0') {
                    jQuery('#inpContact').next('span').html('Please select contact.');
                    isValide = false;
                }
            }
            else {
                var pcontact = jQuery('#inpnonmember').val();
                jQuery('#inpnonmember').next('span').html('');
                if (pcontact == undefined || pcontact == '' || pcontact == null) {
                    jQuery('#inpnonmember').next('span').html('Please select contact.');
                    isValide = false;
                }
            }
        }
        if (arg == 'inpAddr') {
            var pAdd = jQuery('#inpAddr').val();
            jQuery('#inpAddr').next('span').html('');
            if (pAdd == undefined || pAdd == '' || pAdd == null) {
                jQuery('#inpAddr').next('span').html('Please enter your billing address.');
                isValide = false;
            }
        }
        if (arg == 'inpCity') {
            var pcity = jQuery('#inpCity').val();
            jQuery('#inpCity').next('span').html('');
            if (pcity == undefined || pcity == '' || pcity == null) {
                jQuery('#inpCity').next('span').html('Please enter your billing city.');
                isValide = false;
            }
        }
        if (arg == 'inpState') {
            var bState = jQuery('select[id=inpState] option').filter(':selected').text();
            jQuery('#inpState').next('span').html('');
            if (bState == '--Select State--') {
                jQuery('#inpState').next('span').html('Please enter your billing state.');
                isValide = false;
            }
        }
        if (arg == 'inpZip') {
            var pzip = jQuery('#inpZip').val();
            jQuery('#inpZip').next('span').html('');
            if (pzip == undefined || pzip == '' || pzip == null) {
                jQuery('#inpZip').next('span').html('Please enter your billing zipcode.');
                isValide = false;
            }
        }
        if (arg == 'inpPhone') {
            var phone = jQuery('#inpPhone').val();
            jQuery('#inpPhone').next('span').html('');
            if (phone == undefined || phone == '' || phone == null) {
                jQuery('#inpPhone').next('span').html('Please enter phone number.');
                isValide = false;
            }
        }
        if (arg == 'inpEmail') {
            var email = jQuery('#inpEmail').val();
            jQuery('#inpEmail').next('span').html('');
            if (email == undefined || email == '' || email == null) {
                jQuery('#inpEmail').next('span').html('Please enter email.');
                isValide = false;
            }
        }

        if (arg == 'inpDline') {
            var Dline = jQuery('#inpDline').val();
            jQuery('#inpDline').next('span').html('');
            if (Dline == undefined || Dline == '' || Dline == null) {
                jQuery('#inpDline').next('span').html('Please choose deadline date.');
                isValide = false;
            }
        }
        if (arg == 'inpDelivery') {
            var inpDelivery = jQuery('select[id=inpDelivery] option').filter(':selected').val();
            jQuery('#inpDelivery').next('span').html('');
            if (inpDelivery == undefined || inpDelivery == '' || inpDelivery == null) {
                jQuery('#inpDelivery').next('span').html('Please select delivery.');
                isValide = false;
            }

            if (inpDelivery != 1 && inpDelivery != 2 && inpDelivery != 3 && inpDelivery != 4) {
                jQuery(".HideDelivery").css("display", "block");
                jQuery(".modHideShip").css("display", "block")
                if (arg == 'inpDelAddr') {
                    var pAdd = jQuery('#inpDelAddr').val();
                    jQuery('#inpDelAddr').next('span').html('');
                    if (pAdd == undefined || pAdd == '' || pAdd == null) {
                        jQuery('#inpDelAddr').next('span').html('Please enter your deliver address.');
                        isValide = false;
                    }
                }

                if (arg == 'inpDelCity') {
                    var pcity = jQuery('#inpDelCity').val();
                    jQuery('#inpDelCity').next('span').html('');
                    if (pcity == undefined || pcity == '' || pcity == null) {
                        jQuery('#inpDelCity').next('span').html('Please enter your deliver city name.');
                        isValide = false;
                    }
                }
                if (arg == 'inpDelState') {
                    var bState = jQuery('select[id=inpDelState] option').filter(':selected').text();
                    jQuery('#inpDelState').next('span').html('');
                    if (bState == '--Select State--') {
                        jQuery('#inpDelState').next('span').html('Please enter your deliver state address.');
                        isValide = false;
                    }
                }
                if (arg == 'inpDelZip') {
                    var pzip = jQuery('#inpDelZip').val();
                    jQuery('#inpDelZip').next('span').html('');
                    if (pzip == undefined || pzip == '' || pzip == null) {
                        jQuery('#inpDelZip').next('span').html('Please enter your deliver zipcode.');
                        isValide = false;
                    }

                }
            }
            else {
                jQuery(".HideDelivery").css("display", "none");
                jQuery(".modHideShip").css("display", "none");
            }
            if (arg == 'inpInstruction') {
                var textAreaCtrl = jQuery('#inpInstruction')
                var pInstruction = trimfield(textAreaCtrl.val());
                textAreaCtrl.parent('div').parent('div').find('span').html('');
               
            }
            return isValide;
        }

    }
}
function trimfield(str) {
    return str.replace(/^\s+|\s+$/g, '');
}

function OpenForwardModal(FwrdValue, Instruction) {
    jq("#SendTo").val('');
    jq("#CcEmail").val('');
    jq("#BccEmail").val('');
    jq("#FwdValue").val('');
    jq("#MailSubject").val('');
    jq('span.loginError').html('');
    var formate = "<span><b>Additional Instruction:</b> " + Instruction + "</span><br/><br/>" +
        "<span><b>You can review the file at the link :</b></span> " +
        "<a href='" + FwrdValue + "' target='_blank' style='color: blue; ' > Click here</a>";
    jq("#txtspan").html(formate);
    jq(".CcClass").addClass("d-none");
    jq(".BccClass").addClass("d-none");
    jq(".u-modal").modal('show');
}

function SendMail() {
    var CcError = jq("#CcError").html();
    if (CcError != "") {
        return false;
    }
    var BccError = jq("#BccError").html();
    if (BccError != "") {
        return false;
    }
    var sendTo = jq("#SendTo").val();
    jq('#ValidEmail').html('');
    if (sendTo == undefined || sendTo == '' || sendTo == null) {
        jq('#ValidEmail').html('Please enter email.');
        return false;
    }
    var subject = jq("#MailSubject").val();
    jq('#MailSubject').next('span').html('');
    if (subject == undefined || subject == '' || subject == null) {
        jq('#MailSubject').next('span').html('Please enter subject.');
        return false;
    }
    var cc = jq("#CcEmail").val();
    var bcc = jq("#BccEmail").val();

    var strMessage = jq("#txtspan").html();
    var model = {};
    model.EmailTos = sendTo;
    model.BccEmail = bcc;
    model.CcEmail = cc;
    model.Subject = subject;
    model.strMessage = strMessage;
    jq.ajax({
        type: "POST",
        dataType: 'json',
        url: '/StaffAccount/SendForwardMail/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                alert(response.statusMessage);
                jq(".u-modal").modal('hide');
            }
            else {
                alert(response.statusMessage);
                jq(".u-modal").modal('hide');
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

function SpanCc() {
    jq(".CcClass").removeClass("d-none");
    jq(".BccClass").removeClass("d-none");
}
function handleTab(event, element) {
    if (event.key === " ") {
        event.preventDefault();
        const value = element.value;
        const selectionStart = element.selectionStart;
        const selectionEnd = element.selectionEnd;
        element.value = value.substring(0, selectionStart) + ";" + value.substring(selectionEnd);
        element.setSelectionRange(selectionStart + 1, selectionStart + 1);
    }
}

// Function to validate email format
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function validateSendToInput(element) {
    const value = element.value;
    //const id = element.id;
    const errorSpan = element.nextElementSibling;
    const emailList = value.split(';').map(email => email.trim());
    const invalidEmails = emailList.filter(email => !isValidEmail(email));
    if (invalidEmails != null && invalidEmails != undefined && invalidEmails != "") {
        //errorSpan.textContent = "Invalid email address(es): " + invalidEmails.join(", ");
        document.getElementById("ValidEmail").textContent = "Invalid email address(es): " + invalidEmails.join(", ");
        /*jq("#SendTo").focus();*/
    } else {
        document.getElementById("ValidEmail").textContent = "";
    }
}
function validateCcInput(element) {
    const value = element.value;
    //const id = element.id;
    const errorSpan = element.nextElementSibling;
    const emailList = value.split(';').map(email => email.trim());
    const invalidEmails = emailList.filter(email => !isValidEmail(email));
    if (invalidEmails != null && invalidEmails != undefined && invalidEmails != "") {
        //errorSpan.textContent = "Invalid email address(es): " + invalidEmails.join(", ");
        document.getElementById("CcError").textContent = "Invalid email address(es): " + invalidEmails.join(", ");
        /*jq("#CcEmail").focus();*/
    } else {
        document.getElementById("CcError").textContent = "";
    }
}
function validateBccInput(element) {
    const value = element.value;
    //const id = element.id;
    const errorSpan = element.nextElementSibling;
    const emailList = value.split(';').map(email => email.trim());
    const invalidEmails = emailList.filter(email => !isValidEmail(email));
    if (invalidEmails != null && invalidEmails != undefined && invalidEmails != "") {
        //errorSpan.textContent = "Invalid email address(es): " + invalidEmails.join(", ");
        document.getElementById("BccError").textContent = "Invalid email address(es): " + invalidEmails.join(", ");
        /*jq("#BccEmail").focus();*/
    } else {
        document.getElementById("BccError").textContent = "";
    }
}

// Show copycenter price details in popup modal

jQuery(document).ready(function () {
    // Get CopyCenter Price List
    BindPriceDetails();
    // Get Delivery Dropdown List 
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Home/GetDeliveryList',
        data: {},
        async: false,
        success: function (response) {
            var datalist = response.data;
            for (var key in datalist) {
                if (datalist.hasOwnProperty(key)) {
                    var optgroup = jQuery('<optgroup label="' + key + '">');

                    for (var i = 0; i < datalist[key].length; i++) {
                        var option = jQuery('<option value="' + datalist[key][i].DelivOptId + '">' + key + " - " + datalist[key][i].DelivOptName + '</option>');
                        optgroup.append(option);
                    }

                    jQuery('#inpDelivery').append(optgroup);
                }
            }
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
});
function BindPriceDetails() {
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Home/GetCopyCenterPriceList',
        data: {},
        async: false,
        success: function (response) {
            var Member = '';
            var NonMember = '';
            var ColorMemberPrice = '';
            var ColorNonMemberPrice = '';
            Member += '<li class="list-group-item justify-content-center rate-head">' +
                '<legend>Black and White - Print Size</legend>' +
                '</li>';
            NonMember += '<li class="list-group-item justify-content-center rate-head">' +
                '<legend>Black and White - Print Size</legend>' +
                '</li>';
            ColorMemberPrice += '<li class="list-group-item justify-content-center rate-head">' +
                '<legend>Color - Print Size</legend>' +
                '</li>';
            ColorNonMemberPrice += '<li class="list-group-item justify-content-center rate-head">' +
                '<legend>Color - Print Size</legend>' +
                '</li>';
            jQuery.each(response.data, function (index, item) {
                Member += ' <li class="list-group-item">' +
                    '<label>' + item.Size + '</label>' +
                    '<span> $' + parseFloat(item.MemberPrice).toFixed(2) + '</span>' +
                    '</li>';
                NonMember += ' <li class="list-group-item">' +
                    '<label>' + item.Size + '</label>' +
                    '<span> $' + parseFloat(item.NonMemberPrice).toFixed(2) + '</span>' +
                    '</li>';
                ColorMemberPrice += ' <li class="list-group-item">' +
                    '<label>' + item.Size + '</label>' +
                    '<span> $' + parseFloat(item.ColorMemberPrice).toFixed(2) + '</span>' +
                    '</li>';
                ColorNonMemberPrice += ' <li class="list-group-item">' +
                    '<label>' + item.Size + '</label>' +
                    '<span> $' + parseFloat(item.ColorNonMemberPrice).toFixed(2) + '</span>' +
                    '</li>';
            });
            jQuery('ul#MemberPrice').html(Member);
            jQuery('ul#NonMemberPrice').html(NonMember);
            jQuery('ul#ColorMemberPrice').html(ColorMemberPrice);
            jQuery('ul#ColorNonMemberPrice').html(ColorNonMemberPrice);
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
}

jQuery('#inpDelivery').on('change', function () {
    // Get the selected option element
    var selectedOption = jQuery(this).find('option:selected');

    // Get the label of the selected option's parent optgroup
    var selectedOptgroupLabel = selectedOption.parent('optgroup').attr('label');

    // Now you have the selected optgroup label value
    if (selectedOptgroupLabel) {
        SelectedDelOption = selectedOptgroupLabel;
    }
    var id = jQuery('#inpDelivery').val();
    var text = jQuery('select[id=inpDelivery] option').filter(':selected').text();
    jQuery('#HowShip').val(id);
    jQuery('#TextHowShip').val(text);
});
function LoadPage() {
    window.location.href = "/StaffAccount/CopyCenter";
}

jQuery(document).ready(function () {
    jQuery("#CardNumber").on("input", function () {
        var enteredSequence = jQuery(this).val();
        var formattedSequence = formatSequence(enteredSequence);
        jQuery(this).val(formattedSequence);
    });

    function formatSequence(sequence) {
        var formatted = sequence.replace(/\s+/g, "").match(/.{1,4}/g);
        if (formatted) {
            return formatted.join(" ");
        }
        return "";
    }
});
function ValidateExpiryDate() {
    jQuery("#ExpiryDate").next('span').html('');
    var inputValue = jQuery("#ExpiryDate").val();
    if (inputValue == null || inputValue == "") {
        jQuery("#ExpiryDate").next('span').html('Please enter your card detail');
        jQuery('#ExpiryDate').focus();
        return false;
    }
    // Regular expression pattern for MM/YY format
    var pattern = /^(0[1-9]|1[0-2])\/?([0-9]{2})$/;
    // Check if the input matches the pattern
    if (pattern.test(inputValue)) {
        // Valid format
        jQuery("#ExpiryDate").next('span').html('');
    } else {
        // Invalid format
        jQuery("#ExpiryDate").next('span').html('Invalid MM/YY format. Please enter a valid format (MM/YY).');
        jQuery('#ExpiryDate').focus();
        return false;
    }
}
var suggestions = ["00", "15", "30", "45"];
var Tsuggestions = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"];
var Msuggestions = ["AM", "PM"];
//jQuery('#mValue').val('AM');
var inputField = jQuery('#hComp');
function autoCompleteTimer() {
    // Autocomplete for tComp input
    jq('[id *= tComp]').autocomplete({
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
            jq(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            jq(this).autocomplete("widget").removeClass("show-all");
        },
    }).on("focus", function () {
        jq(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === jQuery.ui.keyCode.DOWN) {
            event.preventDefault();
            jq(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            jq(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
        }
    }).on("input", function () {
        var value = $(this).val();
        if (value.length > 2) {
            jq(this).val(value.slice(0, 2));
        }
    });
    jq('[id *= hComp]').autocomplete({
        source: function (request, response) {
            response(suggestions);
        },
        minLength: 0,
        open: function (event, ui) {
            jq(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            jq(this).autocomplete("widget").removeClass("show-all");
        },
        change: function (event, ui) {
            var value = parseInt(this.value);

            if (isNaN(value)) {
                this.value = '00';
            } else if (value !== 15 && value !== 30 && value !== 0 && value !== 45)
                this.value = '00';
        }
    }).on("focus", function () {
        jq(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === jQuery.ui.keyCode.DOWN) {
            event.preventDefault();
            jq(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            jq(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
            //  $.ui.keyCode.ba
        }
    }).on("keyup", function (event) {
        var value = jq(this).val();
        if (value.length > 2) {
            jq(this).val(value.slice(0, 2));
        }
        var inputKeyCode = event.keyCode;
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

    jq('[id *= mValue]').autocomplete({

        source: function (request, response) {
            response(Msuggestions);
        },
        minLength: 0,
        open: function (event, ui) {
            jq(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            jq(this).autocomplete("widget").removeClass("show-all");
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
        jq(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === jQuery.ui.keyCode.DOWN) {
            event.preventDefault();
            jq(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            jq(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            jq(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
        }
    }).on("input", function () {
        var value = jQuery(this).val();
        if (value.length > 2) {
            jq(this).val(value.slice(0, 2));
        }
    });
}

jq(function () {
    autoCompleteTimer()
    jq('[id *= mValue]').val('AM');
});
jq('.datepicker-custom').change(function () {
    jq(this).focus();
});
jq(function () {
    jq(".datepicker-custom").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    });
});
function ReInitializeDatePicker() {
    jq(".datepicker-custom").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    });
};
jq(function () {
    jq(".datepicker-custom-Ar").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    }).datepicker('setDate', 'today');
});
function DeadLineDateTime(id) {

    var chk = validateDateControl(id);
    if (chk != false) {
        var dateStr = document.getElementById(id).value;
        const currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);
        const [month, day, year] = dateStr.split("/");
        const dateObject = new Date(year, month - 1, day); // Month is 0-based in JavaScript Date constructor

        if (dateObject < currentDate) {
            alert("The Date must be greater to today's date")
            document.getElementById(id).value = "";
            jq('#' + id).focus();
            return false;
        }
        var startDate = new Date(dateObject);
        var isWeekday = startDate.getDay() >= 1 && startDate.getDay() <= 5;
        var isWorkingHours = startDate.getHours() >= 8 && startDate.getHours() < 17;
        if (!isWeekday) {
            alert("Copy Center opens between Monday to Friday");
            document.getElementById(id).value = "";
            jQuery('#' + id).focus();
            return false;
        }
        var pub = jQuery('#Publish').val();
        if (pub == 'True' || pub == 'true') {
            jq('#BidDt2').val(jQuery('#HiddenBidDt').val())
            jq('#strBidDt4').val(jQuery('#hdnLastBidDt').val());
            jq('#strBidDt2').val(jQuery('#strBidDt2').val());
            jq('#divPrevBD').css('display', 'block');
        }
    }
};
function validateDateControl(id) {
    var dateStrr = document.getElementById(id).value;
    const inputElement = document.getElementById(id);
    const dateRegex = /^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])\/(?:\d{2}|\d{4})?$|^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])$/;
    const isValid = dateRegex.test(dateStrr);
    // /^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])\/(?:\d{2}|\d{4})?jQuery/

    if (dateStrr != '') {
        if (!isValid) {
            alert("Invalid date/date format. Please use MM/dd/ or MM/dd/yyyy format or check the month date value.");
            document.getElementById(id).value = "";
            jq('#' + id).focus();
            return false;
        }
        else {
            var parts = dateStrr.split("/");

            var formattedValue = (parts[0].length === 1 ? "0" + parts[0] : parts[0]) + "/" +
                (parts[1].length === 1 ? "0" + parts[1] : parts[1]) + "/";
            if (parts[2]) {
                if (parts[2].length === 2) {
                    if (parts[2] == "00") {
                        alert('Year component cannot be zero')
                        jq('#' + id).focus();
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
                jq('#' + id).focus();
                document.getElementById(id).value = "";
                return false;
            }
        }
        else if (monthPart == 1 || monthPart == 3 || monthPart == 5 || monthPart == 7 || monthPart == 8 || monthPart == 10 || monthPart == 12) {
            if (dayPart > 31) {
                alert('You can not enter date more than 31 for month of January, March, May, July, August, October and December');
                jq('#' + id).focus();
                document.getElementById(id).value = "";
                return false;
            }
        }
        else if (monthPart == 2) {
            const leap = checkLeapYear(yearPart);
            if (leap) {
                if (dayPart > 29) {
                    alert('You can not enter date more than 29 for month of February.');
                    jq('#' + id).focus();
                    document.getElementById(id).value = "";
                    return false;
                }
            }
            else {
                if (dayPart > 28) {
                    alert('You can not enter date more than 28 for month of February.');
                    jq('#' + id).focus();
                    document.getElementById(id).value = "";
                    return false;
                }
            }
        }
        else {
            alert('Date is not correct Please check date.');
            jq('#' + id).focus();
            document.getElementById(id).value = "";
            return false;
        }
    }
};