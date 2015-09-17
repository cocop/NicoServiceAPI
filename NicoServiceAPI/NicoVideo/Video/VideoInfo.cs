using System;
using System.Collections.Specialized;

namespace NicoServiceAPI.NicoVideo.Video
{
    /******************************************/
    /// <summary>動画情報</summary>
    /******************************************/
    public class VideoInfo
    {
        /// <summary>内部生成時、使用される。</summary>
        internal VideoInfo()
        {
        }

        /// <summary>IDを指定して作成する。</summary>
        public VideoInfo(string ID)
        {
            this.ID = ID;
        }

        /******************************************/
        /******************************************/

        /// <summary>投稿者ユーザー</summary>
        public User.User User { set; get; }

        /// <summary>動画ID</summary>
        public string ID { set; get; }

        /// <summary>タイトル</summary>
        public string Title { set; get; }

        /// <summary>短い動画説明文</summary>
        public string ShortDescription { set; get; }

        /// <summary>サムネイル画像</summary>
        public Picture Thumbnail { set; get; }

        /// <summary>投稿日時</summary>
        public DateTime PostTime { set; get; }

        /// <summary>再生時間</summary>
        public TimeSpan Length { set; get; }

        /// <summary>再生数</summary>
        public int ViewCounter { set; get; }

        /// <summary>コメント数</summary>
        public int ComentCounter { set; get; }

        /// <summary>マイリスト数</summary>
        public int MylistCounter { set; get; }

        /// <summary>動画説明文</summary>
        public string Description { set; get; }

        /// <summary>タグ</summary>
        public Tag[] Tags { set; get; }

        /// <summary>動画の形式</summary>
        public string VideoType { set; get; }

        /// <summary>動画サイズ</summary> 
        public int VideoSize { set; get; }

        /// <summary>エコノミー時の動画サイズ、0である場合はVideoSizeと同じ</summary>
        public int EconomyVideoSize { set; get; }

        /// <summary>エコノミー時はエコノミー動画のサイズが、そうでなければ通常の動画サイズが取得できる</summary>
        public int UseVideoSize
        {
            get
            {
                if (EconomyVideoSize == 0)
                    return VideoSize;
                else
                    return EconomyVideoSize;
            }
        }

        /// <summary>ニコ生で再生できるかどうか</summary>
        public bool IsLivePlay { set; get; }

        /// <summary>外部再生の可否</summary>
        public bool IsExternalPlay { set; get; }

        /// <summary>動画ページ</summary>
        internal VideoPage videoPage { get; set; }
    }
}