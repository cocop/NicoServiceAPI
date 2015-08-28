using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>コメント</summary>
    /******************************************/
    public class Comment
    {
        /// <summary>スレッドID</summary>
        [XmlAttribute("thread")]
        public int Thread;

        /// <summary>コメント番号</summary>
        [XmlAttribute("no")]
        public int No;

        /// <summary>再生位置</summary>
        [XmlAttribute("vpos")]
        public string PlayTime;

        /// <summary>投稿時間</summary>
        [XmlAttribute("date")]
        public string WriteTime;

        /// <summary>コマンド</summary>
        [XmlAttribute("mail")]
        public string Command;

        /// <summary>ユーザーID</summary>
        [XmlAttribute("user_id")]
        public string UserID;

        /// <summary>暗号ユーザーIDフラグ</summary>
        [XmlAttribute("anonymity")]
        public string Anonymity;

        /// <summary>プレミアムフラグ</summary>
        [XmlAttribute("premium")]
        public bool Premium;

        /// <summary>再生時間（分）</summary>
        [XmlAttribute("leaf")]
        public int Leaf;

        /// <summary>NG共有スコア</summary>
        [XmlAttribute("scores")]
        public string Scores;

        /// <summary>自分のコメントフラグ</summary>
        [XmlAttribute("yourpost")]
        public bool YourPost;
        
        /// <summary>コメント本文</summary>
        [XmlText]
        public string Body;

    }
}