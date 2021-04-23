using SoftAPIClient.MetaData;

namespace SoftAPIClient.Core.Interfaces
{
    public interface IInterceptor
    {
        Request Intercept();
    }
}
