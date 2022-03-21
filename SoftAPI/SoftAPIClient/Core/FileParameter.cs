namespace SoftAPIClient.Core
{
    public class FileParameter
    {
        public string Name { get; }
        public string FileName { get; }
        public byte[] Bytes { get; }
        public string ContentType { get; }

        public FileParameter(string name,
            byte[] bytes,
            string fileName,
            string contentType)
        {
            Name = name;
            FileName = fileName;
            ContentType = contentType;
            Bytes = bytes;
        }

        public override string ToString()
        {
            return $"Name={Name}, FileName={FileName}, ContentType={ContentType}";
        }
    }
}
