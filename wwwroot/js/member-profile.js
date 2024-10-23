var jq = $.noConflict();

const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
});

////////////////////
//function setupCheckbox(checkboxId, divId) {
//    var checkbox = document.getElementById(checkboxId);
//    var myDiv = document.getElementById(divId);

//    myDiv.style.setProperty('display', 'none', 'important');

//    function updateDivDisplay() {
//        var displayValue = checkbox.checked ? 'block' : 'none';
//        myDiv.style.setProperty('display', displayValue, 'important');
//    }
//    // Initial setup
//    updateDivDisplay();

//    // Add a click event listener to the checkbox
//    checkbox.addEventListener('click', updateDivDisplay);
//}
jq('#udpatebtn').click(function (e) {
    e.preventDefault();

    jq('#loader-overlay').show();
    jq('#formprofile').submit();
});

// Example usage
//setupCheckbox('chkDBA', 'compDba');
//setupCheckbox('chkCompany', 'companyName');
//setupCheckbox('chkPC', 'compContactname');
//setupCheckbox('chkPhone', 'compPhone');
//setupCheckbox('chkLogo', 'compLogo');
//setupCheckbox('chkMailAddress', 'consMailAdd');
//setupCheckbox('chkBillAddress', 'consBillingAdd');
//setupCheckbox('chkEmail', 'compEmail');
//setupCheckbox('chkPB', 'compDiv');
//setupCheckbox('chkLicense', 'compLicense');
//setupCheckbox('chkSpeciality', 'compDispline');

