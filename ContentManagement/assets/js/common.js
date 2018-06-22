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

    var $contentDetail = $("#content-detail");
    if ($contentDetail.length > 0) {
        $contentDetail.find('img').addClass('img-fluid img-thumbnail m-1'); //responsive images
    }

    var $pageDetail = $("#page-detail");
    if ($pageDetail.length > 0) {
        $pageDetail.find('img').addClass('img-fluid img-thumbnail m-1'); //responsive images
    }

    var $contentsForm = $("#contents-form");
    if ($contentsForm.length > 0) {
        $contentsForm.on('submit', function (e) {
            e.preventDefault();

            var contentsPath = $('#js-global-info').data('contentsPath');
            var contentsFormType = $contentsForm.find('#ContentType').val();
            var otherContentsForm = $contentsForm.find('#OtherContents').is(':checked');
            var favoriteForm = $contentsForm.find('#Favorite').is(':checked');
            window.location = contentsPath + '?t=' + contentsFormType + '&othercontents=' + otherContentsForm + '&favorite=' + favoriteForm;
        });
    }

    var $searchInput = $("#search-input");
    if ($searchInput.length > 0) {

        var $jsGlobalInfo = $('#js-global-info');

        $searchInput.typeahead({
            minLength: 3,
            dynamic: true,
            delay: 500,
            hint: true,
            maxItem: 0,
            searchOnFocus: true,
            backdrop: {
                "opacity": 0.45,
                "filter": "alpha(opacity=45)",
                "background-color": "#eaf3ff"
            },
            template: function (query, item) {
                console.log(item);
                var moreResults = $jsGlobalInfo.data('searchAutocompleteMoreresult');
                var searchPath = $jsGlobalInfo.data('searchPath');

                var template = '<div class="row no-gutters">' +
                                '<div class="col-12" style="font-size: 0.8rem !important;">' +
                                '<img class="img-thumbnail d-sm-none d-md-inline mr-1" src="{{imagename}}?width=35">' +
                                '<span>{{title}}</span>&nbsp;' +
                                '<span style="top: -2px; position: relative;" class="badge badge-warning">{{contentType}}</span>' +
                                '</div>' +
                                '</div>';
                
                if (item.isLastItem === true) {
                    template = template +
                        '<div class="row no-gutters">' +
                        '<div class="col-12" style="font-size: 0.8rem !important;">' +
                        '<a class="mt-1 float-right" href="' + searchPath + '?q=' + query + '">' + moreResults + '</a>' +
                        '</div>' +
                        '</div>';
                }

                return template;
            },
            emptyTemplate: function (query) {
                return $jsGlobalInfo.data('searchAutocompleteNoresult').replace('##', '{{').replace('**', '}}').replace('{{query}}', '<strong>' + query + '</strong>');
            },
            source: {
                results: {
                    display: ['title', 'text'],
                    href: '{{link}}',
                    ajax: function (query) {
                        return {
                            type: 'POST',
                            url: $jsGlobalInfo.data('searchAutocompletePath'),
                            path: "data.results",
                            data: {
                                q: '{{query}}'
                            }
                        }
                    }
                }
            }
        });
    }
});
