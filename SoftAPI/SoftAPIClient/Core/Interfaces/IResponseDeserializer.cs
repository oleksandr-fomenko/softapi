namespace SoftAPIClient.Core.Interfaces
{
    public interface IResponseDeserializer
    {
        T Convert<T>(string response);
    }
}
