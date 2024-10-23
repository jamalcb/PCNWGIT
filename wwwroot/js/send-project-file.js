

var valProjTypeId = 0;
jQuery("body").on("click", "#del-biddateRow", function () {
    jQuery("#remove-biddate").remove();
})

jQuery(document).ready(function () {
    jQuery('#joinfreebtn').click(function () {
        localStorage.setItem('camefromhome', true);
        window.location.href = '/MemberShip/Register';
    });
});
jQuery(document).ready(function () {
    if (jQuery('#ErrMsg').val() == 'Y')
        alert('Something went wrong');
    if (jQuery('#ErrMsg').val() == 'OK') {
        alert('Project Files Sent successfully');
        jQuery('#ErrMsg').val('N');
    }

    GetProjCode();
    function GetProjCode() {
        jQuery.ajax({
            url: '/Home/GetProjectCode/',
            data: {},
            type: "POST",
            success: function (response) {
                jQuery('#ProjNumber').val(response.data.Result);
            },

            failure: function (response) {
                alert(response.statusMessage);
            }
        });
    }

});

async function UploadPdfFile(e) {
    var FileControls = document.querySelectorAll(".file__input--file");
    var projNum = jQuery('#ProjNumber').val();
    jQuery('#ProjNum').val(projNum);
    jQuery('#uploadError').html('')

    for (i = 0; i < FileControls.length; i++) {
        var element = FileControls[i];
        var elementId = element.getAttribute("id");
        var files = jQuery("#" + elementId).get(0).files;
        if (files.length > 0) {
            for (var k = 0; k < files.length; k++) {
                var file = files[k];
                var fileName = file.name;
                var extension = fileName.replace(/^.*\./, '');
                if (extension != 'pdf' && extension != 'docx' && extension != 'jpg' && extension != 'tif') {
                    jQuery('#uploadError').text(fileName + ' is not a valid file please upload only pdf/docx/jpg/tif file(s)');
                    return false;
                }
            }
        }
    }
    var limit = FileControls.length;
    var allFiles = [];

    for (i = 0; i < limit; i++) {

        var element = FileControls[i];
        var elementId = element.getAttribute("id");
        var files = jQuery("#" + elementId).get(0).files;

        if (files.length > 0) {
            jQuery('#chkUploaded').val('Y')
            allFiles = allFiles.concat(Array.from(files));
        }
    }
    var fileMap = new Map();

    // Iterate through allFiles in reverse order
    for (let i = allFiles.length - 1; i >= 0; i--) {
        let file = allFiles[i];
        if (!fileMap.has(file.name)) {
            fileMap.set(file.name, file);
        }
    }

    // Extract the unique files from the map
    allFiles = Array.from(fileMap.values());
    var ext = jQuery('#chkUploaded').val();
    
    var progressContainer = document.getElementById("progress-container");
    var progressBar = document.getElementById("progress-bar");
    var progressText = document.getElementById("progress-text");
    var filename = document.getElementById("filenameid");
    var ProjNum = jQuery("#ProjNumber").val();
    jQuery("#processText").html('');
    var input = document.getElementById("fileUpload1");
    var totalFiles = 0;
    var successfulUploads = 0;
    for (i = 0; i < allFiles.length; i++) {
        totalFiles = allFiles.length;

        jQuery('#btnfileUpload1').prop('disabled', true);
        progressContainer.style.display = "block";
        progressBar.style.width = "0%";
        progressText.textContent = "0%";
        filename.innerText = "";

        var formData = new FormData();
        formData.append("files", allFiles[i]);
        formData.append("ProjNum", ProjNum);
        formData.append("firstfile", i);
        await new Promise((resolve, reject) => {
            jQuery.ajax({
                url: "/Home/DemoFileUpload",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();

                    xhr.upload.addEventListener("progress", function (e) {
                        if (e.lengthComputable) {
                            var percent = Math.round((e.loaded / e.total) * 100);
                            progressBar.style.width = percent + "%";
                            progressText.textContent = percent + "%";
                            if (allFiles[i].name) {
                                filename.innerText = "Uploading " + allFiles[i].name;
                            }
                        }
                    }, false);

                    return xhr;
                },
                success: function (response) {
                    if (response.success === true) {
                        successfulUploads++;
                        resolve(); // Resolve the promise when the AJAX call is successful

                        progressContainer.style.display = "none";
                        if (successfulUploads === totalFiles) {
                            jQuery('#uploadError').html('File Uploaded successfully').css('color', 'Blue')
                            jQuery('#chkUploaded').val('Y');
                            jQuery('#hdnPath').val(response.data);
                            jQuery('#SucStatus').val(response.success);
                            jQuery("#Uploadfiles").css('display', 'block');
                        }
                    } else {
                        reject(new Error("Upload failed")); // Reject the promise if the AJAX call is not successful

                    }
                },
                error: function (error) {
                    reject(error); // Reject the promise in case of an error
                }
            });
        });


    }
}

