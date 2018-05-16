﻿
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
        External           = 6
    }

    public enum ContentType
    {
        News                    = 1,  // اخبار
        Announcement            = 2,  // اطلاعیه
        UpcomingEvent           = 3,  // رویداد های آینده
        Regulation              = 4,  // دستورالعمل ها و آیین نامه ها
        Congress                = 5,  // همایش ها و سمینارها
        Appointment             = 6,  // انتصابات
        Research                = 7,  // تحقیق ها و پژوهش ها
        Journal                 = 8,  // نشریه ها
        Form                    = 9,  // فرم های آموزشی و پژوهشی  و ...
        Recall                  = 10, // فراخوان ها
        EducationalCalendar     = 11, // تقویم آموزشی
        Education               = 12, // آموزش
        ResearchAndTechnology   = 13, // پژوهش و فناوری
        StudentAndCultural      = 14, // دانشجویی و فرهنگی
        Financial               = 15, // مالی
        PoliticalAndIdeological = 16  // عقیدتی سیاسی
    }
}
