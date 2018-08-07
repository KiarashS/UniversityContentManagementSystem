using System;
using Microsoft.AspNetCore.Identity;

namespace ContentManagement.ViewModels.Settings
{
    public class SiteSettings
    {
        public AdminUserSeed AdminUserSeed { get; set; }
        public Logging Logging { get; set; }
        public string GoogleAnalyticsTrackingId { get; set; }
        public string DomainName { get; set; }
        public string WebMailAddress { get; set; }
        public string BaseColor { get; set; }
        public string[] SubDomainsBanList { get; set; }
        public PagesSize PagesSize { get; set; }
        public MainPortal MainPortal { get; set; }
        public Localization Localization { get; set; }
        public int LoginCookieExpirationDays { get; set; }
        public SocialNetworks SocialNetworks { get; set; }
        public Smtp Smtp { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public bool EnableEmailConfirmation { get; set; }
        public TimeSpan EmailConfirmationTokenProviderLifespan { get; set; }
        public int NotAllowedPreviouslyUsedPasswords { get; set; }
        public int ChangePasswordReminderDays { get; set; }
        public PasswordOptions PasswordOptions { get; set; }
        public ActiveDatabase ActiveDatabase { get; set; }
        public string UsersAvatarsFolder { get; set; }
        public string UserDefaultPhoto { get; set; }
        public CookieOptions CookieOptions { get; set; }
        public LockoutOptions LockoutOptions { get; set; }
        public UserAvatarImageOptions UserAvatarImageOptions { get; set; }
        public string[] EmailsBanList { get; set; }
        public string[] PasswordsBanList { get; set; }
    }
}