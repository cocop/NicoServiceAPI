using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.UpdateMylist
{
    /******************************************/
    /// <summary>マイリスト更新レスポンス</summary>
    /******************************************/
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
