namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>APIへポストするテキスト</summary>
    /******************************************/
    internal static class PostTexts
    {
        public const string Login =
            "mail_tel={0}&password={1}";

        public const string GetVideoComment =
            "<packet><thread thread=\"{0}\" version=\"20090904\"/><thread_leaves scores=\"1\" thread=\"{0}\">0-{1}:100,1000</thread_leaves></packet>";
    }
}