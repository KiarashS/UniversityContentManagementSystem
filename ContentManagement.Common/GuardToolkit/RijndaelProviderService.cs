using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ContentManagement.Common.GuardToolkit
{
    // http://www.dotnettips.info/post/2519
    public static class RijndaelProviderServiceExtensions
    {
        public static IServiceCollection AddRijndaelProviderService(this IServiceCollection services)
        {
            services.AddSingleton<IRijndaelProviderService, RijndaelProviderService>();
            return services;
        }
    }

    public interface IRijndaelProviderService
    {
        string Decrypt(string inputText, string key, string salt);
        string Encrypt(string inputText, string key, string salt);
    }

    public class RijndaelProviderService : IRijndaelProviderService
    {
        public string Decrypt(string inputText, string key, string salt)
        {
            var inputBytes = Convert.FromBase64String(inputText);
            var pdb = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(salt));

            using (var ms = new MemoryStream())
            {
                var alg = Aes.Create();

                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);

                using (var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputBytes, 0, inputBytes.Length);
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public string Encrypt(string inputText, string key, string salt)
        {

            var inputBytes = Encoding.UTF8.GetBytes(inputText);
            var pdb = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(salt));
            using (var ms = new MemoryStream())
            {
                var alg = Aes.Create();

                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);

                using (var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputBytes, 0, inputBytes.Length);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
