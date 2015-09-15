using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetVideoViewHistory
{
    /******************************************/
    /// <summary>動画視聴履歴レスポンス</summary>
    /******************************************/
    [DataContract]
    public class Contract
    {
        /// <summary>動画視聴履歴</summary>
        [DataMember]
        public History[] history;

        /// <summary>成功か失敗か</summary>
        [DataMember]
        public string status;

        /// <summary>エラーコード</summary>
        [DataMember]
        public Error error;

        /// <summary>マイリスト操作用トークン</summary>
        [DataMember]
        public string token;
    }
}