using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetComment
{
    /******************************************/
    /// <summary>コメントリーフ情報</summary>
    /******************************************/
    public class Leaf
    {
        /// <summary>コメントリーフID</summary>
        [XmlAttribute]
        public int leaf;

        /// <summary>スレッドID</summary>
        [XmlAttribute]
        public int thread;

        /// <summary>リーフ内の総コメント数</summary>
        [XmlAttribute]
        public int count;
    }
}