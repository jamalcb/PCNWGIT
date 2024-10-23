$(document).ready(function () {
    BindEntityType(0);
});
//function createDataTables() {
//    setTimeout(function () {
//        $('.eTables').datatables();
//    }, 1000);
//}
//function reinitialLizedDataTables() {
//    $('.eTables').dataTable().fnClearTable();
//    $('.eTables').dataTable().fnDestroy();
//}
function OpenPrintModel2() {
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea, input[type=password],input[type=email]").val('');
    $('span.loginError').html('');
    $("#inpPass").val('');
    $(".m-modal").modal('show');
}
function BindEntityType(isReinitialized) {
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Administration/GetStaffMember',
        data: {},
        async: false,
        success: function (response) {
            console.log(response);
            var rows = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td>' + item.Contact + '</td><td class="">' + item.Phone + '</td><td class="">' + item.Email + '</td><td class="">' + (item.Active == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditStaff(this, ' + item.ConId + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblStaff').html(rows);
   //         createDataTables();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}
function SaveStaff(ConId) {
    var inpContact = $('#inpContact').val();
    $('#inpContact').next('span').html('');
    if (inpContact == undefined || inpContact == '' || inpContact == null) {
        $('#inpContact').next('span').html('Please enter your contact.');
        return false;
    }
    var inpPhone = $('#inpPhone').val();
    $('#inpPhone').next('span').html('');
    if (inpPhone == undefined || inpPhone == '' || inpPhone == null) {
        $('#inpPhone').next('span').html('Please enter your phone number.');
        return false;
    }
    var inpEmail = $('#inpEmail').val();
    $('#inpEmail').next('span').html('');
    if (inpEmail == undefined || inpEmail == '' || inpEmail == null) {
        $('#inpEmail').next('span').html('Please enter your email.');
        return false;
    }
    
    var inpPass = $('#inpPass').val();
    var hdnPass = $('#hdnPass').val();
    $('#inpPass').next('span').html('');
    if (inpPass == undefined || inpPass == '' || inpPass == null) {
        $('#inpPass').next('span').html('Please enter your password.');
        return false;
    }
    var isValidPassword = ValidateCompleaxPassword();
    if (isValidPassword == false) {
        $('#inpPass').next('span').html('Password must contain the following: A Lowerecase letter, A Capital (uppercase) letter,A number,Minimum 8 characters');
        return false;
    }
    var passData = jQuery('input[name = hdnPass]').val();
    var ConfirmPassData = jQuery('input[name = hdnPassConfirm]').val();
    if (passData && ConfirmPassData) {
        if (passData != ConfirmPassData) {
            alert('Password and confirm password should be matching');
            return false;
        }
    }
    var passData = jQuery('input[name = hdnPass]').val();
    var chk = document.getElementById('IsActive');
    var model = {};
    $('#message').val('');
    model.Contact = inpContact;
    model.Email = inpEmail;
    model.Phone = inpPhone;
    model.Password = inpPass;
    model.Active = chk.checked;
    if (ConId != 0)
    {
        model.ConId = ConId;
    }
    console.log(model);
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/Administration/SaveStaffMember/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindEntityType(1);
                alert(response.statusMessage);
            }
            else {
                $('#message').fadeIn();
                $('#message').text(response.statusMessage).fadeOut(5000);
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
var phoneInput = document.getElementById('inpPhone');
var myInput = document.getElementById("inpPass");
var capital = document.getElementById("capital");
var number = document.getElementById("number");
var length = document.getElementById("length");
var letter = document.getElementById("letter");
phoneInput.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});
myInput.onkeyup = function () {
    ValidateCompleaxPassword();
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


function EditStaff(Ctrl, ConID) {
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/Administration/GetEditData/',
        data: { 'ConId': ConID },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                $('#inpContact').val(response.data.Contact);
                $('#inpPhone').val(response.data.Phone);
                $('#inpEmail').val(response.data.Email);
                $('#inpPass').val(response.data.Password);
                $('#hdnPass').val(response.data.Password);
                $('#next-btn').attr('onclick', 'SaveStaff(' + response.data.ConId + ')')
                $('#IsActive').attr('checked', response.data.Active);
                $(".m-modal").modal('show');
            }
            else {
                alert('Something went wrong')
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
jQuery('#inpEmail').on('change', function () {
    checkUniqueEmail();
});
function checkUniqueEmail() {
    var success = true;
    jQuery('#inpEmail').next('span').html('');
    var uniqueName = jQuery('#inpEmail').val();
    var data = { "uniqueName": uniqueName };
    jQuery.ajax({
        type: "POST",
        url: '/Home/UniqueEmail/',
        data: { "uniqueName": uniqueName },
        dataType: "json",
        async: false,
        //contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.success) {
                success = false;
                jQuery('#inpEmail').next('span').html(response.statusMessage);
                jQuery('#inpEmail').focus();
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
function ValidateConfirmPass() {
    var passData = jQuery('input[name = Password]').val();
    var ConfirmPassData = jQuery('input[name = ConfirmPassword]').val();
    if (passData && ConfirmPassData) {

        if (passData != ConfirmPassData) {
            jQuery('#ConfirmPassError').html('Password and confirm password should be matching');
            return false;
        }
        else {
            jQuery('#ConfirmPassError').html('');
        }

    }
}