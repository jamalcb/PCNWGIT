var myInput = document.getElementById("ContactPassword");
var letter = document.getElementById("letter");
var capital = document.getElementById("capital");
var number = document.getElementById("number");
var length = document.getElementById("length");
// When the user clicks on the password field, show the message box
myInput.onfocus = function () {
    document.getElementById("error_message").style.display = "block";
}

// When the user clicks outside of the password field, hide the message box
myInput.onblur = function () {
    document.getElementById("error_message").style.display = "none";
}

// When the user starts to type something inside the password field
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

/************* Phone Number Masking ********************/
//var phoneInput = document.getElementById('ctrlCompanyPhone');
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
function managePhoneNumber() {
   result.value = phoneInput.value.replace(/\D/g, '');
}
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
        //contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.success) {
                success = false;
                jQuery('#ContactEmail').next('span').html(response.statusMessage);
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
function AcceptPay(arg) {
    jQuery('.loginError').html('');
    jQuery('#chgPackage').prev('span').html('');
    if (jQuery('input[name = CheckRadio]').val() != 'pass') {
        jQuery('#chgPackage').prev('span').html('You need to select a valid plan');
        jQuery('#chgPackage').focus();
        return false;
    }
    if (arg == 2)
    {
        var PassVar = jQuery('input[name = ContactPassword]').val();
        if (PassVar == undefined || PassVar == '' || PassVar == null) {
            jQuery('#ContactPassword').next('span').html('Please enter Password.');
            jQuery('#ContactPassword').focus();
            return false;
        }
    }
    var company = jQuery('input[name = Company]').val();
    
    if (company == undefined || company == '' || company == null) {
        jQuery('#Company').next('span').html('Please enter company.');
        jQuery('#Company').focus();
        return false;
    }
    var mAdd = jQuery('input[name = MailAddress]').val();
    if (mAdd == undefined || mAdd == '' || mAdd == null) {
        jQuery('#MailAddress').next('span').html('Please enter address.');
        jQuery('#MailAddress').focus();
        return false;
    }
    var mCity = jQuery('input[name = MailCity]').val();
    if (mCity == undefined || mCity == '' || mCity == null) {
        jQuery('#MailCity').next('span').html('Please enter city.');
        jQuery('#MailCity').focus();
        return false;
    }
    var MailState = jQuery('select[name=MailState] option').filter(':selected').val();
    if (MailState == '0') {
        jQuery('#MailState').next('span').html('Please select a valid mail state');
        jQuery('#MailState').focus();
        return false;
    }
    var mZip = jQuery('input[name = MailZip]').val();
    if (mZip == undefined || mZip == '' || mZip == null) {
        jQuery('#MailZip').next('span').html('Please enter zip code.');
        jQuery('#MailZip').focus();
        return false;
    }
    var CompanyPhone = jQuery('input[name = CompanyPhone]').val();
    if (CompanyPhone == undefined || CompanyPhone == '' || CompanyPhone == null) {
        jQuery('#CompanyPhone').next('span').html('Please enter company phone number.');
        jQuery('#CompanyPhone').focus();
        return false;
    }
    var bAdd = jQuery('input[name = BillAddress]').val();
    if (bAdd == undefined || bAdd == '' || bAdd == null) {
        jQuery('#BillAddress').next('span').html('Please enter billing address.');
        jQuery('#BillAddress').focus();
        return false;
    }
    var bCity = jQuery('input[name = BillCity]').val();
    if (bCity == undefined || bCity == '' || bCity == null) {
        jQuery('#BillCity').next('span').html('Please enter billing city.');
        jQuery('#BillCity').focus();
        return false;
    }
    var BillState = jQuery('select[name=BillState] option').filter(':selected').val();
    if (BillState == '0') {
        jQuery('#BillState').next('span').html('Please select a valid bill state');
        jQuery('#BillState').focus();
        return false;
    }
    var bZip = jQuery('input[name = BillZip]').val();
    if (bZip == undefined || bZip == '' || bZip == null) {
        jQuery('#BillZip').next('span').html('Please enter billing zip.');
        jQuery('#BillZip').focus();
        return false;
    }
    var BillEmail = jQuery('input[name = BillEmail]').val();
    if (BillEmail == undefined || BillEmail == '' || BillEmail == null) {
        jQuery('#BillEmail').next('span').html('Please enter billing email.');
        jQuery('#BillEmail').focus();
        return false;
    }
    var lName = jQuery('input[name = FirstName]').val();
    if (lName == undefined || lName == '' || lName == null) {
        jQuery('#FirstName').next('span').html('Please enter first name.');
        jQuery('#FirstName').focus();
        return false;
    }
    var fName = jQuery('input[name = LastName]').val();
    if (fName == undefined || fName == '' || fName == null) {
        jQuery('#LastName').next('span').html('Please enter last name.');
        jQuery('#LastName').focus();
        return false;
    }
   
    var conPhone = jQuery('input[name = ContactPhone]').val();
    
    var conEmail = jQuery('input[name = ContactEmail]').val();
    if (conEmail == undefined || conEmail == '' || conEmail == null) {
        jQuery('#ContactEmail').next('span').html('Please enter username or email.');
        jQuery('#ContactEmail').focus();
        return false;
    }
   
    var isUniqueEmail = checkUniqueEmail();
    if (isUniqueEmail == false) {
        jQuery('#EmailError').html('This email is already registered');
        jQuery('#EmailError').focus();
        return false;
    }
    var isValidPassword = ValidateCompleaxPassword();
    if (isValidPassword == false) {
        jQuery('#PassError').html('Password must contain the following: A Lowerecase letter, A Capital (uppercase) letter,A number,Minimum 8 characters');
        return false;
    }
    var passData = jQuery('input[name = hdnPass]').val();
    var ConfirmPassData = jQuery('input[name = hdnPassConfirm]').val();
   
    if (passData && ConfirmPassData) {
        if (passData.length < 3) {

            jQuery('#PassError').html('Password lenghth should be greater than 3');
            return false;
        }
        else if (passData != ConfirmPassData) {
            jQuery('#PassError').html('Password and confirm password should be matching');
            return false;
        }


    }
    else {

        if (passData) {
            jQuery('#PassError').html('You need to confirm your password');
            jQuery('#PassError').focus();
            return false;
        }
        else if (ConfirmPassData) {
            jQuery('#PassError').html('Password value required');
            jQuery('#PassError').focus();
            return false;
        }
        else {
            jQuery('#PassError').html('Password value required');
            jQuery('#PassError').focus();
            return false;
        }
    }

    
   
    var CompanyPhone = jQuery('input[name = CompanyPhone]').val();
    jQuery('input[name = CompanyPhone]').val(CompanyPhone);
    var ContactPhone = jQuery('input[name = ContactPhone]').val();
    jQuery('input[name = ContactPhone]').val(ContactPhone);
    var model = {};
    //model.ID = $('#hdID').val();
    if (arg == 1) {
        model.Company = jQuery('#Company').val();
        model.DBA = jQuery('#Dba').val();
        model.MailAddress = jQuery('#MailAddress').val();
        model.MailCity = jQuery('#MailCity').val();
        model.MailState = jQuery('#MailState').val();
        model.MailZip = jQuery('#MailZip').val();
        model.BillAddress = jQuery('#BillAddress').val();
        model.BillCity = jQuery('#BillCity').val();
        model.BillState = jQuery('#BillState').val();
        model.BillZip = jQuery('#BillZip').val();
        model.CompanyPhone = jQuery('#cCompanyPhone').val();
        model.Fax = jQuery('#Fax').val();
        model.FirstName = jQuery('#FirstName').val();
        model.LastName = jQuery('#LastName').val();
        model.ContactPhone = jQuery('#ContactPhone').val();
        model.ContactEmail = jQuery('#ContactEmail').val();
        model.ContactPassword = jQuery('#ContactPassword').val();
        model.hdnTerm = jQuery('#hdnTerm').val();
        model.MemberCost = jQuery('#MemberCost').val();
        model.MemberType = jQuery('#MemberType').val();
        model.BillEmail = jQuery('#BillEmail').val();
        jQuery.ajax({
            type: "POST",
            dataType: 'json',
            url: '/MemberShip/SaveInCompleteSignUp/',
            data: { 'model': model },
            async: false,
            success: function (response) {
                if (response.success) {
                    console.log(response);
                    jQuery("#ToDelete").val(response.data);
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
}

function ValidateConfirmPass() {
    var passData = jQuery('input[name = hdnPass]').val();
    var ConfirmPassData = jQuery('input[name = ContactPasswordCofirmation]').val();
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
jQuery(document).ready(function () {
    jQuery('#Paid').val(jQuery('#RadioExValue').val());

    jQuery('#addcost').val('');
    var tab2cost = jQuery('#hdnPayment').val();
    var tab2cost2 = formatter.format(tab2cost)
    jQuery('#addcost').text(tab2cost2);
    var pkgs = jQuery('#hdnPayment').val();
    var additionalcost = jQuery('#Additionalcost').val();
    var TotalAmt = parseInt(pkgs) + parseInt(additionalcost);
    var ModTotalAmt = formatter.format(TotalAmt)
    jQuery('#TotalAmmount').text(ModTotalAmt);
    jQuery('#hdnPayment').val(TotalAmt);
    //jQuery('#MemberCost').val(TotalAmt);
    jQuery('#chgPackage').prev('span').html('');
    if (jQuery('input[name = CheckRadio]').val() != 'pass') {
        jQuery('#chgPackage').prev('span').html('Select Regional Package and Payment Option Here');
        jQuery('#chgPackage').focus();
        return false;
    }

    jQuery('#NavMember').attr('disabled', 'disabled').attr('onclick', '');
    var isFromFree = jQuery('input[name = isFromFree]').val()
    if (isFromFree == "True") {
        jQuery('input[name = CheckRadio]').val('pass');
        jQuery('input[name = MemberCost]').val('0');
        jQuery('input[name = CheckRadioValue]').val('Free');
    }
    jQuery(".CPBox").click(function () {
        jQuery(".CPBox").toggleClass("isActive");
        jQuery(".paymentPhone").toggleClass("dis-pay");
        jQuery(".paymentInfo").toggleClass("dis-pay");
    });
});
jQuery('#ContactPassword').on('change', function () {
    jQuery('#hdnPass').val(jQuery('#ContactPassword').val());
});
jQuery('#ContactPasswordCofirmation').on('change', function () {
    jQuery('#hdnPassConfirm').val(jQuery('#ContactPasswordCofirmation').val());
});
jQuery('#Certification').on('change', function () {
    var strManipulated = jQuery('#MinorityStatus').val();
    jQuery('#MinorityStatus').val(strManipulated + ' # ' + jQuery('#Certification').val());
    console.log(jQuery('#MinorityStatus').val());
});

function GoToStep4() {
    jQuery('#MemberCost').val(jQuery('#hdnPayment').val());
}
function AdditionCost() {
    jQuery('#addcost').text('');
    var selectElement = document.getElementById("AdditionalCost");
    var selectedValue = selectElement.value;

    var pkg = jQuery('#MemberCost').val();
    var addcost = parseInt(selectedValue) + parseInt(pkg);
    var modAddCost = formatter.format(addcost);
    jQuery('#addcost').text(modAddCost);
    jQuery('#Additionalcost').val(selectedValue);

}

function AcceptTnC() {
    if (jQuery('input[name = CheckRadioEx]').val() != 'Term') {
        alert('You need to agree to terms and condition');
        return false;
    }
    AdditionCost();
    return true;

}

function changeMinority(id) {
    //alert(id);
    var strManipulated = '';
    var strMinorityStatus = jQuery('#MinorityStatus').val();
    var chkValue = '';
    if (jQuery('#' + id).is(':checked')) {
        chkValue = jQuery('#' + id).val();
        //console.log(chkValue);
    }
    var isExists = 'N';
    if (strMinorityStatus != '') {
        var myArray = strMinorityStatus.split(",");
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
    jQuery('#MinorityStatus').val(strManipulated);
    console.log(jQuery('#MinorityStatus').val());
}

jQuery(document).ready(function () {
    var nextStep = parseFloat(jQuery('#hdnnextStep').val());
    var prevStep = parseFloat(jQuery('#hdnpreStep').val());
    var currStep = parseFloat(jQuery('#hdncurrentStep').val());
    var data = jQuery('input[name = example]:checked').val();
    var dataPacific = jQuery('input[name = cardPacific]:checked').val();
    var dataOreg = jQuery('input[name = cardOreg]:checked').val();
    var dataWash = jQuery('input[name = cardWash]:checked').val();
    var dataFree = jQuery('input[name = cardFree]:checked').val();
    jQuery('input[id = hdnTerm]').val(data);
    chnageStep(prevStep, currStep);
});
function changeTab()
{
    var nextStep = parseFloat(jQuery('#hdnnextStep').val());
    var prevStep = parseFloat(jQuery('#hdnpreStep').val());
    var currStep = parseFloat(jQuery('#hdncurrentStep').val());
    var data = jQuery('input[name = example]:checked').val();
    var dataPacific = jQuery('input[name = cardPacific]:checked').val();
    var dataOreg = jQuery('input[name = cardOreg]:checked').val();
    var dataWash = jQuery('input[name = cardWash]:checked').val();
    var dataFree = jQuery('input[name = cardFree]:checked').val();
    jQuery('input[id = hdnTerm]').val(data);
    chnageStep(nextStep, prevStep, 0);
}
function chnageStep(preStep, nextStep) {
    if (nextStep == 1) {
        var rdVal = jQuery('input[name = CheckRadioValue]').val();
        var rdExVal = jQuery('input[name = RadioExValue]').val();
        //    jQuery('input[type="radio"][value=' + rdVal + ']').attr('checked', true);
        if (rdVal) {
            jQuery('.card input[type = radio]').each(function (i) {
                if (this.value == rdVal) {
                    jQuery(this).attr('checked', true);
                } else {
                    jQuery(this).attr('checked', false);
                }
            });
            //jQuery("#customRadio4").attr('checked', true);
        }
        if (rdExVal) {
            jQuery('input[name = example]').each(function (i) {
                if (this.value == rdExVal) {
                    jQuery(this).attr('checked', true);
                } else {
                    jQuery(this).attr('checked', false);
                }
            });
            //jQuery("#customRadio4").attr('checked', true);
        }
        jQuery('#hdnTabPer').val(preStep);
        jQuery('input[name = hdnPass]').val('');
        jQuery('input[name = hdnPassConfirm]').val('');
        jQuery("ul.cusTab").children('li').siblings().removeClass('active');
        jQuery("ul.cusTab").children('li').siblings().children('a').removeClass('active');
        jQuery("div.tab-content").children('div').siblings().removeClass('in').removeClass('active');
        jQuery("div#contact").addClass('in').addClass('active');
        jQuery("li#MemDet").addClass('active');
        jQuery("a#MemDet").addClass('active').attr("data-toggle", "tab");
    }
    else if (nextStep == 2) {
        var TabPer = jQuery('#hdnTabPer').val();
        if (TabPer == 'MemDet')
        {
         return AcceptPay(2);
        }
        jQuery('#hdnTabPer').val(preStep);
        var rdExVal = jQuery('input[name = CheckRadioExValue]').val();
        if (rdExVal == 'ok') {
            jQuery('input[name = secCustom]').attr('checked', true);
            //jQuery("#customRadio4").attr('checked', true);
        }
        var rdVal = jQuery('input[name = CheckRadioValue]').value
        if (rdVal) {
            jQuery('.card input[type = radio]').each(function (i) {
                if (this.value == rdVal) {
                    jQuery(this).attr('checked', true);
                } else {
                    jQuery(this).attr('checked', false);
                }
            });
        }
        var rdValEx = jQuery('input[name = RadioExValue]').val();
        jQuery('input[name = example]').each(function (i) {
            if (this.value == rdValEx) {
                jQuery(this).attr('checked', true);
            } else {
                jQuery(this).attr('checked', false);
            }
        });

        if (jQuery('input[name = CheckRadio]').val() == 'pass') {
            jQuery("ul.cusTab").children('li').siblings().removeClass('active');
            jQuery("ul.cusTab").children('li').siblings().children('a').removeClass('active');
            jQuery("div.tab-content").children('div').siblings().removeClass('in').removeClass('active');
            jQuery("div#preview").addClass('in').addClass('active');
            jQuery("li#MemRev").addClass('active');
            jQuery("a#MemRev").addClass('active').attr("data-toggle", "tab");
        }
        else {
            alert('You need to select a valid plan');
        }
        
    }
    else if (nextStep == 3) {
        var status = true;
        var status2 = true;
        var TabPer = jQuery('#hdnTabPer').val();
        if (TabPer == 'MemDet') {
            var status = AcceptPay(2);
        }
        if (TabPer == 'MemRev') {
            return AcceptTnC();
        }
        jQuery('#hdnTabPer').val(preStep);
        if (status) {
            if (jQuery('input[name = CheckRadioEx]').val() == 'Term') {
                jQuery("ul.cusTab").children('li').siblings().removeClass('active');
                jQuery("ul.cusTab").children('li').siblings().children('a').removeClass('active');
                jQuery("div.tab-content").children('div').siblings().removeClass('in').removeClass('active');
                jQuery("div#payment").addClass('in').addClass('active');
                jQuery("li#MemPay").addClass('active');
                jQuery("a#MemPay").addClass('active').attr("data-toggle", "tab");
            }
            else {
                alert('You need to agree to terms and condition');
            }
        }
        
    }
    else if (nextStep == 4) {
        jQuery("ul.cusTab").children('li').siblings().removeClass('active');
        jQuery("ul.cusTab").children('li').siblings().children('a').removeClass('active');
        jQuery("div.tab-content").children('div').siblings().removeClass('in').removeClass('active');
        jQuery("div#finish").addClass('in').addClass('active');
        jQuery("li#MemFinish").addClass('active');
        jQuery("a#MemFinish").addClass('active').attr("data-toggle", "tab");
    }
}

jQuery(document).ready(function () {
    jQuery('input[name = secCustom]').change(function () {
        jQuery('input[name = CheckRadioEx]').val('Term');
        if (jQuery('input[name = secCustom]').is(':checked')) {
            jQuery('input[name = CheckRadioExValue]').val('ok');
        }
        else {
            jQuery('input[name = CheckRadioExValue]').val('');
        }
    });
});

jQuery('input[name = chkSameEmail]').change(function () {
    var ContactEmail = jQuery('input[name = ContactEmail]').val();

    if (jQuery('input[name = chkSameEmail]').is(':checked')) {
        jQuery('input[name = BillEmail]').val(ContactEmail);
        jQuery('input[name = BillEmail]').attr('readonly', 'readonly');
    }
    else {
        jQuery('input[name = BillEmail]').val('');
        jQuery('input[name = BillEmail]').removeAttr('readonly', 'true');
    }
})

jQuery('input[name = chkSame]').change(function () {

    var mailAdd = jQuery('input[name = MailAddress]').val();

    var mailcity = jQuery('input[name = MailCity]').val();
    var mailState = jQuery('select[name=MailState] option').filter(':selected').val();
    var mailzip = jQuery('input[name = MailZip]').val();

    if (jQuery('input[name = chkSame]').is(':checked')) {
        jQuery('input[name = BillAddress]').val(mailAdd);
        jQuery('input[name = BillCity]').val(mailcity);
        jQuery('input[name = BillZip]').val(mailzip);
        jQuery('input[name = BillAddress]').attr('readonly', 'readonly');
        jQuery('input[name = BillCity]').attr('readonly', 'readonly');
        jQuery('select[name=BillState]').attr('readonly', 'readonly');
        jQuery('input[name = BillZip]').attr('readonly', 'readonly');
        jQuery('select[name=BillState]').val(mailState);
        jQuery('input[name=BillState]').val(mailState);

    }
    else {
        jQuery('input[name = CheckRadioExValue]').val('');
        jQuery('input[name = BillAddress]').removeAttr('readonly', 'true');
        jQuery('input[name = BillCity]').removeAttr('readonly', 'true');
        jQuery('select[name=BillState]').removeAttr('readonly', 'true');
        jQuery('input[name = BillZip]').removeAttr('readonly', 'true');
        jQuery('input[name = BillAddress]').val('');
        jQuery('input[name = BillCity]').val('');
        jQuery('select[name=BillState]').val('');
        jQuery('input[name = BillZip]').val('');

    }
});

jQuery('#onchangevalue').on('input', function () {
    var mailAddress = ("" + jQuery("#MailAddress").val());
    var mailCity = ("" + jQuery("#MailCity").val());
    var mailState = jQuery('select[name=MailState] option').filter(':selected').val();
    var mailZip = ("" + jQuery("#MailZip").val());
    if (jQuery('input[name = chkSame]').is(':checked')) {
        jQuery('input[name = BillAddress]').val(mailAddress);
        jQuery('input[name = BillCity]').val(mailCity);
        jQuery('select[name=BillState]').val(mailState);
        //jQuery('input[name = BillZip]').val(mailZip);
    }

});

if (jQuery("#MSPChk").val() == 'N')
    ShowPayment();

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
//Lowering city
const upperCaseFirstLetter = string =>
    `${string.slice(0, 1).toUpperCase()}${string.slice(1)}`;

const lowerCaseAllWordsExceptFirstLetters = string =>
    string.replaceAll(/\S*/g, word =>
        `${word.slice(0, 1)}${word.slice(1).toLowerCase()}`
    );
jQuery(document).ready(function () {
    //var zipCode = $(this).val();
    jQuery('#BillZip').keyup(function () {
        var zip = jQuery(this).val();
        jQuery('#BillCity').val('');
        jQuery('#BillState').val('');
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

jQuery(".toggle-password").click(function () {
    //alert('sdfsdf')
    jQuery(this).toggleClass("fa-eye fa-eye-slash");
    var input = jQuery(jQuery(this).attr("toggle"));
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});

const radioButtons = document.querySelectorAll('input[type="radio"]');
const PacificDiv = document.getElementById('Popfaq7');
const OrgPremiumDiv = document.getElementById('Popfaq1');
const OrgPlusDiv = document.getElementById('Popfaq2');
const OrgOnlyDiv = document.getElementById('Popfaq3');
const WashPremiumDiv = document.getElementById('PopfaqWash');
const WashPlusDiv = document.getElementById('Popfaq5');
const WashOnlyDiv = document.getElementById('Popfaq6');
const jobposting = document.getElementById('Popfaq8');

for (let i = 0; i < radioButtons.length; i++) {
    radioButtons[i].addEventListener('change', function () {
        if (this.checked && this.id == 'customRadio4') {
            PacificDiv.style.display = 'block';
            OrgPremiumDiv.style.display = 'block';
        }
        if (this.checked && this.id == 'customRadio5') {
            OrgPremiumDiv.style.display = 'block';
            WashPremiumDiv.style.display = 'block';
        }
       
        if (this.checked && this.id == 'customRadio6') {
            OrgPlusDiv.style.display = 'block';
            OrgPremiumDiv.style.display = 'none';
            WashPremiumDiv.style.display = 'block';
        }
        else {
            OrgPlusDiv.style.display = 'none';
            OrgPremiumDiv.style.display = 'block';
            WashPremiumDiv.style.display = 'block';
        }
        if (this.checked && this.id == 'customRadio7') {
            OrgOnlyDiv.style.display = 'block';
            OrgPremiumDiv.style.display = 'none';
            WashPremiumDiv.style.display = 'block';
        }
        else {
            OrgOnlyDiv.style.display = 'none';
        }
        if (this.checked && this.id == 'customRadio8') {
            WashPremiumDiv.style.display = 'block';
        }
        if (this.checked && this.id == 'customRadio9') {
            WashPlusDiv.style.display = 'block';
            WashPremiumDiv.style.display = 'none';
        }
        else {
            WashPlusDiv.style.display = 'none';
        }
        if (this.checked && this.id == 'customRadio10') {
            WashOnlyDiv.style.display = 'block';
            WashPremiumDiv.style.display = 'none';
        }
        else {
            WashOnlyDiv.style.display = 'none';
        }
        if (this.checked && this.id == 'customRadio11') {
            jobposting.style.display = 'block';
        }
    });
}
