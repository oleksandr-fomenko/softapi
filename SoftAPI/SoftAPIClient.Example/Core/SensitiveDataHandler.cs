using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SoftAPIClient.Core.Exceptions;

namespace SoftAPIClient.Example.Core
{
    public static class SensitiveDataHandler
    {
        private static readonly char SplitCharter = char.Parse("=");
        private static readonly List<string> SensitiveKeys = new List<string>
        {
            "Authorization",
        };

        public static string HideSensitiveData(string input)
        {
            var linesToReplace = SensitiveKeys.SelectMany(key => FindLines(input, key)).ToList();
            var modifiedLines = linesToReplace.Select(line =>
            {
                var splitArray = line.Split(SplitCharter);
                if (splitArray.Length < 2)
                {
                    throw new InitializationException($"Can't hide sensitive data. The following line '{line}' is not specified of the format: key{SplitCharter}value");
                }
                var dataToReplace = splitArray.Skip(1).Aggregate((first, next) => first + next);
                return splitArray[0] + SplitCharter + CommonUtils.ReplaceWithStar(dataToReplace);
            }).ToList();

            for (var i = 0; i < linesToReplace.Count; i++)
            {
                var initialLine = linesToReplace[i];
                var newLine = modifiedLines[i];
                input = input.Replace(initialLine, newLine);
            }

            return input;
        }

        private static IEnumerable<string> FindLines(string input, string key)
        {
            var result = Regex.Split(input, "\r\n|\r|\n").ToList();
            return result.Where(r => r.StartsWith(key));
        }
    }
}
