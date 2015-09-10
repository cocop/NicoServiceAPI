using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetDeflist
{
    /******************************************/
    /// <summary>マイリストレスポンス</summary>
    /******************************************/
    [DataContract]
    public class Contract
    {
        /// <summary>マイリストの動画情報</summary>
        [DataMember]
        public Mylistitem[] mylistitem;

        /// <summary>成功か失敗か</summary>
        [DataMember]
        public string status;

        /// <summary>エラーコード</summary>
        [DataMember]
        public Error error;
    }
}