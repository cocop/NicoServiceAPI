using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.AddMylist
{
    /// <summary>マイリスト追加レスポンス</summary>
    [DataContract]
    public class Contract
    {
        /// <summary>追加したマイリストのID</summary>
        [DataMember]
        public string id;

        /// <summary>成功か失敗か</summary>
        [DataMember]
        public string status;

        /// <summary>エラーコード</summary>
        [DataMember]
        public Error error;
    }
}
