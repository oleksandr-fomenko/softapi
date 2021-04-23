namespace SoftAPIClient.MetaData
{
    public class ResponseGeneric<T> : Response
    {
        public ResponseGeneric(Response response)
        {
            HttpStatusCode = response.HttpStatusCode;
            ResponseUri = response.ResponseUri;
            Headers = response.Headers;
            Cookies = response.Cookies;
            ContentType = response.ContentType;
            OriginalRequest = response.OriginalRequest;
            OriginalResponse = response.OriginalResponse;
            ResponseBodyString = response.ResponseBodyString;
            ElapsedTime = response.ElapsedTime;
            Deserializer = response.Deserializer;
        }
        public T Body => Deserializer != null ? Deserializer.Convert<T>(ResponseBodyString) : default;
    }
}
