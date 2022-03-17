using SoftAPIClient.MetaData;

namespace SoftAPIClient.Core.Interfaces
{
    public interface IRequestInterceptor
    {
        Request Intercept();
    }
}
