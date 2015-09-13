using System;

namespace NicoServiceAPI.NicoVideo.Video
{
    /******************************************/
    /// <summary>コメント</summary>
    /******************************************/
    public class Comment
    {
        /// <summary>コメント番号</summary>
        public int No { get; set; }

        /// <summary>再生位置</summary>
        public TimeSpan PlayTime { get; set; }

        /// <summary>投稿時間</summary>
        public DateTime WriteTime { get; set; }

        /// <summary>コマンド</summary>
        public string Command { get; set; }

        /// <summary>ユーザーID</summary>
        public string UserID { get; set; }

        /// <summary>暗号ユーザーIDフラグ</summary>
        public string IsAnonymity { get; set; }

        /// <summary>プレミアムフラグ</summary>
        public bool IsPremium { get; set; }

        /// <summary>再生時間（分）</summary>
        public int Leaf { get; set; }

        /// <summary>NG共有スコア</summary>
        public int Scores { get; set; }

        /// <summary>自分のコメントフラグ</summary>
        public bool IsYourPost { get; set; }

        /// <summary>コメント本文</summary>
        public string Body { get; set; }
    }
}