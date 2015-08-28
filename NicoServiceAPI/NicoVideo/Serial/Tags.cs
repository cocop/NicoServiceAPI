using System.Xml.Serialization;


namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>タグのリスト</summary>
    /******************************************/
    public class Tags
    {
        /// <summary>タグの配列</summary>
        [XmlElement("tag")]
        public Tag[] List { set; get; }
    }
}
