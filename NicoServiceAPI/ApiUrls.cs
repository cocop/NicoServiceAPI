namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>APIのURL</summary>
    /******************************************/
    internal class ApiUrls
    {
        public static string Host = "http://www.nicovideo.jp/";
        public static string Login = "https://secure.nicovideo.jp/secure/login";
        public static string VideoSearch = "http://ext.nicovideo.jp/api/search/{0}/{1}?mode&page={2}&{3}";
        public static string GetVideo = "http://flapi.nicovideo.jp/api/getflv/{0}?as3=1";
        public static string GetVideoInfo = "http://ext.nicovideo.jp/api/getthumbinfo/{0}";
    }
}