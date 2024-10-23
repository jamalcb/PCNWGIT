var jq = $.noConflict();
//Populating multiple counties modal
// Get the current URL
var currentUrl = window.location.href;

// Remove the 'returnUrl' parameter from the URL
var updatedUrl = currentUrl.split('?')[0];

// Update the URL without the 'returnUrl' parameter
window.history.replaceState({}, document.title, updatedUrl);
jq(document).ready(function () {
    PopulateModal();
    // function for adding $, and comma
    let inputElements = document.querySelectorAll('input[id$="EstFrom"], input[id$="EstTo"]');
    inputElements.forEach(function (input) {
        formatNumberInput(input);
    });
    // Get the form element
    var myForm = document.getElementById('EditForm');

    // Add event listener to prevent form submission on Enter key press
    myForm.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault();
        }
    });
});
jq(document).ready(function () {
    jq(".datepicker-custom-Ar").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    }).datepicker('setDate', 'today');
});
jQuery(function ($) {
    function toggleLocalStorageItem(itemName) {
        localStorage.getItem(itemName) === "true" ? localStorage.removeItem(itemName) : localStorage.setItem(itemName, "true");
    }

    function applyToggle(item, div, h2) {
        if (localStorage.getItem(item) === "true") {
            $('.' + h2).addClass("rotate");
            $('.' + div).css({
                'display': 'block',
                'max-height': 'fit-content'
            });
        }
    }

    if (!localStorage.getItem('projectInfo') && !localStorage.getItem('addenda') && !localStorage.getItem('planHoldersList')) {
        toggleLocalStorageItem('projectInfo');
    }

    $(".toggle-option, .toggle-addenda, .toggle-holder").removeClass("rotate");
    $(".pro-info, .addenda, .holder").hide();

    applyToggle('projectInfo', 'pro-info', 'toggle-option');
    applyToggle('addenda', 'addenda', 'toggle-addenda');
    applyToggle('planHoldersList', 'holder', 'toggle-holder');

    $(".toggle-option, .toggle-addenda, .toggle-holder").click(function () {
        var itemName = $(this).hasClass("toggle-option") ? "projectInfo" : $(this).hasClass("toggle-addenda") ? "addenda" : "planHoldersList";
        toggleLocalStorageItem(itemName);
    });
});


