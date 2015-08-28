using System;

namespace NicoServiceAPI.NicoVideo
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
        public string Anonymity;

        /// <summary>プレミアムフラグ</summary>
        public bool Premium;

        /// <summary>再生時間（分）</summary>
        public int Leaf;

        ///// <summary>NG共有スコア</summary>
        //public string Scores;/*!*/詳細調べ中

        /// <summary>自分のコメントフラグ</summary>
        public bool YourPost;

        /// <summary>コメント本文</summary>
        public string Body;
    }
}