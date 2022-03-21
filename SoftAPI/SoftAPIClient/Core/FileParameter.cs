using System.Linq;

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

        public bool Equals(FileParameter x, FileParameter y)
        {
            return string.Equals(x.Name, y.Name)
                   && string.Equals(x.FileName, y.FileName)
                   && string.Equals(x.ContentType, y.ContentType)
                   && x.Bytes.SequenceEqual(y.Bytes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(this, (FileParameter)obj);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public int GetHashCode(FileParameter obj)
        {
            unchecked
            {
                var hashCode = (obj.Name != null ? obj.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.FileName != null ? obj.FileName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.ContentType != null ? obj.ContentType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Bytes != null ? obj.Bytes.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Name={Name}, FileName={FileName}, ContentType={ContentType}";
        }
    }
}
