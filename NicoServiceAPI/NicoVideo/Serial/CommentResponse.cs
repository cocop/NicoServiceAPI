using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>コメントレスポンス</summary>
    /******************************************/
    [XmlRoot("packet")]
    public class CommentResponse
    {
        /// <summary>コメントリーフ情報</summary>
        [XmlElement("leaf")]
        public Leaf[] Leaf;

        /// <summary>スレッド情報</summary>
        [XmlElement("thread")]
        public Thread[] Thread;

        /// <summary>コメント周りのカウンタ</summary>
        [XmlElement("view_counter")]
        public ViewCounter ViewCounter;

        /// <summary>コメント</summary>
        [XmlElement("chat")]
        public Comment[] Comment;
    }
}