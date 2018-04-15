using ContentManagement.Entities;
using System.ComponentModel;

namespace ContentManagement.ViewModels.Settings
{
    public class Localization
    {
        public string[] SupportedLanguages { get; set; }
        public Language DefaultLanguage { get; set; }
        public Direction DefaultDirection { get; set; }
    }
}