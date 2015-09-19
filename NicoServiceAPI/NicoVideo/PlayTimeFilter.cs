
namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>再生時間フィルタ</summary>
    /******************************************/
    public enum PlayTimeFilter
    {
        /// <summary>指定なし</summary>
        None,

        /// <summary>再生時間が5分以内の動画のみ含める</summary>
        Short,

        /// <summary>再生時間が20分以上の動画のみ含める</summary>
        Long,
    }
}
