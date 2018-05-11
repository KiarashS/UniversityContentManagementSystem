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
        "node_modules/select2/dist/css/select2.min.css",
        "assets/css/common-styles.css"
    ],
    rtlCss: [
        //"assets/css/transform/rtl/*.css",
        "assets/css/transform/rtl/bootstrap-rtl.css",
        "assets/css/transform/rtl/sb-admin-rtl.css",
        "assets/css/rtl-styles.css"
    ],
    ltrCss: [
        //"assets/css/transform/ltr/*.css",
        "assets/css/transform/ltr/bootstrap.css",
        "assets/css/transform/ltr/sb-admin.css",
        "assets/css/ltr-styles.css"
    ],
    manageCss: [
        "assets/css/transform/rtl/dataTables.bootstrap4-rtl.css",
        "assets/css/transform/rtl/fixedHeader.bootstrap4-rtl.css",
        "assets/css/transform/rtl/responsive.bootstrap4-rtl.css",
        "assets/css/transform/ltr/fontawesome-iconpicker.css",
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
        "node_modules/fontawesome-iconpicker/dist/css/fontawesome-iconpicker.css",
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
        "node_modules/sweetalert2/dist/sweetalert2.all.min.js",
        "node_modules/axios/dist/axios.min.js",
        "node_modules/select2/dist/js/select2.min.js",
        "node_modules/select2/dist/js/i18n/fa.js",
        "node_modules/clipboard/dist/clipboard.min.js",
        "node_modules/startbootstrap-sb-admin/js/sb-admin.js"
    ],
    rtlJs: [],
    ltrJs: [],
    manageJs: [
        "node_modules/datatables.net/js/jquery.dataTables.js",
        "node_modules/datatables.net-bs4/js/dataTables.bootstrap4.js",
        "node_modules/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
        "node_modules/datatables.net-responsive/js/dataTables.responsive.min.js",
        "node_modules/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js",
        "node_modules/fontawesome-iconpicker/dist/js/fontawesome-iconpicker.min.js",
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
    images: [],
    imagesOutput: "wwwroot/images/",
    fonts: [
        "node_modules/vazir-font/dist/Vazir*.*",
        "node_modules/shabnam-font/dist/Shabnam*.*"
    ],
    fontsFarsi: [
        "node_modules/vazir-font/dist/font-face.css",
        "node_modules/shabnam-font/dist/font-face.css"
    ],
    fontsOutput: "wwwroot/fonts/"
};

gulp.task("default", ["css:transform", "fonts:farsi", "commoncss:min", "rtlcss:min", "ltrcss:min", "managecss:min", "move:css", "commonjs:min", "rtljs:min", "ltrjs:min", "managejs:min", "move:js", "images:move", "fonts:move"]);
gulp.task("clean", ["transformCss:clean", "css:clean", "js:clean", "images:clean", "fonts:clean"]);

gulp.task("watch", ["default"], function () {
    gulp.watch(filesPath.commonCss, ["commoncss:min"]);
    gulp.watch(filesPath.rtlCss, ["rtlcss:min"]);
    gulp.watch(filesPath.ltrCss, ["ltrcss:min"]);
    gulp.watch(filesPath.manageCss, ["managecss:min"]);
    gulp.watch(filesPath.moveCss, ["move:css"]);
    gulp.watch(filesPath.commonJs, ["commonjs:min"]);
    gulp.watch(filesPath.rtlJs, ["rtljs:min"]);
    gulp.watch(filesPath.ltrJs, ["ltrjs:min"]);
    gulp.watch(filesPath.manageJs, ["managejs:min"]);
    gulp.watch(filesPath.moveJs, ["move:js"]);
    gulp.watch(filesPath.images, ["images:move"]);
    gulp.watch(filesPath.fonts, ["fonts:move"]);
    gulp.watch(filesPath.fontsFarsi, ["fonts:farsi"]);
});

// CSS
gulp.task("css:transform", function () {
    if (filesPath.transformCss.length == 0)
        return;

    return gulp.src(filesPath.transformCss)
        //.pipe(sourcemaps.init())
        //.pipe(autoprefixer(["last 2 versions", "> 1%"])) // Other post-processing.
        .pipe(gulp.dest(filesPath.transformCssOutput + 'ltr/')) // Output LTR stylesheets.
        .pipe(rtlcss()) // Convert to RTL.
        .pipe(rename({ suffix: '-rtl' })) // Append "-rtl" to the filename.
        //.pipe(sourcemaps.write('dist')) // Output source maps.
        .pipe(gulp.dest(filesPath.transformCssOutput + 'rtl/')); // Output RTL stylesheets.
});

