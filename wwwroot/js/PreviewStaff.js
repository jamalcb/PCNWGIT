
var jq = $.noConflict();

var $loading = jq('#loader-overlay').hide();
jQuery(document)
    .ajaxStart(function () {
        $loading.show();
    })
    .ajaxStop(function () {
        $loading.hide();
    });

jQuery(document).ready(function () {
    jq("#member").hide();
    var currentUrl = window.location.href;
    var updatedUrl = currentUrl.split('?')[0];
    window.history.replaceState({}, document.title, updatedUrl);
    var count = jQuery("h6[class*='extraBid']").length;
    if (count > 0) {
        jQuery('.bid-preview').text('Note: Primes requests bids at various dates and times.')
    }
    GetDeliverOptions();
    var loadChk = jQuery('#LoadChk').val();
    if (loadChk != 'NoValue') {
        if (loadChk == 'success') {
            $(".modalthanks").modal('show');
            jQuery('#uploadCheck').val('N');
            jQuery("#hdnFileName").val('');
            jQuery('#LoadChk').val('NoValue');
        }
        else {
            alert(loadChk);
        }
    }
    jQuery("#chkAllPlans").click(function () {
        jQuery(".chkPlan").prop('checked', jq(this).prop('checked'));
        jQuery(".chk-td").prop('checked', true);
    });
    jQuery(".chk-td").change(function () {
        if (!jQuery(this).prop("checked")) {
            jQuery("#chkAllPlans").prop("checked", false);
        }
        else {
            var allChk = document.querySelectorAll('.chkPlan');
            var fl = 0;
            for (i = 0; i < allChk.length; i++) {
                var chkVal = allChk[i].checked;
                if (chkVal)
                    fl++;
            }

            if (fl == allChk.length) {
                jQuery("#chkAllPlans").prop("checked", true);
            }
        }
    });
    jQuery(".chkPlan").change(function () {
        if (!jQuery(this).prop("checked")) {
            jQuery("#chkAllPlans").prop("checked", false);
        }
        else {
            var allChk = document.querySelectorAll('.chkPlan');
            var fl = 0;
            for (i = 0; i < allChk.length; i++) {
                var chkVal = allChk[i].checked;
                if (chkVal)
                    fl++;
            }

            if (fl == allChk.length) {
                jQuery("#chkAllPlans").prop("checked", true);
            }
        }
    });

    jQuery("#chkAllSpecs").click(function () {
        jQuery(".chkSpec").prop('checked', jq(this).prop('checked'));
    });
    jQuery(".chkSpec").change(function () {
        if (!jQuery(this).prop("checked")) {
            jQuery("#chkAllSpecs").prop("checked", false);
        }
        else {
            var allChk = document.querySelectorAll('.chkSpec');
            var fl = 0;
            for (i = 0; i < allChk.length; i++) {
                var chkVal = allChk[i].checked;
                if (chkVal)
                    fl++;
            }

            if (fl == allChk.length) {
                jQuery("#chkAllSpecs").prop("checked", true);
            }
        }
    });

    jQuery("#chkAddendum").click(function () {
        // Check if the checkbox is checked
        if ($(this).is(":checked")) {
            // Set a value or perform any action on the OrderAllAddenda input
            jQuery(".chkall").prop("checked", true);
            jQuery(".chkAddenda").prop('checked', jq(this).prop('checked'));
            jQuery(".chkall").addClass('chkAddenda');
        } else {
            // Handle the case when the checkbox is not checked
            jQuery(".chkall").prop("checked", false);
            jQuery(".chkAddenda").prop('checked', jq(this).prop('checked'));
            jQuery(".chkall").removeClass('chkAddenda');
        }
    });
    functionChkAddenda();
});
function functionChkAddenda() {
    jQuery(".chkAddenda").change(function () {
        if (!jQuery(this).prop("checked")) {
            jQuery("#chkAddendum").prop("checked", false);
        }
        else {
            var allChk = document.querySelectorAll('.chkAddenda');
            var fl = 0;
            for (i = 0; i < allChk.length; i++) {
                var chkVal = allChk[i].checked;
                if (chkVal)
                    fl++;
            }
            if (fl == allChk.length) {
                jQuery("#chkAddendum").prop("checked", true);
            }
        }
    });
}

