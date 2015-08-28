namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>タグ</summary>
    /******************************************/
    public class Tag
    {
        /// <summary>タグ名</summary>
        public string Name { set; get; }

        /// <summary>カテゴリ</summary>
        public int Category { set; get; }

        /// <summary>タグロック</summary>
        public int Lock { set; get; }
    }
}