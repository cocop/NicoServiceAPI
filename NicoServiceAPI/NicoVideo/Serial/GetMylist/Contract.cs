using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetMylist
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
    }
}