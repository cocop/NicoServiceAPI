using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.Search
{
    /******************************************/
    /// <summary>動画情報</summary>
    /******************************************/
    public class List
    {
        /// <summary>動画ID</summary>
        [DataMember]
        public string id;

        /// <summary>タイトル</summary>
        [DataMember]
        public string title;

        /// <summary>短い動画説明文</summary>
        [DataMember]
        public string description_short;

        /// <summary>サムネイル画像URL</summary>
        [DataMember]
        public string thumbnail_url;

        /// <summary>投稿日時</summary>
        [DataMember]
        public string first_retrieve;

        /// <summary>再生時間</summary>
        [DataMember]
        public string length;

        /// <summary>再生数</summary>
        [DataMember]
        public int view_counter;

        /// <summary>コメント数</summary>
        [DataMember]
        public int num_res;

        /// <summary>マイリスト数</summary>
        [DataMember]
        public int mylist_counter;
    }
}