gulp.task("transformCss:clean", function () {
    return del([filesPath.transformCssOutput + 'ltr/*', filesPath.transformCssOutput + 'rtl/*']);
});

gulp.task("commoncss:min", function () {
    //gulp.src(filesPath.cssInput + "abovethefold.css")
    //    .pipe(csso())
    //    .pipe(gulp.dest(filesPath.cssOutput));

    //return gulp.src([filesPath.cssInput + "*.css", "!" + filesPath.cssInput + "abovethefold.css"])
    //    .pipe(concat("all.css"))
    //    .pipe(csso())
    //    .pipe(gulp.dest(filesPath.cssOutput));

    if (filesPath.commonCss.length == 0)
        return;

    return gulp.src(filesPath.commonCss)
        .pipe(concat("common.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
});

gulp.task("rtlcss:min", function () {
    if (filesPath.rtlCss.length == 0)
        return;

    return gulp.src(filesPath.rtlCss)
        .pipe(concat("rtl.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
});

gulp.task("ltrcss:min", function () {
    if (filesPath.ltrCss.length == 0)
        return;

    return gulp.src(filesPath.ltrCss)
        .pipe(concat("ltr.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
});

gulp.task("managecss:min", function () {
    if (filesPath.manageCss.length == 0)
        return;

    return gulp.src(filesPath.manageCss)
        .pipe(concat("manage.min.css"))
        .pipe(csso())
        .pipe(gulp.dest(filesPath.cssOutput));
});

gulp.task("move:css", function () {
    if (filesPath.moveCss.length == 0)
        return;

    return gulp.src(filesPath.moveCss)
        .pipe(gulp.dest(filesPath.cssOutput));
});

gulp.task("css:clean", function () {
    return del(filesPath.cssOutput);
});


// JavaScript
gulp.task("commonjs:min", function () {
    //return gulp.src([filesPath.jsInput + "/*.js"])
    //    .pipe(concat("all.js"))
    //    .pipe(uglify())
    //    .pipe(gulp.dest(filesPath.jsOutput));

    if (filesPath.commonJs.length == 0)
        return;

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
            noSource: true
        }))
        .on('error', function (err) { gutil.log(gutil.colors.red('[Error]'), err.toString()); })
        //.pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(filesPath.jsOutput));
});

gulp.task("rtljs:min", function () {
    if (filesPath.rtlJs.length == 0)
        return;

    return gulp.src(filesPath.rtlJs)
        .pipe(concat("rtl.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            noSource: true
        }))
        .pipe(gulp.dest(filesPath.jsOutput));
});

gulp.task("ltrjs:min", function () {
    if (filesPath.ltrJs.length == 0)
        return;

    return gulp.src(filesPath.ltrJs)
        .pipe(concat("ltr.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            noSource: true
        }))
        .pipe(gulp.dest(filesPath.jsOutput));
});

gulp.task("managejs:min", function () {
    if (filesPath.manageJs.length == 0)
        return;

    return gulp.src(filesPath.manageJs)
        .pipe(concat("manage.js"))
        //.pipe(gulp.dest(filesPath.jsOutput))
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            noSource: true
        }))
        .pipe(gulp.dest(filesPath.jsOutput));
});

gulp.task("move:js", function () {
    if (filesPath.moveJs.length == 0)
        return;

    return gulp.src(filesPath.moveJs)
        .pipe(gulp.dest(filesPath.jsOutput));
});

gulp.task("js:clean", function () {
    return del(filesPath.jsOutput);
});

gulp.task('uglify-error-debugging', function (cb) {
    pump([
        gulp.src(filesPath.commonJs),
        uglify(),
        gulp.dest(filesPath.jsOutput)
    ], cb);
});


// Images
gulp.task("images:move", function () {
    return gulp.src(filesPath.images)
                .pipe(gulp.dest(filesPath.imagesOutput));
});

gulp.task("images:clean", function () {
    return del(filesPath.imagesOutput);
});


// Fonts
gulp.task("fonts:move", function () {
    return gulp.src(filesPath.fonts)
                .pipe(gulp.dest(filesPath.fontsOutput));
});

gulp.task("fonts:clean", function () {
    return del(filesPath.fontsOutput);
});

gulp.task("fonts:farsi", function () {
    return gulp.src(filesPath.fontsFarsi)
                .pipe(concat("farsifonts.min.css"))
                .pipe(csso())
                .pipe(gulp.dest(filesPath.fontsOutput));
});
