/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/lodash.js/lodash.js" />

//const jquery = require("../lib/jquery/dist/jquery");

//var jq = $.noConflict();
var model = {};
const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',

    // These options are needed to round to whole numbers if that's what you want.
    //minimumFractionDigits: 0, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
    //maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
});
function ShowPayment() {
    GetPacificCardDetails();
    jQuery("#membershipModal").modal('show');
    jQuery.ajax({
        type: 'GET',
        url: "/Member/FindPriceDetail",
        success: function (data) {
            model = data;
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcPremium').text(memshipInfo[0].YearlyPrice);
            jQuery('#hdnDiscountId').val(memshipInfo[0].DiscountId);
            jQuery('#AddlPrice').val(memshipInfo[0].AddYearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#RadioExValue').val('Yearly');
            jQuery('input[id = MemberType]').val('7');
        }
    });
    // e.preventDefault();
}
function GoToReg(ctrl) {
    var id = jQuery('#hdnPlanId').val();
    var example = jQuery('input[name = example]:checked').val();
    jQuery('#hdnDiscId').val(jQuery('#hdnDiscountId').val());
    jQuery('#hdnTerm').val(example);
    jQuery('#Paid').text(example);
    if (id == 'btnPacific') {
        var Cost = jQuery('#MemberCost').val();
        var data = jQuery('input[name = cardPacific]:checked').val();
        if (data == 'Pacific Northwest Premium') {
            /*jQuery('#pkgMem').text(data);*/
            jQuery('#chckvalue').val(data);
            /*jQuery('#PayPlan').text(jQuery('#MemberCost').val());*/
            jQuery("#membershipModal").modal('hide');
            jQuery("#MSPChk").val('Y');
            if (example == "Yearly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost)) + "/year paid Annually";
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Monthly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 12)) + "/year paid with 12 Monthly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Quarterly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 4)) + "/year paid with 4 Quarterly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            jQuery('#ColorString').val('#FF9900');
            jQuery('#pkgMem').css('color', '#FF9900');
        }
        else {
            jQuery('#errortext').html('You need to select a plan for pacific');
            return false
        }
    }
    else if (id == 'btnOreg') {
        var data = jQuery('input[name = cardOreg]:checked').val();
        var Cost = jQuery('#MemberCost').val();
        if (data == 'Oregon Premium' || data == 'Oregon Plus' || data == 'Oregon Only') {
            if (example == "Yearly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost)) + "/year paid Annually";
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Monthly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 12)) + "/year paid with 12 Monthly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Quarterly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 4)) + "/year paid with 4 Quarterly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            jQuery('#ColorString').val('#6d9d78');
            jQuery('#pkgMem').css('color', '#6d9d78');
            jQuery("#membershipModal").modal('hide');
            jQuery("#MSPChk").val('Y');
        }
        else {
            jQuery('#errortext').html('You need to select a plan for oregon');
            return false
        }
    }
    else if (id == 'btnWash') {
        var Cost = jQuery('#MemberCost').val();
        var data = jQuery('input[name = cardWash]:checked').val();
        if (data == 'Washington Premium' || data == 'Washington Plus' || data == 'Washington Only') {
            //jQuery('#pkgMem').text(data);
            //jQuery('#PayPlan').text(jQuery('#MemberCost').val());
            if (example == "Yearly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost)) + "/year paid Annually";
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Monthly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 12)) + "/year paid with 12 Monthly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            else if (example == "Quarterly") {
                var pkgMemText = data + " - " + formatter.format(parseFloat(Cost * 4)) + "/year paid with 4 Quarterly payments of $" + Cost;
                jQuery('#pkgMem').text(pkgMemText);
                jQuery('#PayCardString').val(pkgMemText);
            }
            jQuery('#ColorString').val('#4c829a');
            jQuery('#pkgMem').css('color', '#4c829a');
            jQuery("#membershipModal").modal('hide');
            jQuery("#MSPChk").val('Y');
        }
        else {
            jQuery('#errortext').html('You need to select a plan for washington');
            return false
        }
    }
    else if (id == 'btnFree') {
        var data = jQuery('input[name = cardFree]:checked').val();
        if (data == 'Free') {
            //jQuery('#pkgMem').text(data);
            //jQuery('#PayPlan').text(jQuery('#MemberCost').val());

            var pkgMemText = "Project Posting Only - Free";
            jQuery('#pkgMem').text(pkgMemText);
            jQuery('#PayCardString').val(pkgMemText);
            jQuery('#ColorString').val('#626c69');
            jQuery('#pkgMem').css('color', '#626c69');
            jQuery("#membershipModal").modal('hide');
            jQuery("#MSPChk").val('Y');
        }
        else {
            jQuery('#errortext').html('You need to select free job posting');
            return false;
        }
    }
    else {
        jQuery('#errortext').html('You need to select a valid plan');
        return false;
    }
    jQuery('#chgPackage').prev('span').html('');
    if (id == 'btnFree')
        jQuery('.BillInfoClass').css('display', 'none');
    else
        jQuery('.BillInfoClass').css('display', 'block');


    jQuery('#errortext').html('');

    chnageStep('MemDet', 1);
    jQuery('#Company').focus();
}

