using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>コメントリーフ情報</summary>
    /******************************************/
    public class Leaf
    {
        /// <summary>コメントリーフID</summary>
        [XmlAttribute("leaf")]
        public int ID;

        /// <summary>スレッドID</summary>
        [XmlAttribute("thread")]
        public int Thread;

        /// <summary>リーフ内の総コメント数</summary>
        [XmlAttribute("count")]
        public int Count;
    }
}