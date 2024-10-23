/// <reference path="../lib/jquery/dist/jquery.js" />
$(document).ready(function () {
    var wrapper = document.getElementById("btnFree");
    $.ajax({
        type: 'GET',
        url: "/GlobalMaster/GetTabData",
        success: function (response) {
            if (response.data.SetTab) {
                wrapper.textContent = "Hide Free Tab"
            }
            else
            {
                wrapper.textContent = "Show Free Tab"
            }
        }
    });
});

function SetFreeTab() {
    var wrapper = document.getElementById("btnFree");
    var SetTab = true;
    if (wrapper.textContent == "Hide Free Tab") {
        SetTab = false;
    }
    $.ajax({
        type: 'POST',
        url: "/GlobalMaster/SetTabData",
        data: { "SetTab": SetTab},
        success: function (response) {
            if (response.success) {
                alert("Value updated successfully");
                location.reload();
            }
            else {
                alert("Something went wrong");
            }
        }
    });
}