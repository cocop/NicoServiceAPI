using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetComment
{
    /******************************************/
    /// <summary>コメントレスポンス</summary>
    /******************************************/
    [XmlRoot("packet")]
    public class Packet
    {
        /// <summary>コメントリーフ情報</summary>
        [XmlElement]
        public Leaf[] leaf;

        /// <summary>スレッド情報</summary>
        [XmlElement]
        public Thread[] thread;

        /// <summary>コメント周りのカウンタ</summary>
        [XmlElement]
        public ViewCounter view_counter;

        /// <summary>コメント</summary>
        [XmlElement]
        public Chat[] chat;
    }
}