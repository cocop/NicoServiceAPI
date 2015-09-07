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
            "<packet><thread thread=\"{0}\" version=\"20090904\"/><thread_leaves scores=\"1\" thread=\"{0}\">0-100:100,1000</thread_leaves></packet>";

        //ここからマイリスト

        public const string ArrayMylistItem = 
            "id_list[0][]={0}&";

        public const string DeflistAddVideo =
            "item_type=0&item_id={0}&description={1}&token={2}";

        public const string DeflistRemoveVideo =
            "{0}token={1}";

        public const string DeflistMoveVideo =
            "{0}token={1}";

        public const string DeflistCopyVideo =
            "{0}token={1}";


        public const string MylistAddVideo =
            "group_id={0}&item_type=0&item_id={1}&description={2}&item_amc={3}&token={4}";

        public const string MylistRemoveVideo =
            "group_id={0}&{1}token={2}";

        public const string MylistMoveVideo =
            "group_id={0}&target_group_id={1}&{2}token={3}";

        public const string MylistCopyVideo =
            "group_id={0}&target_group_id={1}&{2}token={3}";

        //ここまでマイリスト
    }
}