//////////////////////
function EditNote(ctrl, id, e) {
    console.log(id);
    var refCtrl = jQuery(ctrl).parent('div').siblings();
    console.log(refCtrl.children('span.text-date-class').text());
    console.log(refCtrl.children('span.text-note-class').text());
    jq("#hdnNoteId").val(id);
    jq("#spanNotedate").text(refCtrl.children('span.text-date-class').text())
    jq("#edittextNote").val(refCtrl.children('span.text-note-class').text())
    jq(".noteModal").modal('show');
    e.preventDefault();
}
function DeleteNote(memid, id) {
    if (confirm("Are you sure you want to delete note?")) {
        var model = {};
        model.Id = id;
        model.MemId = memid;
        jq.ajax({
            type: "POST",
            dataType: 'json',
            //contentType: "application/json; charset=utf-8",
            url: '/Member/DeleteNote/',
            data: { 'model': model },
            async: false,
            success: function (response) {
                alert(response.statusMessage);
                window.location.reload();
            },
            error: function (response) {
                location.reload();
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    }
    return false;
}
function UpdateNote() {
    var model = {};
    model.Id = jQuery('#hdnNoteId').val();
    model.Note = jQuery('#edittextNote').val();
    model.MemId = jQuery('#hdnId').val();
    model.CompType = jQuery('#CompType').val();
    if (jQuery('#edittextNote').val() == '') {
        jQuery('#edittextNote').next('span').html('This field can not be blank.');
        return false;
    }
    else {
        jQuery('#edittextNote').next('span').html('');
    }
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Member/UpdateNote/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            jq(".noteModal").modal('hide');
            location.reload();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });

    //    jq("#noteModal").modal('hide');
}
function AddNote() {
    var model = {};
    model.Id = jQuery('#hdnNoteId').val();
    model.Note = jQuery('#txtAddNote').val();
    model.MemId = jQuery('#hdnId').val();
    model.CompType = jQuery('#CompType').val();
    if (jQuery('#txtAddNote').val() == '') {
        jQuery('#txtAddNote').next('span').html('You can not add empty note.');
        return false;
    }
    else {
        jQuery('#txtAddNote').next('span').html('');
    }
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Member/AddNote/',
        data: { 'model': model },
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
function SaveDirectory() {
    var chkDirId = document.getElementById("chkDirId");
    var MemId = jQuery('#hdnId').val();
    var chkLogo = document.getElementById("chkLogo");
    var chkCompany = document.getElementById("chkCompany");
    var chkDBA = document.getElementById("chkDBA");
    var chkMailAddress = document.getElementById("chkMailAddress");
    var chkBillAddress = document.getElementById("chkBillAddress");
    var chkPhone = document.getElementById("chkPhone");
    var chkPC = document.getElementById("chkPC");
    var chkEmail = document.getElementById("chkEmail");
    var chkPB = document.getElementById("chkPB");
    var chkLicense = document.getElementById("chkLicense");
    var chkSpeciality = document.getElementById("chkSpeciality");
    var model = {};
    model.DirId = chkDirId.value;
    model.MemId = MemId;
    if (chkLogo != null)
        model.Logo = chkLogo.checked;
    model.Company = chkCompany.checked;
    model.MailAddr = chkMailAddress.checked;
    model.BillAddr = chkBillAddress.checked;
    model.DBA = chkDBA.checked;
    model.PrimaryContact = chkPC.checked;
    model.Phone = chkPhone.checked;
    model.Business = chkPB.checked;
    model.Speciality = chkSpeciality.checked;
    model.Email = chkEmail.checked;
    model.License = chkLicense.checked;
    jq('#loader-overlay').css('display', 'block');
    console.log('Loader shown');
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Member/SaveDirectory/',
        data: { 'model': model },
        async: true, // Using async: true as it's best practice
        beforeSend: function () {
            console.log('AJAX request is about to be sent');
        },
        success: function (response) {
            jq('#UserForm').submit();
        },
        error: function (response) {
            alert(response.statusMessage);
            console.log(response.responseText);
            $('#loader-overlay').css('display', 'none');
            console.log('Loader hidden');
        },
        failure: function (response) {
            console.log(response.responseText);
            $('#loader-overlay').css('display', 'none');
            console.log('Loader hidden');
        },
        complete: function (response) {
            $('#loader-overlay').css('display', 'none');
            console.log('Loader hidden');
        },
    });
}
jq(document).ready(function () {
    TempDataValue = jQuery('#tmpHiddenMsg').val();
    if (TempDataValue != '') {
    }
});
function AddLicense() {
    jq("#frmPrintForm").trigger('reset');
    jq('#frmPrintForm').find("input[type=text], textarea").val('');
    jq("#frmPrintForm").find("span.loginError").html('');
    jq('.li-modal').modal('show');
}
function SaveLic() {
    var idLic = jQuery('#idLic').val();
    if (idLic == undefined || idLic == '' || idLic == null) {
        alert("Please enter license number");
        return false;
    }
    var LicState = jQuery('#LicState').val();
    if (LicState == undefined || LicState == '' || LicState == 0) {
        alert("Please select state");
        return false;
    }

    var lstState = [];
    jq("#LicState :selected").each(function () {
        lstState.push(this.value);
    });
    var LicNum = jq('#idLic').val();
    var LicDesc = jq('#LicDesc').val();
    var MemId = jQuery('#hdnId').val();
    jq('#loader-overlay').css('display', 'block');
    console.log('Loader shown');
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Member/SaveLic/',
        data: { 'lstState': lstState, 'LicNum': LicNum, 'LicDesc': LicDesc, 'MemId': MemId },
        async: true,
        success: function (response) {
            alert(response.statusMessage);
            location.reload();
        },
        error: function (response) {
            alert(response.statusMessage);
            console.log(response.responseText);
            $('#loader-overlay').css('display', 'none');
            console.log('Loader hidden');
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
};
jQuery(document).ready(function () {
    ShowPayment();
    SelectReason();

    jQuery("#editform").click(function () {
        var res = validateform();
        if (res == false)
            return false;
    });
});
function validateform() {
    debugger;
    var isValide = true;
    var address = jQuery('#BillAddress').val();
    if (address == undefined || address == '' || address == null) {
        jQuery('#BillAddress').addClass('error')
        isValide = false;
    }
    //var city = jQuery('#BillCity').val();
    //if (city == undefined || city == '' || city == null) {
    //    jQuery('#BillCity').addClass('error')
    //    isValide = false;
    //}
    var State = jQuery('select[id=BillState] option').filter(':selected').text();
    if (State == '--Select State--') {
        jQuery('#BillState').addClass('error')
        isValide = false;
    }
    var MAddress = jQuery('#MailAddress').val();
    if (MAddress == undefined || MAddress == '' || MAddress == null) {
        jQuery('#MailAddress').addClass('error')
        isValide = false;
    }
    //var MCity = jQuery('#MailCity').val();
    //if (MCity == undefined || MCity == '' || MCity == null) {
    //    jQuery('#MailCity').addClass('error')
    //    isValide = false;
    //}
    //var MZip = jQuery('#MailZip').val();
    //if (MZip == undefined || MZip == '' || MZip == null) {
    //    jQuery('#MailZip').addClass('error')
    //    isValide = false;
    //}
    var MState = jQuery('select[id=MailState] option').filter(':selected').text();
    if (MState == '--Select State--') {
        jQuery('#MailState').addClass('error')
        isValide = false;
    }
    //var phone = jQuery('#Fax').val();
    //if (phone == undefined || phone == '' || phone == null) {
    //    jQuery('#Fax').addClass('error')
    //    isValide = false;
    //}
    var BillingEmail = jQuery('#Email').val();
    if (BillingEmail == undefined || BillingEmail == '' || BillingEmail == null) {
        jQuery('#Email').addClass('error')
        isValide = false;
    }
    //var ContactName = jQuery('#ContactName').val();
    //if (ContactName == undefined || ContactName == '' || ContactName == null) {
    //    jQuery('#ContactName').addClass('error')
    //    isValide = false;
    //}
    var specialties = jQuery('#Discipline').val();
    if (specialties == undefined || specialties == '' || specialties == null) {
        jQuery('#Discipline').addClass('error')
        isValide = false;
    }
    var checklistvalide = true;
    jQuery('div.ContactList').find('input[type=text]').each(function (index, item) {
        console.log("-------");
        console.log(index);
        //                console.log(item);
        console.log(jQuery(this).val());
        var specialties = jQuery(this).val();
        if (specialties == undefined || specialties == '' || specialties == null) {
            jQuery(this).addClass('error')
            checklistvalide = false;
        }
    });
    isValide = checklistvalide;
    return isValide;
};
//add location
function OpenPopup() {
    jq("#frmPrintForm2").trigger('reset');
    jq('#frmPrintForm2').find("input[type=text], textarea").val('');
    jq("#frmPrintForm2").find("span.loginError").html('');
    jq('.lo-modal').modal('show');
};
function AddLocation() {
    var LocationAddress = jQuery('#LocationAddress').val();
    if (LocationAddress == undefined || LocationAddress == '' || LocationAddress == null) {
        alert("Please enter location address");
        return false;
    }
    var LocationCity = jQuery('#LocationCity').val();
    if (LocationCity == undefined || LocationCity == '' || LocationCity == null) {
        alert("Please enter location city");
        return false;
    }
    var LocationState = jQuery('#LocationState').val();
    if (LocationState == undefined || LocationState == '' || LocationState == 0) {
        alert("Please select state");
        return false;
    }
    var LocationCounty = jQuery('#LocationCounty').val();
    if (LocationCounty == undefined || LocationCounty == '' || LocationCounty == null) {
        alert("Please enter location county");
        return false;
    }
    var LocationPhone = jQuery('#LocationPhone').val();
    if (LocationPhone == undefined || LocationPhone == '' || LocationPhone == null) {
        alert("Please enter location phone");
        return false;
    }

    var model = {};
    var LocAddr = jq('#LocationAddress').val();
    var LocCity = jq('#LocationCity').val();
    var LocState = jQuery('select[id=LocationState] option').filter(':selected').text();
    var LocCounty = jq('#LocationCounty').val();
    var LocZip = jQuery('#LocationZip').val();
    var LocPhone = jQuery('#LocationPhone').val();
    var MemId = jQuery('#hdnId').val();
    model.LocAddr = LocAddr;
    model.LocCity = LocCity;
    model.LocState = LocState;
    model.LocCounty = LocCounty;
    model.LocZip = LocZip;
    model.LocPhone = LocPhone;
    model.MemId = MemId;
    console.log(model);
    jq('#loader-overlay').css('display', 'block');
    console.log('Loader shown');
    jq.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/Member/AddLocation/',
        data: { 'model': model },
        async: true,
        success: function (response) {
            alert(response.statusMessage);
            location.reload();
        },
        error: function (response) {
            alert(response.statusMessage);
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        },
        complete: function (response) {
            $('#loader-overlay').css('display', 'none');
            console.log('Loader hidden');
        }
    });
};
function AssignState(id) {
    var changeId = id.replace('LocStateCode', 'LocState');
    var State = jQuery('select[id=' + id + '] option').filter(':selected').text();
    jQuery('#' + changeId + '').val(State);
};

