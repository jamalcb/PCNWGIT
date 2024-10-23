
function Cancel() {
    window.location.reload();
}
function SaveArchitect() {
    
    var model = {};
    model.ID = $('#ID').val();
    model.ArchitectName = $('#ArchitectName').val();
    model.MailAddress = $('#BillAddress').val();
    model.MailCity = $('#BillCity').val();
    model.MailState = $('#BillStateId').val();
    model.MailZip = $('#BillZip').val();
    model.ContactPhone = $('#ContactPhone').val();
    model.Email = $('#Email').val();
    model.Type = $('#Type').val();
    var loading = jQuery('#loader-overlay2');
    loading.show();
    setTimeout(function () {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/StaffAccount/UpdateArchitectProfile/',
            data: { 'model': model },
            async: false,
            beforeSend: function () {
            },
            success: function (response) {
                if (response.success) {
                    alert(response.statusMessage);
                    window.location.reload();
                }
                else {
                    loading.hide();

                }
            },
            error: function (response) {
                loading.hide();
            },
            failure: function (response) {
                loading.hide();

            }
        });
    },100);
}

var phoneInput = document.getElementById('ContactPhone');
var EditPhoneInput = document.getElementById('EditPhone');
var AddPhone = document.getElementById('Phone');
phoneInput.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
EditPhoneInput.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
AddPhone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
$('#Email').on('change', function () {
    checkUniqueEmail();
});

