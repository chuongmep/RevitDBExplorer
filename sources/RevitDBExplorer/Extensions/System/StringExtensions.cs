﻿// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System
{
    internal static class StringExtensions
    {
        /// <summary>
        /// source: https://www.dotnetperls.com/remove-html-tags
        /// </summary>
        public static string StripTags(this string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// source: https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string/30732794#30732794
        /// </summary>
        public static string RemoveWhitespace(this string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }


        /// <summary>
        /// source: https://stackoverflow.com/questions/5284591/how-to-remove-a-suffix-from-end-of-string
        /// </summary>
        public static string RemoveFromEnd(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }

        public static string RemovePrefix(this string input, string prefix)
        {
            if (input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return input.Remove(0, prefix.Length);
            }
            return input;
        }
    }
}