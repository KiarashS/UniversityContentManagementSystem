﻿$(document).ready(function () {
    tippy('[data-tippy]');

    var $headerNavbar = $("#header-navbar");
    if ($headerNavbar.length > 0) {
        $headerNavbar.metisMenu();
    }

    var $essentialLinks = $("#essential-links");
    if ($essentialLinks.length > 0) {
        $essentialLinks.lightSlider({
            item: 7,
            slideMove: 3,
            rtl: isRtl,
            auto: true,
            loop: true,
            keyPress: true,
            responsive: [
                {
                    breakpoint: 800,
                    settings: {
                        item: 3,
                        slideMove: 1,
                        slideMargin: 6,
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        item: 2,
                        slideMove: 1
                    }
                }
            ]
        });
    }
});
