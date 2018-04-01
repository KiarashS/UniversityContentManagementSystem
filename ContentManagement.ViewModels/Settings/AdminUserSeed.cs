namespace ContentManagement.ViewModels.Settings
{
    public class AdminUserSeed
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string[] RolesName { get; set; }
    }
}