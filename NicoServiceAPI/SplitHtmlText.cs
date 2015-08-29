namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>HTML解析用分割テキスト</summary>
    /******************************************/
    public static class SplitHtmlText
    {
        public static string[] VideoDescription
        {
            get
            {
                return new string[]
                {
                    "<p class=\"videoDescription description\">",
                    "</p><div class=\"videoMainInfoContainer\">"
                };

            }
        }
    }
}