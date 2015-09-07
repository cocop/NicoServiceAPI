using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetMylistGroup
{
    /******************************************/
    /// <summary>マイリストグループレスポンス</summary>
    /******************************************/
    [DataContract]
    public class Contract
    {
        /// <summary>マイリストグループ</summary>
        [DataMember]
        public MylistGroup[] mylistgroup;

        /// <summary>成功か失敗か</summary>
        [DataMember]
        public string status;

        /// <summary>エラーコード</summary>
        [DataMember]
        public Error error;
    }
}