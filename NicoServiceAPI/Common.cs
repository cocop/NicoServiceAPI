using NicoServiceAPI.Connection;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>汎用処理</summary>
    /******************************************/
    internal static class Common
    {
        /// <summary>Unicodeエスケープシーケンスをデコードする</summary>
        public static string UnicodeDecode(string Text)
        {
            return Regex.Replace(Text, "\\\\u(?<value>[0-9a-fA-F]{4})", (match) =>
                    ((char)Convert.ToInt32(match.Groups["value"].Value, 16)).ToString());
        }
    }
}