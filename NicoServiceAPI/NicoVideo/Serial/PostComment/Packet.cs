using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.PostComment
{
    /******************************************/
    /// <summary>コメント投稿結果</summary>
    /******************************************/
    [XmlRoot("packet")]
    public class Packet
    {
        /// <summary>コメント投稿結果</summary>
        [XmlElement]
        public ChatResult chat_result;
    }
}