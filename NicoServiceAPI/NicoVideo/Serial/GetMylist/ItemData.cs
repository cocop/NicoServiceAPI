using System.Runtime.Serialization;
namespace NicoServiceAPI.NicoVideo.Serial.GetMylist
{
    /******************************************/
    /// <summary>動画情報</summary>
    /******************************************/
    [DataContract]
    public class ItemData
    {
        /// <summary>動画ID</summary>
        [DataMember]
        public string video_id;

        /// <summary>タイトル</summary>
        [DataMember]
        public string title;

        /// <summary>サムネイル画像URL</summary>
        [DataMember]
        public string thumbnail_url;

        /// <summary>投稿日時</summary>
        [DataMember]
        public string first_retrieve;

        /// <summary>更新時間？</summary>
        [DataMember]
        public string update_time;

        /// <summary>再生数</summary>
        [DataMember]
        public string view_counter;

        /// <summary>マイリスト数</summary>
        [DataMember]
        public string mylist_counter;

        /// <summary>コメント数</summary>
        [DataMember]
        public string num_res;

        /// <summary>調査中</summary>
        [DataMember]
        public string group_type;

        /// <summary>動画再生時間、秒単位</summary>
        [DataMember]
        public string length_seconds;

        /// <summary>削除済みかどうか？</summary>
        [DataMember]
        public string deleted;

        /// <summary>最後に投稿されたコメント文</summary>
        [DataMember]
        public string last_res_body;

        /// <summary>調査中、現状video_idと同じ値が入っている</summary>
        [DataMember]
        public string watch_id;
    }
}