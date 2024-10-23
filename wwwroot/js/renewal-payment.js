// Get the current URL
var currentUrl = window.location.href;

// Remove the 'returnUrl' parameter from the URL
var updatedUrl = currentUrl.split('?')[0];

// Update the URL without the 'returnUrl' parameter
window.history.replaceState({}, document.title, updatedUrl);

jQuery(document).ready(function () {
    var pkg = $("#PkageText").val();
    var cost = $("#Cost").val();
    if (cost == 0 ? $("#TotalAmmount").text('0') : $("#TotalAmmount").text(cost))
   // $("#TotalAmmount").text(cost);

    var splpkg = pkg.split(' ');
    var pkgval = splpkg[0];
    if (pkgval == "Pacific") {
        $("#pkgText").text(pkg);
        $("#pkgText").css('color', '#FF9900');
    }
    else if (pkgval == "Oregon") {
        $("#pkgText").text(pkg);
        $("#pkgText").css('color', '#6d9d78');
    }
    else if (pkgval == "Washington") {
        $("#pkgText").text(pkg);
        $("#pkgText").css('color', '#4c829a');
    }
    
    jQuery(".CPBox").click(function () {

        jQuery(".CPBox").toggleClass("isActive");
        jQuery(".paymentPhone").toggleClass("dis-pay");
        jQuery(".paymentInfo").toggleClass("dis-pay");
    });
});

function validateInput(element) {
    var cardNumber = element.value;
    var minLength = 18;
    var maxLength = 19;
    jQuery('#CardNumber').next('span').html('');
    if (cardNumber == null || cardNumber == "") {
        jQuery('#CardNumber').next('span').html('Please enter your card detail.');
        jQuery('#CardNumber').focus();
        return false;
    }
    if (cardNumber.length < minLength || cardNumber.length > maxLength) {
        jQuery('#CardNumber').next('span').html('Is-Invalid');
        jQuery('#CardNumber').focus();
        return false;
    } else {
        jQuery('#CardNumber').next('span').html('');
    }
}
$(document).ready(function () {
    $("#CardNumber").on("input", function () {
        var enteredSequence = $(this).val();
        var formattedSequence = formatSequence(enteredSequence);
        $(this).val(formattedSequence);
    });

    function formatSequence(sequence) {
        var formatted = sequence.replace(/\s+/g, "").match(/.{1,4}/g);
        if (formatted) {
            return formatted.join(" ");
        }
        return "";
    }
});
function formatExpiryDate(input) {
    let value = input.value.replace(/\D/g, ''); // Remove non-numeric characters

    if (value.length >= 2) {
        value = value.substring(0, 2) + '/' + value.substring(2);
    }

    input.value = value;
}
document.getElementById("ExpiryDate").addEventListener("keydown", function (e) {
    if (e.key === "Backspace") {
        var value = this.value;
        if (value.length === 3 && value.charAt(2) === "/") {
            this.value = value.substring(0, 2);
            e.preventDefault(); // Prevents the default backspace behavior
        }
    }
});



function ValidateExpiryDate() {
    $("#ExpiryDate").next('span').html('');
    var inputValue = $("#ExpiryDate").val();
    if (inputValue == null || inputValue == "") {
        $("#ExpiryDate").next('span').html('Please enter your card detail');
        jQuery('#ExpiryDate').focus();
        return false;
    }
    // Regular expression pattern for MM/YY format
    var pattern = /^(0[1-9]|1[0-2])\/?([0-9]{2})$/;
    // Check if the input matches the pattern
    if (pattern.test(inputValue)) {
        // Valid format
        $("#ExpiryDate").next('span').html('');
    } else {
        // Invalid format
        $("#ExpiryDate").next('span').html('Invalid MM/YY format. Please enter a valid format (MM/YY).');
        jQuery('#ExpiryDate').focus();
        return false;
    }
}

function GoToRenewalPayment() {
    // Get the element with the specified ID
    var element = document.getElementById("ActiveCard");

    // Check if the element has the class "isActive"
    if (element.classList.contains("isActive")) {
        // The element is active
        $("#BankName").next('span').html('');
        $("#BankName").val('');
        $("#CheckNum").next('span').html('');
        $("#CheckNum").val('');

        var name = $("#CardHolderName").val();
        $("#CardHolderName").next('span').html('');
        if (name == null || name == "" || name == undefined) {
            $("#CardHolderName").next('span').html('Please insert card holder name.');
            return false;
        }
        var cardnumber = $("#CardNumber").val();
        $("#CardNumber").next('span').html('');
        if (cardnumber == null || cardnumber == "" || cardnumber == undefined) {
            $("#CardNumber").next('span').html('Please insert card number.');
            return false;
        }
        var expirydate = $("#ExpiryDate").val();
        $("#ExpiryDate").next('span').html('');
        if (expirydate == null || expirydate == "" || expirydate == undefined) {
            $("#ExpiryDate").next('span').html('Please insert card expiry date.');
            return false;
        }
        var cvc = $("#CVC").val();
        $("#CVC").next('span').html('');
        if (cvc == null || cvc == "" || cvc == undefined) {
            $("#CVC").next('span').html('Please insert card cvc number.');
            return false;
        }
        else if (cvc.length < 3) {
            $("#CVC").next('span').html('Please insert valid card cvc number.');
            return false;
        }
        else {
            $("#CVC").next('span').html('');
        }

       
    } else {
        // The element is not active
        $("#CardHolderName").next('span').html('');
        $("#CardHolderName").val('');
        $("#CardNumber").next('span').html('');
        $("#CardNumber").val('');
        $("#ExpiryDate").next('span').html('');
        $("#ExpiryDate").val('');
        $("#CVC").next('span').html('');
        $("#CVC").val('');

        var bankname = $("#BankName").val();
        $("#BankName").next('span').html('');
        if (bankname == null || bankname == "" || bankname == undefined) {
            $("#BankName").next('span').html('Please insert bank name');
            return false;
        }
        var cardnumber = $("#CheckNum").val();
        $("#CheckNum").next('span').html('');
        if (cardnumber == "" || cardnumber == null || cardnumber == undefined) {
            $("#CheckNum").next('span').html('Please insert card number');
            return false;
        }
    }
    var id = $("#HdnId").val();
    var controller = $("#Controller").val();
    var discountId = $("#DiscountId").val();
    if (id == null || id == 0 || id == undefined) {
        alert('Something went wrong');
        return false;
    }
    var cost = $("#Cost").val();
    if (cost == null || cost == undefined || cost == "") {
        alert('Something went wrong');
        return false;
    }
    var term = $("#Term").val();
    var memtype = $("#MemType").val();
    var model = {};
    model.ID = id;
    model.MemberCost = cost;
    model.PayStatus = "N";
    model.Term = term;
    model.MemberType = memtype;
    model.Controller = controller;
    model.DiscountId = discountId;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/App/InitiateAuthRenewal/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.Status = 'success') {
                window.location.href = response.Data;
            }
            else {
                alert("There's been issue with payment please try again");
                window.location.href = "/Member/MemberProfile?id=" + id;
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