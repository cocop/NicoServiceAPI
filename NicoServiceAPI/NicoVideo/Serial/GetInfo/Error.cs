using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetInfo
{
    /// <summary>エラーコード</summary>
    public class Error
    {
        /// <summary>エラーコード</summary>
        [XmlElement]
        public string code;

        /// <summary>エラーの説明</summary>
        [XmlElement]
        public string description;
    }
}
