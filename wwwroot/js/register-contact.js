//var jq = $.noConflict();
//Populating multiple counties modal
//Lowering city
const upperCaseFirstLetter = string =>
    `${string.slice(0, 1).toUpperCase()}${string.slice(1)}`;

const lowerCaseAllWordsExceptFirstLetters = string =>
    string.replaceAll(/\S*/g, word =>
        `${word.slice(0, 1)}${word.slice(1).toLowerCase()}`
    );
$(document).ready(function () {
    $('#ConZip').keyup(function () {
        var zip = $(this).val();
        $('#ConCity').val('');
        $('#ConState').val(0);
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
                            $("#ConState option:contains('" + State + "')").prop("selected", true);
                            var formattedCity = upperCaseFirstLetter(lowerCaseAllWordsExceptFirstLetters(City));
                            $('#ConCity').val(formattedCity);
                        }
                        else {
                            $('#ConCity').val('');
                            $('#ConState').val(0);
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

var ConInput = document.getElementById('ConPhone');
ConInput.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});

$('#ConEmail').on('change', function () {
    validateEmailInput(this);
    checkUniqueEmail();
});

function SaveNewArch() {
    var model = {};
    var ConCompany = jQuery('#ConCompany').val();
    jQuery('#ConCompany').next('span').html('');
    if (ConCompany == undefined || ConCompany == '' || ConCompany == null) {
        jQuery('#ConCompany').next('span').html('Please enter company name.');
        jQuery('#ConCompany').focus();
        return false;
    }

    var addr = jQuery('#ConAddr').val();
    jQuery('#ConAddr').next('span').html('');
    if (addr == undefined || addr == '' || addr == null) {
        jQuery('#ConAddr').next('span').html('Please enter your address.');
        jQuery('#ConAddr').focus();
        return false;
    }

    var ConCity = jQuery('#ConCity').val();
    jQuery('#ConCity').next('span').html('');
    if (ConCity == undefined || ConCity == '' || ConCity == null) {
        jQuery('#ConCity').next('span').html('Please enter city.');
        jQuery('#ConCity').focus();
        return false;
    }

    var ConState = jQuery('#ConState').val();
    var ConStateText = jQuery('#ConState option:selected').text();
    jQuery('#ConState').next('span').html('');
    if (ConState == 0 || ConState == '0') {
        jQuery('#ConState').next('span').html('Please enter state.');
        jQuery('#ConState').focus();
        return false;
    }

    var ConZip = jQuery('#ConZip').val();
    jQuery('#ConZip').next('span').html('');
    if (ConZip == undefined || ConZip == '' || ConZip == null) {
        jQuery('#ConZip').next('span').html('Please enter zip code.');
        jQuery('#ConZip').focus();
        return false;
    }

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
    if (ConPhone.length != 14) {
        jQuery('#ConPhone').next('span').html('Phone number is not correct.');
        return false;
    }
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
    if (Conemail == 0) {
        jQuery('#ConEmail').next('span').html('Invalid email address.');
        return false;
    }
    var chkVal = false;
    var Arch = document.getElementById('ConArch');
    var chkArch = Arch.checked;
    if (chkArch)
    {
        chkVal = true;
    }
    var Cont = document.getElementById('ConCont');
    var chkCont = Cont.checked;
    if (chkCont) {
        chkVal = true;
    }
    if (!chkVal)
    {
        jQuery('#ConError').text('You need to select either contractor or architect.');
        return false;
    }
    jQuery('#spnSave').css('pointer-events', 'none');
    model.Company = ConCompany;
    model.MailCity = ConCity;
    model.ContactPhone = ConPhone;
    model.MailState = ConStateText;
    model.MailZip = ConZip;
    model.Extension = ConExt;
    model.MailAddress = addr;
    model.ContactName = FirstName + " " + Lastname;
    model.Email = ConEmail;
    model.IsArchitect = chkArch; 
    model.IsContractor = chkCont;
    $.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Project/SaveNewContact/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success == true) {
                alert(response.statusMessage);
                // Go to previous working tab
                var pathname = 'about';
                window.location.href = "/StaffAccount/MemberManagement?returnUrl=" + pathname;
            }
            else {
                alert(response.statusMessage);
                window.location.reload();
            }
        },
        error: function (response) {
            alert('Something went wrong please try again');
            alert(response.responseText);
        },
        failure: function (response) {
            alert('Something went wrong please try again');
            alert(response.responseText);
        }
    });
}
var Conemail = 0;
function validateEmailInput(input) {
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
        Conemail++;
    }
}

function checkUniqueEmail() {
    var success = true;
    jQuery('#ConEmail').next('span').html('');
    var uniqueName = jQuery('#ConEmail').val();
    var data = { "uniqueName": uniqueName };
    jQuery.ajax({
        type: "POST",
        url: '/MemberShip/UniqueEmail/',
        data: { "uniqueName": uniqueName },
        dataType: "json",
        async: false,
        //contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.success) {
                success = false;
                jQuery('#ConEmail').next('span').html(response.statusMessage);
            }
            else {
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
const spanElement = document.getElementById('spnSave');

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