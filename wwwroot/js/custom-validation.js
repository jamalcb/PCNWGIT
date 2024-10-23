function validateEmail(email) {
	let res = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
	return res.test(email);
}
function checkEmail(id) {
	let result = $("#" + id).next('span');
	let email = $("#" + id).val();
	result.text("");
	if (validateEmail(email)) {
		result.text(email + " is valid email");
		result.css("color", "green");
	} else {
		result.text(email + " is not valid email");
		result.css("color", "red");
	}
	return false;
}

jQuery(document).ready(function () {
	jQuery('.number-only').keyup(function (e) {
		if (this.value != '-')
			while (isNaN(this.value))
				this.value = this.value.split('').reverse().join('').replace(/[\D]/i, '')
					.split('').reverse().join('');
	})
	.on("cut copy paste", function (e) {
		e.preventDefault();
	});
});

function Undecided1(ctrl) {
	var Assign = ctrl.checked;
	if (Assign) {
		jQuery('#divBids :input').prop('disabled', true);
		jQuery('#divBids :input').val('');
		jQuery('#Undecided').val("TBD");
	}
	else {
		jQuery('#divBids :input').prop('disabled', false);
		jQuery('#divBids :input#strBidDt3').prop('readonly', true);
		jQuery('#divBids :input#strBidDt3').val('PT')
		jQuery('#divBids :input#tComp').val('00')
		jQuery('#divBids :input#hComp').val('00')
		jQuery('#divBids :input#mValue').val('AM')
		jQuery('#Undecided').val('');
	}
	jQuery(ctrl).prop('disabled', false);
	jQuery('#Undecided').prop('disabled', false);
}

