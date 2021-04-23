using SoftAPIClient.MetaData;

namespace SoftAPIClient.Core.Interfaces
{
    public interface IResponseInterceptor
    {
        void ProcessResponse(Response response);
    }
}
