var srcVariable = '';
jQuery(document).ready(function () {
    jQuery('div.small-box-one').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/BESThq.png');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        },

    );
    jQuery('div.small-box-two').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/CSIWebAd2.jpg');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        },
    );
    jQuery('div.small-box-three').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/CTCivilTakeoffsBanner.jpg');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        },

    );
    jQuery('div.small-box-four').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/PamplinMediaGroup.png');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        },
    );
    jQuery('div.small-box-five').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/PileKingBanner.jpg');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        },
    );
    jQuery('div.small-box-six').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/RJBANNER.jpg');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        },
    );
    jQuery('div.small-box-seven').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/Statewide_Banner.jpg');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        });
    jQuery('div.small-box-eight').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/Sterling-Pacific_CPC_banner_ad.jpg');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        },
    );
    jQuery('div.small-box-nine').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/CESSCO.png');

        },
        function () {
            jQuery('div.main-img-box .active-slide').children('img').attr('src', srcVariable);
            jQuery('div.main-img-box .active-slide').removeClass('.active-slide');
            showSlides();
        },
    );
    jQuery('div.small-box-ten').hover(
        function () {
            srcVariable = jQuery('div.main-img-box .active-slide').children('img').attr('src')
            jQuery('div.main-img-box .active-slide').children('img').attr('src', '/assets/images/QuickEye.png');

        },
    );
    jQuery(".small-box div").hover(
        function () {
            jQuery(".small-box div").removeClass('active');
            jQuery(this).addClass('active');
        }
    );
    GetSpecialMsg();
    GetSpecialMsgMain();
});
let slideIndex = 0;
showSlides();

function showSlides() {
    let i;
    let slides = document.getElementsByClassName("mySlides");
    let dots = document.getElementsByClassName("dot");
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
        slides[i].className = slides[i].className.replace(" active-slide", "");
    }
    slideIndex++;
    if (slideIndex > slides.length) { slideIndex = 1 }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace("active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    slides[slideIndex - 1].className += " active-slide";
    dots[slideIndex - 1].className += " active";
    var timer = setTimeout(showSlides, 2000);
    jQuery('.small-box div').hover(function () {
        clearTimeout(timer);
    });
}
function GetSpecialMsg() {
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Home/GetSpecialMsgList',
        data: {},
        async: false,
        success: function (response) {
            if (response.data != null && response.data.length > 0) {
                var Msg = response.data[0];
                jQuery('#splmsgMarquee').removeClass('topMarquee');
                jQuery('.mainbar-container ').addClass('fixed_marquee');
                jQuery('#SpeMsg').text(Msg.SpMessage);
            }
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}
function GetSpecialMsgMain() {
    jQuery.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Home/GetSpecialMsgMain',
        data: {},
        async: false,
        success: function (response) {
            if (response.data != null && response.data.length > 0) {
                var Msg = response.data[0];
                jQuery('#SpeMsgMain').text(Msg.SpMessage);
            }
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}