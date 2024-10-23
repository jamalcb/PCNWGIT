
$(document).ready(function () {
    BindSpecialMsg(0);
    // Get the current URL
    var currentUrl = window.location.href;

    // Remove the 'returnUrl' parameter from the URL
    var updatedUrl = currentUrl.split('?')[0];

    // Update the URL without the 'returnUrl' parameter
    window.history.replaceState({}, document.title, updatedUrl);
});
$(function () {
    $(".datepicker").datepicker();
});
function createDataTables() {
    setTimeout(function () {
        $('.pnTables').DataTable();
    }, 1000);
}
function reinitialLizedDataTables() {
    //$('.pnTables').dataTable().fnClearTable();
    // $('.pnTables').dataTable().fnDestroy();
}
function OpenPrintModel() {
    $("#prev-btn").click();
    $("#frmPrintForm").trigger('reset');
    $('#frmPrintForm').find("input[type=text], textarea").val('');
    $('span.loginError').html('');
    $(".m-modal").modal('show');
    $('.mValue').val('AM');
}
var formattedDateStart = "";
var formattedDateend = "";
var endHour = "";
var endMinute = "";
var endAMPM = "";
var startHour = "";
var startMinute = "";
var startAMPM = "";

function BindSpecialMsg(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetSpecialMsgList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            $.each(response.data, function (index, item) {
                // Convert StartDate
                var startDate = new Date(item.StartDate);
                var formattedStartDate = startDate.toLocaleString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true, hourCycle: 'h12' });

                // Convert EndDate
                var endDate = new Date(item.EndDate);
                var formattedEndDate = endDate.toLocaleString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true, hourCycle: 'h12' });

                // Update the item properties
                item.StartDate = formattedStartDate;
                item.EndDate = formattedEndDate;

                // Build the HTML row
                rows += '<tr><td>' + item.Type + '</td><td style="text-transform: uppercase;">' + item.StartDate + '</td><td style="text-transform: uppercase;">' + item.EndDate + '</td><td>' + item.SpMessage + '</td><td class="">' + (item.IsActive == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditSpecialMsg(this, ' + item.Id + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span>&nbsp;<span title="Delete" class="btn btn-danger icon-del" onclick="DeleteSpecialMsg(' + item.Id + ')" ><i class="fa fa-trash " aria-hidden="true"></i></span></td></tr>';
            });


            $('#tblSpecialMsg').html(rows);
            createDataTables();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}
