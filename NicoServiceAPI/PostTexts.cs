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
            "thread?version=20090904&thread={0}&res_from=1";

        public const string PostVideoComment =
            "<chat thread=\"{0}\" vpos=\"{1}\" mail=\"{2}\" ticket=\"{3}\" user_id=\"{4}\" postkey=\"{5}\">{6}</chat>";

        public const string AddVideoTag =
            "res_type=json&cmd=add&tag={0}&id=undefined&token={1}&watch_auth_key={2}&owner_lock={3}";

        public const string RemoveVideoTag =
            "res_type=json&cmd=remove&tag={0}&id=undefined&token={1}&watch_auth_key={2}&owner_lock={3}";

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