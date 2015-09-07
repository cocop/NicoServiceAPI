using System;

namespace NicoServiceAPI.NicoVideo.Video
{
    /******************************************/
    /// <summary>コメント</summary>
    /******************************************/
    public class Comment
    {
        /// <summary>コメント番号</summary>
        public int No;

        /// <summary>再生位置</summary>
        public TimeSpan PlayTime;

        /// <summary>投稿時間</summary>
        public DateTime WriteTime;

        /// <summary>コマンド</summary>
        public string Command;

        /// <summary>ユーザーID</summary>
        public string UserID;

        /// <summary>暗号ユーザーIDフラグ</summary>
        public string IsAnonymity;

        /// <summary>プレミアムフラグ</summary>
        public bool IsPremium;

        /// <summary>再生時間（分）</summary>
        public int Leaf;

        /// <summary>NG共有スコア</summary>
        public int Scores;

        /// <summary>自分のコメントフラグ</summary>
        public bool IsYourPost;

        /// <summary>コメント本文</summary>
        public string Body;
    }
}