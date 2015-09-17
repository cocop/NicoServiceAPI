using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.EditTag
{
    public class Tags
    {
        /// <summary>タグID？要確認/*!*/</summary>
        [DataMember]
        public string id;

        /// <summary>タグ名</summary>
        [DataMember]
        public string tag;

        /// <summary>タグロック</summary>
        [DataMember]
        public int owner_lock;

        /// <summary>カテゴリであるか</summary>
        [DataMember]
        public bool can_cat;

        /// <summary>can_catと同じ</summary>
        [DataMember]
        public bool? cat;

        /// <summary>大百科が作成されているか</summary>
        [DataMember]
        public bool dic;
    }
}
