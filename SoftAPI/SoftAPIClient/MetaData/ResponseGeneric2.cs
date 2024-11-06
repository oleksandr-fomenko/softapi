namespace SoftAPIClient.MetaData
{
    public class ResponseGeneric2<T, T2> : Response
    {
        public ResponseGeneric2(Response response)
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
            Exception = response.Exception;
        }
        public T Body => Deserializer != null ? Deserializer.Convert<T>(ResponseBodyString) : default;
        public T2 Body2 => Deserializer != null ? Deserializer.Convert<T2>(ResponseBodyString) : default;
    }
}
