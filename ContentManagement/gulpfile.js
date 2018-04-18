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
        "node_modules/font-awesome/css/font-awesome.min.css",
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
    transformCss: [
        "node_modules/bootstrap/dist/css/bootstrap.css",
        "node_modules/startbootstrap-sb-admin/css/sb-admin.css"
    ],
    cssOutput: "wwwroot/css/",
    transformCssOutput: "assets/css/transform/",

    // JS files
    commonJs: [
        "node_modules/jquery/dist/jquery.min.js",
        "node_modules/jquery-validation/dist/jquery.validate.min.js",
        "node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js",
        "node_modules/jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.min.js",
        "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js",
        "node_modules/startbootstrap-sb-admin/js/sb-admin.js"
    ],
    rtlJs: [],
    ltrJs: [],
    jsOutput: "wwwroot/js/",

    // etc. images, fonts, ...
    images: [],
    imagesOutput: "wwwroot/images/",
    fonts: ["node_modules/font-awesome/fonts/*"],
    fontsOutput: "wwwroot/fonts/"
};

gulp.task("default", ["css:transform", "commoncss:min", "rtlcss:min", "ltrcss:min", "commonjs:min", "rtljs:min", "ltrjs:min", "images:move", "fonts:move"]);
gulp.task("clean", ["transformCss:clean", "css:clean", "js:clean", "images:clean", "fonts:clean"]);

gulp.task("watch", ["default"], function () {
    gulp.watch(filesPath.commonCss, ["commoncss:min"]);
    gulp.watch(filesPath.rtlCss, ["rtlcss:min"]);
    gulp.watch(filesPath.ltrCss, ["ltrcss:min"]);
    gulp.watch(filesPath.commonJs, ["commonjs:min"]);
    gulp.watch(filesPath.rtlJs, ["rtljs:min"]);
    gulp.watch(filesPath.ltrJs, ["ltrjs:min"]);
    gulp.watch(filesPath.images, ["images:move"]);
    gulp.watch(filesPath.fonts, ["fonts:move"]);
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
