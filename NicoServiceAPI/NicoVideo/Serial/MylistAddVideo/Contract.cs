using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.MylistAddVideo
{
    /******************************************/
    /// <summary>マイリストへの動画追加レスポンス</summary>
    /******************************************/
    [DataContract]
    public class Contract
    {
        /// <summary>成功か失敗か</summary>
        [DataMember]
        public string status;

        /// <summary>エラーコード</summary>
        [DataMember]
        public Error error;
    }
}