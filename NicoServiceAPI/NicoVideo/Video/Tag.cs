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
        public bool IsCategory { set; get; }

        /// <summary>大百科</summary>
        public bool IsNicopedia { set; get; }

        /// <summary>タグロック</summary>
        public bool IsLock { set; get; }
    }
}