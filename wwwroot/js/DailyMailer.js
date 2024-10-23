$(document).ready(() => {
    $("#fileUpload1").change(function () {
        const file = this.files[0];
        if (file) {
            let reader = new FileReader();
            reader.onload = function (event) {
                $("#mailerImage")
                    .attr("src", event.target.result).css('display', 'block');
            };
            reader.readAsDataURL(file);
        }
    });
});
function OpenPrintModel() {
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea").val('');
    $("#frmPrintForm2").find("img#mailerImage").removeAttr('src').css('display', 'none');
    $("#removeImage").css('display', 'none'); 
    $('span.loginError').html('');
    $(".m-modal").modal('show');
}
function EditDailyMailer(Id, MailerText, MailerPath, IsActive) {
    $("#frmPrintForm2").trigger('reset');
    $('#frmPrintForm2').find("input[type=text], textarea, input[type=hidden]").val('');
    $("#frmPrintForm2").find("span.loginError").html('');
    $("#frmPrintForm2").find("img#mailerImage").removeAttr('src');
    $('#Id').val(Id);
    $('#mailerPath').val(MailerPath);
    $('#hdnFileName').val(MailerPath);
    $("#mailerText").val(MailerText);
    $("#mailerImage").prop('src', MailerPath);
    if (IsActive!='False')
        $('#IsActive').prop('checked', 'checked');
    $(".m-modal").modal('show');
    
};
function RemoveImage() {
    var Id = $('#idMailer').val();
    var Path = $('#mailerPath').val();
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/RemoveImage/',
        data: { 'Id': Id, 'Path': Path },
        async: false,
        success: function (response) {
            if (response.success) {
                $("#frmPrintForm2").find("img#mailerImage").removeAttr('src');
                $('img#mailerImage').css('display', 'none');
                $('#mailerPath').val('');
                $('span#message').html('Image removed successfuly');
            }
            else {
                $('span#message').html('Image not removed');
            }
        },
        error: function (response) {
            $('span#message').html('Image not removed');
        },
        failure: function (response) {
            $('span#message').html('Image not removed');
        }
    });
};

function SaveDailyMailer() {
    var mailertext = $('#mailerText').val();
    $('#mailerText').next('span').html('');
    if (mailertext == undefined || mailertext == '' || mailertext == null) {
        $('#mailerText').next('span').html('Please enter Mailer Text');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.Id = $('#Id').val();
    model.MailerPath = $('#mailerPath').val();
    model.MailerText = $('#mailerText').val();
    var pathChK = jQuery("#hdnFileName").val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveDailyMailer/',
        data: { 'model': model, 'pathChK': pathChK },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                alert(response.statusMessage);
            }
            else {
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
};
//function UpdateDailyMailer() {
//    var mailertext = $('#mailerText').val();
//    $('#mailerText').next('span').html('');
//    if (mailertext == undefined || mailertext == '' || mailertext == null) {
//        $('#mailerText').next('span').html('Please enter Mailer Text');
//        return false;
//    }

//    var model = {};
//    $('#message').val('');
//    model.Id = $('#Id').val();
//    model.MailerPath = $('#mailerImage').val();
//    model.MailerText = $('#mailerText').val();
//    model.IsActive = $('#IsActive').is(':checked') ? true : false;
//    $.ajax({
//        type: "POST",
//        dataType: 'json',
//        url: '/GlobalMaster/UpdateDailyMaile/',
//        data: { 'model': model },
//        async: false,
//        success: function (response) {
//            if (response.success) {
//                console.log(response);
//                alert(response.statusMessage);
//            }
//            else {
//                alert(response.statusMessage);
//            }
//        },
//        error: function (response) {
//            alert(response.responseText);
//        },
//        failure: function (response) {
//            alert(response.responseText);
//        }
//    });
//};

function UploadFile(e) {
    var pdf = jQuery('#fileUpload1').val();
    jQuery('#fileUpload1').next('span').html('');
    if (pdf == undefined || pdf == '' || pdf == null) {
        jQuery('#fileUpload1').next('span').html('Please choose your file');
        return false;
    }
    var formData = new FormData();
    var files = jQuery("#fileUpload1").get(0).files;
    var a = jQuery("#fileUpload1")[0].files[0];
    formData.append("pdfFile", jQuery("#fileUpload1")[0].files[0]);
    jQuery.ajax({
        type: 'POST',
        url: '/GlobalMaster/UploadPdf',
        data: formData,
        processData: false,
        contentType: false
    }).done(function (response) {
        if (response.Status === "success") {
            console.log(response.Data);
            jQuery("#uploadCheck").val(response.Flag)
            //jQuery("#lblModPlan").html(response.Message);
            jQuery('#uploadError').html('File Uploaded successfully').css('color', 'Blue')
            jQuery("#mailerPath").val(response.data);
        }
    });
    
}
