
namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>投稿時間フィルタ</summary>
    /******************************************/
    public enum PostTimeFilter
    {
        /// <summary>指定なし</summary>
        None,

        /// <summary>投稿時間が24時間以内の動画のみ含める</summary>
        WithinDay,

        /// <summary>投稿時間が一週間以内の動画のみ含める</summary>
        WithinWeek,

        /// <summary>投稿時間が一ヶ月以内の動画のみ含める</summary>
        WithinMonth,
    }
}