function UpdateDirectory() {
    var model = {};
    model.ID = jq('#hdnId').val();
    model.CheckDirectory = jq('#CheckDirectory').is(':checked') ? true : false;

    // Show the loader before making the AJAX request
    jq('#loader-overlay').css('display', 'block');
    console.log('Loader shown');

    jq.ajax({
        type: "POST",
        dataType: 'json',
        url: '/Member/UpdateDirectory/',
        data: { 'model': model },
        async: true, // Using async: true as it's best practice
        beforeSend: function () {
            console.log('AJAX request is about to be sent');
        },
        success: function (response) {
            console.log('AJAX request succeeded');
            if (response.success) {
                console.log(response);
                alert(response.statusMessage);
            } else {
                alert(response.statusMessage);
            }
        },
        error: function (response) {
            console.log('AJAX request failed');
            alert(response.responseText);
        },
        complete: function () {
            console.log('AJAX request completed');
            // Hide the loader after the AJAX call is complete
            jq('#loader-overlay').css('display', 'none');
            console.log('Loader hidden');
        }
    });
}
function submitData() {
    jq('#loader-overlay').css('display', 'block');
    console.log('Loader shown');
}
function MemberUserAdmin(ID, Email) {
    var model = {};
    model.ID = ID;
    model.Email = Email;
    jq.ajax({
        type: "POST",
        dataType: 'json',
        url: '/Member/AdminUserContact/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                location.reload();
            }
            else {
                location.reload();
            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
};
function MemberUserDailyReport(ID, Email, ctrl) {
    var model = {};
    model.ID = ID;
    model.Email = Email;
    model.Daily = jq(ctrl).is(':checked') ? true : false;
    jq.ajax({
        type: "POST",
        dataType: 'json',
        url: '/Member/MemberUserDailyReport/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                location.reload();
            }
            else {
                location.reload();
            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
};

function Discipline() {
    jq(".EditDiscClass").css("display", 'block');
    jq(".DiscClass").css("display", 'none');
}
function CancelDiscipline() {
    jq(".DiscClass").removeAttr("style");
    jq(".EditDiscClass").css("display", "none");
}
function ActiveEdit() {
    jq(".LocClass").css("display", "none");
    jq(".LocEditClass").css("display", "block");
}
function CancelActiveEdit() {
    jq(".LocClass").removeAttr("style");
    jq(".LocEditClass").css("display", "none");
}
function OpenEditPopup(ConId, Name, Email, LocId, Phone) {
    jq("#frmEdit").trigger('reset');
    jq('#frmEdit').find("input[type=text], textarea").val('');
    jq("#frmEdit").find("span.loginError").html('');
    jq("#EdhdnCid").val(ConId);
    jq("#EditName").val(Name);
    jq("#EditConEmail").val(Email);
    jq("#EditConEmail").prop("readonly", true);
    //jq("#EdithdnLocUser").val(LocId);
    //jq('select[id=EditLocUser]').val(LocId);
    jq('select[id=EditLocUser] option[value="' + LocId + '"]').attr("selected", "selected");
    jq("#hdnEmail").val(Email);
    jq("#EditPhone").val(Phone);
    jq('.ue-modal').modal('show');
};
function UpdateData() {
    var name = jq('#EditName').val();
    jq('#EditName').next('span').html('');
    if (name == undefined || name == '' || name == null) {
        jq('#EditName').next('span').html('Please enter name.');
        return false;
    }
    var email = jq('#EditConEmail').val();
    jq('#EditConEmail').next('span').html('');
    if (email == undefined || email == '' || email == null) {
        jq('#EditConEmail').next('span').html('Please enter email.');
        return false;
    }
    var phone = jq('#EditPhone').val();
    jq('#EditPhone').next('span').html('');
    if (phone == undefined || phone == '' || phone == null) {
        jq('#EditPhone').next('span').html('Please enter phone number.');
        return false;
    }
    var model = {};
    model.ConId = jQuery('#EdhdnCid').val();
    model.ContactName = jQuery('#EditName').val();
    model.ContactEmail = jQuery('#EditConEmail').val();
    model.ContactPhone = jQuery('#EditPhone').val();
    model.LocId = jQuery('select[id=EditLocUser] option').filter(':selected').val();
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        // contentType: "application/json; charset=utf-8",
        url: '/Member/EditUser/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            location.reload();
            if (response.success) {
                console.log(response);
                alert(response.statusMessage);
            }
            else {
                alert(response.statusMessage);
            }
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
};
function EditvalidateUser() {
    var isUniqueEmail = checkUniqueEmail();
    if (isUniqueEmail == false)
        return false;

    var isValideUser = true;
    var FName = jQuery('#EditName').val();
    if (FName == undefined || FName == '' || FName == null) {
        jQuery("#EditName").css('border-color', 'red');
        isValideUser = false;
    }
    var phone = jQuery('#EditPhone').val();
    if (phone == undefined || phone == '' || phone == null) {
        jQuery("#EditPhone").css('border-color', 'red');
        isValideUser = false;
    }

    return isValideUser;
}
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

var phoneInput = document.getElementById('companyphone');
var EditPhone = document.getElementById('EditPhone');
var phone = document.getElementById('Phone');
var locationPhone = document.getElementById('LocationPhone');

phoneInput.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
EditPhone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
phone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
locationPhone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});

jQuery('#ConEmail').on('change', function () {
    CheckUserEmail();
});
function CheckUserEmail() {
    var success = true;
    jQuery('#ConEmail').next('span').html('');
    var uniqueName = jQuery('#ConEmail').val();
    var data = { "uniqueName": uniqueName };
    jQuery.ajax({
        type: "POST",
        url: '/Home/UniqueEmail/',
        data: { "uniqueName": uniqueName },
        dataType: "json",
        async: false,
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
        }
    });
    return success;
}
jQuery('#EditConEmail').on('change', function () {
    var Email1 = jQuery('#EditConEmail').val();
    var Email2 = jQuery('#hdnEmail').val();
    if (Email1 != Email2) {
        checkUnique();
    }
    else {
        jQuery('#EditConEmail').next('span').html('');
    }
});
function checkUnique() {
    var success = true;
    jQuery('#EditConEmail').next('span').html('');
    var uniqueName = jQuery('#EditConEmail').val();
    var data = { "uniqueName": uniqueName };
    jQuery.ajax({
        type: "POST",
        url: '/Home/UniqueEmail/',
        data: { "uniqueName": uniqueName },
        dataType: "json",
        async: false,
        success: function (response) {
            if (response.success) {
                success = false;
                jQuery('#EditConEmail').next('span').html(response.statusMessage);
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
function OpenUserModal() {
    jQuery("#UserForm").trigger('reset');
    jQuery('#UserForm').find("input[type=text], textarea").val('');
    jQuery('span.loginError').html('');
    jQuery('.u-modal').modal('show');
}
jQuery('.user-modal').on('click', function () {
    jQuery('.u-modal').modal('hide');
})

function UploadFile() {
    var pdf = jQuery('#fileUpload1').val();
    jQuery('#fileUpload1').next('span').html('');
    if (pdf == undefined || pdf == '' || pdf == null) {
        jQuery('#uploadError').html('Please choose your file');
        return false;
    }
    var formData = new FormData();
    var files = jQuery("#fileUpload1").get(0).files;
    var a = jQuery("#fileUpload1")[0].files[0];
    formData.append("pdfFile", jQuery("#fileUpload1")[0].files[0]);
    jQuery.ajax({
        type: 'POST',
        url: '/Member/UploadLogo',
        data: formData,
        processData: false,
        contentType: false
    }).done(function (response) {
        if (response.Status === "success") {
            var filename = a.name;
            var filename = "/Profile/" + filename;
            jQuery("#mailerImage").attr("src", filename);
            jQuery("#Logo").val(filename);
        }
    });
}

var model = {};
function OpenRenewalModel() {
    var PacificDiv = document.getElementById('Popfaq7');
    var OrgPremiumDiv = document.getElementById('Popfaq1');
    var OrgPlusDiv = document.getElementById('Popfaq2');
    var OrgOnlyDiv = document.getElementById('Popfaq3');
    var WashPremiumDiv = document.getElementById('PopfaqWash');
    var WashPlusDiv = document.getElementById('Popfaq5');
    var WashOnlyDiv = document.getElementById('Popfaq6');
    var jobposting = document.getElementById('Popfaq8');

    var check = jQuery("#PKGVal").val();
    if (check != "Free") {
        var parts = check.split('(');
        // Extracting the values and removing any extra spaces
        var plan = parts[0].trim();
        var ExVal = parts[1].trim().slice(0, -1); // Remove the trailing ')'
        //var ExVal = "Monthly";
    }
    if (check == "Free") {
        plan = check;
        ExVal = "Yearly";
    }

    // Select Yearly Quarterly Monthly By -ExVal- value

    if (ExVal == "Yearly") {
        jQuery("#customRadio1").prop('checked', true);
    }
    else if (ExVal == "Quarterly") {
        jQuery("#customRadio3").prop('checked', true);
    }
    else if (ExVal == "Monthly") {
        jQuery("#customRadio2").prop('checked', true);
    }
    else {
    }

    // Select package on card By -plan- data

    jQuery('input[name = example]:checked').val(ExVal);
    if (plan == 'Free') {
        jQuery("#customRadio1").prop('checked', true);
        //jQuery('input[name = cardFree]:checked');
        jQuery("#customRadio11").prop('checked', true);
        changeFree(ExVal, plan);
    }
    else if (plan == 'Pacific Northwest Premium') {
        jQuery("#customRadio4").prop('checked', true);
        PacificDiv.style.display = 'block';
        OrgPremiumDiv.style.display = 'block';
        WashPremiumDiv.style.display = 'block';
        OrgPlusDiv.style.display = 'none';
        OrgOnlyDiv.style.display = 'none';
        WashOnlyDiv.style.display = 'none';
        WashPlusDiv.style.display = 'none';
        document.getElementById('customRadio4').click();
        changePacific(ExVal, plan);
    }
    else if (plan == 'Oregon Premium') {
        jQuery("#customRadio5").prop('checked', true);
        PacificDiv.style.display = 'block';
        OrgPremiumDiv.style.display = 'block';
        WashPremiumDiv.style.display = 'block';
        OrgPlusDiv.style.display = 'none';
        OrgOnlyDiv.style.display = 'none';
        WashOnlyDiv.style.display = 'none';
        WashPlusDiv.style.display = 'none';
        document.getElementById('customRadio5').click();
        changeOreg(ExVal, plan);
    }
    else if (plan == 'Oregon Plus') {
        jQuery("#customRadio6").prop('checked', true);
        OrgPlusDiv.style.display = 'block';
        OrgPremiumDiv.style.display = 'none';
        WashPremiumDiv.style.display = 'block';
        document.getElementById('customRadio6').click();
        changeOreg(ExVal, plan);
    }
    else if (plan == 'Oregon Only') {
        jQuery("#customRadio7").prop('checked', true);
        PacificDiv.style.display = 'block';
        OrgPremiumDiv.style.display = 'none';
        WashPremiumDiv.style.display = 'block';
        OrgPlusDiv.style.display = 'none';
        OrgOnlyDiv.style.display = 'block';
        WashOnlyDiv.style.display = 'none';
        WashPlusDiv.style.display = 'none';
        document.getElementById('customRadio7').click();
        changeOreg(ExVal, plan);
    }
    else if (plan == 'Washington Premium') {
        jQuery("#customRadio8").prop('checked', true);
        PacificDiv.style.display = 'block';
        OrgPremiumDiv.style.display = 'block';
        WashPremiumDiv.style.display = 'block';
        OrgPlusDiv.style.display = 'none';
        OrgOnlyDiv.style.display = 'none';
        WashOnlyDiv.style.display = 'none';
        WashPlusDiv.style.display = 'none';
        document.getElementById('customRadio8').click();
        changeWash(ExVal, plan);
    }
    else if (plan == 'Washington Plus') {
        jQuery("#customRadio9").prop('checked', true);
        PacificDiv.style.display = 'block';
        OrgPremiumDiv.style.display = 'block';
        WashPremiumDiv.style.display = 'none';
        OrgPlusDiv.style.display = 'none';
        OrgOnlyDiv.style.display = 'none';
        WashOnlyDiv.style.display = 'none';
        WashPlusDiv.style.display = 'block';
        document.getElementById('customRadio9').click();
        changeWash(ExVal, plan);
    }
    else if (plan == 'Washington Only') {
        jQuery("#customRadio10").prop('checked', true);
        PacificDiv.style.display = 'block';
        OrgPremiumDiv.style.display = 'block';
        WashPremiumDiv.style.display = 'none';
        OrgPlusDiv.style.display = 'none';
        OrgOnlyDiv.style.display = 'none';
        WashOnlyDiv.style.display = 'block';
        WashPlusDiv.style.display = 'none';
        document.getElementById('customRadio10').click();
        changeWash(ExVal, plan);
    }

    jQuery(".re-modal").modal('show');
    SelectReason();
}
function CloseRenewalModal() {
    jQuery(".re-modal").modal('hide');
    //window.location.reload();
}

function showmodal() {
    jQuery(".re-modal").modal('show');
}
function ShowPayment() {
    GetPacificCardDetails();
    jQuery("#membershipModal").modal('show');
    jQuery.ajax({
        type: 'GET',
        url: "/Member/FindPriceDetail",
        success: function (data) {
            model = data;
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcPremium').text(memshipInfo[0].YearlyPrice);
            jQuery('#hdnDiscountId').val(memshipInfo[0].DiscountId);
            jQuery('#AddlPrice').val(memshipInfo[0].AddYearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#RadioExValue').val('Yearly');
            jQuery('input[id = MemberType]').val('7');
        }
    });
    // e.preventDefault();
}
function GetPacificCardDetails() {
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Member/GetPacificCardDetails',
        data: {},
        async: false,
        success: function (response) {
            var Pacific = response.data[0];
            var OregonPre = response.data[1];
            var OregonPlus = response.data[2];
            var OregonOnly = response.data[3];
            var WashingtonPre = response.data[4];
            var WashingtonPlu = response.data[5];
            var WashingtonOnly = response.data[6];
            var Free = response.data[7];
            /*    Pacific Northwest Premium   */
            jQuery('#PacificPremium').text(Pacific.PackageName);
            jQuery('#PacificPremiu').text(Pacific.PackageName);
            jQuery('#PrUserText').text(Pacific.UserText);
            jQuery('#PrRegionHead').text(Pacific.RegionHead);
            jQuery('#RegionText').text(Pacific.RegionText);
            /*          Oregon Premium          */
            jQuery('#OrPrePackage').text(OregonPre.PackageName);
            jQuery('#OrPreUser').text(OregonPre.UserText);
            jQuery('#OrPreRegion').text(OregonPre.RegionHead);
            jQuery('#OrPreText').text(OregonPre.RegionText);
            /*       Oregon Plus     */
            jQuery('#OrPluPackage').text(OregonPlus.PackageName);
            jQuery('#OrPluUser').text(OregonPlus.UserText);
            jQuery('#OrPluRegion').text(OregonPlus.RegionHead);
            jQuery('#OrPluText').text(OregonPlus.RegionText);
            /*        Oregon Only    */
            jQuery('#OrOnlPackage').text(OregonOnly.PackageName);
            jQuery('#OrOnlUser').text(OregonOnly.UserText);
            jQuery('#OrOnlRegion').text(OregonOnly.RegionHead);
            jQuery('#OrOnlText').text(OregonOnly.RegionText);
            /*         Washington Premium     */
            jQuery('#WaPrePackage').text(WashingtonPre.PackageName);
            jQuery('#WaPreUser').text(WashingtonPre.UserText);
            jQuery('#WaPreRegion').text(WashingtonPre.RegionHead);
            jQuery('#WaPreText').text(WashingtonPre.RegionText);
            /*         Washington Plus     */
            jQuery('#WaPluPackage').text(WashingtonPlu.PackageName);
            jQuery('#WaPluUser').text(WashingtonPlu.UserText);
            jQuery('#WaPluRegion').text(WashingtonPlu.RegionHead);
            jQuery('#WaPluText').text(WashingtonPlu.RegionText);
            /*         Washington Only     */
            jQuery('#WaOnlPackage').text(WashingtonOnly.PackageName);
            jQuery('#WaOnlUser').text(WashingtonOnly.UserText);
            jQuery('#WaOnlRegion').text(WashingtonOnly.RegionHead);
            jQuery('#WaOnlText').text(WashingtonOnly.RegionText);
            /*         Project Posting Only Or Free     */
            jQuery('#prcFree').text(Free.PackageName);
            jQuery('#FreeRegion').text(Free.RegionHead);
            jQuery('#FreeText').text(Free.RegionText);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}

function SelectReason() {
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
                document.getElementById('customRadio6').click();
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
}

jQuery('input[name = example]').change(function () {
    var data = jQuery('input[name = example]:checked').val();
    jQuery('input[name = RadioExValue]').val(data);
    var dataPacific = jQuery('input[name = cardPacific]:checked').val();
    var dataOreg = jQuery('input[name = cardOreg]:checked').val();
    var dataWash = jQuery('input[name = cardWash]:checked').val();
    var dataFree = jQuery('input[name = cardFree]:checked').val();
    //jQuery('#AddlPrice').val('');
    var cost;
    var addlPrice = 0;
    if (dataFree == 'Free') {
        cost = '0';
        addlPrice = 0;
    }
    else {
        if (data == 'Yearly') {
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcPremium').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            if (dataPacific == 'Pacific Northwest Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataPacific) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcPremium').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcPremium').text();
            }
            else if (dataOreg == 'Oregon Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcOregon').text();
            }
            else if (dataOreg == 'Oregon Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#prcOregon').text();
            }
            else if (dataOreg == 'Oregon Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#prcOregon').text();
            }

            else if (dataWash == 'Washington Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcWash').text();
            }
            else if (dataWash == 'Washington Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcWash').text();
            }
            else if (dataWash == 'Washington Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcWash').text();
            }
        }
        else if (data == 'Monthly') {
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#YprcPremium').html(MonthlyInfo[0].MonthlyPrice * 12);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            if (dataPacific == 'Pacific Northwest Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataPacific) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + PaymentInfo[0].MonthlyPrice + '</span>');
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#YprcPremium').html(PaymentInfo[0].MonthlyPrice * 12);
                cost = jQuery('#spnPremium').text();
            }
            if (dataOreg == 'Oregon Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }
            else if (dataOreg == 'Oregon Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }
            else if (dataOreg == 'Oregon Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }

            else if (dataWash == 'Washington Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            else if (dataWash == 'Washington Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            else if (dataWash == 'Washington Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
        }
        else if (data == 'Quarterly') {
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#YprcPremium').html(QInfo[0].QuarterlyPrice * 4);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfo[0].QuarterlyPrice + '</span>');
            //addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            //addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            //addlPrice = QInfo[0].AddQuarterlyPrice;
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            if (dataPacific == 'Pacific Northwest Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataPacific) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                jQuery('#YprcPremium').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                cost = jQuery('#spnPremium').text();
            }
            if (dataOreg == 'Oregon Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }
            else if (dataOreg == 'Oregon Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }
            else if (dataOreg == 'Oregon Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }

            else if (dataWash == 'Washington Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            else if (dataWash == 'Washington Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            else if (dataWash == 'Washington Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
        }
    }
    jQuery('input[id = MemberCost]').val(cost);
    jQuery('input[id = AddlPrice]').val(addlPrice);
    if (dataFree == 'Free') {
        jQuery('input[id = hdnTerm]').val('Free Member');
    }
    else {
        jQuery('input[id = hdnTerm]').val(data);
    }
});

jQuery('input[name = cardPacific]').change(function () {
    var dataEx = jQuery('input[name = example]:checked').val();
    var data = jQuery('input[name = cardPacific]:checked').val();
    jQuery('#hdnPlanId').val('btnPacific');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(data);
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    // jQuery('#btnFree').css('display','none');
    // jQuery('#btnPay').css('display','block');
    var addlPrice = 0;
    if (dataEx == 'Yearly') {
        var memshipInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
        });
        memshipInfoCost = _.without(memshipInfoCost, undefined);
        jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
        addlPrice = memshipInfoCost[0].AddYearlyPrice;
        var memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
        addlPrice = memshipInfoCost[0].AddYearlyPrice;
        memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Washington Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
        addlPrice = memshipInfoCost[0].AddYearlyPrice;
        jQuery('#prcPremium').html('');
        jQuery('#prcOregon').html('');
        jQuery('#prcWashington').html('');
        jQuery('input[id = MemberCost]').val(memshipInfoCost[0].YearlyPrice);
    }
    else if (dataEx == 'Monthly') {
        var MonthlyInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
        jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
        jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
        addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
        var MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
        addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
        MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
        addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
        jQuery('input[id = MemberCost]').val(MonthlyInfoCost[0].MonthlyPrice);
    }
    else if (dataEx == 'Quarterly') {
        var QInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        QInfoCost = _.without(QInfoCost, undefined);
        jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
        jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
        addlPrice = QInfoCost[0].AddQuarterlyPrice;
        var QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
        addlPrice = QInfoCost[0].AddQuarterlyPrice;
        QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
        addlPrice = QInfoCost[0].AddQuarterlyPrice;
        jQuery('input[id = MemberCost]').val(QInfoCost[0].QuarterlyPrice);
    }
    jQuery('input[id = MemberType]').val('7');
    jQuery('input[id = AddlPrice]').val(addlPrice);
});

jQuery('input[name = cardOreg]').change(function () {
    var data = jQuery('input[name = example]:checked').val();
    var dataOreg = jQuery('input[name = cardOreg]:checked').val();
    jQuery('#hdnPlanId').val('btnOreg');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(dataOreg);
    var cost = jQuery('#prcOregon').text();
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);

    var addlPrice = 0;
    if (dataOreg == 'Oregon Premium') {
        jQuery('[id^=Popfaq]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=PopfaqWash]').addClass('show');
        jQuery('[id=Popfaq]').addClass('show');
        jQuery('input[id = MemberType]').val('9')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            // addlPrice = memshipInfoCost[0].AddYearlyPrice;
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfoCost[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
            cost = jQuery('#spnOreg').text();
        }

        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            cost = jQuery('#spnOreg').text();
        }
    }

    if (dataOreg == 'Oregon Only') {
        jQuery('[id^=Popfaq]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=PopfaqWash]').addClass('show');
        jQuery('[id=Popfaq]').addClass('show');
        jQuery('input[id = MemberType]').val('4')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            //addlPrice = memshipInfoCost[0].AddYearlyPrice;
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnOreg').text();
        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            //addlPrice = QInfo[0].AddQuarterlyPrice;
            cost = jQuery('#spnOreg').text();
        }
    }

    else if (dataOreg == 'Oregon Plus') {
        jQuery('[id^=Popfaq]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=PopfaqWash]').addClass('show');
        jQuery('[id=Popfaq]').addClass('show');
        jQuery('input[id = MemberType]').val('5')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            //addlPrice = memshipInfoCost[0].AddYearlyPrice;
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            // addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnOreg').text();
        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            cost = jQuery('#spnOreg').text();
        }
    }

    jQuery('input[id = MemberCost]').val(cost);
    jQuery('input[id = AddlPrice]').val(addlPrice);
});

jQuery('input[name = cardWash]').change(function () {
    var data = jQuery('input[name = example]:checked').val();
    var dataWash = jQuery('input[name = cardWash]:checked').val();
    jQuery('#hdnPlanId').val('btnWash');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(dataWash);
    var cost = jQuery('#prcWashington').text();
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    // jQuery('#btnFree').css('display','none');
    var addlPrice = 0;
    if (dataWash == 'Washington Premium') {
        jQuery('[id^=Popfaq6]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=Popfaq1]').addClass('show');
        jQuery('input[id = MemberType]').val('10')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnWash').text();
        }

        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            cost = jQuery('#spnWash').text();
        }
    }

    else if (dataWash == 'Washington Only') {
        jQuery('[id^=Popfaq6]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=Popfaq1]').addClass('show');
        jQuery('input[id = MemberType]').val('8')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnWash').text();
        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            cost = jQuery('#spnWash').text();
        }
    }

    else if (dataWash == 'Washington Plus') {
        jQuery('[id^=Popfaq6]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=Popfaq1]').addClass('show');
        jQuery('input[id = MemberType]').val('11')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnWash').text();
        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            cost = jQuery('#spnWash').text();
        }
    }
    jQuery('input[id = MemberCost]').val(cost);
    jQuery('input[id = AddlPrice]').val(addlPrice);
});

