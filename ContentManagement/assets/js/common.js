var $jsGlobalInfo = $('#js-global-info');

function setCookie(name, value, days, path) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=" + (path || "/");
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

function serializeFormToObject(formSelector) {
    var paramObjData = {};
    $.each($(formSelector).serializeArray(), function (_, kv) {
        if (paramObjData.hasOwnProperty(kv.name)) {
            paramObjData[kv.name] = $.makeArray(paramObjData[kv.name]);
            paramObjData[kv.name].push(kv.value);
        }
        else {
            paramObjData[kv.name] = kv.value;
        }
    });

    return paramObjData;
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

$(document).ready(function () {
    //tippy('[data-tippy]');
    tippy(document.querySelectorAll('[data-tippy]'));

    var $headerNavbar = $("#header-navbar");
    if ($headerNavbar.length > 0)
    {
        $headerNavbar.css('visibility', 'visible').metisMenu();
        //$headerNavbar.metisMenu();
        stickybits('#menu-hamberger-container');

        $(document).on('click', '#menu-hamburger-expand', function (e) {
            e.stopPropagation();
            $('#header-navbar-container').toggle(1);
            return false;
        });
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
            enableDrag: false,
            adaptiveHeight: true,
            responsive: [
                {
                    breakpoint: 800,
                    settings: {
                        item: 3,
                        slideMove: 1,
                        slideMargin: 6
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

        if (type && type.toLowerCase() !== 'all')
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
        $contentDetail.find('img').addClass('img-fluid img-thumbnail m-4px'); //responsive images
    }

    var $pageDetail = $("#page-detail");
    if ($pageDetail.length > 0) {
        $pageDetail.find('img').addClass('img-fluid img-thumbnail m-4px'); //responsive images
    }

    var $contentsForm = $("#contents-form");
    if ($contentsForm.length > 0) {
        $contentsForm.on('submit', function (e) {
            e.preventDefault();

            var contentsPath = $('#js-global-info').data('contentsPath');
            var contentsFormType = $contentsForm.find('#cct').val();
            var otherContentsForm = $contentsForm.find('#OtherContents').is(':checked');
            var favoriteForm = $contentsForm.find('#Favorite').is(':checked');
            window.location = contentsPath + '?t=' + contentsFormType + '&othercontents=' + otherContentsForm + '&favorite=' + favoriteForm;
        });
    }

    var $searchInput = $("#search-input");
    if ($searchInput.length > 0) {

        //var $jsGlobalInfo = $('#js-global-info');

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
                var moreResults = $jsGlobalInfo.data('searchAutocompleteMoreresult');
                var searchPath = $jsGlobalInfo.data('searchPath');
                var randomNumber = Math.floor(Math.random() * (10000000 - 100000 + 1)) + 100000;

                var template = '<div class="row no-gutters">' +
                    '<div class="col-12" style="font-size: 0.8rem !important;">' +
                    '<img class="img-thumbnail d-sm-none d-md-inline mr-1" style="max-width: 35px; max-height: 35px;" src="{{imagename}}?width=35&height=35&rmode=pad&bgcolor=white&v=' + randomNumber + '">' +
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

    var $qrcodeButton = $("#qrcode-button");
    if ($qrcodeButton.length > 0) {
        var url = $qrcodeButton.data('url');
        if (url && url !== "#")
        {
            var typeNumber = 0;
            var errorCorrectionLevel = 'M';
            var qr = qrcode(typeNumber, errorCorrectionLevel);
            qr.addData(url);
            qr.make();

            $(document).on('click', '#qrcode-button', function (e) {
                swal({
                    type: null,
                    html: qr.createImgTag(),
                    target: document.getElementById('swal-container'),
                    showCloseButton: true,
                    showConfirmButton: false,
                    //focusConfirm: true,
                    //confirmButtonText: '<i class="fa fa-thumbs-up"></i>'
                });

                e.preventDefault();
                return false;
            });

            $(document).on('click', '#print-button', function (e) {
                window.print();

                e.preventDefault();
                return false;
            });
        }
    }

    var $searchForm = $("#search-form");
    if ($searchForm.length > 0) {
        $searchForm.submit(function (e) {
            var $searchInput = $("#search-input");
            if ($searchInput.val().trim().length <= 0) {
                $searchInput.val('');
                $searchInput.focus();
                e.preventDefault();
                return false;
            }
        });
    }

    var $quickLinksSidebar = $("#quickLinksSidebarContent");
    if ($quickLinksSidebar.length > 0) {
        $(document).on('click', '#quickLinksSidebarSwitch a, #quick-links-expand', function (e) {
            $('#quickLinksSidebarContent')
                .sidebar({
                    transition: 'overlay	', mobileTransition: 'overlay', silent: true,
                    onHidden: function () {
                        $('.pusher').css('overflow', 'visible');
                    },
                    onVisible: function () {
                        $('.pusher').css('overflow', 'hidden');
                    }
                })
                .sidebar('toggle');

            e.preventDefault();
            return false;
        });

        $(document).on('click', '#close-quick-links', function (e) {
            $('#quickLinksSidebarContent')
                .sidebar('hide');

            e.preventDefault();
            return false;
        });
    }

    var $contentGalleryItems = $("#content-gallery-items");
    if ($contentGalleryItems.length > 0) {

        $("#content-gallery-progress").hide();
        $("#content-gallery-container").show();
        var contentLightGallery;

        var pagerPosition = isRtl ? 'right' : 'left';
        $contentGalleryItems.lightSlider({
            gallery: true,
            item: 1,
            auto: true,
            loop: true,
            thumbItem: 8,
            slideMargin: 0,
            enableDrag: false,
            pause: 3500,
            rtl: isRtl,
            currentPagerPosition: pagerPosition,
            onSliderLoad: function (el) {
                el.lightGallery({
                    selector: '#content-gallery-items .lslide',
                    share: false,
                    mode: 'lg-slide-circular'
                });
            }
        });

        tippy('#content-gallery-container .lSSlideWrapper', {
            content: $jsGlobalInfo.data('clickZoomin')
        });

        var tippyGallery = document.querySelector('#content-gallery-container .lSSlideWrapper');
        $(document).on('onBeforeOpen.lg', function (event) {
            tippyGallery._tippy.hide();
        });
    }

    if (window.location.pathname.indexOf('/add/') < 0 && window.location.pathname.indexOf('/update/') < 0) {
        $(document).on('change', '#PortalId', function () {
            var paths = window.location.pathname.split("/");
            setCookie('PortalIdDD', $(this).val(), 365, '/' + paths[1] + '/' + paths[2]);
        });
    }

    if (window.location.pathname.indexOf('/add/') < 0 && window.location.pathname.indexOf('/update/') < 0) {
        $(document).on('change', '#Language', function () {
            var paths = window.location.pathname.split("/");
            setCookie('LanguageDD', $(this).val(), 365, '/' + paths[1] + '/' + paths[2]);
        });
    }

    if (window.location.pathname.indexOf('/add/') < 0 && window.location.pathname.indexOf('/update/') < 0) {
        $(document).on('change', '#ContentType', function () {
            var paths = window.location.pathname.split("/");
            setCookie('ContentTypeDD', $(this).val(), 365, '/' + paths[1] + '/' + paths[2]);
        });
    }

    if (window.location.pathname.indexOf('/add/') < 0 && window.location.pathname.indexOf('/update/') < 0) {
        $(document).on('change', '#LinkType', function () {
            var paths = window.location.pathname.split("/");
            setCookie('LinkTypeDD', $(this).val(), 365, '/' + paths[1] + '/' + paths[2]);
        });
    }

    var $portalIdDD = $("#PortalId");
    if ($portalIdDD.length > 0 && window.location.pathname.indexOf('/update/') < 0) {
        var valPortalIdDD = getCookie('PortalIdDD');
        $portalIdDD.val(valPortalIdDD ? parseInt(valPortalIdDD) : $("#PortalId option:first").val()).trigger('change');
    }

    if ($("#PortalId option").length === 1)
    {
        $portalIdDD.val($("#PortalId option:first").val());
    }


    var $languageDD = $("#Language");
    if ($languageDD.length > 0 && window.location.pathname.indexOf('/update/') < 0) {
        var valLanguageDD = getCookie('LanguageDD');
        $languageDD.val(valLanguageDD ? parseInt(valLanguageDD) : 1).trigger('change');
    }

    var $contentTypeDD = $("#ContentType");
    if ($contentTypeDD.length > 0 && window.location.pathname.indexOf('/update/') < 0 && !getParameterByName('t')) {
        var valContentTypeDD = getCookie('ContentTypeDD');
        $contentTypeDD.val(valContentTypeDD ? parseInt(valContentTypeDD) : 0).trigger('change');
    }

    var $linkTypeDD = $("#LinkType");
    if ($linkTypeDD.length > 0 && window.location.pathname.indexOf('/update/') < 0 && !getParameterByName('t')) {
        var valLinkTypeDD = getCookie('LinkTypeDD');
        $linkTypeDD.val(valLinkTypeDD ? parseInt(valLinkTypeDD) : 0).trigger('change');
    }

    var $voteSubmitForm = $('#vote-submit-form');
    if ($voteSubmitForm.length > 0) {
        $(document).on('click', '#vote-submit-btn', function (e) {
            if ($('[name="voteItem"]:checked').length === 0) {
                swal({
                    type: 'info',
                    text: $jsGlobalInfo.data('voteRequiredMsg'),
                    target: document.getElementById('swal-container'),
                    showCloseButton: true,
                    showConfirmButton: true,
                    confirmButtonText: $jsGlobalInfo.data('ok') 
                });

                e.preventDefault();
                return false;
            }

            var $voteId = $('#vid');
            var voteCookieName = 'vote-' + $voteId.val();
            if (getCookie(voteCookieName)) {
                swal({
                    type: 'info',
                    text: $jsGlobalInfo.data('votePreSubmitedMsg'),
                    target: document.getElementById('swal-container'),
                    showCloseButton: true,
                    showConfirmButton: true,
                    confirmButtonText: $jsGlobalInfo.data('ok') 
                });

                e.preventDefault();
                return false;
            }

            if ($('#IsActiveVote').val() === 'false') {
                swal({
                    type: 'info',
                    text: $jsGlobalInfo.data('voteDisabledMsg'),
                    target: document.getElementById('swal-container'),
                    showCloseButton: true,
                    showConfirmButton: true,
                    confirmButtonText: $jsGlobalInfo.data('ok')
                });

                e.preventDefault();
                return false;
            }

            if ($('#IsExpiredVote').val() === 'true') {
                swal({
                    type: 'info',
                    text: $jsGlobalInfo.data('voteExpiredMsg'),
                    target: document.getElementById('swal-container'),
                    showCloseButton: true,
                    showConfirmButton: true,
                    confirmButtonText: $jsGlobalInfo.data('ok')
                });

                e.preventDefault();
                return false;
            }

            //var formDataObj = serializeFormToObject('#vote-submit-form');
            var voteFormData = new FormData();
            var voteFormValues = $voteSubmitForm.serialize().split('&');
            for (var i in voteFormValues) {
                var itemValue = voteFormValues[i].split('=');
                voteFormData.append(itemValue[0], itemValue[1]);
            }
            axios.post($voteSubmitForm.attr('action'), voteFormData)
                .then(function (response) {
                    setCookie(voteCookieName, $voteId.val(), 186);

                    swal({
                        type: 'success',
                        text: $jsGlobalInfo.data('voteSuccessMsg'),
                        target: document.getElementById('swal-container'),
                        showCloseButton: true,
                        showConfirmButton: true,
                        confirmButtonText: $jsGlobalInfo.data('ok') 
                    });
                })
                .catch(function (error) {
                    swal({
                        type: 'error',
                        text: $jsGlobalInfo.data('errorOccurred'),
                        target: document.getElementById('swal-container'),
                        showCloseButton: true,
                        showConfirmButton: true,
                        confirmButtonText: $jsGlobalInfo.data('ok') 
                    });
                });
        });
    }

    // close menu root elemts after click on theme
    $(document).on('click', 'body', function (e) {
        var $expandedMenu = $('ul#header-navbar.metismenu > li.mm-active');
        $expandedMenu.removeClass('mm-active');
        $('ul.mm-collapse').removeClass('mm-show');
        $($expandedMenu.children()[0]).attr('aria-expanded', 'false');
    });

    // close menu root elemts after click on theme
    $('ul#header-navbar > li').hover(function () { }, function (e) {
        var $expandedMenu = $('ul#header-navbar.metismenu > li.mm-active');
        $expandedMenu.removeClass('mm-active');
        $('ul.mm-collapse').removeClass('mm-show');
        $($expandedMenu.children()[0]).attr('aria-expanded', 'false');
    });
});
