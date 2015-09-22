using System;

namespace NicoServiceAPI.Connection
{
    /******************************************/
    /// <summary>ポストするコンテンツタイプ</summary>
    /******************************************/
    internal enum ContentType
    {
        None,
        Form,
        XML,
    }
    internal static class ContentTypeForMethod
    {
        /// <summary>キーの取得</summary>
        public static string ToKey(this ContentType ContentType)
        {
            switch (ContentType)
            {
                case ContentType.None: return "";
                case ContentType.Form: return "application/x-www-form-urlencoded";
                case ContentType.XML: return "text/xml";
                default:
                    throw new Exception("コンテンツタイプが不正です");
            }
        }
    }
}