jQuery('input[name = cardFree]').change(function () {
    var data = jQuery('input[name = cardFree]:checked').val();
    var dataEx = jQuery('input[name = example]:checked').val();
    jQuery('#hdnPlanId').val('btnFree');
    jQuery('input[id = MemberType]').val('1')
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(data);
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    var addlPrice = 0;
    if (dataEx == 'Yearly') {
        var memshipInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
        });
        memshipInfoCost = _.without(memshipInfoCost, undefined);
        jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
        var memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
        memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Washington Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
        jQuery('#prcPremium').html('');
        jQuery('#prcOregon').html('');
        jQuery('#prcWashington').html('');
    }
    else if (dataEx == 'Monthly') {
        var MonthlyInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
        jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
        jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
        var MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
        MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
    }
    else if (dataEx == 'Quarterly') {
        var QInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        QInfoCost = _.without(QInfoCost, undefined);
        jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
        jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
        var QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
        QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
    } jQuery('input[id = MemberCost]').val('0');
    // jQuery('#btnPay').css('display','none');
    jQuery('input[id = AddlPrice]').val(addlPrice);
});

function changePacific(dataEx, data) {
    jQuery('#hdnPlanId').val('btnPacific');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(data);
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    if (dataEx == 'Yearly') {
        var memshipInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
        });
        memshipInfoCost = _.without(memshipInfoCost, undefined);
        jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
        var memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
        memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Washington Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
        jQuery('#prcPremium').html('');
        jQuery('#prcOregon').html('');
        jQuery('#prcWashington').html('');
        jQuery('input[id = MemberCost]').val(memshipInfoCost[0].YearlyPrice);
    }
    else if (dataEx == 'Monthly') {
        var MonthlyInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
        jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
        jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
        var MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
        MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
        jQuery('input[id = MemberCost]').val(MonthlyInfoCost[0].MonthlyPrice);
    }
    else if (dataEx == 'Quarterly') {
        var QInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        QInfoCost = _.without(QInfoCost, undefined);
        jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
        jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
        var QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
        QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
        jQuery('input[id = MemberCost]').val(QInfoCost[0].QuarterlyPrice);
    }
    jQuery('input[id = MemberType]').val('7')
}
function changeOreg(data, dataOreg) {
    jQuery('#hdnPlanId').val('btnOreg');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(dataOreg);
    var cost = jQuery('#prcOregon').text();
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    if (dataOreg == 'Oregon Premium') {
        jQuery('input[id = MemberType]').val('9')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnOreg').text();
        }

        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnOreg').text();
        }
    }

    if (dataOreg == 'Oregon Only') {
        jQuery('input[id = MemberType]').val('4');
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnOreg').text();
        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnOreg').text();
        }
    }

    else if (dataOreg == 'Oregon Plus') {
        jQuery('input[id = MemberType]').val('5')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnOreg').text();
        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnOreg').text();
        }
    }

    jQuery('input[id = MemberCost]').val(cost);
}
function changeWash(data, dataWash) {
    jQuery('#hdnPlanId').val('btnWash');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(dataWash);
    var cost = jQuery('#prcWashington').text();
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    // jQuery('#btnFree').css('display','none');

    if (dataWash == 'Washington Premium') {
        jQuery('input[id = MemberType]').val('10')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnWash').text();
        }

        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnWash').text();
        }
    }

    else if (dataWash == 'Washington Only') {
        jQuery('input[id = MemberType]').val('8')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnWash').text();
        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnWash').text();
        }
    }

    else if (dataWash == 'Washington Plus') {
        jQuery('input[id = MemberType]').val('11')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnWash').text();
        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnWash').text();
        }
    }
    jQuery('input[id = MemberCost]').val(cost);
}
function changeFree(dataEx, data) {
    jQuery('#hdnPlanId').val('btnFree');
    jQuery('input[id = MemberType]').val('1')
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(data);
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    if (dataEx == 'Yearly') {
        var memshipInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
        });
        memshipInfoCost = _.without(memshipInfoCost, undefined);
        jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
        var memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
        memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Washington Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
        jQuery('#prcPremium').html('');
        jQuery('#prcOregon').html('');
        jQuery('#prcWashington').html('');
    }
    else if (dataEx == 'Monthly') {
        var MonthlyInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
        jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
        jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
        var MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
        MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
    }
    else if (dataEx == 'Quarterly') {
        var QInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        QInfoCost = _.without(QInfoCost, undefined);
        jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
        jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
        var QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
        QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
    } jQuery('input[id = MemberCost]').val('0');
}
function GoToReg(ctrl) {
    var Cost = 0;
    var pkgMemText = "";
    var data = "";
    var id = jQuery('#hdnPlanId').val();
    var example = jQuery('input[name = example]:checked').val();
    if (example == null || example == "" || example == undefined) {
        alert("Please select membership package term(Yearly/Quarterly/Monthly)");
        return false;
    }
    jQuery('#hdnTerm').val(example);
    jQuery('#Paid').text(example);
    if (id == 'btnPacific') {
        Cost = jQuery('#MemberCost').val();
        data = jQuery('input[name = cardPacific]:checked').val();
        if (data == 'Pacific Northwest Premium') {
            jQuery('#chckvalue').val(data);
            jQuery("#membershipModal").modal('hide');
            jQuery("#MSPChk").val('Y');
            if (example == "Yearly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost)) + "/year paid Annually";
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Monthly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 12)) + "/year paid with 12 Monthly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Quarterly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 4)) + "/year paid with 4 Quarterly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            jQuery('#ColorString').val('#FF9900');
            jQuery('#pkgMem').css('color', '#FF9900');
        }
        else {
            jQuery(ctrl).next('span').html('You need to select a plan for pacific');
            return false
        }
    }
    else if (id == 'btnOreg') {
        data = jQuery('input[name = cardOreg]:checked').val();
        Cost = jQuery('#MemberCost').val();
        if (data == 'Oregon Premium' || data == 'Oregon Plus' || data == 'Oregon Only') {
            if (example == "Yearly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost)) + "/year paid Annually";
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Monthly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 12)) + "/year paid with 12 Monthly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Quarterly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 4)) + "/year paid with 4 Quarterly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            jQuery('#ColorString').val('#6d9d78');
            jQuery('#pkgMem').css('color', '#6d9d78');
            jQuery("#membershipModal").modal('hide');
            jQuery("#MSPChk").val('Y');
        }
        else {
            jQuery(ctrl).next('span').html('You need to select a plan for oregon');
            return false
        }
    }
    else if (id == 'btnWash') {
        Cost = jQuery('#MemberCost').val();
        data = jQuery('input[name = cardWash]:checked').val();
        if (data == 'Washington Premium' || data == 'Washington Plus' || data == 'Washington Only') {
            if (example == "Yearly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost)) + "/year paid Annually";
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Monthly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 12)) + "/year paid with 12 Monthly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Quarterly") {
                pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 4)) + "/year paid with 4 Quarterly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            jQuery('#ColorString').val('#4c829a');
            jQuery('#pkgMem').css('color', '#4c829a');
            jQuery("#membershipModal").modal('hide');
            jQuery("#MSPChk").val('Y');
        }
        else {
            jQuery(ctrl).next('span').html('You need to select a plan for washington');
            return false
        }
    }
    else if (id == 'btnFree') {
        data = jQuery('input[name = cardFree]:checked').val();
        if (data == 'Free') {
            pkgMemText = "Project Posting Only - Free";
            jQuery('#pkgMem').text(pkgMemText);
            jQuery('#PayCardString').val(pkgMemText);
            jQuery('#ColorString').val('#626c69');
            jQuery('#pkgMem').css('color', '#626c69');
            jQuery("#membershipModal").modal('hide');
            jQuery("#MSPChk").val('Y');
        }
        else {
            jQuery(ctrl).next('span').html('You need to select free job posting');
            return false;
        }
    }
    else {
        jQuery(ctrl).next('span').html('You need to select a valid plan');
        return false;
    }
    jQuery('#chgPackage').prev('span').html('');
    if (id == 'btnFree')
        jQuery('.BillInfoClass').css('display', 'none');
    else
        jQuery('.BillInfoClass').css('display', 'block');

    if (data != "" && data != null) {
        if (data == "Pacific Northwest Premium") {
            jQuery('#MemberType').val('7');
        }
        else if (data == "Oregon Premium") {
            jQuery('#MemberType').val('9');
        }
        else if (data == "Oregon Plus") {
            jQuery('#MemberType').val('5');
        }
        else if (data == "Oregon Only") {
            jQuery('#MemberType').val('4');
        }
        else if (data == "Washington Premium") {
            jQuery('#MemberType').val('10');
        }
        else if (data == "Washington Plus") {
            jQuery('#MemberType').val('11');
        }
        else if (data == "Washington Only") {
            jQuery('#MemberType').val('8');
        }
        else if (data == "Free") {
            jQuery('#MemberType').val('1');
        }
    }
    var id = jQuery('#hdnId').val();
    var ex = jQuery('#hdnTerm').val();
    var memtype = jQuery('#MemberType').val();
    var hdnDiscountId = jQuery('#hdnDiscountId').val();
    var url = "/Member/RenewalPayment" +
        "?PkgText=" + pkgMemText +
        "&Cost=" + Cost + "&Id=" + id + "&Term=" + ex + "&MemType=" + memtype + "&DiscountId=" + hdnDiscountId;
    window.location.href = url;
}
function DeleteUserManagement(Id, ctrl) {
    if (confirm("Are you sure you want to delete ?")) {
        jQuery.ajax({
            type: "POST",
            dataType: 'json',
            url: '/Member/DeleteUserManagement/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                alert(response.statusMessage);
                jQuery(ctrl).parents("tr").remove();
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
    return false;
}
function OpenResetModal(email) {
    jQuery("#resetform").trigger('reset');
    jQuery('#resetform').find("input[type=text]").val('');
    jQuery("#UserEmail").val(email);
    jQuery("#UserEmail").prop("readonly", true);
    jQuery(".usermodal").modal('show');
}
function ResetClose() {
    jQuery(".usermodal").modal('hide');
}
function validateInputLength(arg) {
    var minLength = 14;
    var maxLength = 14;

    var inputVal = jQuery("#" + arg).val();
    var inputLength = inputVal.length;
    jQuery("#" + arg).next('span').html('');
    if (inputLength < minLength) {
        jQuery("#" + arg).next('span').html('Input valid phone number');
        return false;
    } else if (inputLength > maxLength) {
        jQuery("#" + arg).next('span').html('Input valid phone number');
        return false;
    } else {
        jQuery("#" + arg).next('span').html(''); // Clear the validation message
    }
}
function validateEditLocInputLength(id) {
    var editLocElement = jQuery('#' + id); // Get the jQuery object for the input field
    editLocElement.on('input', function () {
        var x = editLocElement.val().replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
        editLocElement.val(!x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : ''));
    });
}

jQuery(document).ready(function () {
    jQuery('#updatePaymentMethodButton').on('click', function () {
        jQuery.ajax({
            url: '/ChargeBee/CreateUpdatePaymentMethodPage',
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                if (data.Status === 'success') {
                    window.location.href = data.Data;
                } else {
                    alert(data.Message);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error:', textStatus, errorThrown);
                alert('There was an issue with updating the payment method. Please try again later.');
            }
        });
    });
    jQuery('#closehistory').on('click', function () {
        jQuery('#paymentHistoryModal').modal('hide');
    });
    jQuery('#paymentHistoryButton').on('click', function () {
        jQuery.ajax({
            url: '/ChargeBee/GetPaymentHistory',
            type: 'POST',
            data: { email: '0', login: "member" },
            success: function (data) {
                jQuery('#paymentHistoryTableBody').html(data);
                const amountCells = document.querySelectorAll('.amount-cents');
                amountCells.forEach(function (cell) {
                    const amountInCents = parseInt(cell.textContent, 10);
                    const amountInDollars = (amountInCents / 100).toFixed(2);
                    cell.textContent = `$${amountInDollars}`;
                });
                jQuery('#paymentHistoryModal').modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error:', textStatus, errorThrown);
                alert('There was an issue retrieving payment history. Please try again later.');
            }
        });
    });
});