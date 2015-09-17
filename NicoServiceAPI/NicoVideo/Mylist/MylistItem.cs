using System;

namespace NicoServiceAPI.NicoVideo.Mylist
{
    /******************************************/
    /// <summary>マイリストアイテム</summary>
    /******************************************/
    public class MylistItem
    {
        /// <summary>説明文</summary>
        public string Description;

        /// <summary>登録時間</summary>
        public DateTime RegisterTime;

        /// <summary>更新時間</summary>
        public DateTime UpdateTime;

        /// <summary>動画情報</summary>
        public Video.VideoInfo VideoInfo;
    }
}