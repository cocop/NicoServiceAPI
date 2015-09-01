using System.Text.RegularExpressions;

namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>HTMLから各種情報を抜き取る</summary>
    /******************************************/
    internal static class HtmlTextRegex
    {
        public static readonly Regex VideoDescription = new Regex("<p class=\"videoDescription description\">(?<value>.*?)</p>");
        public static readonly Regex VideoMylistToken = new Regex("NicoAPI.token = \"(?<value>.*?)\";");
        public static readonly Regex VideoMylistUserInfo = new Regex("(Jarty.globals\\(\\{(?<value>.*?)\\}\\);)", RegexOptions.Singleline);
        public static readonly Regex VideoMylistInfo = new Regex("MylistGroup.preloadSingle\\(.*?, \\{(?<value>.*?)\\}\\);", RegexOptions.Singleline);
        public static readonly Regex VideoMylist = new Regex("Mylist.preload\\(.*?, (?<value>.*?)\\);", RegexOptions.Singleline);

        public static readonly Regex VideoMylistUserInfoCutout = new Regex("base_url: \"(?<base_url>.*?)\",\n\t\tseiga_base_url: \"(?<seiga_base_url>.*?)\",\n\t\tch_base_url: \"(?<ch_base_url>.*?)\",\n\t\tuad_base_url: \"(?<uad_base_url>.*?)\",\n\t\tsecure_base_url: \"(?<secure_base_url>.*?)\",\n\t\there: \"(?<here>.*?)\",\n\t\tis_owner: (?<is_owner>.*?),\n\t\tis_premium : (?<is_premium>.*?),\n\t\tmylist_owner: { user_id: (?<user_id>.*?), nickname: \"(?<nickname>.*?)\" },\n\t\tedit: (?<edit>.*?),\n\t\tfolders: \"(?<folders>.*?)\".split\\(/,/\\),\n\t\tplaylist_stop: (?<playlist_stop>.*?),\n\t\tplaylist_tmp_stop: (?<playlist_tmp_stop>.*?),\n\t\tuse_nicorepo: (?<use_nicorepo>.*?),\n\t\tnicorepo_mode: (?<nicorepo_mode>.*?),\n\t\tfavmylist_available: (?<favmylist_available>.*?),\n\t\twatched_count: '(?<watched_count>.*?)',\n\t\twatching_full: (?<watching_full>.*?),\n\t\twatching_near_limit: (?<watching_near_limit>.*?),\n\t\tis_general_harajuku: (?<is_general_harajuku>.*?),\n\t\tis_watching_this_mylist: (?<is_watching_this_mylist>.*?),\n\t\tmylist_ref_parameter: \"(?<mylist_ref_parameter>.*?)\"");
        public static readonly Regex VideoMylistInfoCutout = new Regex("\n\t\tid: (?<id>.*?),\n\t\tuser_id: (?<user_id>.*?),\n\t\tname: \"(?<name>.*?)\",\n\t\tdescription: \"(?<description>.*?)\",\n\t\tpublic: (?<public>.*?),\n\t\tdefault_sort: (?<default_sort>.*?),\n\t\tcreate_time: (?<create_time>.*?),\n\t\tupdate_time: (?<update_time>.*?),\n\t\ticon_id: (?<icon_id>.*?),\n\t\twatching_list: (?<watching_list>.*?)", RegexOptions.Singleline);
    }
}