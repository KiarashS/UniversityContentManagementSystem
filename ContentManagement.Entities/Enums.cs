
using ContentManagement.Common.WebToolkit.Attributes;

namespace ContentManagement.Entities
{
    public enum Language
    {
        //[Description("fa-IR")]
        [LanguageTextInAdmin("فارسی")]
        FA = 1,
        [LanguageTextInAdmin("انگلیسی")]
        EN = 2
    }

    public enum Direction
    {
        RightToLeft = 1,
        LeftToRight = 2
    }

    public enum LinkType
    {
        [LinkTypeTextInAdmin("لینک ضروری")]
        Essential          = 1,
        [LinkTypeTextInAdmin("لینک دسترسی سریع")]
        Quick              = 2,
        [LinkTypeTextInAdmin("لینک مفید")]
        Useful             = 3,
        [LinkTypeTextInAdmin("لینک خدمات الکترونیک")]
        ElectronicService  = 4,
        [LinkTypeTextInAdmin("لینک منابع الکترونیک")]
        ElectronicResource = 5,
        [LinkTypeTextInAdmin("لینک خروجی (پیوندها)")]
        External           = 6,
        [LinkTypeTextInAdmin("لینک هدر(بخش بالایی)")]
        Header             = 7,
        [LinkTypeTextInAdmin("لینک فوتر(بخش پایینی)")]
        Footer             = 8
    }

    public enum ContentType
    {
        [ContentTypeTextInAdmin("اخبار")]
        [ContentTypeTitleInAdmin("خبر")]
        [ContentTypeTextFa("خبر")]
        [ContentTypeTextEn("News")]
        News                    = 1,  // اخبار

        [ContentTypeTextInAdmin("اطلاعیه")]
        [ContentTypeTitleInAdmin("اطلاعیه")]
        [ContentTypeTextFa("اطلاعیه")]
        [ContentTypeTextEn("Announcement")]
        Announcement = 2,  // اطلاعیه

        [ContentTypeTextInAdmin("رویدادهای آینده")]
        [ContentTypeTitleInAdmin("رویداد")]
        [ContentTypeTextFa("رویدادهای آینده")]
        [ContentTypeTextEn("Future Event")]
        UpcomingEvent = 3,  // رویدادهای آینده

        [ContentTypeTextInAdmin("دستورالعمل ها")]
        [ContentTypeTitleInAdmin("دستورالعمل")]
        [ContentTypeTextFa("دستورالعمل")]
        [ContentTypeTextEn("Regulation")]
        Regulation = 4,  // دستورالعمل ها و آیین نامه ها

        [ContentTypeTextInAdmin("همایش ها و سمینارها")]
        [ContentTypeTitleInAdmin("همایش/سمینار")]
        [ContentTypeTextFa("همایش/سمینار")]
        [ContentTypeTextEn("Congress/Seminar")]
        Congress = 5,  // همایش ها و سمینارها

        [ContentTypeTextInAdmin("انتصابات")]
        [ContentTypeTitleInAdmin("اخبر نتصاب")]
        [ContentTypeTextFa("انتصابات")]
        [ContentTypeTextEn("Appointment")]
        Appointment = 6,  // انتصابات

        [ContentTypeTextInAdmin("تحقیق ها و پژوهش ها")]
        [ContentTypeTitleInAdmin("خبر تحقیقی/پژوهشی")]
        [ContentTypeTextFa("تحقیق/پژوهش")]
        [ContentTypeTextEn("Research")]
        Research = 7,  // تحقیق ها و پژوهش ها

        [ContentTypeTextInAdmin("نشریه ها")]
        [ContentTypeTitleInAdmin("خبر نشریه ای")]
        [ContentTypeTextFa("نشریه")]
        [ContentTypeTextEn("Journal")]
        Journal = 8,  // نشریه ها

        [ContentTypeTextInAdmin("فرم های آموزشی و پژوهشی")]
        [ContentTypeTitleInAdmin("فرم آموزشی/پژوهشی")]
        [ContentTypeTextFa("فرم ها")]
        [ContentTypeTextEn("Forms")]
        Form = 9,  // فرم های آموزشی و پژوهشی  و ...

        [ContentTypeTextInAdmin("فراخوان ها")]
        [ContentTypeTitleInAdmin("فراخوان")]
        [ContentTypeTextFa("فراخوان")]
        [ContentTypeTextEn("Recall")]
        Recall = 10, // فراخوان ها

        [ContentTypeTextInAdmin("تقویم آموزشی")]
        [ContentTypeTitleInAdmin("تقویم آموزشی")]
        [ContentTypeTextFa("تقویم آموزشی")]
        [ContentTypeTextEn("Educational Calendar")]
        EducationalCalendar = 11, // تقویم آموزشی

        [ContentTypeTextInAdmin("آموزش")]
        [ContentTypeTitleInAdmin("خبر آموزشی")]
        [ContentTypeTextFa("آموزش")]
        [ContentTypeTextEn("Education")]
        Education = 12, // آموزش

        [ContentTypeTextInAdmin("پژوهش و فناوری")]
        [ContentTypeTitleInAdmin("خبر پژوهشی و فناوری")]
        [ContentTypeTextFa("پژوهش و فناوری")]
        [ContentTypeTextEn("Research and Technology")]
        ResearchAndTechnology = 13, // پژوهش و فناوری

        [ContentTypeTextInAdmin("دانشجویی و فرهنگی")]
        [ContentTypeTitleInAdmin("خبر دانشجویی و فرهنگی")]
        [ContentTypeTextFa("دانشجویی و فرهنگی")]
        [ContentTypeTextEn("Student and Cultural")]
        StudentAndCultural = 14, // دانشجویی و فرهنگی

        [ContentTypeTextInAdmin("مالی")]
        [ContentTypeTitleInAdmin("خبر مالی")]
        [ContentTypeTextFa("مالی")]
        [ContentTypeTextEn("Financial")]
        Financial = 15, // مالی

        [ContentTypeTextInAdmin("عقیدتی سیاسی")]
        [ContentTypeTitleInAdmin("خبر عقیدتی سیاسی")]
        [ContentTypeTextFa("عقیدتی سیاسی")]
        [ContentTypeTextEn("Political and Ideological")]
        PoliticalAndIdeological = 16, // عقیدتی سیاسی

        [ContentTypeTextInAdmin("آموزش مجازی")]
        [ContentTypeTitleInAdmin("خبر آموزش مجازی")]
        [ContentTypeTextFa("آموزش مجازی")]
        [ContentTypeTextEn("Virtual Learning")]
        VirtualLearning = 17  // آموزش مجازی
    }
}
