namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>APIのURL</summary>
    /******************************************/
    internal static class ApiUrls
    {
        public const string Host = "http://www.nicovideo.jp/";
        public const string Login = "https://secure.nicovideo.jp/secure/login";
        public const string VideoSearch = "http://ext.nicovideo.jp/api/search/{0}/{1}?mode&page={2}&{3}";
        public const string GetVideo = "http://flapi.nicovideo.jp/api/getflv/{0}?as3=1";
        public const string GetVideoInfo = "http://ext.nicovideo.jp/api/getthumbinfo/{0}";
    }
}