namespace ContentManagement.Common.GuardToolkit.Contracts
{
    public interface IPasswordHasher
    {
        byte[] Hash(string password, byte[] salt);
    }
}