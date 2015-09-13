using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.PostComment
{
    /******************************************/
    /// <summary>コメント投稿結果</summary>
    /******************************************/
    public class ChatResult
    {
        /// <summary>スレッドID</summary>
        [XmlAttribute]
        public string thread;

        /// <summary>リザルトステータス</summary>
        [XmlAttribute]
        public string status;

        /// <summary>処理終了後のラストコメントID</summary>
        [XmlAttribute]
        public int no;
    }
}