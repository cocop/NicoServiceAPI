using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>タグ</summary>
    /******************************************/
    public class Tag
    {
        /// <summary>タグ名</summary>
        [XmlText]
        public string Name { set; get; }

        /// <summary>カテゴリ</summary>
        [XmlAttribute("category")]
        public int Category { set; get; }

        /// <summary>タグロック</summary>
        [XmlAttribute("lock")]
        public int Lock { set; get; }
    }
}
