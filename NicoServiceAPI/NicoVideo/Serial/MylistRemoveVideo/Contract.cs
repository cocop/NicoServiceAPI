using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.MylistRemoveVideo
{
    /******************************************/
    /// <summary>マイリストからの動画削除レスポンス</summary>
    /******************************************/
    [DataContract]
    public class Contract
    {
        /// <summary>削除した動画数</summary>
        [DataMember]
        public string delete_count;

        /// <summary>成功か失敗か</summary>
        [DataMember]
        public string status;

        /// <summary>エラーコード</summary>
        [DataMember]
        public Error error;
    }
}