function checkUniqueEmail() {
    var success = true;
    jQuery('#Email').next('span').html('');
    var uniqueName = jQuery('#Email').val();
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
                jQuery('#Email').next('span').html(response.statusMessage);
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
function OpenEditPopup(ConId, Name, Email, Phone) {
    $("#frmEdit").trigger('reset');
    $("#frmEdit").find("span.loginError").html('');
    $("#EdhdnCid").val(ConId);
    $("#EditName").val(Name);
    $("#EditConEmail").val(Email);
    $("#hdnEmail").val(Email);
    $("#EditPhone").val(Phone);
    $('.ue-modal').modal('show');
};
$('#EditConEmail').on('change', function () {
    var Email1 = jQuery('#EditConEmail').val();
    var Email2 = jQuery('#hdnEmail').val();
    if (Email1 != Email2) {
        checkUniqueUserEmail();
    }
    else {
        $('#EditConEmail').next('span').html('');
    }
});
function checkUniqueUserEmail() {
    var success = true;
    jQuery('#EditConEmail').next('span').html('');
    var uniqueName = jQuery('#EditConEmail').val();
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
function UpdateData() {
    var name = $('#EditName').val();
    $('#EditName').next('span').html('');
    if (name == undefined || name == '' || name == null) {
        $('#EditName').next('span').html('Please enter name.');
        return false;
    }
    var email = $('#EditConEmail').val();
    $('#EditConEmail').next('span').html('');
    if (email == undefined || email == '' || email == null) {
        $('#EditConEmail').next('span').html('Please enter email.');
        return false;
    }
    var phone = $('#EditPhone').val();
    $('#EditPhone').next('span').html('');
    if (phone == undefined || phone == '' || phone == null) {
        $('#EditPhone').next('span').html('Please enter name.');
        return false;
    }
    var model = {};
    model.ConId = $('#EdhdnCid').val();
    model.ContactName = $('#EditName').val();
    model.ContactEmail = $('#EditConEmail').val();
    model.ContactPhone = $('#EditPhone').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/StaffAccount/EditUser/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            location.reload();
            if (response.success) {
                console.log(response);
                alert(response.statusMessage);
            }
            else {
                
            }
        },
        error: function (response) {
           
        },
        failure: function (response) {
            
        }
    });
};
$('#ConEmail').on('change', function () {
    AddcheckUniqueEmail();
});
function AddcheckUniqueEmail() {
    var success = true;
    $('#ConEmail').next('span').html('');
    var uniqueName = jQuery('#ConEmail').val();
    var data = { "uniqueName": uniqueName };
    $.ajax({
        type: "POST",
        url: '/MemberShip/UniqueEmail/',
        data: { "uniqueName": uniqueName },
        dataType: "json",
        async: false,
        //contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.success) {
                success = false;
                $('#ConEmail').next('span').html(response.statusMessage);
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
function submitData() {
    var name = $('#Firstname').val();
    $('#Firstname').next('span').html('');
    if (name == undefined || name == '' || name == null) {
        $('#Firstname').next('span').html('Please enter first name.');
        return false;
    }
    var email = $('#ConEmail').val();
    $('#ConEmail').next('span').html('');
    if (email == undefined || email == '' || email == null) {
        $('#ConEmail').next('span').html('Please enter email.');
        return false;
    }
    var phone = $('#Phone').val();
    $('#Phone').next('span').html('');
    if (phone == undefined || phone == '' || phone == null) {
        $('#Phone').next('span').html('Please enter phone number.');
        return false;
    }
    var model = {};
    model.ID = $('#ID').val();
    model.FirstName = $('#Firstname').val();
    model.LastName = $('#Lastname').val();
    model.ContactEmail = $('#ConEmail').val();
    model.ContactPhone = $('#Phone').val();
    model.CompType = 3;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/StaffAccount/AddNewUser/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            location.reload();
            if (response.success) {
                alert(response.statusMessage);
            }
            else {
                alert(response.statusMessage);
            }
        },
        error: function (response) {
        },
        failure: function (response) {
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
jQuery(document).ready(function () {
    //var zipCode = $(this).val();
    jQuery('#BillZip').keyup(function () {
        var zip = jQuery(this).val();
        jQuery('#BillCity').val('');
        jQuery('#BillStateId').val('');
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
                                    if (response.success) {
                                        var stateId = response.data[0].StateId;
                                        var formattedCity = upperCaseFirstLetter(lowerCaseAllWordsExceptFirstLetters(City));
                                        jQuery('#BillCity').val(formattedCity);
                                        jQuery('#BillStateId').val(stateId);
                                    }
                                    else {
                                        jQuery('#BillCity').val('');
                                        jQuery('#BillStateId').val('');
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
                        else {
                            jQuery('#BillCity').val('');
                            jQuery('#BillStateId').val('');
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
function OpenUserModal() {
    jQuery("#UserForm").trigger('reset');
    jQuery('#UserForm').find("input[type=text], textarea").val('');
    jQuery('span.loginError').html('');
    jQuery('.u-modal').modal('show');
}
jQuery('.user-modal').on('click', function () {
    jQuery('.u-modal').modal('hide');
})

jQuery(document).ready(function () {
    jQuery("#formButton").click(function () {
        jQuery("#addnote").toggle();
    });
    jQuery("#editNote").click(function () {
        jQuery("#editNotes").toggle();
    });
});

function EditNote(ctrl, id, e) {
    console.log(id);
    var refCtrl = jQuery(ctrl).parent('div').siblings();
    console.log(refCtrl.children('span.text-date-class').text());
    console.log(refCtrl.children('span.text-note-class').text());
    jQuery("#hdnNoteId").val(id);
    jQuery("#spanNotedate").text(refCtrl.children('span.text-date-class').text())
    jQuery("#edittextNote").val(refCtrl.children('span.text-note-class').text())
    jQuery(".noteModal").modal('show');
    e.preventDefault();
}
function DeleteNote(memid, id) {
    if (confirm("Are you sure you want to delete note?")) {
        var model = {};
        model.Id = id;
        model.MemId = memid;
        jQuery.ajax({
            type: "POST",
            dataType: 'json',
            //contentType: "application/json; charset=utf-8",
            url: '/StaffAccount/DeleteNote/',
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
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/StaffAccount/UpdateNote/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            jQuery(".noteModal").modal('hide');
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
    model.MemId = jQuery('#ID').val();
    model.CompType = jQuery('#CompType').val();
    if (jQuery('#txtAddNote').val() == '') {
        jQuery('#txtAddNote').next('span').html('You can not add empty note.');
        return false;
    }
    else {
        jQuery('#txtAddNote').next('span').html('');
    }
    jQuery.ajax({
        type: "POST",
        dataType: 'json',
        //contentType: "application/json; charset=utf-8",
        url: '/StaffAccount/AddNote/',
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

function CheckUniqCompany(ctrl, uniqueCompany, id) {
    var form = $(ctrl).parent();
    jQuery.ajax({
        type: "POST",
        url: '/MemberShip/UniqueComForConOrArch/',
        data: { "uniqueCompany": uniqueCompany, "id": id },
        dataType: "json",
        async: false,
        //contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.success) {
                success = false;
                alert('This company is already exist.')
                return false;
            }
            else {
                form.submit();
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });

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