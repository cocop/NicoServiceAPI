using System;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>検索タイプ</summary>
    /******************************************/
    public enum SearchType
    {
        /// <summary>キーワード検索</summary>
        Word,

        /// <summary>タグ検索</summary>
        Tag,
    }
    internal static class SearchTypeForMethod
    {
        /// <summary>キーの取得</summary>
        public static string ToKey(this SearchType SearchType)
        {
            switch (SearchType)
            {
                case SearchType.Word:   return "search";
                case SearchType.Tag:    return "tag";
                default:
                    throw new Exception("設定したサーチオプションが不正です");
            }
        }
    }

}
