// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
var fileCount = 0;
function FileValue() {
	jQuery("#BtnUpload").removeClass("disabled");
	jQuery("#progress").css("display", "none");
	jQuery("#processText").html('');
}
async function uploadFiles() {

	var allFiles = [];
	var progressContainer = document.getElementById("progress-container");
	var progressBar = document.getElementById("progress-bar");
	var progressText = document.getElementById("progress-text");
	var filename = document.getElementById("filenameid");
	var ProjNum = jQuery("#ProjNumber").val();
	jQuery("#processText").html('');
	var input = document.getElementById("fileUpload1");
	var files = jQuery("#fileUpload1").get(0).files;
	fileCount = files.length;
	if (files.length > 0) {
		var files = input.files;
		if (files.length > 0) {
			allFiles = allFiles.concat(Array.from(files));
		}

		jQuery("#BtnUpload").prop("disabled", true);
		for (i = 0; i < allFiles.length; i++) {
			var name = allFiles[i].name;
			var formData = new FormData();
			formData.append("files", allFiles[i]);
			formData.append("ProjNum", ProjNum);
			formData.append("firstfile", i);
			//startUpdatingProgressIndicator();
			progressContainer.style.display = "block";
			progressBar.style.width = "0%";
			progressText.textContent = "0%";
			filename.innerText = "";
			await new Promise((resolve, reject) => {
				jQuery.ajax(
					{
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
									if (name) {
										filename.innerText = "Uploading (" + (i+1) + "/" + allFiles.length + ")  File:" + name;
									}
								}
							}, false);

							return xhr;
						},
						success: function (data) {
							if (data.success == true) {
								resolve();
								//stopUpdatingProgressIndicator();
								jQuery('#hdnPath').val(data.data);
								jQuery('#SucStatus').val(data.success);
							}

							setTimeout(function () {
								var progress = 100;
								jQuery("#bar").css({ width: progress + "%" });
								jQuery("#label").html(progress + "%");
								//alert("Files Uploaded!"); 
								jQuery("#BtnUpload").css('display', 'none');
								jQuery("#Uploadfiles").css('display', 'block');
								//jQuery("#processText").html("All files uploaded!");
							}, 1000);
						}
					}
				);
			});
		}
	}
	else {
		alert('Please select file(s) to upload');
	}

}

var intervalId;

function startUpdatingProgressIndicator() {
	jQuery("#progress").show();
	jQuery("#FileText").html('0 of ' + fileCount + ' files uploaded');
	var pCompleted = 0;
	intervalId = setInterval(
		function () {
			// We use the POST requests here to avoid caching problems (we could use the GET requests and disable the cache instead)
			jQuery.post(
				"/Home/progress",
				function (progress) {
					//console.log('********************************');
					//console.log('progress : ' + progress);
					//console.log('pCompleted : ' + pCompleted);

					console.log(progress);
					pCompleted = parseFloat(progress.progress);
					if (parseFloat(progress.progress) == 0) {
						progress.progress = parseFloat(progress.progress);
						progress.progress = progress.progress + parseFloat(pCompleted);
					}
					jQuery("#bar").css({ width: progress.progress + "%" });
					jQuery("#label").html(progress.progress + "%");
					jQuery("#FileText").html(progress.Count + ' of ' + fileCount + ' files uploaded');
					//console.log('progress : ' + progress);
					// console.log('pCompleted : ' + pCompleted);
				}
			);
		},
		10
	);
}

function stopUpdatingProgressIndicator() {
	clearInterval(intervalId);
}

