using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.Search
{
    /******************************************/
    /// <summary>動画情報レスポンス</summary>
    /******************************************/
    [DataContract]
    public class Contract
    {
        /// <summary>動画情報のリスト</summary>
        [DataMember]
        public List[] list;

        /// <summary>指定した条件で宣伝された動画を取得できるか</summary>
        [DataMember]
        public bool has_ng_video_for_adsense_on_listing;

        /// <summary>検索したタグの説明文</summary>
        [DataMember]
        public Nicopedia nicopedia;

        /// <summary>検索したタグ一覧</summary>
        [DataMember]
        public string[] tags;

        /// <summary>成功か失敗か</summary>
        [DataMember]
        public string status;

        /// <summary>失敗した場合のメッセージ</summary>
        [DataMember]
        public string message;
    }
}