using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetInfo
{
    /******************************************/
    /// <summary>タグリスト</summary>
    /******************************************/
    public class Tags
    {
        /// <summary>言語</summary>
        [XmlElement]
        public string domain;

        /// <summary>タグ</summary>
        [XmlElement]
        public Tag[] tag;
    }
}