jQuery('.card input[type="radio"]').change(onCheckboxChange);
var atLeastOneChecked = false;
function onCheckboxChange() {
    jQuery('#pkgMem').text('');
    //jQuery('.card').each(function () {
    //    if (jQuery(this).find('input[type="radio"]:checked').length > 0) {
    //        atLeastOneChecked = true;
    //        return false; 
    //    }
    //});    
}
jQuery('input[name = example]').change(function () {
    var data = jQuery('input[name = example]:checked').val();
    jQuery('input[name = RadioExValue]').val(data);
    var dataPacific = jQuery('input[name = cardPacific]:checked').val();
    var dataOreg = jQuery('input[name = cardOreg]:checked').val();
    var dataWash = jQuery('input[name = cardWash]:checked').val();
    var dataFree = jQuery('input[name = cardFree]:checked').val();
    //jQuery('#AddlPrice').val('');
    var cost;
    var addlPrice = 0;
    if (dataFree == 'Free') {
        cost = '0';
        addlPrice = 0;
    }
    else {
        if (data == 'Yearly') {
            var memshipInfo = _.map(model, function (o) {

                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcPremium').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            if (dataPacific == 'Pacific Northwest Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataPacific) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcPremium').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcPremium').text();

            }
            else if (dataOreg == 'Oregon Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcOregon').text();
            }
            else if (dataOreg == 'Oregon Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#prcOregon').text();
            }
            else if (dataOreg == 'Oregon Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#prcOregon').text();
            }

            else if (dataWash == 'Washington Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcWash').text();
            }
            else if (dataWash == 'Washington Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcWash').text();
            }
            else if (dataWash == 'Washington Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').text(PaymentInfo[0].YearlyPrice);
                addlPrice = PaymentInfo[0].AddYearlyPrice;
                cost = jQuery('#YprcWash').text();
            }
            //else {
            //    jQuery('#prcOregon').html('$1900/Year');
            //    jQuery('#prcOregon').html('$1500/Year');
            //    jQuery('#prcWashington').html('$1500/Year');
            //}

        }
        else if (data == 'Monthly') {
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#YprcPremium').html(MonthlyInfo[0].MonthlyPrice * 12);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            if (dataPacific == 'Pacific Northwest Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataPacific) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + PaymentInfo[0].MonthlyPrice + '</span>');
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#YprcPremium').html(PaymentInfo[0].MonthlyPrice * 12);
                cost = jQuery('#spnPremium').text();
            }
            if (dataOreg == 'Oregon Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }
            else if (dataOreg == 'Oregon Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }
            else if (dataOreg == 'Oregon Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }

            else if (dataWash == 'Washington Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            else if (dataWash == 'Washington Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            else if (dataWash == 'Washington Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].MonthlyPrice * 12);
                addlPrice = PaymentInfo[0].AddMonthlyPrice;
                jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + PaymentInfo[0].MonthlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            //else {
            //    jQuery('#prcOregon').html('$200/Month');
            //    jQuery('#prcWashington').html('$200/Month');
            //}
        }
        else if (data == 'Quarterly') {
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#YprcPremium').html(QInfo[0].QuarterlyPrice * 4);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfo[0].QuarterlyPrice + '</span>');
            //addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            //addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            //addlPrice = QInfo[0].AddQuarterlyPrice;
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            if (dataPacific == 'Pacific Northwest Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataPacific) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                jQuery('#YprcPremium').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                cost = jQuery('#spnPremium').text();
            }
            if (dataOreg == 'Oregon Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }
            else if (dataOreg == 'Oregon Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }
            else if (dataOreg == 'Oregon Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataOreg) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcOregon').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnOreg').text();
            }

            else if (dataWash == 'Washington Premium') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            else if (dataWash == 'Washington Plus') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }
            else if (dataWash == 'Washington Only') {
                var PaymentInfo = _.map(model, function (o) {
                    if (o.SubMemberShipPlanName == dataWash) return o;
                });
                PaymentInfo = _.without(PaymentInfo, undefined);
                jQuery('#YprcWash').html(PaymentInfo[0].QuarterlyPrice * 4);
                addlPrice = PaymentInfo[0].AddQuarterlyPrice;
                jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + PaymentInfo[0].QuarterlyPrice + '</span>');
                cost = jQuery('#spnWash').text();
            }


        }
    }
    jQuery('input[id = MemberCost]').val(cost);
    jQuery('input[id = AddlPrice]').val(addlPrice);
    if (dataFree == 'Free') {
        jQuery('input[id = hdnTerm]').val('Free Member');
    }
    else {
        jQuery('input[id = hdnTerm]').val(data);
    }
});

