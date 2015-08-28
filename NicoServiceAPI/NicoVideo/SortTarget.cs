
namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>ソートに使用する値を指定する</summary>
    /******************************************/
    public enum SortTarget
    {
        /// <summary>指定なし</summary>
        None,

        /// <summary>最新のコメント</summary>
        Comment,

        /// <summary>再生数</summary>
        ViewCount,

        /// <summary>コメント数</summary>
        CommentCount,

        /// <summary>マイリスト数</summary>
        MylistCount,

        /// <summary>投稿日時</summary>
        PostTime,

        /// <summary>動画再生時間</summary>
        VideoTime,
    }
}