jQuery(document).ready(function () {
    jQuery('.toggle-option.edit-Info').click(function () {
        jQuery('.cst-optional.pro-info').slideToggle(300);
        jQuery('.cst-optional.pro-info').css('max-height', 'fit-content');
        jQuery(this).toggleClass('rotate');
    });
});
jQuery(document).ready(function () {
    jQuery('.toggle-addenda').click(function () {
        jQuery('.cst-optional.addenda').slideToggle(300);
        jQuery('.cst-optional.addenda').css('max-height', 'fit-content');
        jQuery('.toggle-addenda').toggleClass('rotate');
    });
});
jQuery(document).ready(function () {
    jQuery('.toggle-holder').click(function () {
        jQuery('.cst-optional.holder').slideToggle(300);
        jQuery('.cst-optional.holder').css('max-height', 'fit-content');
        jQuery(this).toggleClass('rotate');
    });
});
function PopulateModal() {
    var stateHtml = '';
    var ORhtml = '';
    var WAhtml = '';
    var Ohtml = '';
    var counties = jq('#Counties').val();
    var myArray = [];
    if (counties) {
        myArray = counties.split(",");
    }
    console.log(myArray);
    //  jq('.classCounty').html('')
    jq.ajax({
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
                var AllOr = jq.inArray('-1', myArray);
                if (AllOr !== -1) {
                    jq.each(model, function (index, item) {
                        ORhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                            + '<input type="checkbox" class="filled-in form-check-input classMOcounties" value="' + item.CountyId + '" checked onchange="OrCheck(this.checked)">'
                            + '<label class="form-check-label">' + item.County + '</label></div>'
                    });
                }
                else {
                    jq.each(model, function (index, item) {
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
                var AllWa = jq.inArray('-2', myArray);
                if (AllWa !== -1) {
                    jq.each(WModel, function (index, item) {
                        WAhtml += '<div class="col-md-2 m-2 form-check form-check-inline">'
                            + '<input type="checkbox" class="filled-in form-check-input classMWcounties" value="' + item.CountyId + '" checked onchange="WaCheck(this.checked)">'
                            + '<label class="form-check-label">' + item.County + '</label></div>'
                    });
                }
                else {
                    jq.each(WModel, function (index, item) {
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
                jq.each(OModel, function (index, item) {
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
            jq('.classCounty').html(stateHtml);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
};
//Get Issue date for Addenda
function fillHiddenField(e) {
    var date = jq('input[id=' + e + ']').val();
    jq('input[id=' + e + ']').next('input').val(date);
}
//Checking if data is not uppdated
jq(document).ready(function () {
    TempDataValue = jQuery('#tmpHiddenMsg').val();
    if (TempDataValue != '') {
        alert(TempDataValue)
    }
    SuccessValue = jQuery('#hdnSuccessMsg').val();
    if (SuccessValue != '') {
        alert(SuccessValue)
    }
});
//Arrivaldt calendar
jq(function () {
    jq(".datepicker-custom-Ar").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    }).datepicker('setDate', 'today');
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

//Bid date design
jq(function () {
    jq(".datepicker").datepicker({
        showOn: "button",
        buttonImage: "../../images/date-img.png",
        buttonImageOnly: true,
        buttonText: "Select Date",
        defaultDate: new Date(2022, 11, 01)
    });
    jq(".datepicker1").datepicker();
});
var valProjTypeId = 0;
//Autofill from project type input
function autofill() {
    var term = jq("#ProjTypeIdString").val();
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/autofill/',
        data: { 'term': term },
        async: false,
        success: function (response) {
            jq("#ProjTypeIdString").val(response[0].label);
            jq("#ProjTypeId").val(response[0].val);
            valProjTypeId = response[0].val;

        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
//Autocomplete project type input
jq(function () {
    jq("#ProjTypeIdString").autocomplete({
        source: function (request, response) {
            jq.ajax({
                url: '/Project/GetProjectType/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response(jq.map(data, function (item) {
                        return item;
                    }));
                    const input = jq('[id*=ProjTypeIdString]')[0];
                    if (data.length > 0) {
                        const selectedValue = data[0].label; // Choose the first suggestion
                        jq("#ProjTypeIdString").val(selectedValue);
                        jQuery('input[id = ProjTypeId]').val(data[0].val);
                        valProjTypeId = data[0].val;
                        const startIndex = request.term.length;
                        const endIndex = selectedValue.length;
                        // Set the cursor position to overwrite mode
                        input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        jQuery('input[id = ProjTypeId]').val(0);
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
            jQuery('input[id = ProjTypeId]').val(i.item.val);
            valProjTypeId = i.item.val;

            //jq("#ProjTypeId").val();
        },
        change: function (e, i) {
            var chkVal = jq(this).val();
            if (chkVal == '') {
                valProjTypeId = 0;
                jQuery('input[id = ProjTypeId]').val(0);
                jQuery('input[id = ProjSubTypeId]').val(0);
                jQuery('input[id = ProjSubTypeIdString]').val('');
            }
            //jq("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        minLength: 0
    }).focus(function () {
        jq(this).autocomplete("search");
    });
});


function openMemModal(chkId, chkReg) {
    jQuery("#frmPrintForm").trigger('reset');
    jQuery('#frmPrintForm').find("input[type=text]").html('');
    jQuery('#frmPrintForm').find("span.text-danger").html('');
    jQuery('#frmPrintForm').find("input[type=hidden]").val(chkId);
    jQuery('#frmPrintForm').find("input[id=EntityCompany]").val(chkReg);
    jq(".mem-modal").modal('show');
    jq(".mem-modal").on('shown.bs.modal', function () {
        jq("#EntityCompany").focus();
    });

}
function openMemModal1(chkId, chkVal) {
    jQuery("#frmPrintForm1").trigger('reset');
    jQuery('#frmPrintForm1').find("input[type=text]").html('');
    jQuery('#frmPrintForm1').find("span.text-danger").html('');
    jQuery('#frmPrintForm1').find("input[type=hidden]").val(chkId);
    jQuery('#frmPrintForm1').find("input[id=PhlCompany]").val(chkVal);
    jq(".mem-modal1").modal('show');
    jq(".mem-modal1").on('shown.bs.modal', function () {
        jq("#PhlCompany").focus();
    });
}
function openEntModal(chkId, chkReg) {
    jQuery("#frmPrintForm3").trigger('reset');
    jQuery('#frmPrintForm3').find("input[type=text]").html('');
    jQuery('#frmPrintForm3').find("span.text-danger").html('');
    jQuery('#frmPrintForm3').find("input[type=hidden]").val(chkId);
    jQuery('#frmPrintForm3').find("input[id=EntityType]").val(chkReg);
    jq(".ent-modal").modal('show');
    jq(".ent-modal").on('shown.bs.modal', function () {
        jq("#EntityType").focus();
    });
}
function openConModel(chkId, chkReg, chkVal) {
    jQuery("#frmPrintForm5").trigger('reset');
    jQuery('#frmPrintForm5').find("input[type=text]").html('');
    jQuery('#frmPrintForm5').find("span.text-danger").html('');
    jQuery('#frmPrintForm5').find("input[id=refConId]").val(chkId);
    jQuery('#frmPrintForm5').find("input[id=ConFirstName]").val(chkReg);
    jQuery('#frmPrintForm5').find("input[id=refComId]").val(chkVal);
    jq(".con-modal").modal('show');
    jq(".con-modal").on('shown.bs.modal', function () {
        jq("#ConFirstName").focus();
    });
}
var subTypeData = [];

var ProjTypeId = jQuery('input[id = ProjTypeId]').val();
var subTypeArr = new Array();
function Autocomp() {


};



jq("#ProjSubTypeIdString").autocomplete({
    source: function (request, response) {
        jq.ajax({
            url: '/Project/GetProjectSubType/',
            data: { "prefix": request.term, "ProjTypeId": valProjTypeId },
            type: "POST",
            success: function (data) {
                response(jq.map(data, function (item) {
                    return item;
                }))
                //        subTypeArr = data;
                //           allowBackspace(subTypeArr[0].val, subTypeArr[0].label);
                const input = jq('[id*=ProjSubTypeIdString]')[0];
                if (data.length > 0) {
                    //  jq("#ProjSubTypeId").val(data[0].val);
                    //jq("#ProjSubTypeIdString").val(data[0].label);

                    const selectedValue = data[0].label; // Choose the first suggestion
                    const startIndex = request.term.length;
                    const endIndex = selectedValue.length;
                    // Set the cursor position to overwrite mode
                    input.setSelectionRange(startIndex, endIndex);
                }
                else {
                    jQuery('input[id = ProjSubTypeId]').val(0);
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
        jq("#ProjSubTypeId").val(i.item.val);
    },
    change: function (e, i) {
        var chkVal = jq(this).val();
        if (chkVal == '') {
            jQuery('input[id = ProjSubTypeId]').val(0);
        }
        //jq("#hfProjTypeId").val(i.item.val);
        //valProjTypeId = i.item.val;
    },
    minLength: 0
}).focus(function () {
    jq(this).autocomplete("search");
});
function setInputValue(value) {
    // Set the input value programmatically
    jq("#ProjSubTypeIdString").val(value);
};
jq(document).ready(function () {
    jq('#LocZip').keyup(function () {
        var zip = jq(this).val();
        jq('#LocCity').val('');
        jq('#LocState').val('');
        jq('#LocAddr2').val('');
        jq('#CountyId').val('');
        var xmldata = '<CityStateLookupRequest USERID="405PLANC0113"><ZipCode><Zip5>' + zip + '</Zip5></ZipCode></CityStateLookupRequest>';
        //var apiKey = 'YOUR_API_KEY_HERE';
        var apiUrl = 'https://production.shippingapis.com/ShippingAPI.dll?API=CityStateLookup&XML=' + xmldata;
        if (zip.length == 5) {
            jq.ajax({
                url: apiUrl,
                type: 'GET',
                dataType: 'xml',
                success: function (data) {
                    jq(data).find('ZipCode').each(function () {
                        var City = jq(this).find('City').text();
                        var State = jq(this).find('State').text();
                        if (City != null && State != null) {
                            jq.ajax({
                                type: "POST",
                                dataType: 'json',
                                url: '/Project/CheckCounty/',
                                data: { 'City': City, 'State': State },
                                async: false,
                                success: function (response) {
                                    var county = '';
                                    var countyId = 0;
                                    if (response.data != null) {
                                        debugger;
                                        county = response.data.County;
                                        countyId = response.data.CountyId;
                                    }
                                    City = upperCaseFirstLetter(lowerCaseAllWordsExceptFirstLetters(City));
                                    jq('#LocCity').val(City);
                                    jq('#LocState').val(State);
                                    jq('#LocAddr2').val(county);
                                    jq('#CountyId').val(countyId);
                                    jq('#Longitude').val(response.data.Longitude);
                                    jq('#Latitude').val(response.data.Latitude);

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
                            jq('#LocCity').val('');
                            jq('#LocState').val('');
                            jq('#LocAddr2').val('');
                            jq('#CountyId').val('');
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
    });
    jq('#EntityZip').keyup(function () {
        var zip = jq(this).val();
        jq('#EntityCity').val('');
        jq('#EntityState').val('');
        jq('#EntityCounty').val('');
        var xmldata = '<CityStateLookupRequest USERID="405PLANC0113"><ZipCode><Zip5>' + zip + '</Zip5></ZipCode></CityStateLookupRequest>';
        //var apiKey = 'YOUR_API_KEY_HERE';
        var apiUrl = 'https://production.shippingapis.com/ShippingAPI.dll?API=CityStateLookup&XML=' + xmldata;
        if (zip.length == 5) {
            jq.ajax({
                url: apiUrl,
                type: 'GET',
                dataType: 'xml',
                success: function (data) {
                    jq(data).find('ZipCode').each(function () {
                        var City = jq(this).find('City').text();
                        var State = jq(this).find('State').text();
                        if (City != null && State != null) {
                            jq.ajax({
                                type: "POST",
                                dataType: 'json',
                                url: '/Project/CheckCounty/',
                                data: { 'City': City, 'State': State },
                                async: false,
                                success: function (response) {
                                    var county = '';
                                    var countyId = 0;
                                    if (response.data != null) {
                                        debugger;
                                        county = response.data.County;
                                        countyId = response.data.CountyId;
                                    }

                                    var formattedCity = upperCaseFirstLetter(lowerCaseAllWordsExceptFirstLetters(City));
                                    jq('#EntityCity').val(formattedCity);
                                    jq('#EntityState').val(State);
                                    jq('#EntityCounty').val(county);

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
                            jq('#EntityCity').val('');
                            jq('#EntityState').val('');
                            jq('#EntityCounty').val('');
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //console.log(textStatus + ': ' + errorThrown);
                }
            });
        }
    });
    jq('#PhlZip').keyup(function () {
        var zip = jq(this).val();
        jq('#PhlCity').val('');
        jq('#PhlState').val('');
        jq('#PhlCounty').val('');
        var xmldata = '<CityStateLookupRequest USERID="405PLANC0113"><ZipCode><Zip5>' + zip + '</Zip5></ZipCode></CityStateLookupRequest>';
        //var apiKey = 'YOUR_API_KEY_HERE';
        var apiUrl = 'https://production.shippingapis.com/ShippingAPI.dll?API=CityStateLookup&XML=' + xmldata;
        if (zip.length == 5) {
            jq.ajax({
                url: apiUrl,
                type: 'GET',
                dataType: 'xml',
                success: function (data) {
                    jq(data).find('ZipCode').each(function () {
                        var City = jq(this).find('City').text();
                        var State = jq(this).find('State').text();
                        if (City != null && State != null) {
                            jq.ajax({
                                type: "POST",
                                dataType: 'json',
                                url: '/Project/CheckCounty/',
                                data: { 'City': City, 'State': State },
                                async: false,
                                success: function (response) {
                                    var county = '';
                                    var countyId = 0;
                                    if (response.data != null) {
                                        county = response.data.County;
                                        countyId = response.data.CountyId;
                                    }
                                    var formattedCity = upperCaseFirstLetter(lowerCaseAllWordsExceptFirstLetters(City));
                                    jq('#PhlCity').val(formattedCity);
                                    jq('#PhlState').val(State);
                                    jq('#PhlCounty').val(county);

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
                            jq('#PhlCity').val('');
                            jq('#PhlState').val('');
                            jq('#PhlCounty').val('');
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //console.log(textStatus + ': ' + errorThrown);
                }
            });
        }
    });
    InitializeEntName();
    InitializeEntType();
    InitializePhlName();
    InitializePhlCon();
    checkAddenda();
    delSapn();

});
var EntSuggestion = new Array();
function InitializeEntName() {
    var term = '';
    var ctrl = '';
    jq('[id*=EntityName]').autocomplete({
        source: function (request, response) {
            term = request.term;
            var Entid = jq(this.element).prop("id");
            var index = Entid.replace('Entities_', '');
            index = index.replace('__EntityName', '');
            index = parseInt(index);
            jq.ajax({
                url: '/Project/GetEntityName/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    console.log(data);
                    response(jq.map(data, function (item) {
                        return item;
                    }));
                    EntSuggestion = data;
                    const input = jq('[id*=EntityName]')[index];
                    var nameId = Entid.replace('__EntityName', '__NameId');
                    var comId = Entid.replace('__EntityName', '__CompType');
                    if (EntSuggestion.length > 0) {
                        const selectedValue = EntSuggestion[0].label; // Choose the first suggestion
                        jq('#' + nameId).val(EntSuggestion[0].val);
                        jq('#' + comId).val(EntSuggestion[0].CompType);
                        const startIndex = request.term.length;
                        const endIndex = selectedValue.length;
                        jq(input).data('autocomplete-fill', true);
                        // Set the cursor position to overwrite mode
                        jq(input).val(selectedValue);

                        input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        jq(input).data('autocomplete-fill', false);
                        jq('#' + nameId).val('');
                        jq('#' + comId).val('');
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

            jq(this).next('input').val(i.item.val);
            var Entid = jq(this).prop("id");
            var comId = Entid.replace('__EntityName', '__CompType');
            jq('#' + comId).val(i.item.CompType);
            //jq("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        change: function (e, i) {
            var isAutocompleteFill = jq(this).data('autocomplete-fill');
            if (isAutocompleteFill != true && isAutocompleteFill != "true" && isAutocompleteFill != "True") {
                var chkVal = jq(this).val();
                var chkId = jq(this).attr('id');
                var chkReg = jq(this).next('input').val();
                if (chkReg == '') {
                    if (confirm("The company you entered does not exist. Do you want to add new company?")) { openMemModal(chkId, chkVal); }
                    else { jq(this).val(''); }
                }
            }

        },
        minLength: 1,

    }).focus(function () {
        jq(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            ctrl.setSelectionRange(term.length - 1, jq(this).val().length);
        }
    });

}
var EntTypeSuggestion = new Array();
function InitializeEntType() {
    var term = '';
    var ctrl = '';
    var entid = '';
    jq('[id*=EntityTypeString]').autocomplete({
        source: function (request, response) {
            term = request.term;
            var Entid = jq(this.element).prop("id");
            var index = Entid.replace('Entities_', '');
            entid = Entid;
            index = index.replace('__EntityTypeString', '');
            index = parseInt(index);
            jq.ajax({
                url: '/Project/GetEntityType/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response(jq.map(data, function (item) {
                        return item;
                    }));
                    EntTypeSuggestion = data;
                    const input = jq('[id*=EntityTypeString]')[index];
                    ctrl = input;
                    var nameId = Entid.replace('__EntityTypeString', '__EntityType');
                    if (EntTypeSuggestion.length > 0) {
                        const selectedValue = EntTypeSuggestion[0].label; // Choose the first suggestion
                        jq('#' + nameId).val(EntTypeSuggestion[0].val);
                        const startIndex = request.term.length;
                        const endIndex = selectedValue.length;
                        jq(input).data('autocomplete-fill', true);
                        // Set the cursor position to overwrite mode
                        jq(input).val(selectedValue);
                        input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        jq(input).data('autocomplete-fill', false);
                        jq('#' + nameId).val('');
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
            var nameId = entid.replace('__EntityTypeString', '__EntityType');
            jq('#' + nameId).val(i.item.val);
        },
        change: function (e, i) {
            var isAutocompleteFill = jq(this).data('autocomplete-fill');
            if (isAutocompleteFill != true && isAutocompleteFill != "true" && isAutocompleteFill != "True") {
                var chkVal = jq(this).val();
                var chkId = jq(this).attr('id');
                var chkReg = jq(this).next('input').val();
                if (chkReg == '') {
                    if (confirm("The entity type you entered does not exist. Do you want to add new entity type?")) {
                        jq.ajax({
                            type: "POST",
                            dataType: 'json',
                            //contentType: "application/json; charset=utf-8",
                            url: '/Project/SaveEntType/',
                            data: { 'EntityType': chkVal },
                            async: false,
                            success: function (response) {
                                if (response.success == true) {
                                    jq('#' + chkId).val(response.data.EntityType);
                                    jq('#' + chkId).next('input').val(response.data.EntityID);
                                    jq('#' + chkId).focus();
                                }
                                else {
                                    alert('Something went wrong please try again');
                                    jq('#' + chkId).val('');
                                    jq('#' + chkId).focus();
                                }
                            },
                            error: function (response) {
                                alert('Something went wrong please try again');
                                alert(response.responseText);
                                jq('#' + chkId).focus();
                            },
                            failure: function (response) {
                                alert('Something went wrong please try again');
                                alert(response.responseText);
                                jq('#' + chkId).focus();
                            }
                        });
                        //openEntModal(chkId, chkVal);
                    }
                    else { jq(this).val(''); }
                }
            }

        },
        minLength: 0
    }).focus(function () {
        jq(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            //         const selObj = window.getSelection();
            ctrl.setSelectionRange(term.length - 1, jq(this).val().length);
            //                    alert(selObj);
            //             console.log(selObj);
        }
    });


}
var PhlSuggestion = new Array();
function InitializePhlName() {
    var term = '';
    var ctrl = '';
    jq('input[id$="__Company"]').autocomplete({
        source: function (request, response) {
            console.log(request.term);
            term = request.term;
            var Id = jq(this.element).prop("id");
            var index = Id.replace('phlInfo_', '');
            index = index.replace('__Company', '');
            index = parseInt(index);
            jq.ajax({
                url: '/Project/GetCompanyName/',
                data: { "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {
                    response(jq.map(data, function (item) {
                        return item;
                    }));
                    PhlSuggestion = data;
                    const input = jq('[id*=__Company]')[index];
                    ctrl = input;
                    if (PhlSuggestion.length > 0) {
                        //jq('#' + Id).val(data[0].label);
                        var CompId = Id.replace("phlInfo_", "");
                        CompId = CompId.replace("__Company", "");
                        var hdnComp = "phlInfo_" + CompId + "__MemId";
                        CompId = "phlInfo_" + CompId + "__Uid";
                        var selectedValue = PhlSuggestion[0].val;
                        var arrVal = selectedValue.split(':');
                        var idVal = arrVal[0];
                        var UidVal = arrVal[1];
                        jq('#' + hdnComp).val(idVal);
                        jq('#' + CompId).val(UidVal);
                        const selectedText = PhlSuggestion[0].label; // Choose the first suggestion
                        const startIndex = request.term.length;
                        const endIndex = selectedText.length;
                        //jq(input).data('autocomplete-fill', true);
                        //jq(input).data('id-fill', data[0].val);
                        // Set the cursor position to overwrite mode
                        jq(input).val(selectedText);
                        input.setSelectionRange(startIndex, endIndex);

                    }
                    else {
                        Id = Id.replace("phlInfo_", "");
                        Id = Id.replace("__Company", "");
                        var hdnComp = "phlInfo_" + Id + "__MemId";
                        // Find the hidden field relative to the selected text input
                        jq('#' + hdnComp).val('');
                        jq('#' + Id).val('');
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
            var Id = jq(this).attr('id');
            Id = Id.replace("phlInfo_", "");
            Id = Id.replace("__Company", "");
            Id = "phlInfo_" + Id + "__Uid";
            var selectedValue = ui.item.val;
            var arrVal = selectedValue.split(':');
            var idVal = arrVal[0];
            var UidVal = arrVal[1];
            // Find the hidden field relative to the selected text input
            var hiddenField = jq(this).next('input[type=hidden]');
            // Assign the selected value to the hidden field
            hiddenField.val(idVal);
            jq('#' + Id).val(UidVal);
        },
        change: function (e, i) {
            e.preventDefault();
            var chkReg = jq(this).next('input').val();
            var chkId = jq(this).attr('id');
            var chkVal = jq(this).val();
            if (chkReg == '') {
                if (confirm("The contractor you entered does not exist. Do you want to add new contractor?")) { openMemModal1(chkId, chkVal); }
                else { jq(this).val(''); }
            }
            //jq("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        blur: function (e) { e.preventDefault(); },
        minLength: 1
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
    jq('input[class$="valContact"]').autocomplete({
        source: function (request, response) {
            term = request.term;
            var Id = jq(this.element).prop("id");
            var index = Id.replace('phlInfo_', '');
            index = index.replace('__Company', '');
            index = parseInt(index);
            var CompId = Id.replace("phlInfo_", "");
            CompId = CompId.replace("__StrContact", "");
            CompId = "phlInfo_" + CompId + "__MemId";
            var ItemId = jq('#' + CompId).val();
            jq.ajax({
                url: '/Project/GetContactName/',
                data: { "Id": ItemId, "prefix": request.term },
                type: "POST",
                async: false,
                success: function (data) {

                    response(jq.map(data, function (item) {
                        return item;
                    }));
                    ConSuggestion = data;
                    var ConId = Id.replace("__StrContact", "__ConId");
                    const input = jq('[id*=__StrContact]')[index];
                    ctrl = input;
                    if (ConSuggestion.length > 0) {
                        //jq('#' + Id).val(data[0].label);
                        var selectedValue = ConSuggestion[0].val;
                        //jq('#' + ConId).val(selectedValue);
                        //const selectedText = ConSuggestion[0].label; // Choose the first suggestion
                        const startIndex = request.term.length;
                        const endIndex = selectedText.length;
                        //jq(input).data('autocomplete-fill', true);
                        //jq(input).data('id-fill', data[0].val);
                        // Set the cursor position to overwrite mode
                        //jq(input).val(selectedText);
                        //input.setSelectionRange(startIndex, endIndex);
                    }
                    else {
                        // Find the hidden field relative to the selected text input
                        jq('#' + ConId).val('');
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
            var Id = jq(this).prop("id");
            var index = Id.replace('phlInfo_', '');
            index = index.replace('__StrContact', '');
            index = "phlInfo_" + index + "__contactEmail"
            var CompId = Id.replace("phlInfo_", "");
            CompId = CompId.replace("__StrContact", "");
            CompId = "phlInfo_" + CompId + "__contactPhone";
            // Find the hidden field relative to the selected text input
            var hiddenField = jq(this).next('input[type=hidden]');

            // Assign the selected value to the hidden field
            hiddenField.val(selectedValue);
            jq.ajax({
                url: '/Project/GetContactDetail/',
                data: { "ConId": ui.item.val },
                type: "POST",
                async: false,
                success: function (data) {
                    console.log(data)
                    jq('#' + CompId).val(data.PhoneNum);
                    jq('#' + index).val(data.Email);
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
            var Id = jq(this).prop("id");
            var ConId = Id.replace('phlInfo_', '');
            ConId = ConId.replace('__StrContact', '');
            ConId = "phlInfo_" + ConId + "__ConId";
            var conVal = jq('#' + ConId).val();
            var index = Id.replace('phlInfo_', '');
            index = index.replace('__StrContact', '');
            index = "phlInfo_" + index + "__contactEmail"
            var CompId = Id.replace("phlInfo_", "");
            CompId = CompId.replace("__StrContact", "");
            CompId = "phlInfo_" + CompId + "__contactPhone";
            jq.ajax({
                url: '/Project/GetContactDetail/',
                data: { "ConId": conVal },
                type: "POST",
                async: false,
                success: function (data) {
                    console.log(data)
                    jq('#' + CompId).val(data.PhoneNum);
                    jq('#' + index).val(data.Email);

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
            //jq("#hfProjTypeId").val(i.item.val);
            //valProjTypeId = i.item.val;
        },
        minLength: 0
    }).focus(function () {
        jq(this).autocomplete("search");
    }).keydown(function (e) {
        if (e.key == "Backspace") {
            //         const selObj = window.getSelection();
            ctrl.setSelectionRange(term.length - 1, jq(this).val().length);
            //                    alert(selObj);
            //             console.log(selObj);
        }
    }).blur(function () {
        var chkReg = jq(this).next('input').val();
        var chkId = jq(this).attr('id');
        var chkName = jq(this).val();
        var CompId = chkId.replace("phlInfo_", "");
        CompId = CompId.replace("__StrContact", "");
        CompId = "phlInfo_" + CompId + "__MemId";
        var CompText = chkId.replace("__StrContact", "__Company");
        var CId = jq('#' + CompId).val()
        if (CId == '' || CId == undefined || CId == null) {
            //alert('No contractor is selected. Please select a contractor.')
            jq('#' + chkId).val('');
            jq('#' + CompText).focus();
            return false;
        }
        if (chkReg === '' && chkName != '') {
            if (confirm("The contact for contractor you entered does not exist. Do you want to add new contact?")) { openConModel(chkId, chkName, CId); }
            else { jq(this).val(''); }
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
//function fillCityAndStateFields(localities) {
//    var locality = localities[0]; //use the first city/state object
//    jq('#LocCity').val(locality.city);
//    jq('#LocState').val(locality.state_code);
//    jq('#LocAddr2').val(locality.province);
//}
jq(function () {
    var val = jq('#hdnArrivalDt').val();
    if (val != '')
        jq('#ArrivalDt').val(val);
});
function ToggleNote() {
    var hdnVal = jq("#hdnPHLNote").val();
    if (hdnVal == 'Y') {
        jq('#IdPhl').css('display', 'block')
        jq("#hdnPHLNote").val('N');
    }
    else if (hdnVal == 'N') {
        jq('#IdPhl').css('display', 'none')
        jq("#hdnPHLNote").val('Y');
    }
}
function AddPhlNote() {
    var projId = jq("#ProjId").val();
    var note = jq("#txtPhl").val();
    if (note == undefined || note == '' || note == null) {
        jQuery('#txtPhl').next('span').html('This can not be blank.');
        return false;
    }
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/AddPhlNote/',
        data: { 'ProjId': projId, 'PHLNote': note },
        async: false,
        success: function (response) {
            jQuery('#txtPhl').next('span').html('Phl note saved.').css('color', 'Green');
        },
        error: function (response) {
            jQuery('#txtPhl').next('span').html('Some error occurred please try again.');
        },
        failure: function (response) {
            jQuery('#txtPhl').next('span').html('Operation aborted.');
        }
    });
}
function AddBrNote() {
    var projId = jq("#ProjId").val();
    var note = jq("#BrNote").val();
    if (note == undefined || note == '' || note == null) {
        jQuery('#BrNote').next('span').html('This can not be blank.');
        return false;
    }
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/AddBrNote/',
        data: { 'ProjId': projId, 'BrNote': note },
        async: false,
        success: function (response) {
            jQuery('#BrNote').next('span').html('Bid Result posted.').css('color', 'Green');
        },
        error: function (response) {
            jQuery('#BrNote').next('span').html('Some error occurred please try again.');
        },
        failure: function (response) {
            jQuery('#BrNote').next('span').html('Operation aborted.');
        }
    });
}
// script for adding input on server files icon
//var jq = $.noConflict();

jQuery("#d-input").click(() => {
    jQuery("#show-input").show() && jQuery("#d-input").hide()
})
// script for adding new row on project team section.
var i = 1;
//Adding row in enty functionEntities_4__
jQuery("#project-row-adder").click(function () {
    i = jQuery("input[class*='EntityNameClass']").length;
    var tabIndex = parseFloat(jQuery('#project-row-adder').attr('tabindex'));
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
        '<input type="hidden" id="Entities_' + i + '__CompType" name="Entities[' + i + '].CompType" />' +
        '</div>' +
        '<div class="col-1 col-lg-1 col-xl-1 pr-0">' +
        '<input type="hidden" tabindex="' + (tabIndex + 3) + '" id="Entities_' + i + '__chkIssue" name="Entities[' + i + '].chkIssue" class="chk-Issue" />' +
        '<input type="checkbox" class="mx-1 align-middle" tabindex="' + (tabIndex + 3) + '" value="" id="chkBoxIssue" onchange="AssignIssue(this)" />' +
        '</div>' +
        '<div class="col-1 col-lg-1 col-xl-1">' +
        '<div class="remove_prebid_row pb-1"><span tabindex="' + (tabIndex + 4) + '" class="action-del popClassDel" id="DeleteRow"><i class="fa fa-trash"></i></span></div>' +
        '</div>' +
        '</div>';

    jQuery('#newinput').append(newRowAdd);
    InitializeEntName();
    InitializeEntType();
    delSapn();
    jQuery('#project-row-adder').attr('tabindex', tabIndex + 5);
    jQuery('#' + Fid).focus();

});
// deleting the row on del-icon click
jQuery("body").on("click", "#DeleteRow", function () {
    if (jQuery(this).closest("#project-team-row").find('input.chk-Issue').val() == 'True') {
        if (confirm("Are you sure you want to delete project team information? It will also delete the issuing office.")) {
            jQuery(this).closest("#project-team-row").find('input.Del-Ent').val('false');
            jQuery(this).parents("#project-team-row").hide();
            jQuery('#IssuingOffice').val('');
        }
    }
    else {
        if (confirm("Are you sure you want to delete project team information?")) {
            jQuery(this).closest("#project-team-row").find('input.Del-Ent').val('false');
            jQuery(this).parents("#project-team-row").hide();
        }
    }
});

jQuery("body").on("click", "#del-addedRow", () => {
    jQuery("#new-addedRow").remove();
})


// script to add pre-bid info row

jQuery("#bidInfo-row-add").click(() => {
    var delCount = jQuery("div[class*='delPreBidCount']").length;
    var preText = jQuery("div[class*='prebidconveniancecount']").length;
    var prebidconveniancecount = jQuery("div[class*='prebidconveniancecount']").length;
    prebidconveniancecount = prebidconveniancecount + delCount;
    var tabIndex = parseFloat(jQuery('#bidInfo-row-add').attr('tabindex'));
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
    //    jQuery('#' + EFid).focus();
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
        '<div class="col-lg-2 col-md-2">' +
        '<input type="hidden" id="preBidInfos_' + prebidconveniancecount + '__Id" name="preBidInfos[' + prebidconveniancecount + '].Id">' +

    '<input type="text" placeholder="Select Date" tabindex="' + (tabIndex + 1) + '" class="form-control datepicker-custom " id="preBidInfos_' + prebidconveniancecount + '__PreBidDate" name="preBidInfos[' + prebidconveniancecount + '].PreBidDate" onchange="getPreDateIndex((this).id,event)" />' +
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
        '<div class="col-2 col-lg-1 col-md-2 col-sm-2 pr-0">' +
        '<input class="form-control" type="text" id="preBidInfos_' + prebidconveniancecount + '__PST" name="preBidInfos[' + prebidconveniancecount + '].PST" value="PT" readonly>' +
        '</div>' +
        //'<div class="col-2 col-lg-1  col-md-1 col-sm-1 ">' +
        //'<div class="form-check form-check-inline">' +
        //'<input class="form-check-input" type="checkbox" tabindex="' + (tabIndex + 6) + '" onchange="ChangeAnd(this)">' +
        //' <label class="form-check-label" for="and">and</label>' +
        //'<input type="hidden" id="preBidInfos_' + prebidconveniancecount + '__PreBidAnd" name="preBidInfos[' + prebidconveniancecount + '].PreBidAnd">' +
        //' </div>' +
        //' </div>' +
        '<div class="col-2 col-lg-2 col-md-2 col-sm-2">' +
        ' <div class="form-check form-check-inline">' +
        '<input class="form-check-input" tabindex="' + (tabIndex + 5) + '" type="checkbox" onchange="ChangeMandatory(this)">' +
        '<label class="form-check-label" for="">Mandatory</label>' +
        '<input type="hidden" id="preBidInfos_' + prebidconveniancecount + '__Mandatory" name="preBidInfos[' + prebidconveniancecount + '].Mandatory" >' +
        '</div>' +
        '</div>' +
        '<div class="col-2 col-lg-1 col-md-1 col-sm-1">' +

        '<div class=" remove_prebid_row "><span id="delPrebid-row" class="p-0 popClassDel"><i class="fa fa-trash"></i></span></div>' +
        '</div>' +
        '<div class="col-9 col-lg-8 col-md-8 col-sm-9 mt-2">' +
        '<input tabindex="' + (tabIndex + 6) + '" class="form-control" type="text" id="preBidInfos_' + prebidconveniancecount + '__Location" name="preBidInfos[' + prebidconveniancecount + '].Location" placeholder="Location">' +
        '</div>' +
        '<div class="col-lg-2 mt-auto">' +
    '<div class="form-check form-check-inline" >' +
    '<input type="hidden" name="preBidInfos[' + prebidconveniancecount + '].UndecidedPreBid" id="preBidInfos_' + prebidconveniancecount + '__UndecidedPreBid" value="false"/>'+
    '<input class="form-check-input tbdcheck" type="checkbox"  tabindex="' + (tabIndex + 8) + '" id="inlineCheckbox1" value="option1" tabindex="150" onchange="UndecidedPre(this)">' +
        '<label class="form-check-label font-weight-bold" >T B D</label>' +
        '</div>' +
        '</div >' +
        //'<div class="col-2 col-sm-2 col-lg-1 col-xl-1">' +tcomp"
        //'<div class="remove_prebid_row pt-2"><span id="delPrebid-row"><i class="fa fa-trash"></i></span></div>' +
        //'</div>' +
        '</div>'
    '</div>'
    '</div>';

    if (preText < 5)
        jQuery("#bidInfo-input").append(preBidRow);
    else {
        preBidRow = '<p class="text-danger error-message" id="TempMessage">You can only have upto 5 pre bid date</p>'
        jQuery("#bidInfo-input").append(preBidRow);
        jQuery('.error-message').delay(1000).fadeOut();
    }
    delSapn();
    ReInitializeDatePicker();
    jQuery('#' + Fid).focus();
    jQuery('#bidInfo-row-add').attr('tabindex', tabIndex + 9);
    //}
    autoCompleteTimer();
    dateValidate();

})

jQuery("body").on("click", "#delPrebid-row", function () {
    if (confirm("Are you sure you want to delete prebid info?")) {
        jQuery(this).closest("#prebid-info-row").find('input.Pre-Del').val('true');
        jQuery(this).parents("#prebid-info-row").hide();
        var rootDiv = jQuery(this).parents("#prebid-info-row").children('div.Pre-Text');
        rootDiv.removeClass('Pre-Text');
        let DivRemove = document.querySelectorAll('[class$="Pre-Text"]');
        var x = 1;
        DivRemove.forEach(function (input) {
            input.textContent = '<label>Meeting ' + x + '</label>';
            input.innerHTML = '<label>Meeting ' + x + '</label>';
            x++
        });
        jQuery(this).parents("#prebid-info-row").removeClass("prebidconveniancecount");
        jQuery(this).parents("#prebid-info-row").addClass("delPreBidCount");

    }
})
//avoid prebid from today
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
        jQuery('div.date-index').find('input[class=chkBidDup]').each(function (index, item) {
            if (jQuery(this).attr('id') == id) {
                date = jQuery(this).val();
            }
            if (jQuery(this).attr('id') != id)
                dateArr.push(jQuery(this).val());
        });
        for (var idx = 0; idx < dateArr.length; idx++) {
            if (dateArr[idx] === date) {
                alert('date already exists');
                jQuery('#' + id).val('');
                checklistvalide = false;
            }
        }
        return checklistvalide;
    }
}
// script to add cost 2 cost row
jQuery("#cost-row-add").click(function () {
    var costTextCount = jQuery("div[class*='costconveniancecount']").length;
    var costconveniancecount = jQuery("div[class*='costconveniancecount']").length;
    var delCount = jQuery("div[class*='delCostCount']").length;
    costconveniancecount = costconveniancecount + delCount;
    var tabIndex = parseFloat(jQuery(this).attr('tabindex'));
    var Fid = 'EstCostDetails_' + costconveniancecount + '__EstFrom';
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
    //    jQuery('#' + EFid).focus();
    //}
    //else {
    newCostRow =
        '<div class="row align-items-center pb-2 costconveniancecount" id="remove-cost">' +
        '<div class="col-12 col-sm-12 col-md-12 col-lg-12 Cost-Text"><label>Estimated Cost ' + (costTextCount + 1) + '</label></div>' +
        '<div class="col-md-2 col-lg-2 pr-0" style="flex:0 0 9%;width:fit-content;">' +
    '<select class="form-control range-sign p-0"  tabindex="' + (tabIndex + 1) + '" id="EstCostDetails_' + costconveniancecount + '__RangeSign" name="EstCostDetails[' + costconveniancecount + '].RangeSign" data-index="' + costconveniancecount + '" ><option selected value="0"> </option><option value="1">&lt;</option><option value="2">&gt;</option></select>' +
        '</div > ' +
        '<div class="col-md-5 col-lg-3 text-nowrap ">' +
        '<input type="hidden" id="EstCostDetails_' + costconveniancecount + '__Removed" name="EstCostDetails[' + costconveniancecount + '].Removed" class="Del-CostClass">' +
        '<input type="text" tabindex="' + (tabIndex + 1) + '" class="form-control pr-0 inpRem" id="EstCostDetails_' + costconveniancecount + '__EstFrom" name="EstCostDetails[' + costconveniancecount + '].EstFrom" placeholder="Cost" onkeyup="formatNumberInput(this)"/>' +
        '<input type="hidden" id="EstCostDetails_' + costconveniancecount + '__Id" name="EstCostDetails[' + costconveniancecount + '].Id">' +
        '</div>' +
        '<span class="">TO</span>' +
        '<div class="col-md-5 col-lg-3">' +
        '<input type="text" class="form-control inpRem" tabindex="' + (tabIndex + 2) + '" id="EstCostDetails_' + costconveniancecount + '__EstTo" name="EstCostDetails[' + costconveniancecount + '].EstTo" placeholder="Cost" onkeyup="formatNumberInput(this)"/>' +
        '</div>' +
        '<div class="col-md-5 col-lg-3">' +
        '<input type="text" class="form-control" tabindex="' + (tabIndex + 3) + '" id="EstCostDetails_' + costconveniancecount + '__Description" name="EstCostDetails[' + costconveniancecount + '].Description" placeholder="Description" />' +
        '</div>' +
        '<div class="col-2 col-md-2 col-lg-1 added-icon">' +
        '<span class="action-del popClassDel" id="del-costRow" tabindex="' + (tabIndex + 4) + '"><i class="fa fa-trash" aria-hidden="true"></i></span>' +
        '</div>' +
        '</div>';

    if (costTextCount < 5) {

        jQuery("#costInput").append(newCostRow);
    }
    else {
        costDateRow = '<p class="text-danger error-msg" id="TempMessage">You can only have upto 5 Estimated cost</p>'
        jQuery("#costInput").append(costDateRow);
        jQuery('.error-msg').delay(1000).fadeOut();
    }
    delSapn();
    jQuery(this).attr('tabindex', tabIndex + 5);
    jq('#' + Fid).focus();
    //}
});

// script to delete cost 2 cost row
jQuery("body").on("click", "#del-costRow", function () {
    if (confirm("Are you sure you want to delete estimated cost info?")) {
        jQuery(this).parents("#remove-cost").hide();
        var rootDiv = jQuery(this).parents("#remove-cost").children('div.Cost-Text');
        rootDiv.removeClass('Cost-Text');
        let DivRemove = document.querySelectorAll('div[class$="Cost-Text"]');
        var x = 1;
        DivRemove.forEach(function (input) {
            input.textContent = 'Estimated Cost ' + x;
            input.innerHTML = '<label>Estimated Cost ' + x + '</label>';
            x++
        });
        jQuery(this).parents("#remove-cost").removeClass("costconveniancecount");
        jQuery(this).parents("#remove-cost").addClass("delCostCount");
        jQuery(this).closest("#remove-cost").find('input.Del-CostClass').val('true');
    }
});
//jQuery("#bidDate-add").click((e) => {
//    e.preventDefault();

//    var biddateconveniancecount = jQuery("div[class*='biddateconveniancecount']").length + 2;
//    var chkBidVal = "BidDt" + biddateconveniancecount;
//    var ctrlvalue = '';
//    if (chkBidVal == "BidDt2")
//        ctrlvalue = jQuery('#hdnBidDt2').val();
//    if (chkBidVal == "BidDt3")
//        ctrlvalue = jQuery('#hdnBidDt3').val();
//    if (chkBidVal == "BidDt4")
//        ctrlvalue = jQuery('#hdnBidDt4').val();
//    if (chkBidVal == "BidDt5")
//        ctrlvalue = jQuery('#hdnBidDt5').val();
//    console.log(ctrlvalue);
//    bidDateRow =
//        '<div class="ml-3">' +
//        '<div class="row mt-3 biddateconveniancecount" id="remove-biddate">' +
//        '<div class="col-12 col-md-6 col-lg-6 ">' +
//        '<input type="Date" id="BidDt' + biddateconveniancecount + '" name="BidDt' + biddateconveniancecount + '" class="form-control datepicker-custom" Value="' + ctrlvalue + '" placeholder="Select Date" /> ' +
//        '</div>' +
//        '<div class="col-10 col-sm-10 col-md-4 col-lg-4 mt-767">' +
//        '<input type="text" class="form-control" placeholder="PST" id="strBidDt' + biddateconveniancecount + '" name="strBidDt' + biddateconveniancecount + '" />' +
//        '</div>' +
//        '<div class="col-md-2 col-lg-2 col-sm-2 col-2 remove_prebid_row">' +
//        '<span id="del-biddateRow" class="action-del"><i class="fa fa-trash pt-3" aria-hidden="true"></i></span>' +
//        '</div>' +
//        '</div>' +
//        '</div>';
//    if (biddateconveniancecount < 6)
//        jQuery("#bidDate-input").append(bidDateRow);
//    else {
//        bidDateRow = '<p class="text-danger err-message" id="TempMessage">You can only have upto 5 bidDate</p>';
//        jQuery("#bidDate-input").append(bidDateRow);
//        jQuery('.err-message').delay(1000).fadeOut();
//    }
//})
var i = 1;
jQuery("#bidDate-add").click(function () {
    var count = jQuery("input[class*='BidCount']").length
    newRowAdd =
        '<div id="bid_Date-row" class="row pt-2 align-items-center">' +
        '<div class="col-md-1 col-lg-1 col-1">' +
        '<label class="font-weight-bold">Bid </label>' +
        '</div>' +
        '<div class="col-md-3 col-lg-3 col-12">' +
        '<input type="date" class="form-control BidCount" id="BidDateTimes[' + count + '].BidDate" name="BidDateTimes[' + count + '].BidDate" placeholder="Select Date" onchange="getDateIndex((this).id,event)" />' +
        '</div>' +
        '<div class="col-md-3 col-lg-3 col-12">' +
        '<input type="text" class="form-control" placeholder="PST" id="BidDateTimes[' + count + '].PST" name="BidDateTimes[' + count + '].PST" value="PT" readonly/>' +
        '</div>' +
        '<div class="col-2 col-sm-2 col-md-2 col-lg-2 mt-575"">' +
        '<input type="text" class="form-control" placeholder="PST" id="BidDateTimes___PST" name="BidDateTimes[].PST" oninput="convertToUppercase(this)" />' +
        '</div>' +
        '<div class="col-md-3 col-lg-3 col-sm-3 text-right col-2 remove_prebid_row">' +
        '<span id="remove-biddate" class="action-del popClassDel"><i class="fa fa-trash pt-1" aria-hidden="true"></i></span>' +
        '</div>' +
        '</div>';

    jQuery('#bidDate-input').append(newRowAdd);
    delSapn();

});

jQuery("body").on("click", "#del-biddateRow", function () {
    $(this).parents("#bid_date").remove();
})
jQuery("body").on("click", "#remove-biddate", () => {
    jQuery("#bid_Date-row").remove();
})

jQuery("#planHolder-add").click((e) => {
    e.preventDefault();
    var ddlHtml = '';
    var BidHtml = ''
    var planPHLecount = jQuery("div[class*='planPHLecount']").length;
    var tbCount = parseFloat(planPHLecount - 1);
    var tabIndex = parseFloat(jQuery(".count-class-" + tbCount).attr('tabindex'));
    var Fid = 'phlInfo_' + planPHLecount + '__PHLType';
    jq.ajax({
        type: "GET",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/GetPHLType/',
        async: false,
        success: function (response) {
            ddlHtml += '<select tabindex="' + (tabIndex + 1) + '" class="form-control p-0" name="phlInfo[' + planPHLecount + '].PHLType" id="phlInfo_' + planPHLecount + '__PHLType">';
            var selectElement = jq('<select>'); // Create a new <select> element

            // Loop through the values and create <option> elements
            jq.each(response, function (index, value) {
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
    jq.ajax({
        type: "GET",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/GetBidOption/',
        async: false,
        success: function (response) {
            BidHtml += '<select tabindex="' + (tabIndex + 12) + '" class="form-control p-0" name="phlInfo[' + planPHLecount + '].BidStatus" id="phlInfo_' + planPHLecount + '__BidStatus">';
            BidHtml += '<option value="0">--No Selection--</option>'
            // Loop through the values and create <option> elements
            jq.each(response, function (index, value) {
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
        '<input class="form-control" type="text" id="phlInfo_' + planPHLecount + '__PHLNote" name="phlInfo[' + planPHLecount + '].PHLNote" placeholder="Note" >' +
        '</div>' +
        '<div class="col-lg-3 mb-2">' +
        '<input class="form-control" id="phlInfo_' + planPHLecount + '__contactEmail" name="phlInfo[' + planPHLecount + '].contactEmail" type="text" placeholder="Email" tabindex="' + (tabIndex + 5) + '">' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-2" style="border-left: 3px solid #5b7576;">' +
        '<div class="col-12 date-img px-2">' +
        '<div>' +
        '<input class="form-control px-2" type="text" id="phlInfo_' + planPHLecount + '__BidDate" name="phlInfo[' + planPHLecount + '].BidDate" placeholder="Select Date" />' +
        '<div class="d-flex">' +
        '<div id="time_input" class="my-1">' +
        '<label for="hours">' +
        '<input type="number" style="width:45px" id="phlInfo_' + planPHLecount + '__tComp" name="phlInfo[' + planPHLecount + '].tComp"  value="00">' +
        '</label>' +
        '<span>:</span>' +
        '<label for="minutes">' +
        '<input type="number" style="width:45px" id="phlInfo_' + planPHLecount + '__hComp" name="phlInfo[' + planPHLecount + '].hComp"  value="00">' +
        '</label>' +
        '<span>:</span>' +
        '<label for="seconds">' +
        '<input type="Text" style="width:45px" id="phlInfo_' + planPHLecount + '__mValue" name="phlInfo[' + planPHLecount + '].mValue" value="AM">' +
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

    jQuery("#planholder-input").append(planholderRow);
    delSapn();
    InitializePhlName();
    InitializePhlCon();
    ReInitializeDatePicker();
    autoCompleteTimer();
    //jQuery('#' + Fid).focus();
    jQuery('#' + Fid)[0].focus();
    //jQuery("#planHolder-add").attr('tabindex', tabIndex + 7);
    dateValidate();

});

jQuery("body").on("click", "#remove-PHL", function () {
    if (confirm("Are you sure you want to delete plan holder list?")) {
        {
            jQuery(this).closest("#PHL-row").find('input.Dec-Class').val('false');
            jQuery(this).parents("#PHL-row").hide();

        }
    }
})

//jQuery("body").on("click", "#del-biddateRow", function () {
//    jQuery("#remove-biddate").remove();
//})
//jQuery("#bidInfo-row-add").click(() => {
//    var conveniancecount = jQuery("div[class*='conveniancecount']").length + 2;
//    preBidRow =
//        '<div class="row mt-3 pb-2 border-bottom conveniancecount" id="prebid-info-row">' +
//        '<div class="col-md-4 mb-lg-0 mb-2">' +
//        '<p>Meeting 1</p>' +
//        '</div>' +
//        '<div class="col-md-8 mb-lg-0 mb-2">' +
//        '<input type="checkbox" class="mx-1" value="" />Mandatory ' +
//        '</div>' +
//        '<div class="col-lg-6 col-md-6 mb-lg-0 mb-2 pr-0">' +
//        '<input type="datetime-local" class="form-control" id="PreBidDate2" name="PreBidDate2"/>' +
//        '</div>' +
//        '<div class="col-lg-4 col-xl-4 pr-0">' +
//        '<input tabindex="21" class="form-control" type="text" value="" id="PreBidLoc2" name="PreBidLoc2"/>' +
//        '</div>' +
//        '<div class="col-lg-1 col-xl-1">' +
//        '<div class="remove_prebid_row pt-2"><span class="action-del" id="delPrebid-row"><i class="fa fa-trash"></i></span></div>' +
//        '</div>' +
//        '</div>';
//    if (conveniancecount < 3)
//        jQuery("#bidInfo-input").append(preBidRow);
//    else {
//        preBidRow = '<p class="text-danger" id="TempMessage">You can only have upto 2 pre bid date</p>'
//        jQuery("#bidInfo-input").append(preBidRow);
//        jQuery('#TempMessage').delay(2000).fadeOut();

//    })

//jQuery("body").on("click", "#delPrebid-row", function () {
//    $(this).parents("#prebid-info-row").remove();
//})

// script for adding tables on bid results
jQuery(document).ready(function () {
    var ScopeVal = jQuery('input[name = ProjScope]').val();

    if (ScopeVal.indexOf('New') != -1) {
        jQuery('#chk_0').prop('checked', true);
    }
    if (ScopeVal.indexOf('Remodel') != -1) {
        jQuery('#chk_1').prop('checked', true);
    }
    if (ScopeVal.indexOf('Addition') != -1) {
        jQuery('#chk_2').prop('checked', true);
    }

});
function changeScope(id) {
    var strManipulated = '';
    var strProjectScope = jQuery('#ProjScope').val();
    var chkValue = jQuery('#' + id).val();
    var isExists = 'N';

    if (jQuery('#' + id).is(':checked')) {
        isExists = 'Y';
    }

    if (strProjectScope != '') {
        var myArray = strProjectScope.split(",");
        for (var i = 0; i < myArray.length; i++) {
            var strValue = myArray[i].trim();
            if (strValue != "") {
                if (strValue != chkValue) {
                    strManipulated += strValue + ",";
                }
            }
        }
    }

    if (isExists == 'Y') {
        strManipulated += chkValue + ",";
    }
    if (strManipulated.endsWith(",")) {
        strManipulated = strManipulated.slice(0, -1);
    }
    jQuery('#ProjScope').val(strManipulated);
} 

jQuery('.dropdown-item').click(function () {
    jQuery('#loader-overlay2').show();
});

//function titlevalidate()
//{
//    var projecttitle = jQuery('#Title').val();
//    jQuery('#Title').next('span').html('');
//    if (projecttitle == undefined || projecttitle == '' || projecttitle == null) {
//        jQuery('#Title').next('span').html('Please enter project title.');
//        return  false;
//    }
//    return true;
//}
function getDateIndex(id, e) {
    var BidDate = document.getElementsById("BidDt").value;
    var currentDate = new Date();
    var month = currentDate.getMonth() + 1;
    if (month < 10) month = "0" + month;
    var dateOfMonth = currentDate.getDate();
    if (dateOfMonth < 10) dateOfMonth = "0" + dateOfMonth;
    var year = currentDate.getFullYear();
    var formattedDate = year + "-" + month + "-" + dateOfMonth;

    if (BidDate <= formattedDate) {
        alert("The Bid Date must be grater to today date")
        document.getElementById("BidDt").value = "";
        return false;
    }

    var checklistvalide = true;
    var dateArr = [];
    var date = '';
    jQuery('div.dateindex').find('input[type=date]').each(function (index, item) {
        if (jQuery(this).attr('id') == id) {
            date = jQuery(this).val();
        }
        if (jQuery(this).attr('id') != id)
            dateArr.push(jQuery(this).val());
    });
    for (var idx = 0; idx < dateArr.length; idx++) {
        if (dateArr[idx] == date) {
            alert('date already exists');
            jQuery('#' + id).val('');
            checklistvalide = false;
        }
    }
    checkUniqueDate();
    return checklistvalide;
}
function AssignIssue(ctrl) {
    if (jQuery(ctrl).is(':checked')) {
        var entityName = jQuery(ctrl).closest('.row').find('input[name*="EntityName"]').val();
        //jQuery('#Entities_1__chkIssue').val(true);
        var checkissue = document.querySelectorAll('[id*="chkIssue"]');
        checkissue.forEach(function (checkbox) {
            // if the checkbox is not the one that was just checked, uncheck it
            checkbox.value = 'False';

        });
        jQuery(ctrl).closest('.row').find('input[type="hidden"][name*="chkIssue"]').val('True');
        jQuery('#IssuingOffice').val(entityName);
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
        jQuery(ctrl).closest('.row').find('input[type="hidden"][name*="chkIssue"]').val('False');
        jQuery('#IssuingOffice').val(entityName);
    }
}

function AssignBidding(checkbox) {
    var hiddenInput = checkbox.nextElementSibling; // Get the next sibling element, which is the hidden input
    hiddenInput.value = checkbox.checked ? "true" : "false"; // Assign the value based on the checkbox checked state
}
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
        jq(".classMOcounties").each(function () {
            var checked = jq(this).prop('checked');
            if (checked) {
                cHtml += jq(this).val() + ', ';
                cTextHtml += jq(this).next('label').text() + ', ';
            }
        });
    }
    if (WCounty.checked) {
        cHtml += WCounty.value + ', ';
        cTextHtml += WCounty.nextElementSibling.innerText + ', ';
    }
    else {
        jq(".classMWcounties").each(function () {
            var checked = jq(this).prop('checked');
            if (checked) {
                cHtml += jq(this).val() + ', ';
                cTextHtml += jq(this).next('label').text() + ', ';
            }
        });
    }

    jq(".classMcounties").each(function () {
        var checked = jq(this).prop('checked');
        if (checked) {
            cHtml += jq(this).val() + ', ';
            cTextHtml += jq(this).next('label').text() + ', ';
        }

    });
    cHtml = cHtml.replace(/,\s*$/, '');
    cTextHtml = cTextHtml.replace(/,\s*$/, '');
    if (typeof cHtml !== 'undefined' && cHtml !== null && cHtml.trim() !== "") {
        jq('#Counties').val(cHtml);
    }
    if (typeof cTextHtml !== 'undefined' && cTextHtml !== null && cTextHtml.trim() !== "") {
        jq('#divsCounty').html(cTextHtml);
        jq('#divCounty').css('display', 'block');
        jq('#divsCounty').css('display', 'block');
    }
    jq(".m-modal").modal('hide');
    //jq('div.modal-backdrop').remove();
}
function checkLeapYear(year) {

    const leap = new Date(year, 1, 29).getDate() === 29;
    if (leap) {
        return true;
    } else {
        return false;
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
            jq('#' + id).focus();
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

            //if ((year % 4 == 0) && (year % 100 != 0) || (year % 400 == 0)) {
            //    if (dayPart > 29) {
            //        alert('You can not enter date more than 29 for month of February.');
            //    }
            //}
        }
        else {
            alert('Date is not correct Please check date.');
            jq('#' + id).focus();
            document.getElementById(id).value = "";
            return false;
        }
    }
}

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
            jq('#' + id).focus();
            return false;
        }
        var pub = jq('#Publish').val();
        var Lbdidt = jq('#HiddenBidDt').val()
        if (pub == 'True' || pub == 'true') {
            if (Lbdidt != undefined && Lbdidt != '' && Lbdidt != null) {
                jq('#BidDt2').val(jq('#HiddenBidDt').val())
                jq('#strBidDt4').val(jq('#hdnLastBidDt').val());
                jq('#strBidDt2').val(jq('#strBidDt2').val());
                jq('#lastTComp').val(jq('#tComp').val());
                jq('#lastHComp').val(jq('#hComp').val());
                jq('#lastMValue').val(jq('#mValue').val());
                jq('#divPrevBD').css('display', 'block');
            }
        }
    }
}
function GetProjCode() {
    var projId = parseFloat(jq('#ProjId').val());
    if (projId == 0) {
        jq.ajax({
            url: '/Project/GetProjectCode/',
            data: {},
            type: "POST",
            success: function (response) {
                jq('#ProjNumber').val(response.data.Result);
            },

            failure: function (response) {
                alert(response.statusMessage);
            }
        });
    }
}
GetProjCode();
function AssignBidDate(ctrl) {
    if (jQuery(ctrl).is(':checked')) {
        var checkboxes = document.querySelectorAll('[id*="chkAddBox"]');

        // loop through each checkbox
        checkboxes.forEach(function (checkbox) {
            // if the checkbox is not the one that was just checked, uncheck it
            if (checkbox !== event.target) {
                checkbox.checked = false;
            }
        });
        jq('#BidDt').focus();
        var rows = jq(ctrl).parent('td').parent('tr');
        jq('#strAddenda').val(rows.find('td:eq(0)').text());
    }
    checkAddenda();
}
jQuery(document).ready(function () {
    // Find all TBD checkboxes and trigger the change event to execute the UndecidedPre function
    jQuery('.form-check-input.tbdcheck[type="checkbox"]').each(function () {
        if (jQuery(this).is(':checked')) {
            UndecidedPre(this);
        }
    });

});
function UndecidedPre(ctrl) {
    var Assign = ctrl.checked;
    var TBD = "TBD";

    var parentDiv = jq(ctrl).closest('.prebidconveniancecount');

    if (Assign) {
        jq('#strPreBidDt').val(TBD);
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
    jq(ctrl).prop('disabled', false);
    jQuery('[id *= UndecidedPreBid]').prop('disabled', false);
    jq(ctrl).prop('checked', Assign);
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
function BlockCounties(id) {
    var State = '';
    var checkBox = document.getElementById(id);
    if (id == 'AllOr') {
        if (checkBox.checked == true) {
            jq(".classMOcounties").each(function () {

                jq(this).prop("checked", true);   // Uncheck the checkbox
                //     jq(this).prop("disabled", true);   // Disable the checkbox

            });
        }
        else {
            jq(".classMOcounties").each(function () {
                jq(this).prop("checked", false);   // Uncheck the checkbox
                //     jq(this).prop("disabled", true);   // Disable the checkbox

            });
        }
    }
    //State = 'OR';
    else if (id == 'AllWa') {
        if (checkBox.checked == true) {
            jq(".classMWcounties").each(function () {
                var checkboxValue = jq(this).val();
                // Check if checkbox value is in the disabledValues array
                jq(this).prop("checked", true);   // Uncheck the checkbox
                //     jq(this).prop("disabled", true);   // Disable the checkbox

            });
        }
        else {
            jq(".classMWcounties").each(function () {
                var checkboxValue = jq(this).val();
                // Check if checkbox value is in the disabledValues array
                jq(this).prop("checked", false);   // Uncheck the checkbox
                //     jq(this).prop("disabled", true);   // Disable the checkbox
            });
        }
    }

};
//function formatNumberInput(input) {
//    // Remove non-numeric characters from the input value
//    let numericValue = input.value.replace(/\D/g, '');

//    // Format the numeric value with commas
//    let formattedValue = numericValue.replace(/\B(?=(\d{3})+(?!\d))/g, ',');

//    // Add a dollar sign prefix only if the input is not empty
//    formattedValue = numericValue.length === 0 ? '' : '$' + formattedValue;

//    // Set the formatted value back to the input
//    input.value = formattedValue;
//};
jq(document).on('change', '.range-sign', function () {
    var index = jq(this).data('index');
    var rangeid = 'EstCostDetails_' + index + '__EstTo';
    var estToInput = jq('#' + rangeid); // Assuming 'inpRem' class is used for EstTo inputs
    //var estToInput = jq('.inpRem').eq(index); // Assuming 'inpRem' class is used for EstTo inputs
    var disable = (jq(this).val() == '1' || jq(this).val() == '2');
    // Enable or disable based on the selected value
    estToInput.val('');
    estToInput.prop('disabled', disable);
});
jq('.range-sign').each(handleRangeSelectChange);
//jq(document).on('change', '.range-sign', handleRangeSelectChange);
// Function to handle the change event
function handleRangeSelectChange() {
    var selectedIndex = jq(this).val();
    var estToField = jq(this).closest('.costconveniancecount').find('.toinpRem');

    if (selectedIndex == "1" || selectedIndex == "2") {
        estToField.prop('disabled', true);
    } else {
        estToField.prop('disabled', false);
    }
}
//var dropdowns = document.querySelectorAll('.range-sign');

//dropdowns.forEach(function (dropdown) {
//    dropdown.addEventListener('change', function () {
//        var selectedValue = this.value;
//        var estToId = selectedValue === '0' ? this.id.replace('RangeSign', 'EstTo') : '';
//        var estToInput = document.getElementById(estToId);
//        var disabled = (selectedValue === '1' || selectedValue === '2');
//        // Enable or disable based on the selected value
//        estToInput.disabled = disabled;
//    });
//});
function formatNumberInput(input) {
    // Remove non-numeric characters from the input value
    let numericValue = input.value.replace(/\D/g, '');

    // Format the numeric value with commas
    let formattedValue = numericValue.replace(/\B(?=(\d{3})+(?!\d))/g, ',');

    // Add a dollar sign prefix only if the input is not empty
    formattedValue = numericValue.length === 0 ? '' : '$' + formattedValue;

    // Set the formatted value back to the input
    input.value = formattedValue;
    var inputId = jq(input).attr('id');

    // Determine whether the input is for "from" or "to"
    var isFromField = inputId.includes('__EstFrom');
    var counterpartId = isFromField ? inputId.replace('__EstFrom', '__EstTo') : inputId.replace('__EstTo', '__EstFrom');
    var comparisonSignId = isFromField ? inputId.replace('__EstFrom', '__ComparisonSign') : inputId.replace('__EstTo', '__ComparisonSign');

    function extractNumericValue(value) {
        if (value && /[\$,]/.test(value)) {
            // Remove dollar sign from the input value
            return parseFloat(value.replace(/[\$,]/g, ''));
        }
        else {
            return parseFloat(value);
        }
    }
    var fromValue = isFromField ? extractNumericValue(jq(input).val()) : extractNumericValue(jq('#' + counterpartId).val());
    var toValue = isFromField ? extractNumericValue(jq('#' + counterpartId).val()) : extractNumericValue(jq(input).val());


    if (!isNaN(fromValue) && !isNaN(toValue) && fromValue !== "" && toValue !== "") {
        jq('#' + comparisonSignId).text('');
        // Compare values and update display
        if (fromValue < toValue) {
            jq('#' + comparisonSignId).text('<');
        } else if (fromValue > toValue) {
            jq('#' + comparisonSignId).text('>');
        } else {
            jq('#' + comparisonSignId).text('=');
        }
    } else {
        jq('#' + comparisonSignId).text('');
    }
};
// Get the span element
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
const phlSpanElement = document.getElementById('planHolder-add');

// Add a keydown event listener
phlSpanElement.addEventListener('keydown', function (event) {
    // Check if the Enter key was pressed (key code 13)
    if (event.keyCode === 13 || event.which === 13) {
        // Perform the desired action when Enter is pressed
        // For example, you can trigger a click event
        phlSpanElement.click();
        event.preventDefault();
    }
});
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
var inputField = jq('#hComp');
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
        if (event.keyCode === jq.ui.keyCode.DOWN) {
            event.preventDefault();
            jq(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            jq(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
        }
    }).on("input", function () {
        var value = jq(this).val();
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
            }
            else if (value >= 60)
                this.value = '00';
        }
    }).on("focus", function () {
        jq(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === jq.ui.keyCode.DOWN) {
            event.preventDefault();
            jq(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            jq(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
            //  jq.ui.keyCode.ba
        }
    }).on("keyup", function (event) {
        var value = jq(this).val();
        if (value.length > 2) {
            jq(this).val(value.slice(0, 2));
        }
        //var inputKeyCode = event.keyCode;
        //value = parseInt(this.value);

        //if (inputKeyCode != 8) {
        //    if (value === 0) {
        //        this.value = '00';
        //    }
        //    else if (value === 1) {
        //        this.value = '15';
        //    }
        //    else if (value === 3) {
        //        this.value = '30';
        //    }
        //    else if (value === 4) {
        //        this.value = '45';
        //    }
        //}

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
        if (event.keyCode === jq.ui.keyCode.DOWN) {
            event.preventDefault();
            jq(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            jq(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
        }
    }).on("input", function () {
        var value = jq(this).val();
        if (value.length > 2) {
            jq(this).val(value.slice(0, 2));
        }
    });
}

jq(function () {
    autoCompleteTimer()
});

function SaveNewEntity() {
    var model = {};
    var refId = jQuery('#refEntId').val();
    var refTypeId = refId.replace('__EntityName', '__EntityTypeString');
    var EntityCompany = jQuery('#EntityCompany').val();
    jQuery('#EntityCompany').next('span').html('');
    if (EntityCompany == undefined || EntityCompany == '' || EntityCompany == null) {
        jQuery('#EntityCompany').next('span').html('Please enter company name.');
        jQuery('#EntityCompany').focus();
        return false;
    }

    var addr = jQuery('#EntityAddr').val();
    jQuery('#EntityAddr').next('span').html('');
    //if (addr == undefined || addr == '' || addr == null) {
    //    jQuery('#EntityAddr').next('span').html('Please enter your address.');
    //    jQuery('#EntityAddr').focus();
    //    return false;
    //}

    var EntityCity = jQuery('#EntityCity').val();
    jQuery('#EntityCity').next('span').html('');
    //if (EntityCity == undefined || EntityCity == '' || EntityCity == null) {
    //    jQuery('#EntityCity').next('span').html('Please enter city.');
    //    jQuery('#EntityCity').focus();
    //    return false;
    //}

    var EntityState = jQuery('#EntityState').val();
    jQuery('#EntityState').next('span').html('');
    //if (EntityState == undefined || EntityState == '' || EntityState == null) {
    //    jQuery('#EntityState').next('span').html('Please enter state.');
    //    jQuery('#EntityState').focus();
    //    return false;
    //}

    var EntityCounty = jQuery('#EntityCounty').val();
    jQuery('#EntityCounty').next('span').html('');
    //if (EntityCounty == undefined || EntityCounty == '' || EntityCounty == null) {
    //    jQuery('#EntityCounty').next('span').html('Please enter county.');
    //    jQuery('#EntityCounty').focus();
    //    return false;
    //}

    var EntityZip = jQuery('#EntityZip').val();
    jQuery('#EntityZip').next('span').html('');
    //if (EntityZip == undefined || EntityZip == '' || EntityZip == null) {
    //    jQuery('#EntityZip').next('span').html('Please enter zip code.');
    //    jQuery('#EntityZip').focus();
    //    return false;
    //}

    var FirstName = jQuery('#FirstName').val();
    jQuery('#FirstName').next('span').html('');
    //if (FirstName == undefined || FirstName == '' || FirstName == null) {
    //    jQuery('#FirstName').next('span').html('Please enter your first name.');
    //    jQuery('#FirstName').focus();
    //    return false;
    //}

    var Lastname = jQuery('#Lastname').val();

    var EntityPhone = jQuery('#EntityPhone').val();
    jQuery('#EntityPhone').next('span').html('');
    //if (EntityPhone == undefined || EntityPhone == '' || EntityPhone == null) {
    //    jQuery('#EntityPhone').next('span').html('Please enter contact number.');
    //    jQuery('#EntityPhone').focus();
    //    return false;
    //}

    var EntityExt = jQuery('#EntityExt').val();
    jQuery('#EntityExt').next('span').html('');
    //if (EntityExt == undefined || EntityExt == '' || EntityExt == null) {
    //    jQuery('#EntityExt').next('span').html('Please enter extension.');
    //    jQuery('#EntityExt').focus();
    //    return false;
    //}
    var EntityEmail = jQuery('#EntityEmail').val();
    jQuery('#EntityEmail').next('span').html('');
    //if (EntityEmail == undefined || EntityEmail == '' || EntityEmail == null) {
    //    jQuery('#EntityEmail').next('span').html('Please enter email.');
    //    jQuery('#EntityEmail').focus();
    //    return false;
    //}
    if (chkEntityEmail == 0) {
        jQuery('#EntityEmail').next('span').html('Invalid email address.');
        return false;
    }
    model.Company = EntityCompany;
    model.MailCity = EntityCity;
    model.ContactPhone = EntityPhone;
    model.MailState = EntityState;
    model.MailZip = EntityZip;
    model.Extension = EntityExt;
    model.MailAddress = addr;
    model.ContactName = FirstName + " " + Lastname;
    model.MemberType = '13';
    model.Inactive = false;
    model.Email = EntityEmail;
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/RegNonMember/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            jq('.mem-modal').modal("hide");
            if (response.success == true) {
                alert('Company Added Successfully.');
                jq('#' + refId).val(response.data.Company);
                jq('#' + refId).next('input').val(response.data.ID);
                jq('#' + refId).focus();
            }
            else {
                alert('Something went wrong please try again');
                jq('#' + refId).val('');
                jq('#' + refId).focus();
            }
        },
        error: function (response) {
            jq('.mem-modal').modal("hide");
            alert('Something went wrong please try again');
            alert(response.responseText);
            jq('#' + refId).focus();
        },
        failure: function (response) {
            jq('.mem-modal').modal("hide");
            alert('Something went wrong please try again');
            alert(response.responseText);
            jq('#' + refId).focus();
        }
    });
}
function SaveNewEntityType() {
    var refId = jQuery('#refEntTypeId').val();
    var EntityType = jQuery('#EntityType').val();
    jQuery('#EntityType').next('span').html('');
    if (EntityType == undefined || EntityType == '' || EntityType == null) {
        jQuery('#EntityType').next('span').html('Please enter company name.');
        jQuery('#EntityType').focus();
        return false;
    }
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/SaveEntType/',
        data: { 'EntityType': EntityType },
        async: false,
        success: function (response) {
            jq('.ent-modal').modal("hide");
            console.log(response);
            if (response.success == true) {
                jq('#' + refId).val(response.data.EntityType);
                jq('#' + refId).next('input').val(response.data.EntityID);
                jq('#' + refId).focus();
            }
            else {
                alert('Something went wrong please try again');
                jq('#' + refId).val('');
                jq('#' + refId).focus();
            }
        },
        error: function (response) {
            jq('.mem-modal').modal("hide");
            alert('Something went wrong please try again');
            alert(response.responseText);
            jq('#' + refId).focus();
        },
        failure: function (response) {
            jq('.mem-modal').modal("hide");
            alert('Something went wrong please try again');
            alert(response.responseText);
            jq('#' + refId).focus();
        }
    });
}

var phoneInput = document.getElementById('EntityPhone');
phoneInput.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});

var PhlInput = document.getElementById('PhlPhone');
PhlInput.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
var ConPhone = document.getElementById('ConPhone');
ConPhone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
jQuery('#EntityEmail').on('change', function () {
    checkUniqueEmail();
});
jQuery('#PhlEmail').on('change', function () {
    checkConUniqueEmail('PhlEmail');
});
jQuery('#ConEmail').on('change', function () {
    checkConUniqueEmail('ConEmail');
});
function checkUniqueEmail() {
    var success = true;
    jQuery('#EntityEmail').next('span').html('');
    var uniqueName = jQuery('#EntityEmail').val();
    var data = { "uniqueName": uniqueName };
    jQuery.ajax({
        type: "POST",
        url: '/Project/UniqueEmail/',
        data: { "uniqueName": uniqueName },
        dataType: "json",
        async: false,
        success: function (response) {
            if (response.success) {
                success = false;
                jQuery('#EntityEmail').next('span').html(response.statusMessage);
            }
            else {
                return true;
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return success;
}
function checkConUniqueEmail(id) {
    var success = true;
    jQuery('#' + id).next('span').html('');
    var uniqueName = jQuery('#' + id).val();
    jQuery.ajax({
        type: "POST",
        url: '/Project/ConUniqueEmail/',
        data: { "uniqueName": uniqueName, CompType: 2 },
        dataType: "json",
        async: false,
        success: function (response) {
            if (response.success) {
                success = false;
                jQuery('#' + id).next('span').html(response.statusMessage);
            }
            else {
                return true;
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return success;
}
function SaveNewPhl() {
    var model = {};
    var refId = jQuery('#refPhlId').val();
    var PhlCompany = jQuery('#PhlCompany').val();
    jQuery('#PhlCompany').next('span').html('');
    if (PhlCompany == undefined || PhlCompany == '' || PhlCompany == null) {
        jQuery('#PhlCompany').next('span').html('Please enter company name.');
        jQuery('#PhlCompany').focus();
        return false;
    }

    var addr = jQuery('#PhlAddr').val();
    jQuery('#PhlAddr').next('span').html('');
    if (addr == undefined || addr == '' || addr == null) {
        jQuery('#PhlAddr').next('span').html('Please enter your address.');
        jQuery('#PhlAddr').focus();
        return false;
    }

    var PhlCity = jQuery('#PhlCity').val();
    jQuery('#PhlCity').next('span').html('');
    if (PhlCity == undefined || PhlCity == '' || PhlCity == null) {
        jQuery('#PhlCity').next('span').html('Please enter city.');
        jQuery('#PhlCity').focus();
        return false;
    }

    var PhlState = jQuery('#PhlState').val();
    jQuery('#PhlState').next('span').html('');
    if (PhlState == undefined || PhlState == '' || PhlState == null) {
        jQuery('#PhlState').next('span').html('Please enter state.');
        jQuery('#PhlState').focus();
        return false;
    }

    var PhlCounty = jQuery('#PhlCounty').val();
    jQuery('#PhlCounty').next('span').html('');
    if (PhlCounty == undefined || PhlCounty == '' || PhlCounty == null) {
        jQuery('#PhlCounty').next('span').html('Please enter county.');
        jQuery('#PhlCounty').focus();
        return false;
    }

    var PhlZip = jQuery('#PhlZip').val();
    jQuery('#PhlZip').next('span').html('');
    if (PhlZip == undefined || PhlZip == '' || PhlZip == null) {
        jQuery('#PhlZip').next('span').html('Please enter zip code.');
        jQuery('#PhlZip').focus();
        return false;
    }

    var FirstName = jQuery('#PhlFirstName').val();
    jQuery('#PhlFirstName').next('span').html('');
    if (FirstName == undefined || FirstName == '' || FirstName == null) {
        jQuery('#PhlFirstName').next('span').html('Please enter your first name.');
        jQuery('#PhlFirstName').focus();
        return false;
    }

    var Lastname = jQuery('#PhlLastname').val();

    var PhlPhone = jQuery('#PhlPhone').val();
    jQuery('#PhlPhone').next('span').html('');
    if (PhlPhone == undefined || PhlPhone == '' || PhlPhone == null) {
        jQuery('#PhlPhone').next('span').html('Please enter contact number.');
        jQuery('#PhlPhone').focus();
        return false;
    }

    var PhlExt = jQuery('#PhlExt').val();
    jQuery('#PhlExt').next('span').html('');
    if (PhlExt == undefined || PhlExt == '' || PhlExt == null) {
        jQuery('#PhlExt').next('span').html('Please enter extension.');
        jQuery('#PhlExt').focus();
        return false;
    }
    var PhlEmail = jQuery('#PhlEmail').val();
    jQuery('#PhlEmail').next('span').html('');
    if (PhlEmail == undefined || PhlEmail == '' || PhlEmail == null) {
        jQuery('#PhlEmail').next('span').html('Please enter email.');
        jQuery('#PhlEmail').focus();
        return false;
    }
    if (Phlemail == 0) {
        jQuery('#PhlEmail').next('span').html('Invalid email address.');
        return false;
    }
    model.Company = PhlCompany;
    model.MailCity = PhlCity;
    model.ContactPhone = PhlPhone;
    model.MailState = PhlState;
    model.MailZip = PhlZip;
    model.Extension = PhlExt;
    model.MailAddress = addr;
    model.ContactName = FirstName + " " + Lastname;
    model.MemberType = '13';
    model.Inactive = false;
    model.Email = PhlEmail;
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/StaffAccount/RegisterContractor/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            jq('.mem-modal1').modal("hide");
            if (response.success == true) {
                jq('#' + refId).val(response.data.Company);
                jq('#' + refId).next('input').val(response.data.ID);
                jq('#' + refId).focus();
            }
            else {
                alert('Something went wrong please try again');
                jq('#' + refId).val('');
                jq('#' + refId).focus();
            }
        },
        error: function (response) {
            jq('.mem-modal1').modal("hide");
            alert('Something went wrong please try again');
            alert(response.responseText);
        },
        failure: function (response) {
            jq('.mem-modal1').modal("hide");
            alert('Something went wrong please try again');
            alert(response.responseText);
        }
    });
}
function SaveNewCon() {
    var model = {};
    var refId = jQuery('#refConId').val();
    var CompId = jQuery('#refComId').val();
    var FirstName = jQuery('#ConFirstName').val();
    jQuery('#ConFirstName').next('span').html('');
    if (FirstName == undefined || FirstName == '' || FirstName == null) {
        jQuery('#ConFirstName').next('span').html('Please enter your first name.');
        jQuery('#ConFirstName').focus();
        return false;
    }

    var Lastname = jQuery('#ConLastname').val();

    var ConPhone = jQuery('#ConPhone').val();
    jQuery('#ConPhone').next('span').html('');
    if (ConPhone == undefined || ConPhone == '' || ConPhone == null) {
        jQuery('#ConPhone').next('span').html('Please enter contact number.');
        jQuery('#ConPhone').focus();
        return false;
    }

    var ConExt = jQuery('#ConExt').val();
    jQuery('#ConExt').next('span').html('');
    if (ConExt == undefined || ConExt == '' || ConExt == null) {
        jQuery('#ConExt').next('span').html('Please enter extension.');
        jQuery('#ConExt').focus();
        return false;
    }
    var ConEmail = jQuery('#ConEmail').val();
    jQuery('#ConEmail').next('span').html('');
    if (ConEmail == undefined || ConEmail == '' || ConEmail == null) {
        jQuery('#ConEmail').next('span').html('Please enter email.');
        jQuery('#ConEmail').focus();
        return false;
    }
    if (chkConEmail == 0) {
        jQuery('#ConEmail').next('span').html('Invalid email address.');
        return false;
    }
    model.ContactPhone = ConPhone;
    model.Extension = ConExt;
    model.ContactName = FirstName + " " + Lastname;
    model.Email = ConEmail;
    model.CompId = CompId;
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/RegPhlCon/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            jq('.con-modal').modal("hide");
            if (response.success == true) {
                jq('#' + refId).val(response.data.Contact);
                jq('#' + refId).next('input').val(response.data.ConID);
                jq('#' + refId).focus();

            }
            else {
                alert('Something went wrong please try again');
                jq('#' + refId).val('');
                jq('#' + refId).focus();
            }
        },
        error: function (response) {
            jq('.con-modal').modal("hide");
            alert('Something went wrong please try again');
            alert(response.responseText);
        },
        failure: function (response) {
            jq('.con-modal').modal("hide");
            alert('Something went wrong please try again');
            alert(response.responseText);
        }
    });
}

var Phlemail = 0;
var chkEntityEmail = 3;
var chkConEmail = 0
function validateEntityEmailInput(input) {
    const email = input.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (email == '') {
        chkEntityEmail = 2;
    }
    else if (!emailRegex.test(email)) {
        input.classList.add('invalid');
        jQuery('#EntityEmail').next('span').html('Invalid email address.');
        chkEntityEmail = 0
        return false;
        //input.setCustomValidity('Invalid email address');
    } else {
        input.classList.remove('invalid');
        jQuery('#EntityEmail').next('span').html('');
        chkEntityEmail = 1;
    }
}
function validateEmailInput(input) {
    const email = input.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!emailRegex.test(email)) {
        input.classList.add('invalid');
        jQuery('#PhlEmail').next('span').html('Invalid email address.');
        return false;
        //input.setCustomValidity('Invalid email address');
    } else {
        input.classList.remove('invalid');
        jQuery('#PhlEmail').next('span').html('');
        Phlemail = 1;
    }
}
function validateConEmailInput(input) {
    const email = input.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!emailRegex.test(email)) {
        input.classList.add('invalid');
        jQuery('#ConEmail').next('span').html('Invalid email address.');
        return false;
        //input.setCustomValidity('Invalid email address');
    } else {
        input.classList.remove('invalid');
        jQuery('#ConEmail').next('span').html('');
        chkConEmail = 1;
    }
}
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
}

document.addEventListener("DOMContentLoaded", function () {
    const dateInput = document.getElementById("BidDt");

    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();

    // dateInput.placeholder = `MM/dd/${currentYear}`;

    dateValidate()
});

function dateValidate() {
    const dateInputs = document.querySelectorAll('[id$="BidDate"]');

    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();

    dateInputs.forEach(function (dateInput) {
        //// Set the placeholder text with the current year
        //dateInput.placeholder = `MM/dd/${currentYear}`;

        dateInput.addEventListener("change", function () {
            const value = dateInput.value;
            var callingElementId = event.target.id;
            validateDateControl(callingElementId);
        });
    });
}
function checkAddenda() {
    var hiddenFieldValue = jq("#strAddenda").val();

    var checkbox = jq(".chkAddBox[data-value='" + hiddenFieldValue + "']");
    console.log(checkbox);
    // Check the checkbox if it exists
    //if (checkbox.length > 0) {
    //    checkbox.prop("checked", true);
    //}
}
function funClose() {
    if (confirm("Close without saving?")) {
        window.location.href = "/StaffAccount/Dashboard";
    }
}