jQuery('input[name = cardPacific]').change(function () {
    var dataEx = jQuery('input[name = example]:checked').val();
    var data = jQuery('input[name = cardPacific]:checked').val();
    jQuery('#hdnPlanId').val('btnPacific');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(data);
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    // jQuery('#btnFree').css('display','none');
    // jQuery('#btnPay').css('display','block');
    var addlPrice = 0;
    if (dataEx == 'Yearly') {
        var memshipInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
        });
        memshipInfoCost = _.without(memshipInfoCost, undefined);
        jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
        addlPrice = memshipInfoCost[0].AddYearlyPrice;
        var memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
        addlPrice = memshipInfoCost[0].AddYearlyPrice;
        memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Washington Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
        addlPrice = memshipInfoCost[0].AddYearlyPrice;
        jQuery('#prcPremium').html('');
        jQuery('#prcOregon').html('');
        jQuery('#prcWashington').html('');
        jQuery('input[id = MemberCost]').val(memshipInfoCost[0].YearlyPrice);
    }
    else if (dataEx == 'Monthly') {
        var MonthlyInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
        jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
        jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
        addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
        var MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
        addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
        MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
        addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
        jQuery('input[id = MemberCost]').val(MonthlyInfoCost[0].MonthlyPrice);
    }
    else if (dataEx == 'Quarterly') {
        var QInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        QInfoCost = _.without(QInfoCost, undefined);
        jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
        jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
        addlPrice = QInfoCost[0].AddQuarterlyPrice;
        var QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
        addlPrice = QInfoCost[0].AddQuarterlyPrice;
        QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
        addlPrice = QInfoCost[0].AddQuarterlyPrice;
        jQuery('input[id = MemberCost]').val(QInfoCost[0].QuarterlyPrice);
    }
    jQuery('input[id = MemberType]').val('7');
    jQuery('input[id = AddlPrice]').val(addlPrice);
});

jQuery('input[name = cardOreg]').change(function () {
    var data = jQuery('input[name = example]:checked').val();
    var dataOreg = jQuery('input[name = cardOreg]:checked').val();
    jQuery('#hdnPlanId').val('btnOreg');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(dataOreg);
    var cost = jQuery('#prcOregon').text();
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    //jQuery('#btnFree').css('display', 'none');
    //jQuery('#btnPay').css('display','block');
    var addlPrice = 0;
    if (dataOreg == 'Oregon Premium') {
        jQuery('[id^=Popfaq]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=PopfaqWash]').addClass('show');
        jQuery('[id=Popfaq]').addClass('show');
        jQuery('input[id = MemberType]').val('9')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            // addlPrice = memshipInfoCost[0].AddYearlyPrice;
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfoCost[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
            cost = jQuery('#spnOreg').text();

        }

        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            cost = jQuery('#spnOreg').text();

        }
    }

    if (dataOreg == 'Oregon Only') {
        jQuery('[id^=Popfaq]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=PopfaqWash]').addClass('show');
        jQuery('[id=Popfaq]').addClass('show');
        jQuery('input[id = MemberType]').val('4')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            //addlPrice = memshipInfoCost[0].AddYearlyPrice;
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            //addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnOreg').text();

        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            //addlPrice = QInfo[0].AddQuarterlyPrice;
            cost = jQuery('#spnOreg').text();

        }
    }

    else if (dataOreg == 'Oregon Plus') {
        jQuery('[id^=Popfaq]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=PopfaqWash]').addClass('show');
        jQuery('[id=Popfaq]').addClass('show');
        jQuery('input[id = MemberType]').val('5')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            //addlPrice = memshipInfoCost[0].AddYearlyPrice;
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            // addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfoCost[0].AddMonthlyPrice;
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            //addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnOreg').text();

        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            //addlPrice = QInfoCost[0].AddQuarterlyPrice;
            cost = jQuery('#spnOreg').text();

        }
    }

    jQuery('input[id = MemberCost]').val(cost);
    jQuery('input[id = AddlPrice]').val(addlPrice);
});

