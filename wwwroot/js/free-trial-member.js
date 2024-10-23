
function FreeTrialMember() {
    var company = jQuery('input[name = Company]').val();
    if (company == undefined || company == '' || company == null) {
        alert('Please enter company.');
        return false;
    }
    var mAdd = jQuery('input[name = MailAddress]').val();
    if (mAdd == undefined || mAdd == '' || mAdd == null) {
        alert('Please enter address.');
        return false;
    }
    var mCity = jQuery('input[name = MailCity]').val();
    if (mCity == undefined || mCity == '' || mCity == null) {
        alert('Please enter City.');
        return false;
    }
    var mZip = jQuery('input[name = MailZip]').val();
    if (mZip == undefined || mZip == '' || mZip == null) {
        alert('Please enter zip code.');
        return false;
    }
    var lName = jQuery('input[name = FirstName]').val();
    if (lName == undefined || lName == '' || lName == null) {
        alert('Please enter first name.');
        return false;
    }
    var lastName = jQuery('input[name = LastName]').val();
    var conPhone = jQuery('input[name = ContactPhone]').val();
    if (conPhone == undefined || conPhone == '' || conPhone == null) {
        alert('Please enter contact phone number');
        return false;
    }
    var ContactPhone = jQuery('input[name = ContactPhone]').val();
    jQuery('input[name = ContactPhone]').val(ContactPhone);
    var model = {};
    //model.ID = $('#hdID').val();
    model.Company = jQuery('#Company').val();
    model.MailAddress = jQuery('#MailAddress').val();
    model.MailCity = jQuery('#MailCity').val();
    model.MailState = jQuery('#MailState').val();
    model.LocAddr2 = jQuery('#LocAddr2').val();
    model.MailZip = jQuery('#MailZip').val();
    if (lastName != null && lastName != "") {
        model.FirstName = jQuery('#FirstName').val() + lastName;
    }
    else {
        model.FirstName = jQuery('#FirstName').val();
    }

    model.ContactPhone = jQuery('#ContactPhone').val();
    model.Email = jQuery('#ContactEmail').val();
    model.Term = "Free Trial";
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        url: '/MemberShip/SaveFreeTrialMember/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
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

var contactPhone = document.getElementById('ContactPhone');
var result = document.getElementById('ContactPhone');// only for debugging purposes

contactPhone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
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
    jQuery('#MailZip').keyup(function () {
        var zip = jQuery(this).val();
        jQuery('#MailCity').val('');
        jQuery('#MailState').val('');
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