jQuery(document).ready(function () {
	jQuery.ajax({
		type: "GET",
		dataType: 'json',
		//contentType: "application/json; charset=utf-8",
		url: '/Home/GetCurrentBiddingProject/',
		data: {},
		async: false,
		success: function (response) {
			console.log(response);
			if (response.success)
				jQuery('#spanProjCount').text(response.data.Result);
		},
		error: function (response) {
			console.log(response.responseText);
		},
		failure: function (response) {
			console.log(response.responseText);
		}
	});
});

function valiadteLogin() {
	var email = jQuery('#Email');
	var password = jQuery('#Password');
	if (email.val() == '' || email.val() == undefined) {
		email.attr('style', 'border:solid 1px red;');
		email.focus();
		return false;
	}
	email.removeAttr('style');
	if (password.val() == '' || password.val() == undefined) {
		password.attr('style', 'border:solid 1px red;');
		password.focus();
		return false;
	}
	password.removeAttr('style');
	jQuery.ajax({
		type: "POST",
		dataType: 'json',
		//contentType: "application/json; charset=utf-8",
		url: '/Account/VerifyLogin/',
		data: { 'Email': email.val(), 'Password': password.val(), 'RememberMe': jQuery('#RememberMe').val() },
		async: false,
		success: function (response) {
			if (response.success)
				window.location = response.data;
			else {
				jQuery('#login-error').text(response.statusMessage).fadeOut(10000);;
			}
			jQuery("#exampleModal").modal('show');
		},
		error: function (response) {
			console.log(response.responseText);
		},
		failure: function (response) {
			console.log(response.responseText);
		}
	});
	return false;
}