jQuery('input[name = cardWash]').change(function () {
    var data = jQuery('input[name = example]:checked').val();
    var dataWash = jQuery('input[name = cardWash]:checked').val();
    jQuery('#hdnPlanId').val('btnWash');
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(dataWash);
    var cost = jQuery('#prcWashington').text();
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    // jQuery('#btnFree').css('display','none');
    var addlPrice = 0;
    if (dataWash == 'Washington Premium') {
        jQuery('[id^=Popfaq6]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=Popfaq1]').addClass('show');
        jQuery('input[id = MemberType]').val('10')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnWash').text();
        }

        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            cost = jQuery('#spnWash').text();
        }
    }

    else if (dataWash == 'Washington Only') {
        jQuery('[id^=Popfaq6]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=Popfaq1]').addClass('show');
        jQuery('input[id = MemberType]').val('8')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnWash').text();

        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            cost = jQuery('#spnWash').text();

        }
    }

    else if (dataWash == 'Washington Plus') {
        jQuery('[id^=Popfaq6]').removeClass('show');
        jQuery('[id=Popfaq7]').addClass('show');
        jQuery('[id=Popfaq8]').addClass('show');
        jQuery('[id=Popfaq1]').addClass('show');
        jQuery('input[id = MemberType]').val('11')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            addlPrice = memshipInfo[0].AddYearlyPrice;
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            addlPrice = MonthlyInfo[0].AddMonthlyPrice;
            cost = jQuery('#spnWash').text();

        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            addlPrice = QInfo[0].AddQuarterlyPrice;
            cost = jQuery('#spnWash').text();

        }
    }
    jQuery('input[id = MemberCost]').val(cost);
    jQuery('input[id = AddlPrice]').val(addlPrice);
});

jQuery('input[name = cardFree]').change(function () {
    var data = jQuery('input[name = cardFree]:checked').val();
    var dataEx = jQuery('input[name = example]:checked').val();
    jQuery('#hdnPlanId').val('btnFree');
    jQuery('input[id = MemberType]').val('1')
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(data);
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    var addlPrice = 0;
    if (dataEx == 'Yearly') {
        var memshipInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
        });
        memshipInfoCost = _.without(memshipInfoCost, undefined);
        jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
        var memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
        memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Washington Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
        jQuery('#prcPremium').html('');
        jQuery('#prcOregon').html('');
        jQuery('#prcWashington').html('');
    }
    else if (dataEx == 'Monthly') {
        var MonthlyInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
        jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
        jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
        var MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
        MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
    }
    else if (dataEx == 'Quarterly') {
        var QInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        QInfoCost = _.without(QInfoCost, undefined);
        jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
        jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
        var QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
        QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
    } jQuery('input[id = MemberCost]').val('0');
    // jQuery('#btnPay').css('display','none');
    jQuery('input[id = AddlPrice]').val(addlPrice);
});