function ShowCard(Id, ConId) {
    jq("#frmPrintFormMem").trigger('reset');
    jq('#frmPrintFormMem').find("input[type=text], textarea").val('');
    jq('#frmPrintFormMem').find("label").html('');
    jq("#frmPrintFormMem").find("span#message").html('');
    jq.ajax({
        type: "POST",
        dataType: 'json',
        url: '/Project/ShowCard/',
        data: { 'Id': Id, 'ConId': ConId },
        async: false,
        success: function (response) {
            if (response.success) {
                jq('#lblCompany').html(response.data.Company);
                jq('#lblMail').html(response.data.Email);
                jq('#lblContact').html(response.data.ContactName);
                jq('#lblPhone').html(response.data.ContactPhone);
                console.log(response.data);
            }
            else {
                jq('#message').fadeIn();
                jq('#message').text(response.statusMessage).fadeOut(5000);
            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
    $('.li-modal').modal('show');
}
function ShowPHL(e) {
    $('.m-model').modal('show');
}
var filehtml = '';

function OpenPrintModel1(ctrl) {
    jQuery(".OrderAllCopies").css("display", "block");
    jQuery("#prev-btn").click();
    jQuery("#frmPrintForm").trigger('reset');
    jQuery('#frmPrintForm').find("input[type=text], input[type=hidden], input[type=File], textarea").val('');
    jQuery('#inlineRadio1').prop('checked', true);
    jQuery('#fileUpload1').removeAttr('disabled');
    jQuery('#btnfileUpload1').removeAttr('disabled');
    jQuery('span.loginError').html('');
    var model = [];
    setTimeout(() => {
        if (ctrl == 'Plan') {
            var count = document.querySelectorAll('.chkPlan:checked').length;
            if (count == 0) {
                alert('Please select file(s)');
                jQuery(".OrderAllCopies").css("display", "none");
                return false;
            }
            jQuery('.chkPlan').each(function (idx, elem) {
                if (jq(this).prop('checked')) {
                    var orderAllClassValue = jq(this).siblings('input.OrderAllPlans').val();
                    const myArray = orderAllClassValue.split("$");
                    var classModel = {};
                    classModel.PathInfo = myArray[0];
                    classModel.FileInfo = myArray[1];
                    model.push(classModel)
                }
            });
        }
        else if (ctrl == 'Specs') {
            var count = document.querySelectorAll('.chkSpec:checked').length;
            if (count == 0) {
                alert('Please select file(s)');
                jQuery(".OrderAllCopies").css("display", "none");
                return false;
            }
            jQuery('.chkSpec').each(function (idx, elem) {
                if (jq(this).prop('checked')) {
                    var orderAllClassValue = jq(this).siblings('input.OrderAllSpecs').val();
                    const myArray = orderAllClassValue.split("$");
                    var classModel = {};
                    classModel.PathInfo = myArray[0];
                    classModel.FileInfo = myArray[1];
                    model.push(classModel)
                }
            });
        }
        else if (ctrl == 'Addenda') {
            var count = document.querySelectorAll('.chkAddenda:checked').length;
            if (count == 0) {
                alert('Please select file(s)');
                jQuery(".OrderAllCopies").css("display", "none");
                return false;
            }
            jQuery('.chkAddenda').each(function (idx, elem) {
                if (jq(this).prop('checked')) {
                    var orderAllClassValue = jq(this).siblings('input.OrderAllAddenda').val();
                    const myArray = orderAllClassValue.split("$");
                    var classModel = {};
                    var PathInfo = myArray[0];
                    var FileInfo = myArray[1];
                    var fileExtension = FileInfo.split('.').pop();
                    if (fileExtension == "pdf") {
                        classModel.PathInfo = PathInfo;
                        classModel.FileInfo = FileInfo;
                        model.push(classModel)
                    }

                }
            });
        }
        jq.ajax({
            type: "POST",
            dataType: 'json',
            url: '/StaffAccount/OrderCopiesAll/',
            data: { 'model': model },
            async: false,
            success: function (response) {
                if (response.Status == "success") {
                    filehtml = '';
                    jQuery(response.DirData).each(function (idx, elem) {
                        filehtml += '<tr class="trUpload"><td class="tdUpoadFile">' + elem.FileName + '</td><td class="tdUpload">' + elem.NoP + '</td><td><input type="text" class="form-control cus-upload-input"/><span class="loginError"></span></td><td><select class="form-control change-size"><option value="1">B & W</option><option value="2">Color</option> </select><span class="loginError"></span></td><td><select class="form-control cus-upload-select text-right black-select"><option value="0" hidden="">-Select-</option>' + optionHtml + '<option value="">Custom</option> </select><select class="form-control cus-upload-select color-select"><option value="0" hidden="">-Select-</option>' + ColoroptionHtml + '<option value="">Custom</option> </select><span class="loginError"></span></td></tr>';
                    });
                    jQuery("#hdnFileName").val(response.data);
                    DisplayFiles();
                }
                else {
                    jQuery('#message').fadeIn();
                    jQuery('#message').text(response.statusMessage).fadeOut(5000);

                }
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }, 0);
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
//function OpenPrintModelAll() {
//    jQuery(".OrderAllCopies").css("display", "block");
//    jQuery("#prev-btn").click();
//    jQuery("#frmPrintForm").trigger('reset');
//    jQuery('#frmPrintForm').find("input[type=text], input[type=hidden], input[type=File], textarea").val('');
//    jQuery('#btnfileUpload1').removeAttr('disabled');
//    jQuery('span.loginError').html('');

//    var model = [];
//    setTimeout(() => {
//    jQuery('.OrderAllClass').each(function (idx, elem) {
//        var PathFile = jq(this).val();
//        const myArray = PathFile.split("$");
//        var classModel = {};
//        classModel.PathInfo = myArray[0];
//        classModel.FileInfo = myArray[1];
//        model.push(classModel)
//    });
//    jq.ajax({
//        type: "POST",
//        dataType: 'json',
//        url: '/StaffAccount/OrderCopiesAll/',
//        data: { 'model': model },
//        async: false,
//        success: function (response) {
//            if (response.Status == "success") {
//                filehtml = '';
//                jQuery(response.DirData).each(function (idx, elem) {
//                    console.log(idx);
//                    console.log(elem);
//                    filehtml += '<tr class="trUpload"><td class="tdUpoadFile">' + elem.FileName + '</td><td class="tdUpload">' + elem.NoP + '</td><td><input type="text" class="form-control cus-upload-input"/><span class="loginError"></span></td><td><select class="form-control change-size"><option value="1">B & W</option><option value="2">Color</option> </select><span class="loginError"></span></td><td><select class="form-control cus-upload-select text-right black-select"><option value="0" hidden="">-Select-</option>' + optionHtml + '<option value="">Custom</option> </select><select class="form-control cus-upload-select color-select"><option value="0" hidden="">-Select-</option>' + ColoroptionHtml + '<option value="">Custom</option> </select><span class="loginError"></span></td></tr>';
//                });
//                jQuery("#hdnFileName").val(response.data);
//                DisplayFiles();

//            }
//            else {
//                jQuery('#message').fadeIn();
//                jQuery('#message').text(response.statusMessage).fadeOut(5000);

//            }
//        },
//        error: function (response) {
//            alert(response.responseText);
//        },
//        failure: function (response) {
//            alert(response.responseText);
//        }
//    });
//    }, 0);
//}

function OpenPrintModelAll() {
    jQuery(".OrderAllCopies").css("display", "block");
    jQuery("#prev-btn").click();
    jQuery("#frmPrintForm").trigger('reset');
    jQuery('#frmPrintForm').find("input[type=text], input[type=hidden], input[type=File], textarea").val('');
    jQuery('#btnfileUpload1').removeAttr('disabled');
    jQuery('span.loginError').html('');
    var model = [];
    setTimeout(() => {
    jQuery(".chkPlan:checked").each(function () {
        var orderAll = jQuery(this).closest("tr").find(".OrderAllPlans").val();
        const myArray = orderAll.split("$");
        var classModel = {};
        classModel.PathInfo = myArray[0];
        classModel.FileInfo = myArray[1];
        model.push(classModel)
    });
    jq.ajax({
        type: "POST",
        dataType: 'json',
        url: '/StaffAccount/OrderCopiesAll/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.Status == "success") {
                filehtml = '';
                jQuery(response.DirData).each(function (idx, elem) {
                    console.log(idx);
                    console.log(elem);
                    filehtml += '<tr class="trUpload"><td class="tdUpoadFile">' + elem.FileName + '</td><td class="tdUpload">' + elem.NoP + '</td><td><input type="text" class="form-control cus-upload-input"/><span class="loginError"></span></td><td><select class="form-control change-size"><option value="1">B & W</option><option value="2">Color</option> </select><span class="loginError"></span></td><td><select class="form-control cus-upload-select text-right black-select"><option value="0" hidden="">-Select-</option>' + optionHtml + '<option value="">Custom</option> </select><select class="form-control cus-upload-select color-select"><option value="0" hidden="">-Select-</option>' + ColoroptionHtml + '<option value="">Custom</option> </select><span class="loginError"></span></td></tr>';
                });
                jQuery("#hdnFileName").val(response.data);
                DisplayFiles();

            }
            else {
                jQuery('#message').fadeIn();
                jQuery('#message').text(response.statusMessage).fadeOut(5000);

            }
        },
        error: function (response) {
            alert(response.responseText);

        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
    }, 0);
}
function DisplayFiles() {
    setTimeout(function () {
        jq('#ftbody').html('');
        jq('#ftbody').html(filehtml);
        jq('.price-body').html('');
        jq('.price-body').html(priceHtml);
        jq('.nprice-body').html('');
        jq('.nprice-body').html(priceNHtml);
        jQuery('.color-price-body').html('');
        jQuery('.color-price-body').html(ColorpriceHtml);
        jQuery('.color-nprice-body').html('');
        jQuery('.color-nprice-body').html(ColorpriceNHtml);
        jq('.divUpload').removeClass("d-none");
        $('.m-modal').modal('show');
        jQuery(".OrderAllCopies").css("display", "none"); 
        jQuery(".cus-upload-input").val(1); 
    }, 1000);
}
var totalPages = 0;
var totalCopy = 0;
var totalPrice = 0;
var obj = [];
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
function SaveUploadInfo(e) {
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
        item.price = price;
        obj.push(item);
        totalPages = totalPages + parseInt(pages);
        totalCopy = totalCopy + parseInt(copy) * parseInt(pages);
        jq('#inpPages').val(totalPages);
        jq('#inpCopy').val(totalCopy);
        jQuery('#inpPages').next('span').html('');
        jQuery('#inpCopy').next('span').html('');

    });
    //jq('.prcTbody').html('');
    jq(".prcTbody tr").remove();
    for (var i = 0; i < obj.length; i++) {
        console.log(obj[i]);
        console.log(obj[i].Filename);
        uploadHtml += '<tr class="prc-row" id="prc-row"><td class="">' + (i + 1) + '</td><td class="prc-name" id="prc-name"> ' + obj[i].Filename + '</td><td class="prc-page" id="prc-page">' + obj[i].pages + '</td><td class="prc-copy" id="prc-copy">' + obj[i].copy + '</td><td class="prc-size" id="prc-size">' + obj[i].size + '</td><td class="prc-printsize" id="prc-printsize">' + obj[i].PrintSelect + '</td><td class="prc-price" id="prc-price">' + obj[i].price + '</td><td class="prc-total text-right border" id="prc-total">' + (parseFloat(obj[i].price) * parseFloat(obj[i].copy) * parseFloat(obj[i].pages)).toFixed(2) + '</td></tr > ';
        totalPrice = totalPrice + (parseFloat(obj[i].price) * parseFloat(obj[i].copy) * parseFloat(obj[i].pages));
    }
    console.log(totalPrice);
    jQuery('#hdnTotalPrice').val(totalPrice);
    jq('.prcTbody').html(uploadHtml);
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
jQuery("#next-btn").click(() => {
    var res = validateform("next");
    if (res == false)
        return false;

    var Del = jQuery("#inpDelivery").children("option").filter(":selected").val()
    if (Del == 1)
        jQuery("input[name=HowShip]").val('Pickup');
    if (Del == 2)
        jQuery("input[name=HowShip]").val('Local Delivery');
    if (Del == 3)
        jQuery("input[name=HowShip]").val('UPS');
    jQuery('#lblModCompany').text(jQuery('#inpCompany').val());
    var member = jQuery('#memberid').val();
    var member1 = jQuery("#inpnonmember").val()
    if (member == '1') {

        jQuery('#lblModContact').text(jQuery("#inpContact").children("option").filter(":selected").text());
        jQuery('#ModUid').val(jQuery("#inpContact").children("option").filter(":selected").val());

    }
    else {
        jQuery('#lblModContact').text(jQuery("#inpnonmember").val());
    }
    jQuery('#lblModBillAddr').text(jQuery('#inpAddr').val());
    jQuery('#lblModCity').text(jQuery('#inpCity').val());
    jQuery('#lblModState').text(jQuery("#inpState").children("option").filter(":selected").text());
    jQuery('#lblModDelState').text(jQuery("#inpDelState").children("option").filter(":selected").text() == '--Select State--' ? '' : jQuery("#inpDelState").children("option").filter(":selected").text());
    jQuery('#lblModDelivery').text(jQuery("#TextHowShip").val());
    jQuery('#lblModZip').text(jQuery('#inpZip').val());
    jQuery('#lblModPhone').text(jQuery('#inpPhone').val());
    jQuery('#lblModEmail').text(jQuery('#inpEmail').val());
    jQuery('#lblModProjNo').text(jQuery('#inpProjNo').val());
    jQuery('#lblModPO').text(jQuery('#inpPO').val());
    jQuery('#lblModPages').text(jQuery('#inpPages').val());
    jQuery('#lblModCopy').text(jQuery('#inpCopy').val());
    jQuery('#lblModDLine').text(jQuery('#inpDline').val());
    jQuery('#lblModDelAddr').text(jQuery('#inpDelAddr').val());
    jQuery('#lblModDelCity').text(jQuery('#inpDelCity').val());
    jQuery('#lblModDelZip').text(jQuery('#inpDelZip').val());
    jQuery('#lblModDelInstruction').text(jQuery('#inpInstruction').val());
    //jQuery('#lblModPlan').text(jQuery('#hdnFileName').val());
    //jQuery('#lblModDelInstruction').text(jQuery('#inpInstruction').val());
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
    jQuery('.review-form').css("display", "none") &&
        e.preventDefault();
    jQuery("#next-btn").show() &&
        jQuery('.order-form').css("display", "block") &&
        jQuery("#orderform-head").css("display", "block") &&
        jQuery("#ordersummary-head").css("display", "none")
})

function DelFunction() {
    var checkBox = document.getElementById("chkDel");
    if (checkBox.checked == true) {
        jq("#inpDelAddr").val(jQuery('#inpAddr').val());
        jq("#inpDelCity").val(jQuery('#inpCity').val());
        jQuery("#inpDelState option:selected").text(jQuery("#inpState").children("option").filter(":selected").text());
        jq("#inpDelZip").val(jQuery('#inpZip').val());
    }
}
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
            jQuery('#inpDline').next('span').html('Please choose deadline date.');
            isValide = false;
        }
        var inpDelivery = jQuery('select[id=inpDelivery] option').filter(':selected').val();
        jQuery('#inpDelivery').next('span').html('');
        if (inpDelivery == undefined || inpDelivery == '' || inpDelivery == null) {
            jQuery('#inpDelivery').next('span').html('Please select delivery.');
            isValide = false;
        }
        if (inpDelivery != 1 && inpDelivery != 2 && inpDelivery != 3 && inpDelivery != 4) {
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
                //if (pInstruction == undefined || pInstruction == '' || pInstruction == null) {
                //    textAreaCtrl.parent('div').parent('div').find('span').html('Please enter additional instructions.');
                //    isValide = false;
                //}
            }
            return isValide;
        }

    }
}
function trimfield(str) {
    return str.replace(/^\s+|\s+$/g, '');
}
function submitData(e, ProjId) {
    var PayMode = jQuery("#inpPayInfo").val();
    if (PayMode == '1') {
        $(".m-modal").modal('hide');
        $(".modalPay").modal('show');
    }
    else {
        PayData(ProjId);
    }

}
function PayData(ProjId) {
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
    // assign datalist
    model.GetTblProjs = ProjOrderDet; console.log(model.GetTblProjs);
    // Main model
    model.Company = jQuery("label[name=Company]").text();
    model.Name = jQuery("label[name=Name]").text();
    model.Addr = jQuery("label[name=Addr]").text();
    model.City = jQuery("label[name=City]").text();
    model.State = jQuery("label[name=State]").text();
    model.Zip = jQuery("label[name=Zip]").text();
    model.Phone = jQuery("label[name=Phone]").text();
    model.Email = jQuery("label[name=Email]").text();
    model.ShipAmt = parseFloat(jQuery("td[id=priceTotal]").text().replace('$', ''));
    model.ProjId = jQuery("label[name=ProjId]").text();
    model.PO = jQuery("label[name=PO]").text();
    model.Prints = jQuery("label[name=Prints]").text();
    model.DeliveryDt = jQuery("label[name=DeliveryDt]").text();
    model.HowShip = jQuery("input[name=HowShip]").val();
    model.NotifyAddress = jQuery("label[name=NotifyAddress]").text();
    model.Instructions = jQuery("label[name=Instructions]").text();
    model.FileName = jQuery("label[id=FileName]").text();
    model.PayStatus = 'N';
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
    var isMember = jQuery('#memberid').val();
    if (isMember == '1') {
        model.NonMember = false;
    }
    else {
        model.NonMember = true;
    }
    var PayMode = jQuery("#inpPayInfo").val();
    if (PayMode == '1') {
        model.PaymentMode = 'CC';
    }
    else {
        model.PaymentMode = 'Invoice';
    }
    model.OrderChrgId = jQuery('#hdninpCompany').val();
    model.ProjOrderId = ProjId;
    model.UID = jQuery("#ModUid").val();
    var pathChK = jQuery("#hdnFileName").val();
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/App/InitiateAuthCopyCenter/',
        data: { 'model': model, 'pathChK': pathChK, 'ContPath': 'StaffAccount', 'ActPath': 'Preview', 'SuccessPath': 'SaveCopyCenterInfo' },
        async: false,
        success: function (response) {
            if (response.Status = 'success') {
                window.location.href = response.Data;
            }
            else {
                alert("There's been issue with payment please try again");
                jQuery(".modalPay").modal('hide');
            }
            //jQuery(".modalPay").modal('hide');
            //jQuery(".modalthanks").modal('show');
            //jQuery('#uploadCheck').val('N');
            //jQuery("#hdnFileName").val('');
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
    var term = ''
    var ctrl = ''
    $("#inpCompany").autocomplete({

        source: function (request, response) {
            term = request.term;
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
                    ctrl = input;
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
        minLength: 1
    }).focus(function () {
        $(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            //         const selObj = window.getSelection();
            ctrl.setSelectionRange(term.length - 1, jq(this).val().length)
            //                    alert(selObj);
            //             console.log(selObj);
        }
    });
    // Onchange events
    $("#inpCompany").on('change', function () {
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
    //console.log(lstTemp);
});
var contactPhone = document.getElementById('inpPhone');
var result = document.getElementById('inpPhone');

contactPhone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
jq("body").on("click", "#printPHL", function () {
    html2canvas(jq('#pdf1')[0], {
        quality: 4,
        onrendered: function (canvas) {
            var data = canvas.toDataURL();
            var docDefinition = {
                content: [{
                    image: data,
                    width: 500
                }]
            };
            pdfMake.createPdf(docDefinition).download("List.pdf");
        }
    });
});
jq("body").on("click", "#phlpdf", function () {
    html2canvas(jq('#pdf1')[0], {
        quality: 4,
        onrendered: function (canvas) {
            var data = canvas.toDataURL();
            var docDefinition = {
                content: [{
                    image: data,
                    width: 500
                }]
            };
            pdfMake.createPdf(docDefinition).download("List.pdf");
        }
    });
});
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
        if (jq('#memberid').val() == 1)
            price = jq('#hdnsz_' + price).val();
        else {
            price = jq('#hdnszn_' + price).val();
        }
        item.price = price;
        obj.push(item);
        totalPages = totalPages + parseInt(pages);
        totalCopy = totalCopy + parseInt(copy) * parseInt(pages);
        jQuery('#selectCheck').val('Y');

    });
    jq(".prcTbody tr").remove();
    for (var i = 0; i < obj.length; i++) {

        uploadHtml += '<tr class="prc-row" id="prc-row"><td class="prc-name" id="prc-name">' + obj[i].Filename + '</td><td class="prc-page" id="prc-page">' + obj[i].pages + '</td><td class="prc-copy" id="prc-copy">' + obj[i].copy + '</td><td class="prc-size" id="prc-size">' + obj[i].size + '</td><td class="prc-printsize" id="prc-printsize">' + obj[i].PrintSelect + '</td><td class="prc-price" id="prc-price">' + obj[i].price + '</td><td class="prc-total" id="prc-total">' + (parseFloat(obj[i].price) * parseFloat(obj[i].copy) * parseFloat(obj[i].pages)).toFixed(2) + '</td></tr>';
        totalPrice = totalPrice + (parseFloat(obj[i].price) * parseFloat(obj[i].copy) * parseFloat(obj[i].pages));
    }

    jQuery('#hdnTotalPrice').val(totalPrice);
    jq('.prcTbody').html(uploadHtml);
    return isCorrect;
}
function GetDeliverOptions() {
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
                        var option = jQuery('<option value="' + datalist[key][i].DelivOptId + '">' + datalist[key][i].DelivOptName + '</option>');
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
}
jQuery('#inpDelivery').on('change', function () {
    var id = jQuery('#inpDelivery').val();
    var text = jQuery('select[id=inpDelivery] option').filter(':selected').text();
    jQuery('#HowShip').val(id);
    jQuery('#TextHowShip').val(text);
});
jQuery(document).ready(function () {
    jQuery('.pdf.file-name').click(function () {
        jQuery(this).siblings('.tbl-chevron').toggleClass('rotated');
    });
    jQuery('.toggle-row').click(function (e) {
        e.preventDefault(); // Prevent the default link behavior
        var targetId = jQuery(this).attr('href'); // Get the target <tr> ID
        var targetClass = targetId.replace('#', ".");
        var allChk = document.querySelectorAll(targetClass);
        var fl = 0;
        jQuery(targetClass).toggle();
        var lastChar = targetId.substring((targetId.indexOf('_') + 1), targetId.length);
        var inpClass = 'chkAddenda_' + lastChar;
        for (i = 0; i < allChk.length; i++) {
            var element = allChk[i];
            var status = element.getAttribute('style');
            if (status == 'display: table-row;') {
                $($(element).children('td:eq(3)')).children('input.' + inpClass).addClass('chkAddenda');
                functionChkAddenda();
            }
            else if (status == 'display: none;') {
                $($(element).children('td:eq(3)')).children('input.' + inpClass).removeClass('chkAddenda');
                functionChkAddenda();
            }
        }
    });

    jQuery('.chk-td').on('change', function (e) {
        e.preventDefault(); // Prevent the default link behavior
        var id = jQuery(this).attr('id');
        var targetId = jQuery(this).attr('href'); // Get the target <tr> ID
        var targetClass = targetId.replace('#', ".");
        var allChk = document.querySelectorAll(targetClass);
        var fl = 0;
        var lastChar = targetId.substring((targetId.indexOf('_') + 1), targetId.length);
        var inpClass = 'chkAddenda_' + lastChar;
        for (i = 0; i < allChk.length; i++) {
            var element = allChk[i];
            if (jQuery(this).prop("checked")) {
                $($(element).children('td:eq(3)')).children('input.' + inpClass).addClass('chkAddenda');
                $($(element).children('td:eq(3)')).children('input.' + inpClass).prop("checked", true);
                functionChkAddenda();
                $('#' + id).prop("checked", true);
            } else {
                $($(element).children('td:eq(3)')).children('input.' + inpClass).removeClass('chkAddenda');
                $($(element).children('td:eq(3)')).children('input.' + inpClass).prop('checked', false);
                functionChkAddenda();
                $('#' + id).prop("checked", false);
            }
        }
    });
})

function ChangeParentChkBox(chk, id) {
    var unchk = document.querySelectorAll('#' + id);
    var lastChar = id.substring((id.indexOf('_') + 1), id.length);
    var RootId = 'multiCollapseExample_' + lastChar;
    var fl = 0;
    if (chk) {
        for (i = 0; i < unchk.length; i++) {
            var chkVal = unchk[i].checked;
            if (chkVal) {
                fl++;
            }
            else {
                jQuery('#' + RootId).prop("checked", false);
            }
        }
        if (fl == unchk.length) {
            jQuery('#' + RootId).prop("checked", true);
        }
    }
    else {
        jQuery('#' + RootId).prop("checked", false);
    }
}
function GOTOMap(address) {
    // Open a new window with the map URL based on the provided address
    var mapUrl = 'https://maps.google.com/maps?q=' + encodeURIComponent(address);
    window.open(mapUrl, '_blank');
}