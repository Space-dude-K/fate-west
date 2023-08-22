using System;

namespace wc3_fate_west_parser_replay_parser.Utility
{
    public static class StringExtensions
    {
        public static string FirstFromSplit(this string source, char delimiter)
        {
            var i = source.IndexOf(delimiter);

            return i == -1 ? source : source.Substring(0, i);
        }

        public static bool EqualsIgnoreCase(this string source, string target)
        {
            return String.Equals(source, target, StringComparison.OrdinalIgnoreCase);
        }
    }
}