function changePacific(dataEx, data) {
    //var dataEx = jQuery('input[name = example]:checked').val();
    //var data = jQuery('input[name = cardPacific]:checked').val();
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(data);
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    // jQuery('#btnFree').css('display','none');
    // jQuery('#btnPay').css('display','block');
    if (dataEx == 'Yearly') {
        var memshipInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
        });
        memshipInfoCost = _.without(memshipInfoCost, undefined);
        jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
        var memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
        memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Washington Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
        jQuery('#prcPremium').html('');
        jQuery('#prcOregon').html('');
        jQuery('#prcWashington').html('');
        jQuery('input[id = MemberCost]').val(memshipInfoCost[0].YearlyPrice);
    }
    else if (dataEx == 'Monthly') {
        var MonthlyInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
        jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
        jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
        var MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
        MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
        jQuery('input[id = MemberCost]').val(MonthlyInfoCost[0].MonthlyPrice);
    }
    else if (dataEx == 'Quarterly') {
        var QInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        QInfoCost = _.without(QInfoCost, undefined);
        jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
        jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
        var QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
        QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
        jQuery('input[id = MemberCost]').val(QInfoCost[0].QuarterlyPrice);
    }
    jQuery('input[id = MemberType]').val('7')
}
function changeOreg(data, dataOreg) {
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(dataOreg);
    var cost = jQuery('#prcOregon').text();
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    //jQuery('#btnFree').css('display', 'none');
    //jQuery('#btnPay').css('display','block');
    if (dataOreg == 'Oregon Premium') {
        jQuery('input[id = MemberType]').val('9')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnOreg').text();

        }

        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnOreg').text();

        }
    }

    if (dataOreg == 'Oregon Only') {
        jQuery('input[id = MemberType]').val('4')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnOreg').text();

        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnOreg').text();

        }
    }

    else if (dataOreg == 'Oregon Plus') {
        jQuery('input[id = MemberType]').val('5')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcOregon').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnOreg').text();

        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataOreg) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnOreg').text();

        }
    }

    jQuery('input[id = MemberCost]').val(cost);
}
function changeWash(data, dataWash) {
    //var data = jQuery('input[name = example]:checked').val();
    //var dataWash = jQuery('input[name = cardWash]:checked').val();
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(dataWash);
    var cost = jQuery('#prcWashington').text();
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    jQuery('input[name = cardFree]').attr("checked", false);
    // jQuery('#btnFree').css('display','none');

    if (dataWash == 'Washington Premium') {
        jQuery('input[id = MemberType]').val('10')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Oregon Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Washington Premium") return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnWash').text();
        }

        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Washington Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnWash').text();
        }
    }

    else if (dataWash == 'Washington Only') {
        jQuery('input[id = MemberType]').val('8')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnWash').text();

        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnWash').text();

        }
    }

    else if (dataWash == 'Washington Plus') {
        jQuery('input[id = MemberType]').val('11')
        if (data == 'Yearly') {
            var memshipInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
            });
            memshipInfoCost = _.without(memshipInfoCost, undefined);
            jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
            var memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
            memshipInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            memshipInfo = _.without(memshipInfo, undefined);
            jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
            jQuery('#prcPremium').html('');
            jQuery('#prcOregon').html('');
            jQuery('#prcWashington').html('');
            cost = jQuery('#YprcWash').text();
        }
        else if (data == 'Monthly') {
            var MonthlyInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
            jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
            jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
            var MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
            MonthlyInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            MonthlyInfo = _.without(MonthlyInfo, undefined);
            jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
            jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
            cost = jQuery('#spnWash').text();

        }
        else if (data == 'Quarterly') {
            var QInfoCost = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
            });
            QInfoCost = _.without(QInfoCost, undefined);
            jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
            jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
            var QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
            QInfo = _.map(model, function (o) {
                if (o.SubMemberShipPlanName == dataWash) return o;
            });
            QInfo = _.without(QInfo, undefined);
            jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
            jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
            cost = jQuery('#spnWash').text();

        }
    }
    jQuery('input[id = MemberCost]').val(cost);

}
function changeFree(dataEx, data) {
    //var data = jQuery('input[name = cardFree]:checked').val();
    //var dataEx = jQuery('input[name = example]:checked').val();
    jQuery('input[id = MemberType]').val('1')
    jQuery('input[name = CheckRadio]').val('pass');
    jQuery('input[name = CheckRadioValue]').val(data);
    jQuery('input[name = cardOreg]').attr("checked", false);
    jQuery('input[name = cardWash]').attr("checked", false);
    jQuery('input[name = cardPacific]').attr("checked", false);
    if (dataEx == 'Yearly') {
        var memshipInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
        });
        memshipInfoCost = _.without(memshipInfoCost, undefined);
        jQuery('#YprcPremium').text(memshipInfoCost[0].YearlyPrice);
        var memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
        memshipInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == "Washington Premium") return o;
        });
        memshipInfo = _.without(memshipInfo, undefined);
        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);
        jQuery('#prcPremium').html('');
        jQuery('#prcOregon').html('');
        jQuery('#prcWashington').html('');
    }
    else if (dataEx == 'Monthly') {
        var MonthlyInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        MonthlyInfoCost = _.without(MonthlyInfoCost, undefined);
        jQuery('#prcPremium').html('12 monthly payments of $<span id="spnPremium">' + MonthlyInfoCost[0].MonthlyPrice + '</span>');
        jQuery('#YprcPremium').html(MonthlyInfoCost[0].MonthlyPrice * 12);
        var MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcOregon').html('12 monthly payments of $<span id="spnOreg">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcOregon').html(MonthlyInfo[0].MonthlyPrice * 12);
        MonthlyInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        MonthlyInfo = _.without(MonthlyInfo, undefined);
        jQuery('#prcWashington').html('12 monthly payments of $<span id="spnWash">' + MonthlyInfo[0].MonthlyPrice + '</span>');
        jQuery('#YprcWash').html(MonthlyInfo[0].MonthlyPrice * 12);
    }
    else if (dataEx == 'Quarterly') {
        var QInfoCost = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Pacific Northwest Premium') return o;
        });
        QInfoCost = _.without(QInfoCost, undefined);
        jQuery('#prcPremium').html('4 Quarterly payments of $<span id="spnPremium">' + QInfoCost[0].QuarterlyPrice + '</span>');
        jQuery('#YprcPremium').html(QInfoCost[0].QuarterlyPrice * 4);
        var QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Oregon Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcOregon').html('4 Quarterly payments of $<span id="spnOreg">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcOregon').html(QInfo[0].QuarterlyPrice * 4);
        QInfo = _.map(model, function (o) {
            if (o.SubMemberShipPlanName == 'Washington Premium') return o;
        });
        QInfo = _.without(QInfo, undefined);
        jQuery('#prcWashington').html('4 Quarterly payments of $<span id="spnWash">' + QInfo[0].QuarterlyPrice + '</span>');
        jQuery('#YprcWash').html(QInfo[0].QuarterlyPrice * 4);
    } jQuery('input[id = MemberCost]').val('0');
}
function UpdatePayment() {
    jQuery('input[name = cardFree]:checked')
    var plan = jQuery('#CheckRadioValue').val();
    var ExVal = jQuery('#RadioExValue').val();
    jQuery('input[name = example]:checked').val(ExVal);
    if (plan == 'Free') {
        jQuery('input[name = cardFree]:checked');
        changeFree(ExVal, plan);
    }
    else if (plan == 'Pacific Northwest Premium') {
        jQuery('input[name = cardPacific]:checked');
        changePacific(ExVal, plan);
    }
    else if (plan == 'Oregon Premium') {
        jQuery('input[name = cardOreg][value="' + plan + '"]:checked');
        changeOreg(ExVal, plan);
    }
    else if (plan == 'Oregon Plus') {
        jQuery('input[name = cardOreg][value="' + plan + '"]:checked');
        changeOreg(ExVal, plan);
    }
    else if (plan == 'Oregon Only') {
        jQuery('input[name = cardOreg][value="' + plan + '"]:checked');
        changeOreg(ExVal, plan);
    }
    else if (plan == 'Washington Premium') {
        jQuery('input[name = cardWash][value="' + plan + '"]:checked');
        changeWash(ExVal, plan);
    }
    else if (plan == 'Washington Plus') {
        jQuery('input[name = cardWash][value="' + plan + '"]:checked');
        changeWash(ExVal, plan);
    }
    else if (plan == 'Washington Only') {
        jQuery('input[name = cardWash][value="' + plan + '"]:checked');
        changeWash(ExVal, plan);
    }
    jQuery("#membershipModal").modal('show');

    //jQuery.ajax({
    //    type: 'GET',
    //    url: "/Member/FindPriceDetail",
    //    success: function (data) {
    //        model = data;
    //        var memshipInfo = _.map(model, function (o) {
    //            if (o.SubMemberShipPlanName == "Pacific Northwest Premium") return o;
    //        });
    //        memshipInfo = _.without(memshipInfo, undefined);
    //        jQuery('#YprcPremium').text(memshipInfo[0].YearlyPrice);
    //        memshipInfo = _.map(model, function (o) {
    //            if (o.SubMemberShipPlanName == "Oregon Premium") return o;
    //        });
    //        memshipInfo = _.without(memshipInfo, undefined);
    //        jQuery('#YprcOregon').text(memshipInfo[0].YearlyPrice);
    //        memshipInfo = _.map(model, function (o) {
    //            if (o.SubMemberShipPlanName == "Washington Premium") return o;
    //        });
    //        memshipInfo = _.without(memshipInfo, undefined);
    //        jQuery('#YprcWash').text(memshipInfo[0].YearlyPrice);

    //    }
    //});
    // e.preventDefault();
}

