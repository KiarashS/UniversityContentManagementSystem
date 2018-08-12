/// <binding BeforeBuild='default' Clean='clean' ProjectOpened='watch' />

var del = require("del"),
    gulp = require("gulp"),
    concat = require("gulp-concat"),
    csso = require("gulp-csso"),
    uglify = require("gulp-uglify"),
    rtlcss = require("gulp-rtlcss"),
    rename = require('gulp-rename'),
    gutil = require('gulp-util'),
    pump = require('pump'),
    minify = require('gulp-minify');


var filesPath = {
    // CSS files
    commonCss: [
        "node_modules/hover.css/css/hover-min.css",
        "node_modules/select2/dist/css/select2.min.css",
        "node_modules/metismenu/dist/metisMenu.min.css",
        "node_modules/lightslider/dist/css/lightslider.min.css",
        "assets/css/lib/bootstrap-tabs-x-bs4.css",
        "node_modules/semantic-ui-sidebar/sidebar.min.css",
        "node_modules/lightgallery/dist/css/lightgallery.min.css",
        "node_modules/lightgallery/dist/css/lg-transitions.min.css",
        "assets/css/common.css"
    ],
    rtlCss: [
        //"assets/css/transform/rtl/*.css",
        "assets/css/transform/rtl/bootstrap-rtl.css",
        "assets/css/transform/rtl/sb-admin-rtl.css",
        "assets/css/transform/rtl/jquery.typeahead.min-rtl.css",
        "assets/css/rtl-styles.css"
    ],
    ltrCss: [
        //"assets/css/transform/ltr/*.css",
        "assets/css/transform/ltr/bootstrap.css",
        "assets/css/transform/ltr/sb-admin.css",
        "node_modules/jquery-typeahead/dist/jquery.typeahead.min.css",
        "assets/css/ltr-styles.css"
    ],
    manageCss: [
        "assets/css/transform/rtl/dataTables.bootstrap4-rtl.css",
        "assets/css/transform/rtl/fixedHeader.bootstrap4-rtl.css",
        "assets/css/transform/rtl/responsive.bootstrap4-rtl.css",
        "node_modules/fontawesome-iconpicker/dist/css/fontawesome-iconpicker.css",
    ],
    systemRtlCss: [
        //"assets/css/transform/rtl/mdb.min-rtl.css",
    ],
    systemLtrCss: [
        //"assets/css/transform/ltr/mdb.min.css",
    ],
    moveCss: [
        //"assets/css/kendoui/kendo.common.min.css",
        //"assets/css/kendoui/kendo.bootstrap-v4.min.css",
        //"assets/css/kendoui/kendo.bootstrap.mobile.min.css",
        //"assets/css/kendoui/kendo.rtl.min.css",
        "assets/css/kendoui/**",
    ],
    transformCss: [
        "node_modules/bootstrap/dist/css/bootstrap.css",
        "node_modules/startbootstrap-sb-admin/css/sb-admin.css",
        "node_modules/datatables.net-bs4/css/dataTables.bootstrap4.css",
        "node_modules/datatables.net-fixedheader-bs4/css/fixedHeader.bootstrap4.css",
        "node_modules/datatables.net-responsive-bs4/css/responsive.bootstrap4.css",
        "node_modules/jquery-typeahead/dist/jquery.typeahead.min.css",
        //"assets/css/lib/mdb.min.css",
    ],
    cssOutput: "wwwroot/css/",
    transformCssOutput: "assets/css/transform/",

    // JS files
    commonJs: [
        "node_modules/jquery/dist/jquery.min.js",
        //"node_modules/moment/min/moment.min.js",
        //"node_modules/moment-jalaali/build/moment-jalaali.js",
        "node_modules/jquery-validation/dist/jquery.validate.min.js",
        "node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js",
        "node_modules/jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.min.js",
        "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js",
        "node_modules/jquery.easing/jquery.easing.min.js",
        "node_modules/sweetalert2/dist/sweetalert2.all.min.js",
        "node_modules/axios/dist/axios.min.js",
        "node_modules/select2/dist/js/select2.min.js",
        "node_modules/select2/dist/js/i18n/fa.js",
        "node_modules/clipboard/dist/clipboard.min.js",
        "node_modules/persianjs/persian.min.js",
        "node_modules/startbootstrap-sb-admin/js/sb-admin.js",
        "node_modules/metismenu/dist/metisMenu.min.js",
        "node_modules/lightslider/dist/js/lightslider.min.js",
        "assets/js/lib/bootstrap-tabs-x.js",
        "node_modules/jquery-typeahead/dist/jquery.typeahead.min.js",
        "node_modules/qrcode-generator/qrcode.js",
        "node_modules/qrcode-generator/qrcode_UTF8.js",
        "node_modules/semantic-ui-sidebar/sidebar.min.js",
        "node_modules/stickybits/dist/stickybits.min.js",
        "node_modules/lightgallery/dist/js/lightgallery-all.min.js",
        "assets/js/common.js",
    ],
    rtlJs: [],
    ltrJs: [],
    systemRtlJs: [
        //"assets/js/lib/mdb.min.js",
    ],
    systemLtrJs: [
        //"assets/js/lib/mdb.min.js",
    ],
    manageJs: [
        "node_modules/datatables.net/js/jquery.dataTables.js",
        "node_modules/datatables.net-bs4/js/dataTables.bootstrap4.js",
        "node_modules/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
        "node_modules/datatables.net-responsive/js/dataTables.responsive.min.js",
        "node_modules/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js",
        "node_modules/fontawesome-iconpicker/dist/js/fontawesome-iconpicker.min.js",
        "assets/js/lib/jscolor.js",
    ],
    moveJs: [
        //"assets/js/kendoui/kendo.all.min.js",
        //"assets/js/kendoui/kendo.culture.fa-IR.min.js",
        //"assets/js/kendoui/kendo.culture.fa.min.js",
        //"assets/js/kendoui/kendo.messages.fa-IR.min.js",
        //"assets/js/kendoui/kendo-global.fa-IR.js",
        "assets/js/kendoui/**",
    ],
    jsOutput: "wwwroot/js/",

    // etc. images, fonts, ...
    images: [
        "assets/images/**",
    ],
    img: [
        "node_modules/lightslider/dist/img/controls.png",
        "assets/img/**",
        "node_modules/lightgallery/dist/img/**"
    ],
    imagesOutput: "wwwroot/images/",
    imgOutput: "wwwroot/img/",
    fonts: [
        "node_modules/vazir-font/dist/Vazir*.*",
        "node_modules/shabnam-font/dist/Shabnam*.*",
        "assets/fonts/**",
        "node_modules/lightgallery/dist/fonts/**"
    ],
    fontsFarsi: [
        "node_modules/vazir-font/dist/font-face.css",
        "node_modules/shabnam-font/dist/font-face.css"
    ],
    fontsOutput: "wwwroot/fonts/"
};

