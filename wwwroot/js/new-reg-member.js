// Get the current URL
var currentUrl = window.location.href;

// Remove the 'returnUrl' parameter from the URL
var updatedUrl = currentUrl.split('?')[0];

// Update the URL without the 'returnUrl' parameter
window.history.replaceState({}, document.title, updatedUrl);
jQuery(document).ready(function () {
    $('#SelectPKG').prop('disabled', true);
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/StaffAccount/GetPKGList',
        data: {},
        async: false,
        success: function (response) {
            var datalist = response.data;
            for (var key in datalist) {
                if (datalist.hasOwnProperty(key)) {
                    var optgroup = jQuery('<optgroup label="' + key + '">');

                    for (var i = 0; i < datalist[key].length; i++) {
                        var option = jQuery('<option value="' + datalist[key][i].SubMemberShipPlanId + '">' + datalist[key][i].SubMemberShipPlanName + '</option>');
                        optgroup.append(option);
                    }
                    jQuery('#SelectPKG').append(optgroup);
                }
            }
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
});
jQuery('#SelectTerm').on('change', function () {
    var text = jQuery('select[id=SelectTerm] option').filter(':selected').text();
    var id = jQuery('select[id=SelectTerm] option').filter(':selected').val();
    if (id == 0) {
        jQuery('#SelectPKG').val(0);
        jQuery('#hdnPKG').val('');
        $('#SelectPKG').prop('disabled', true);
    }
    else {
        $('#SelectPKG').prop('disabled', false);
        jQuery('#hdnTerm').val(text);
    }
});
jQuery('#SelectPKG').on('change', function () {
    var id = jQuery('#SelectPKG').val();
    var text = jQuery('select[id=SelectPKG] option').filter(':selected').text();
    var term = jQuery('select[id=SelectTerm] option').filter(':selected').text();
    jQuery('#hdnPKGId').val(id);
    jQuery('#hdnPKG').val(text);
    jQuery.ajax({
        type: "POST",
        url: '/StaffAccount/GetPkgDetails/',
        data: { "pkg": text, "term": term },
        dataType: "json",
        async: false,
        success: function (response) {
            if (response.success) {
                // Check if the properties exist in the response
                if (response.MemberTypeCounty) {
                    var memberTypeCounty = response.MemberTypeCounty;
                    // Access properties of memberTypeCounty as needed
                    $('#MemberType').val(memberTypeCounty.MemberType);
                }
                if (response.MemberShipSubPlans) {
                    var memberShipSubPlans = response.MemberShipSubPlans;
                    // Access properties of memberShipSubPlans as needed
                    console.log("SubMemberShipPlanName: " + memberShipSubPlans.SubMemberShipPlanName);
                    console.log("YearlyPrice: " + memberShipSubPlans.YearlyPrice);
                    console.log("QuarterlyPrice: " + memberShipSubPlans.QuarterlyPrice);
                    console.log("MonthlyPrice: " + memberShipSubPlans.MonthlyPrice);
                    if (term == "Yearly") {
                        $('#PKGCost').val(memberShipSubPlans.YearlyPrice);
                    }
                    if (term == "Quarterly") {
                        $('#PKGCost').val(memberShipSubPlans.QuarterlyPrice);
                    }
                    if (term == "Monthly") {
                        $('#PKGCost').val(memberShipSubPlans.MonthlyPrice);
                    }
                }
            } else {
                // Handle the case where the response indicates failure
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR.responseText);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });

});
jQuery('#MailState').on('change', function () {
    var id = jQuery('select[id=MailState] option').filter(':selected').val();
    jQuery('#hdnState').val(id);
});
jQuery('#ContactEmail').on('change', function () {
    checkUniqueEmail();
});
function checkUniqueEmail() {
    var success = true;
    jQuery('#ContactEmail').next('span').html('');
    var uniqueName = jQuery('#ContactEmail').val();
    var data = { "uniqueName": uniqueName };
    jQuery.ajax({
        type: "POST",
        url: '/MemberShip/UniqueEmail/',
        data: { "uniqueName": uniqueName },
        dataType: "json",
        async: false,
        success: function (response) {
            if (response.success) {
                success = false;
                jQuery('#ContactEmail').next('span').html(response.statusMessage);
            }
            else {
                jQuery('#ContactEmail').next('span').html('');
                return true;
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR.responseText);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
    return success;
}
jQuery('#Company').on('change', function () {
    checkUniqueCompany();
});
function checkUniqueCompany() {
    var success = true;
    jQuery('#Company').next('span').html('');
    var uniqueCompany = jQuery('#Company').val();
    jQuery.ajax({
        type: "POST",
        url: '/MemberShip/UniqueCompany/',
        data: { "uniqueCompany": uniqueCompany },
        dataType: "json",
        async: false,
        //contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.success) {
                success = false;
                jQuery('#Company').next('span').html(response.statusMessage);
                return false;
            }
            else {
                jQuery('#Company').next('span').html('');
                return true;
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
    return success;
}
function ValidateConfirmPass() {
    var passData = jQuery('input[id = ContactPassword]').val();
    var ConfirmPassData = jQuery('input[id = ContactPasswordCofirmation]').val();
    if (passData && ConfirmPassData) {

        if (passData != ConfirmPassData) {
            jQuery('#PassError').html('Password and confirm password should be matching');
            return false;
        }
        else {
            jQuery('#PassError').html('');
        }

    }
}
function validateInputLength(arg) {
    var minLength = 14;
    var maxLength = 14;

    var inputVal = jQuery("#" + arg).val();
    var inputLength = inputVal.length;
    jQuery("#" + arg).next('span').html('');
    if (inputLength < minLength) {
        jQuery("#" + arg).next('span').html('Input valid phone number');
        jQuery("#" + arg).focus();
        return false;
    } else if (inputLength > maxLength) {
        jQuery("#" + arg).next('span').html('Input valid phone number');
        jQuery("#" + arg).focus();
        return false;
    } else {
        jQuery("#" + arg).next('span').html(''); // Clear the validation message
    }
}
var phoneInput = document.getElementById('CompanyPhone');

var contactPhone = document.getElementById('ContactPhone');
//var result = document.getElementById('ContactPhone');// only for debugging purposes

phoneInput.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});

contactPhone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
var myInput = document.getElementById("ContactPassword");
var capital = document.getElementById("capital");
var number = document.getElementById("number");
var length = document.getElementById("length");
var letter = document.getElementById("letter");

myInput.onkeyup = function () {
    ValidateCompleaxPassword();
}
function ValidateCompleaxPassword() {
    var validatePassword = true;
    // Validate lowercase letters
    var lowerCaseLetters = /[a-z]/g;
    if (myInput.value.match(lowerCaseLetters)) {
        letter.classList.remove("invalid");
        letter.classList.add("valid");
    } else {
        letter.classList.remove("valid");
        letter.classList.add("invalid");
        validatePassword = false;
    }

    // Validate capital letters
    var upperCaseLetters = /[A-Z]/g;
    if (myInput.value.match(upperCaseLetters)) {
        capital.classList.remove("invalid");
        capital.classList.add("valid");
    } else {
        capital.classList.remove("valid");
        capital.classList.add("invalid");
        validatePassword = false;
    }

    // Validate numbers
    var numbers = /[0-9]/g;
    if (myInput.value.match(numbers)) {
        number.classList.remove("invalid");
        number.classList.add("valid");
    } else {
        number.classList.remove("valid");
        number.classList.add("invalid");
        validatePassword = false;
    }

    // Validate length
    if (myInput.value.length >= 8) {
        length.classList.remove("invalid");
        length.classList.add("valid");
    } else {
        length.classList.remove("valid");
        length.classList.add("invalid");
        validatePassword = false;
    }
    return validatePassword;
}
//Lowering city
const upperCaseFirstLetter = string =>
    `${string.slice(0, 1).toUpperCase()}${string.slice(1)}`;

const lowerCaseAllWordsExceptFirstLetters = string =>
    string.replaceAll(/\S*/g, word =>
        `${word.slice(0, 1)}${word.slice(1).toLowerCase()}`
    );
jQuery(document).ready(function () {
    jQuery('#MailZip').keyup(function () {
        var zip = jQuery(this).val();
        jQuery('#MailCity').val('');
        jQuery('#MailState').val('');
        var xmldata = '<CityStateLookupRequest USERID="405PLANC0113"><ZipCode><Zip5>' + zip + '</Zip5></ZipCode></CityStateLookupRequest>';
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
                                    jQuery('#MailCity').val(formattedCity);
                                    jQuery('#MailState').val(stateId);
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
                            jQuery('#MailCity').val('');
                            jQuery('#MailState').val('');
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
    });
});
jQuery(document).ready(function () {
    jQuery('#BillZip').keyup(function () {
        var zip = jQuery(this).val();
        jQuery('#BillCity').val('');
        jQuery('#BillState').val('');
        var xmldata = '<CityStateLookupRequest USERID="405PLANC0113"><ZipCode><Zip5>' + zip + '</Zip5></ZipCode></CityStateLookupRequest>';
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
                                    jQuery('#BillCity').val(formattedCity);
                                    jQuery('#BillState').val(stateId);
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
                            jQuery('#BillCity').val('');
                            jQuery('#BillState').val('');
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
    });
});
function SaveMember() {
    var term = jQuery('select[id=SelectTerm] option').filter(':selected').val();
    $('#SelectTerm').next('span').html('');
    if (term == null || term == "" || term == undefined || term == 0) {
        $('#SelectTerm').next('span').html('Please select term.');
        $('#SelectTerm').focus();
        return false;
    }
    var pkg = jQuery('select[id=SelectPKG] option').filter(':selected').val();
    $('#SelectPKG').next('span').html('');
    if (pkg == null || pkg == "" || pkg == undefined || pkg == 0) {
        $('#SelectPKG').next('span').html('Please select package.');
        $('#SelectPKG').focus();
        return false;
    }
    var company = jQuery('#Company').val();
    var dba = jQuery('#Dba').val();
    var existcompany = jQuery('#Company').next('span').html();
    if (company == null || company == "" || company == undefined) {
        $('#Company').next('span').html('Please enter company name.');
        $('#Company').focus();
        return false;
    }
    else if (existcompany != "") {
        jQuery('#Company').next('span').html('Something went wrong');
        $('#Company').focus();
        return false;
    }
    var addr = jQuery('#MailAddress').val();
    $('#MailAddress').next('span').html('');
    if (addr == null || addr == "" || addr == undefined) {
        $('#MailAddress').next('span').html('Please enter address.');
        $('#MailAddress').focus();
        return false;
    }
    var city = jQuery('#MailCity').val();
    $('#MailCity').next('span').html('');
    if (city == null || city == "" || city == undefined) {
        $('#MailCity').next('span').html('Please enter city.');
        $('#MailCity').focus();
        return false;
    }
    var state = jQuery('#MailState').val();
    $('#MailState').next('span').html('');
    if (state == null || state == "" || state == undefined || state == 0) {
        $('#MailState').next('span').html('Please enter state.');
        $('#MailState').focus();
        return false;
    }
    var zip = jQuery('#MailZip').val();
    $('#MailZip').next('span').html('');
    if (zip == null || zip == "" || zip == undefined) {
        $('#MailZip').next('span').html('Please enter zip code.');
        $('#MailZip').focus();
        return false;
    }
    var cPhone = jQuery('#CompanyPhone').val();
    var ContactPhone = jQuery('#ContactPhone').val();
    var extention = jQuery('#Extension').val();
    $('#CompanyPhone').next('span').html('');
    if (cPhone == null || cPhone == "" || cPhone == undefined) {
        $('#CompanyPhone').next('span').html('Please enter company phone.');
        $('#CompanyPhone').focus();
        return false;
    }
    var firstName = jQuery('#FirstName').val();
    var lastName = jQuery('#LastName').val();
    $('#FirstName').next('span').html('');
    if (firstName == null || firstName == "" || firstName == undefined) {
        $('#FirstName').next('span').html('Please enter first name.');
        $('#FirstName').focus();
        return false;
    }
    var ContactEmail = jQuery('#ContactEmail').val();
    var chkemail = $('#ContactEmail').next('span').html();
    if (ContactEmail == null || ContactEmail == "" || ContactEmail == undefined) {
        $('#ContactEmail').next('span').html('Please enter email.');
        $('#ContactEmail').focus();
        return false;
    }
    if (chkemail != "") {
        $('#ContactEmail').next('span').html('Please enter valid email.');
        $('#ContactEmail').focus();
        return false;
    }
    var ContactPassword = jQuery('#ContactPassword').val();
    $('#ContactPassword').next('span').html('');
    if (ContactPassword == null || ContactPassword == "" || ContactPassword == undefined) {
        $('#ContactPassword').next('span').html('Please enter password.');
        $('#ContactPassword').focus();
        return false;
    }
    if (ContactPassword.length < 8) {
        $('#ContactPassword').next('span').html('Password must be at least 8 characters long.');
        $('#ContactPassword').focus();
        return false;
    }
    var ContactPasswordCofirmation = jQuery('#ContactPasswordCofirmation').val();
    var passError = jQuery('#PassError').html();
    $('#ContactPasswordCofirmation').next('span').html('');
    if (ContactPasswordCofirmation == null || ContactPasswordCofirmation == "" || ContactPasswordCofirmation == undefined) {
        $('#ContactPasswordCofirmation').next('span').html('Please enter confirm password.');
        $('#ContactPasswordCofirmation').focus();
        return false;
    }
    if (passError != "") {
        jQuery('#PassError').html('Password and confirm password should be matching');
        return false;
    }
    var model = {};
    var bAdd = jQuery('input[name = BillAddress]').val();
    if (bAdd == null || bAdd == "") {
        model.BillAddress = addr;
    }
    else {
        model.BillAddress = jQuery('input[name = BillAddress]').val();
    }

    var bCity = jQuery('input[name = BillCity]').val();
    if (bCity == null || bCity == "") {
        model.BillCity = city;
    }
    else {
        model.BillCity = jQuery('#BillCity').val();
    }

    var BillState = jQuery('select[name=BillState] option').filter(':selected').val();
    if (BillState == '0') {
        var MState = jQuery('select[name=MailState] option').filter(':selected').val();
        model.BillState = state;
    }
    else {
        model.BillState = jQuery('select[name=BillState] option').filter(':selected').val();
    }

    var bZip = jQuery('input[name = BillZip]').val();
    if (bZip == null || bZip == "") {
        model.BillZip = zip;
    }
    else {
        model.BillZip = jQuery('#BillZip').val();
    }
    var BillEmail = jQuery('input[name = BillEmail]').val();
    if (BillEmail == null || BillEmail == "") {
        model.BillEmail = jQuery('input[name = ContactEmail]').val();
    }
    else {
        model.BillEmail = jQuery('input[name = BillEmail]').val();
    }
    model.Company = company;
    model.DBA = dba;
    model.MailAddress = addr;
    model.MailCity = city;
    model.MailState = state;
    model.MailZip = zip;
    model.CompanyPhone = cPhone;
    model.FirstName = firstName
    model.LastName = lastName;
    model.ContactPhone = ContactPhone;
    model.Extension = extention;
    model.ContactEmail = ContactEmail;
    model.ContactPassword = ContactPassword;
    model.hdnTerm = jQuery('#hdnTerm').val();
    model.MemberType = jQuery('#MemberType').val();
    model.MemberCost = jQuery('#PKGCost').val();
    model.ID = jQuery('#ID').val();
    model.ContractorName = jQuery('#ContractorName').val();
    model.ArchitectName = jQuery('#ArchitectName').val();
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        url: '/StaffAccount/SaveNewRegMember/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                alert(response.statusMessage);
                window.location.href = "/StaffAccount/MemberManagement";
            }
            else {
                alert('Something went wrong');
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