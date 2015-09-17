using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.EditTag
{
    /******************************************/
    /// <summary>タグ編集レスポンス</summary>
    /******************************************/
    [DataContract]
    public class Contract
    {
        /// <summary>詳細調べ中/*!*/</summary>
        [DataMember]
        public bool is_owner;

        /// <summary>詳細調べ中/*!*/</summary>
        [DataMember]
        public bool isuneditable_tag;

        /// <summary>タグ情報</summary>
        [DataMember]
        public Tags[] tags;

        /// <summary>成功か失敗か</summary>
        [DataMember]
        public string status;

        /// <summary>エラーコード</summary>
        [DataMember]
        public Error error;
    }
}