gulp.task("default", gulp.series(cssTransform, fontsFarsi, commonCssMin, rtlCssMin, ltrCssMin, manageCssMin, systemRtlCssMin, systemLtrCssMin, moveCss, commonJsMin, rtlJsMin, ltrJsMin, manageJsMin, systemRtlJsMin, systemLtrJsMin, moveJs, imagesMove, fontsMove));
gulp.task("clean", gulp.series(transformCssClean, cssClean, jsClean, imagesClean, fontsClean));

gulp.task("watch", gulp.series("default", function watch() {
    gulp.watch(filesPath.commonCss, commonCssMin);
    gulp.watch(filesPath.rtlCss, rtlCssMin);
    gulp.watch(filesPath.ltrCss, ltrCssMin);
    gulp.watch(filesPath.manageCss, manageCssMin);
    gulp.watch(filesPath.systemRtlCss, systemRtlCssMin);
    gulp.watch(filesPath.systemLtrCss, systemLtrCssMin);
    gulp.watch(filesPath.moveCss, moveCss);
    gulp.watch(filesPath.commonJs, commonJsMin);
    gulp.watch(filesPath.rtlJs, rtlJsMin);
    gulp.watch(filesPath.ltrJs, ltrJsMin);
    gulp.watch(filesPath.manageJs, manageJsMin);
    gulp.watch(filesPath.systemRtlJs, systemRtlJsMin);
    gulp.watch(filesPath.systemLtrJs, systemLtrJsMin);
    gulp.watch(filesPath.moveJs, moveJs);
    gulp.watch(filesPath.images, imagesMove);
    gulp.watch(filesPath.fonts, fontsMove);
    gulp.watch(filesPath.fontsFarsi, fontsFarsi);
    //done();
}));

// CSS
function cssTransform() {
    if (filesPath.transformCss.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.transformCss)
        //.pipe(sourcemaps.init())
        //.pipe(autoprefixer(["last 2 versions", "> 1%"])) // Other post-processing.
        .pipe(gulp.dest(filesPath.transformCssOutput + 'ltr/')) // Output LTR stylesheets.
        .pipe(rtlcss()) // Convert to RTL.
        .pipe(rename({ suffix: '-rtl' })) // Append "-rtl" to the filename.
        //.pipe(sourcemaps.write('dist')) // Output source maps.
        .pipe(gulp.dest(filesPath.transformCssOutput + 'rtl/')); // Output RTL stylesheets.
};

function transformCssClean() {
    return del([filesPath.transformCssOutput + 'ltr/*', filesPath.transformCssOutput + 'rtl/*']);
};

