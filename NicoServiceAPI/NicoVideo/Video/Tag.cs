namespace NicoServiceAPI.NicoVideo.Video
{
    /******************************************/
    /// <summary>タグ</summary>
    /******************************************/
    public class Tag
    {
        /// <summary>タグ名</summary>
        public string Name { set; get; }

        /// <summary>カテゴリ</summary>
        public bool Category { set; get; }

        /// <summary>タグロック</summary>
        public bool Lock { set; get; }
    }
}