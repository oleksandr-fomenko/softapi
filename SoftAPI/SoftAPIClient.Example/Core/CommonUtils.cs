using System.Linq;
using Newtonsoft.Json;

namespace SoftAPIClient.Example.Core
{
    public static class CommonUtils
    {
        public static string WrapToJson(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public static string ReplaceWithStar(string input)
        {
            return input.ToCharArray().Select(s => "*").Aggregate((first, next) => first + next);
        }
    }
}
