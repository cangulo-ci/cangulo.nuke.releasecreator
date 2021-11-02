using cangulo.nuke.releasecreator.Models;
using System;
using System.Linq;

namespace cangulo.nuke.releasecreator.Services
{
    public interface IReleaseNumberParser
    {
        ReleaseNumber Parse(string releaseNumber);
    }

    public class ReleaseNumberParser : IReleaseNumberParser
    {
        private const string InvalidReleaseNumberProvidedMsg = "Invalid release number provided. Please follow the format: X.Y.Z without characters.";

        public ReleaseNumber Parse(string releaseNumber)
        {
            var parts = releaseNumber.Split(".", StringSplitOptions.TrimEntries);

            if (parts.Length != 3 || parts.Any(string.IsNullOrEmpty))
                throw new ArgumentException(InvalidReleaseNumberProvidedMsg);

            var numbers = parts
                .Select(x =>
                {
                    if (int.TryParse(x, out int number))
                        return number;
                    throw new InvalidOperationException(InvalidReleaseNumberProvidedMsg);
                }).ToArray();

            return new ReleaseNumber
            {
                Major = numbers[0],
                Minor = numbers[1],
                Patch = numbers[2]
            };
        }
    }
}
