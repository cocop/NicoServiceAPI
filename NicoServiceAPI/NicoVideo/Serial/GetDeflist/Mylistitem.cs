using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetDeflist
{
    /******************************************/
    /// <summary>マイリストの動画情報</summary>
    /******************************************/
    [DataContract]
    public class Mylistitem
    {
        /// <summary>調査中</summary>
        [DataMember]
        public string item_type;

        /// <summary>説明文</summary>
        [DataMember]
        public string description;

        /// <summary>動画情報</summary>
        [DataMember]
        public ItemData item_data;

        /// <summary>調査中</summary>
        [DataMember]
        public string watch;

        /// <summary>作成時間？</summary>
        [DataMember]
        public string create_time;

        /// <summary>更新時間？</summary>
        [DataMember]
        public string update_time;
    }
}