// ==============================================================================================================================
// script for adding addenda row section
function selFiles(ctrl, event) {
    var files = event.target.files;
    var IdClass = ctrl.id;
    jQuery('#uploadError').html('');
    var delCount = jQuery("div[class*='drag-file']").length + 1;
    var checkName = document.querySelectorAll('[class*="file__value--text"]');
    var chkFile = new Array();
    if (checkName.length > 0) {
        for (var i = 0; i < checkName.length; i++) {
            chkFile.push(checkName[i].innerHTML);
        }
    }

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var fileHT = file.name;
        var innerhtml = "";

        if (chkFile.includes(fileHT)) {
            if (confirm('Already selected' + ' ' + fileHT + ' ' + 'name of the file, do you want to replace it?')) {
                chkFile = chkFile.filter(function (value) {
                    return value !== fileHT;
                });
                removeFile(IdClass, fileHT);
            } else {
                // Handle the case when the user chooses not to replace
            }
        } else {
            innerhtml = "<div class='file__value' tempid='" + IdClass + "'><div class='file__value--text'>" +
                fileHT +
                "</div></div>";
        }
        jQuery('#file_input').append(innerhtml);
    }

    jQuery(ctrl).parents('div.drag-file').css('display', 'none');
    uploadHtml = '<div class="drag-file" >' +
        '<div class="file__input" id ="file__input" style = "height: 100px; border: 3px dotted;" >' +
        '<input class="file__input--file" id="fileUpload' + delCount + '" tabindex="9" name="pdfFile" type="file" multiple="multiple" onchange="selFiles(this,event)">' +
        '<label class="file__input--label" for="fileUpload' + delCount + '" data-text-btn="Upload"> Drag and Drop a file: </label>' +
        '</div></div>';
    jQuery(ctrl).parents('div.drag-append').append(uploadHtml);

    UploadPdfFile();
}
function removeFile(tempid, filename) {
    var fileElement = jQuery('[tempid="' + tempid + '"] [class*="file__value--text"]').filter(function () {
        return jQuery(this).text() === filename;
    }).closest('.file__value');
    fileElement.click();
}
jQuery(document).ready(function () {
    // ------------  File upload BEGIN ------------

    var fileHtml = ''
    jQuery(".file__input--file").on("change", function (event) {
        selFiles(this, event);
    });

    //Click to remove item
    jQuery("body").on("click", ".file__value", function () {
        var filename = jQuery(this).find('div.file__value--text').text();

        var id = jQuery(this).attr('tempid');
        var inputElement = jQuery('#' + id)[0]; // Replace 'your-input-element-id' with the actual ID of your input element

        var dataTransfer = new DataTransfer();

        // Iterate through the files in the input element
        for (var i = 0; i < inputElement.files.length; i++) {
            var file = inputElement.files[i];

            // Add the file to the new DataTransfer object if it doesn't match the filename
            if (file.name !== filename) {
                dataTransfer.items.add(file);
            }
        }
        // Replace the files in the input element with the new files
        inputElement.files = dataTransfer.files;
        if (inputElement.files.length === 0) {
            jQuery("#uploadCheck").val('N');
            jQuery("#selectCheck").val('N');
        }
        jQuery(this).remove();
        UploadPdfFile();
    });
    // ------------ File upload END ------------
});

