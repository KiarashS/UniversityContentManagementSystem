using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Services.Contracts
{
    public interface ISecurityService
    {
        string GetSha256Hash(string input);
    }
}
