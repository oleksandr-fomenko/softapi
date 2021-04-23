using SoftAPIClient.MetaData;

namespace SoftAPIClient.Core.Interfaces
{
    public interface IRestLogger
    {
        void LogBefore(string message);
        void LogRequest(Request request);
        void LogResponse(Response response);
    }
}