function changeScope(id) {
    //alert(id);
    var strManipulated = '';
    var strProjectScope = jQuery('#ProjScope').val();
    var chkValue = '';
    if (jQuery('#' + id).is(':checked')) {
        chkValue = jQuery('#' + id).val();
        //console.log(chkValue);
    }
    var isExists = 'N';
    if (strProjectScope != '') {
        var myArray = strProjectScope.split(",");
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
    jQuery('#ProjScope').val(strManipulated);
    console.log(jQuery('#ProjScope').val());
}
function checkUpload() {
    var Title = jQuery('#Title').val();
    jQuery('#Title').next('span').html('');
    if (Title == undefined || Title == '' || Title == null) {
        jQuery('#Title').next('span').html('Please enter project name');
        return false;
    }
    var chk = jQuery('#chkUploaded').val();
    if (chk == 'N') {
        alert('Please upload document(s)');
        return false;
    }

    else {
        return true;
    }
}
function UploadFile() {
    var name = jQuery('#ContactName').val();
    if (name == undefined || name == '' || name == null) {
        alert('Please enter contact name');
        return false;
    }
    var email = jQuery('#ContactEmail').val('');
    if (email == undefined || email == '' || email == null) {
        alert('Please enter email address');
        return false;
    }
    var validemail = jQuery('#InvalidEmail').text();
    if (validemail != '') {
        alert('Please enter a valid email address');
        return false;
    }
    var phone = jQuery('#ContactPhone').val();
    if (phone == undefined || phone == '' || phone == null) {
        alert('Please enter contact phone number');
        return false;
    }
    var title = jQuery('#Title').val();
    if (title == undefined || title == '' || title == null) {
        alert('Please enter project name');
        return false;
    }
    var name = jQuery('#ContactName').val();
    var email = jQuery('#ConEmail').val();
    var title = jQuery('#Title').val();
    var phone = jQuery('#ContactPhone').val();
    var projNum = jQuery('#ProjNumber').val();
    var DiPath = jQuery('#hdnPath').val();
    var sucStatus = jQuery('#SucStatus').val();
    /*jQuery('#ProjNum').val(projNum);*/
    var model = {};
    model.ContactName = name; 
    model.ContactEmail =  email
    model.Title =  title
    model.ContactPhone =  phone
    model.ProjNumber =  projNum
    model.LocalPath = DiPath 
    document.getElementById("loader-overlay").style.display = "block";
    if (sucStatus == 'true' || sucStatus == 'True' || sucStatus == true) {
        jQuery.ajax({
            type: 'POST',
            dataType: 'json',
            url: '/Home/SendToS3',
            data: { 'model': model },
            success: function (response) {
                if (response.Status == 'success') {
                    alert('Project file(s) sent');
                    document.getElementById("loader-overlay").style.display = "none";
                    window.location.reload();

                }
                else if (response.Status == 'error') {
                    alert(response.statusmessage);
                    document.getElementById("loader-overlay").style.display = "none";
                }
                else if (!response.success) {
                    alert(response.statusmessage);
                    document.getElementById("loader-overlay").style.display = "none";
                }                
                else {
                    alert('Something went wrong please try again');
                    document.getElementById("loader-overlay").style.display = "none";
                    window.location.reload();
                }
            },
            error: function (response) {
                alert(response.statusmessage);
                alert('Something went wrong please try again');
                document.getElementById("loader-overlay").style.display = "none";
                window.location.reload();
            },
            failure: function (response) {
                alert(response.statusmessage);
                alert('Something went wrong please try again');
                document.getElementById("loader-overlay").style.display = "none";
                window.location.reload();
            }
        });
    }
    else { alert('Something went wrong please try again'); document.getElementById("loader-overlay").style.display = "none"; }
    
}
var Phone = document.getElementById('ContactPhone');
var result = document.getElementById('ContactPhone');

Phone.addEventListener('input', function (e) {
    var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
    e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
});

jQuery('#ConEmail').on('change', function () {
    ValidateEmailaddress();
});

function ValidateEmailaddress() {
    var email = jQuery('#ConEmail').val();
    jQuery('#ConEmail').next('span').html('');
    var validRegex = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (email.match(validRegex)) {
        jQuery('#InvalidEmail').val('');
        //checkUniqueemail();
        return true;
    }
    else {
        jQuery('#ConEmail').next('span').html('Invalid Email address');

        return false;

    }

}

