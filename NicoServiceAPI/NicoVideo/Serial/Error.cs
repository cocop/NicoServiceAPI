using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>エラー情報</summary>
    /******************************************/
    [DataContract]
    public class Error
    {
        /// <summary>エラーコード</summary>
        [XmlElement("code")]
        public string Code { set; get; }

        /// <summary>エラー内容</summary>
        [XmlElement("description")]
        public string Description { set; get; }
    }
}
