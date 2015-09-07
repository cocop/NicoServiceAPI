using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetMylistGroup
{
    /******************************************/
    /// <summary></summary>
    /******************************************/
    public class MylistGroup
    {
        /// <summary>マイリストID</summary>
        [DataMember]
        public string id;

        /// <summary>ユーザーID</summary>
        [DataMember]
        public string user_id;

        /// <summary>名前</summary>
        [DataMember]
        public string name;

        /// <summary>説明文</summary>
        [DataMember]
        public string description;

        /// <summary>公開マイリストか</summary>
        [DataMember(Name = "public")]
        public bool _public;

        /// <summary>デフォルトソート順</summary>
        [DataMember]
        public int default_sort;

        /// <summary>作成時間</summary>
        [DataMember]
        public int create_time;

        /// <summary>更新時間</summary>
        [DataMember]
        public int update_time;

        /// <summary>ソート順</summary>
        [DataMember]
        public int sort_orderl;

        /// <summary>アイコンID</summary>
        [DataMember]
        public int icon_id;
    }
}