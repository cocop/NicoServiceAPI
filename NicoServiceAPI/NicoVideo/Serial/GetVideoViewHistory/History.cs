using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetVideoViewHistory
{
    /******************************************/
    /// <summary>動画視聴履歴</summary>
    /******************************************/
    public class History
    {
        /// <summary>削除されているかどうか</summary>
        [DataMember]
        public int deleted;

        /// <summary>詳細調べ中/*!*/</summary>
        [DataMember]
        public int device;

        /// <summary>動画ID</summary>
        [DataMember]
        public string item_id;

        /// <summary>再生時間</summary>
        [DataMember]
        public string length;

        /// <summary>サムネイルURL</summary>
        [DataMember]
        public string thumbnail_url;

        /// <summary>タイトル</summary>
        [DataMember]
        public string title;

        /// <summary>動画ID</summary>
        [DataMember]
        public string video_id;

        /// <summary>自分が再生した再生数</summary>
        [DataMember]
        public int watch_count;

        /// <summary>調べ中/*!*/</summary>
        [DataMember]
        public string watch_date;
    }
}
