using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetMylist
{
    /******************************************/
    /// <summary>マイリストアイテム</summary>
    /******************************************/
    [DataContract]
    public class List
    {
        /// <summary>サムネイルURL</summary>
        [DataMember]
        public string thumbnail_url;

        /// <summary>動画時間</summary>
        [DataMember]
        public string length;

        /// <summary>動画時間秒単位</summary>
        [DataMember]
        public int length_seconds;

        /// <summary>タイトル</summary>
        [DataMember]
        public string title;

        /// <summary>再生数</summary>
        [DataMember]
        public int view_counter;

        /// <summary>コメント数</summary>
        [DataMember]
        public int num_res;

        /// <summary>マイリストにされた数</summary>
        [DataMember]
        public int mylist_counter;

        /// <summary>投稿時間</summary>
        [DataMember]
        public string first_retrieve;

        /// <summary>短い動画説明文</summary>
        [DataMember]
        public string description_short;

        /// <summary>最後に投稿されたコメント</summary>
        [DataMember]
        public string last_res_body;

        /// <summary>サムネイルスタイル</summary>
        [DataMember]
        public ThumbnailStyle thumbnail_style;

        /// <summary>中央のサムネイル</summary>
        [DataMember]
        public string is_middle_thumbnail;

        /// <summary>動画ID</summary>
        [DataMember]
        public string id;

        /// <summary>登録時間</summary>
        [DataMember]
        public int create_time;

        /// <summary>更新時間</summary>
        [DataMember]
        public string thread_update_time;

        /// <summary>マイリストコメント</summary>
        [DataMember]
        public string mylist_comment;
    }
}