function commonCssMin() {
    //gulp.src(filesPath.cssInput + "abovethefold.css")
    //    .pipe(csso())
    //    .pipe(gulp.dest(filesPath.cssOutput));

    //return gulp.src([filesPath.cssInput + "*.css", "!" + filesPath.cssInput + "abovethefold.css"])
    //    .pipe(concat("all.css"))
    //    .pipe(csso())
    //    .pipe(gulp.dest(filesPath.cssOutput));

    if (filesPath.commonCss.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.commonCss)
        .pipe(concat("common.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
};

function rtlCssMin() {
    if (filesPath.rtlCss.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.rtlCss)
        .pipe(concat("rtl.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
};

function ltrCssMin() {
    if (filesPath.ltrCss.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.ltrCss)
        .pipe(concat("ltr.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
};

function manageCssMin() {
    if (filesPath.manageCss.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.manageCss)
        .pipe(concat("manage.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
};

function systemRtlCssMin(done) {
    if (filesPath.systemRtlCss.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.systemRtlCss)
        .pipe(concat("portal.rtl.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
};

function systemLtrCssMin(done) {
    if (filesPath.systemLtrCss.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.systemLtrCss)
        .pipe(concat("portal.ltr.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
};

function moveCss(done) {
    if (filesPath.moveCss.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.moveCss)
        .pipe(gulp.dest(filesPath.cssOutput));
};

function cssClean() {
    return del(filesPath.cssOutput);
};


// JavaScript
function commonJsMin(done) {
    //return gulp.src([filesPath.jsInput + "/*.js"])
    //    .pipe(concat("all.js"))
    //    .pipe(uglify())
    //    .pipe(gulp.dest(filesPath.jsOutput));

    if (filesPath.commonJs.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.commonJs)
        .pipe(concat("common.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        //.pipe(babel({
        //    presets: ['@babel/env']
        //}))
        //.pipe(uglify())
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            noSource: true,
            preserveComments: function () {
                return false;
            }
        }))
        .on('error', function (err) { gutil.log(gutil.colors.red('[Error]'), err.toString()); })
        //.pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(filesPath.jsOutput));
};

function rtlJsMin(done) {
    if (filesPath.rtlJs.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.rtlJs)
        .pipe(concat("rtl.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            noSource: true,
            preserveComments: function () {
                return false;
            }
        }))
        .pipe(gulp.dest(filesPath.jsOutput));
};

function ltrJsMin(done) {
    if (filesPath.ltrJs.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.ltrJs)
        .pipe(concat("ltr.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            noSource: true,
            preserveComments: function () {
                return false;
            }
        }))
        .pipe(gulp.dest(filesPath.jsOutput));
};

function manageJsMin(done) {
    if (filesPath.manageJs.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.manageJs)
        .pipe(concat("manage.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            noSource: true,
            preserveComments: function () {
                return false;
            }
        }))
        .pipe(gulp.dest(filesPath.jsOutput));
};

function systemRtlJsMin(done) {
    if (filesPath.systemRtlJs.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.systemRtlJs)
        .pipe(concat("portal.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        .pipe(minify({
            ext: {
                min: '.rtl.min.js'
            },
            noSource: true,
            preserveComments: function () {
                return false;
            }
        }))
        .pipe(gulp.dest(filesPath.jsOutput));
};

function systemLtrJsMin(done) {
    if (filesPath.systemLtrJs.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.systemLtrJs)
        .pipe(concat("portal.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        .pipe(minify({
            ext: {
                min: '.ltr.min.js'
            },
            noSource: true,
            preserveComments: function () {
                return false;
            }
        }))
        .pipe(gulp.dest(filesPath.jsOutput));
};

function moveJs(done) {
    if (filesPath.moveJs.length == 0) {
        done();
        return;
    }

    return gulp.src(filesPath.moveJs)
        .pipe(gulp.dest(filesPath.jsOutput));
};

function jsClean() {
    return del(filesPath.jsOutput);
};

function uglifyErrorDebugging(cb) {
    pump([
        gulp.src(filesPath.commonJs),
        minify({
            ext: {
                min: '.min.js'
            },
            noSource: true,
            preserveComments: function () {
                return false;
            }
        }),
        uglify(),
        gulp.dest(filesPath.jsOutput)
    ], cb);
};


// Images
function imagesMove() {
    gulp.src(filesPath.img)
        .pipe(gulp.dest(filesPath.imgOutput));

    return gulp.src(filesPath.images)
                .pipe(gulp.dest(filesPath.imagesOutput));
};

function imagesClean() {
    return del(filesPath.imagesOutput);
};


// Fonts
function fontsMove() {
    return gulp.src(filesPath.fonts)
                .pipe(gulp.dest(filesPath.fontsOutput));
};

function fontsClean() {
    return del(filesPath.fontsOutput);
};

function fontsFarsi() {
    return gulp.src(filesPath.fontsFarsi)
                .pipe(concat("farsifonts.min.css"))
                .pipe(csso())
                .pipe(gulp.dest(filesPath.fontsOutput));
};
