$(document).ready(function () {
    tippy('[data-tippy]');

    var $headerNavbar = $("#header-navbar");
    if ($headerNavbar.length > 0)
    {
        $headerNavbar.metisMenu();
    }

    var $essentialLinks = $("#essential-links");
    if ($essentialLinks.length > 0)
    {
        $essentialLinks.lightSlider({
            item: 7,
            slideMove: 3,
            rtl: isRtl,
            auto: true,
            loop: true,
            keyPress: true,
            speed: 350,
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

    var $linksTab = $("#links-tab");
    if ($linksTab.length > 0)
    {
        $linksTab.tabsX({
            ajaxSettings: {
                dataType: 'html'
            }
        });
    }

    var $newsTab = $("#newsandfavorite-tab");
    if ($newsTab.length > 0)
    {
        $newsTab.tabsX({
            ajaxSettings: {
                dataType: 'html'
            }
        });

        $newsTab.on('tabsX:success', function (event, data, status, jqXHR) {
            tippy('[data-tippy]');
        });
    }

    var $contentsTab = $("#contents-tab");
    if ($contentsTab.length > 0)
    {
        $contentsTab.tabsX({
            ajaxSettings: {
                dataType: 'html'
            }
        });

        $contentsTab.on('tabsX:success', function (event, data, status, jqXHR) {
            tippy('[data-tippy]');
        });
    }

        $(document).on('change', '#othercontentstype', function () {
            var type = this.value;
            var fetchContentsBasePath = $('#js-global-info').data('fetchContentsPath');
            var fetchContentsPath = fetchContentsBasePath + '?othercontents=true';

            axios.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
            document.body.style.cursor = 'wait';

            if (type && type.toLowerCase() != 'all')
            {
                axios.post(fetchContentsPath + '&t=' + type.toLowerCase()/*, { headers: { 'X-Requested-With': 'XMLHttpRequest' } }*/)
                    .then(function (response) {
                        $('#othercontents-tabs-above').html(response.data).hide().fadeIn();
                        tippy('[data-tippy]');
                    });

                document.body.style.cursor = 'default';
                return;
            }

            axios.post(fetchContentsPath/*, { headers: { 'X-Requested-With': 'XMLHttpRequest' } }*/)
                .then(function (response) {
                    $('#othercontents-tabs-above').html(response.data).hide().fadeIn();
                    tippy('[data-tippy]');
                });

            document.body.style.cursor = 'default';
        });
});