function GetPacificCardDetails() {
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Home/GetPacificCardDetails',
        data: {},
        async: false,
        success: function (response) {
            var Pacific = response.data[0];
            var OregonPre = response.data[1];
            var OregonPlus = response.data[2];
            var OregonOnly = response.data[3];
            var WashingtonPre = response.data[4];
            var WashingtonPlu = response.data[5];
            var WashingtonOnly = response.data[6];
            var Free = response.data[7];
            /*    Pacific Northwest Premium   */
            jQuery('#PacificPremium').text(Pacific.PackageName);
            jQuery('#PacificPremiu').text(Pacific.PackageName);
            jQuery('#PrUserText').text(Pacific.UserText);
            jQuery('#PrRegionHead').text(Pacific.RegionHead);
            jQuery('#RegionText').text(Pacific.RegionText);
            /*          Oregon Premium          */
            jQuery('#OrPrePackage').text(OregonPre.PackageName);
            jQuery('#OrPreUser').text(OregonPre.UserText);
            jQuery('#OrPreRegion').text(OregonPre.RegionHead);
            jQuery('#OrPreText').text(OregonPre.RegionText);
            /*       Oregon Plus     */
            jQuery('#OrPluPackage').text(OregonPlus.PackageName);
            jQuery('#OrPluUser').text(OregonPlus.UserText);
            jQuery('#OrPluRegion').text(OregonPlus.RegionHead);
            jQuery('#OrPluText').text(OregonPlus.RegionText);
            /*        Oregon Only    */
            jQuery('#OrOnlPackage').text(OregonOnly.PackageName);
            jQuery('#OrOnlUser').text(OregonOnly.UserText);
            jQuery('#OrOnlRegion').text(OregonOnly.RegionHead);
            jQuery('#OrOnlText').text(OregonOnly.RegionText);
            /*         Washington Premium     */
            jQuery('#WaPrePackage').text(WashingtonPre.PackageName);
            jQuery('#WaPreUser').text(WashingtonPre.UserText);
            jQuery('#WaPreRegion').text(WashingtonPre.RegionHead);
            jQuery('#WaPreText').text(WashingtonPre.RegionText);
            /*         Washington Plus     */
            jQuery('#WaPluPackage').text(WashingtonPlu.PackageName);
            jQuery('#WaPluUser').text(WashingtonPlu.UserText);
            jQuery('#WaPluRegion').text(WashingtonPlu.RegionHead);
            jQuery('#WaPluText').text(WashingtonPlu.RegionText);
            /*         Washington Only     */
            jQuery('#WaOnlPackage').text(WashingtonOnly.PackageName);
            jQuery('#WaOnlUser').text(WashingtonOnly.UserText);
            jQuery('#WaOnlRegion').text(WashingtonOnly.RegionHead);
            jQuery('#WaOnlText').text(WashingtonOnly.RegionText);
            /*         Project Posting Only Or Free     */
            jQuery('#prcFree').text(Free.PackageName);
            jQuery('#FreepostText').text(Free.UserText);
            jQuery('#FreeRegion').text(Free.RegionHead);
            //jQuery('#FreeText').text(Free.RegionText);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
jQuery(document).ready(function () {
    var Email = jQuery('#hdnLogId').val();
    const d = new Date();
    let year = d.getFullYear();
    jQuery('#CopyYear').html(year);
    var Email = jQuery('#hdnLogId').val();
    jQuery.ajax({
        type: 'POST',
        url: "/Home/GetUserInfo",
        data: { "Email": Email },
        success: function (data) {

            jQuery('#nameloader').hide();
            jQuery('#hdnConId').val(data.ConId);
            jQuery('#hdnId').val(data.Id);
            jQuery('#hdnName').val(data.Name);
            jQuery('#hdnEmail').val(data.Email);
            jQuery('#hdnCompany').val(data.Company);
            jQuery('#hdnPhone').val(data.hdnPhone);
            jQuery('#lblLogin').text(data.Name);
            jQuery('#hdnPhone').val(data.Role);
            jQuery('#hdnUid').val(data.Uid);
        },
        error: function () {
            jQuery('#nameloader').hide();
            jQuery('#lblLogin').text('Guest');
            jQuery('#lbllogin').css('width', 'auto');
        }
    });
});