function SaveSpecialMsg() {

    var ddlType = $('#ddlType').val();
    $('#ddlType').next('span').html('');
    if (ddlType == undefined || ddlType == '' || ddlType == null) {
        $('#ddlType').next('span').html('Please Select a Type.');
        return false;
    }
    var startDate = $('#StartDate').val();
    $('#StartDate').next('span').html('');
    if (startDate == undefined || startDate == '' || startDate == null) {
        $('#StartDate').next('span').html('Please enter your start date.');
        return false;
    }
    var endDate = $('#EndDate').val();
    $('#EndDate').next('span').html('');
    if (endDate == undefined || endDate == '' || endDate == null) {
        $('#EndDate').next('span').html('Please enter your end date');
        return false;
    }
    var spMessage = $('#SpMessage').val();
    $('#SpMessage').next('span').html('');
    if (spMessage == undefined || spMessage == '' || spMessage == null) {
        $('#SpMessage').next('span').html('Please enter your message');
        return false;
    }


    var model = {};
    $('#message').val('');
    model.Id = $('#Id').val();
    model.Type = $('#ddlType').find(":selected").val();
    var starttComp = $('#tComp').val();
    var starthComp = $('#hComp').val();
    var startmValue = $('#mValue').val();
    var startDateValue = $('#StartDate').val();
    //var dateArray = startDateValue.split('/');
    //var formattedDate = dateArray[1] + '/' + dateArray[0] + '/' + dateArray[2];
    model.StartDate = startDateValue + " " + starttComp + ":" + starthComp + ":00 " + startmValue;

    var endtComp = $('#tCompend').val();
    var endhComp = $('#hCompend').val();
    var endmValue = $('#mValueend').val();
    var endDateValue = $('#EndDate').val();
    //dateArray = endDateValue.split('/');
    //formattedDate = dateArray[1] + '/' + dateArray[0] + '/' + dateArray[2];
    model.EndDate = endDateValue + " " + endtComp + ":" + endhComp + ":00 " + endmValue;

    model.SpMessage = $('#SpMessage').val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;

    if (model.StartDate && model.EndDate) {
        var startDate = new Date(moment($('#StartDate').val(), 'MM/DD/YYYY').format('MM/DD/YYYY') + " " + $('#tComp').val() + ":" + $('#hComp').val() + ":00 " + $('#mValue').val());
        var endDate = new Date(moment($('#EndDate').val(), 'MM/DD/YYYY').format('MM/DD/YYYY') + " " + $('#tCompend').val() + ":" + $('#hCompend').val() + ":00 " + $('#mValueend').val());


        if (endDate < startDate) {
            alert("Start Time should be less than End Date.");
            $('#tCompend').val("00");
            $('#hCompend').val("00");
            $('#mValueend').val("AM");
            return false;
        }
    }

    
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveSpecialMsg/',
        data: { 'model': model },
        beforeSend: function () {
            $("#loader-overlay").show();
        },
        success: function (response) {
            if (response.success) {
                debugger;
                console.log(response);
                BindSpecialMsg(1);
                alert(response.statusMessage);
                window.location.reload();
            }
            else {
                alert(response.statusMessage);
                $("#loader-overlay").hide();
                return false;
            }
        },
        error: function (response) {
            alert(response.responseText);
            $("#loader-overlay").hide();
        },
        failure: function (response) {
            alert(response.responseText);
            $("#loader-overlay").hide();
        },
        complete: function () {
            
        }
    });
}
function EditSpecialMsg(Ctrl, Id) {
    OpenPrintModel();
    $('#Id').val(Id);
    var rows = $(Ctrl).parent('td').parent('tr');
    var type = rows.find('td:eq(0)').text();
    $("#ddlType").val(type);
    var start_date = rows.find('td:eq(1)').text();
    var startdateTimeParts = start_date.split(', '); // Splitting date and time parts

    // Extracting time parts
    var timeParts = startdateTimeParts[1].split(':');
    var starthour = timeParts[0];
    var startminute = timeParts[1].split(' ')[0];

    // Extracting AM/PM
    var startampm = timeParts[1].split(' ')[1].toUpperCase();;
    // Set values in their respective fields
    if (startdateTimeParts[0]) {
        $('#StartDate').val(startdateTimeParts[0]);
    }
    if (starthour) {
        $('#tComp').val(starthour);
    }
    if (startminute) {
        $('#hComp').val(startminute);
    }
    if (startampm) {
        $('#mValue').val(startampm);
    }


    var end_date = rows.find('td:eq(2)').text();
    var enddateTimeParts = end_date.split(', '); // Splitting date and time parts

    // Extracting time parts
    var timeParts = enddateTimeParts[1].split(':');
    var endhour = timeParts[0];
    var endminute = timeParts[1].split(' ')[0];

    // Extracting AM/PM
    var endampm = timeParts[1].split(' ')[1].toUpperCase();
    if (enddateTimeParts[0]) {
        $('#EndDate').val(enddateTimeParts[0]);
    }
    if (endhour) {
        $('#tCompend').val(endhour);
    }
    if (endminute) {
        $('#hCompend').val(endminute);
    }
    if (endampm) {
        $('#mValueend').val(endampm);
    }

    $('#SpMessage').val(rows.find('td:eq(3)').text());


    var activeText = rows.find('td:eq(4)').text();
    if (activeText == "Active")
        $('#IsActive').prop('checked', true);
    else
        $('#IsActive').prop('checked', false);
}
function DeleteSpecialMsg(Id) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/GlobalMaster/DeleteSpecialMsg/',
            data: { 'Id': Id },
            async: false,
            success: function (response) {
                console.log(response);
                BindSpecialMsg(1);
                alert(response.statusMessage);
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
function datevalidate() {
    var startDate = document.getElementById("StartDate").value;
    var currentDate = new Date();
    var month = currentDate.getMonth() + 1;
    if (month < 10) month = "0" + month;
    var dateOfMonth = currentDate.getDate();
    if (dateOfMonth < 10) dateOfMonth = "0" + dateOfMonth;
    var year = currentDate.getFullYear();
    var formattedDate = month + "/" + dateOfMonth + "/" + year;
    if (startDate < formattedDate) {
        alert("The Date must be Bigger or Equal to today date")
        document.getElementById("StartDate").value = "";
        return false;
    }
    return true;
}
function enddatevalidate() {
    var startDateString = document.getElementById("StartDate").value;
    var endDateString = document.getElementById("EndDate").value;

    // Convert date strings to Date objects
    var startDate = new Date(startDateString);
    var endDate = new Date(endDateString);

    // Check if endDate is greater than or equal to startDate
    if (startDate > endDate) {
        alert("The end date must be equal to or greater than the start date");
        document.getElementById("EndDate").value = "";
        return false;
    }

    return true;
}

var suggestions = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"];
var Tsuggestions = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"];
var Msuggestions = ["AM", "PM"];
//$('#mValue').val('AM');
var inputField = $('#hComp');
function autoCompleteTimer() {
    // Autocomplete for tComp input
    $('[id *= tComp]').autocomplete({
        source: function (request, response) {
            response(Tsuggestions);
        },
        minLength: 0,
        change: function (event, ui) {
            var value = parseInt(this.value);
            if (value < 1 || value > 12) {
                this.value = '00';
            }
            else if (value < 10) {
                this.value = '0' + value;
            }
        },
        open: function (event, ui) {
            $(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            $(this).autocomplete("widget").removeClass("show-all");
        },
    }).on("focus", function () {
        $(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.DOWN) {
            event.preventDefault();
            $(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            $(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
        }
    }).on("input", function () {
        var value = $(this).val();
        if (value.length > 2) {
            $(this).val(value.slice(0, 2));
        }
    });
    $('[id *= hComp]').autocomplete({
        source: function (request, response) {
            response(suggestions);
        },
        minLength: 0,
        open: function (event, ui) {
            $(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            $(this).autocomplete("widget").removeClass("show-all");
        },
        change: function (event, ui) {
            var value = parseInt(this.value);

            if (isNaN(value)) {
                this.value = '00';
            }
        }
    }).on("focus", function () {
        $(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.DOWN) {
            event.preventDefault();
            $(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            $(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
            //  $.ui.keyCode.ba
        }
    }).on("keyup", function (event) {
        var value = $(this).val();
        if (value.length > 2) {
            $(this).val(value.slice(0, 2));
        }
        var inputKeyCode = event.keyCode;
        value = parseInt(this.value);


    });

    $('[id *= mValue]').autocomplete({

        source: function (request, response) {
            response(Msuggestions);
        },
        minLength: 0,
        open: function (event, ui) {
            $(this).autocomplete("widget").addClass("show-all");
        },
        close: function (event, ui) {
            $(this).autocomplete("widget").removeClass("show-all");
        },
        change: function (event, ui) {
            var value = this.value.toUpperCase();
            if (value == 'A') {
                this.value = 'AM';
            }
            else if (value == 'P') {
                this.value = 'PM';
            }

            else if (value !== 'AM' && value !== 'PM') {
                this.value = 'AM';
            }
        }
    }).on("focus", function () {
        $(this).autocomplete("search", "");
    }).on("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.DOWN) {
            event.preventDefault();
            $(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            $(this).autocomplete("widget").find(".ui-state-focus").removeClass("ui-state-focus");
            $(this).autocomplete("widget").find(".ui-menu-item:first").addClass("ui-state-focus");
        }
    }).on("input", function () {
        var value = $(this).val();
        if (value.length > 2) {
            $(this).val(value.slice(0, 2));
        }
    });
}

$(function () {
    autoCompleteTimer()
    $('[id *= mValue]').val('AM');
});

$(function () {
    $(".datepicker-custom").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    });
});
$('.datepicker-custom').change(function () {
    $(this).focus();
});
function ReInitializeDatePicker() {
    $(".datepicker-custom").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    });
};
$(function () {
    $(".datepicker-custom-Ar").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy'
    }).datepicker('setDate', 'today');
});
function DeadLineDateTime(id) {

    var chk = validateDateControl(id);
    if (chk != false) {
        var dateStr = document.getElementById(id).value;
        const currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);
        const [month, day, year] = dateStr.split("/");
        const dateObject = new Date(year, month - 1, day); // Month is 0-based in JavaScript Date constructor

        if (dateObject <= currentDate) {
            alert("The Date must be greater to today's date")
            document.getElementById(id).value = "";
            $('#' + id).focus();
            return false;
        }
        var pub = $('#Publish').val();
        if (pub == 'True' || pub == 'true') {
            $('#BidDt2').val($('#HiddenBidDt').val())
            $('#strBidDt4').val($('#hdnLastBidDt').val());
            $('#strBidDt2').val($('#strBidDt2').val());
            $('#divPrevBD').css('display', 'block');
        }
    }
};
function validateDateControl(id) {
    var dateStrr = document.getElementById(id).value;
    const inputElement = document.getElementById(id);
    const dateRegex = /^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])\/(?:\d{2}|\d{4})?$|^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])$/;
    const isValid = dateRegex.test(dateStrr);
    // /^(0?[1-9]|1[0-2])\/(0?[1-9]|1\d|2\d|3[01])\/(?:\d{2}|\d{4})?$/

    if (dateStrr != '') {
        if (!isValid) {
            alert("Invalid date/date format. Please use MM/dd/ or MM/dd/yyyy format or check the month date value.");
            document.getElementById(id).value = "";
            $('#' + id).focus();
            return false;
        }
        else {
            var parts = dateStrr.split("/");

            var formattedValue = (parts[0].length === 1 ? "0" + parts[0] : parts[0]) + "/" +
                (parts[1].length === 1 ? "0" + parts[1] : parts[1]) + "/";
            if (parts[2]) {
                if (parts[2].length === 2) {
                    if (parts[2] == "00") {
                        alert('Year component cannot be zero')
                        $('#' + id).focus();
                        document.getElementById(id).value = "";
                        return false;
                    }
                    else { formattedValue += "20" + parts[2]; }

                } else {
                    formattedValue += parts[2];
                }
            }
            else {
                var currentYear = new Date().getFullYear();
                var currentYearLastTwoDigits = currentYear.toString();

                if (parts[0] <= new Date().getMonth() + 1) {
                    if (parts[0] == new Date().getMonth() + 1) {
                        if (parts[1] < new Date().getDate())
                            currentYearLastTwoDigits = (currentYear + 1).toString();
                    }
                    else {
                        currentYearLastTwoDigits = (currentYear + 1).toString();
                    }
                }

                formattedValue += currentYearLastTwoDigits;
            }
            document.getElementById(id).value = formattedValue;
            dateStr = formattedValue;
        }
        var validArr = dateStr.split('/');
        var monthPart = parseInt(validArr[0]);
        var dayPart = parseInt(validArr[1]);
        var yearPart = parseInt(validArr[2]);
        if (monthPart == 4 || monthPart == 9 || monthPart == 11 || monthPart == 6) {
            if (dayPart > 30) {
                alert('You can not enter date more than 30 for month of April, June, September and November');
                $('#' + id).focus();
                document.getElementById(id).value = "";
                return false;
            }
        }
        else if (monthPart == 1 || monthPart == 3 || monthPart == 5 || monthPart == 7 || monthPart == 8 || monthPart == 10 || monthPart == 12) {
            if (dayPart > 31) {
                alert('You can not enter date more than 31 for month of January, March, May, July, August, October and December');
                $('#' + id).focus();
                document.getElementById(id).value = "";
                return false;
            }
        }
        else if (monthPart == 2) {
            const leap = checkLeapYear(yearPart);
            if (leap) {
                if (dayPart > 29) {
                    alert('You can not enter date more than 29 for month of February.');
                    $('#' + id).focus();
                    document.getElementById(id).value = "";
                    return false;
                }
            }
            else {
                if (dayPart > 28) {
                    alert('You can not enter date more than 28 for month of February.');
                    $('#' + id).focus();
                    document.getElementById(id).value = "";
                    return false;
                }
            }

            //if ((year % 4 == 0) && (year % 100 != 0) || (year % 400 == 0)) {
            //    if (dayPart > 29) {
            //        alert('You can not enter date more than 29 for month of February.');
            //    }
            //}
        }
        else {
            alert('Date is not correct Please check date.');
            $('#' + id).focus();
            document.getElementById(id).value = "";
            return false;
        }
    }
};