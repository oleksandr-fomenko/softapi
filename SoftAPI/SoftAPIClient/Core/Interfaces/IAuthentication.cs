using System.Collections.Generic;

namespace SoftAPIClient.Core.Interfaces
{
    public interface IAuthentication
    {
        KeyValuePair<string, string> GetHeader();
    }
}
