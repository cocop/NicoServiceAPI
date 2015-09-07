using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetInfo
{
    /******************************************/
    /// <summary>タグ</summary>
    /******************************************/
    public class Tag
    {
        /// <summary>タグ名</summary>
        [XmlText]
        public string tag;

        /// <summary>カテゴリ</summary>
        [XmlAttribute]
        public int category;

        /// <summary>タグロック</summary>
        [XmlAttribute("lock")]
        public int _lock; //予約語なので先頭にハイフン
    }
}