using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetComment
{
    /******************************************/
    /// <summary>コメント</summary>
    /******************************************/
    public class Chat
    {
        /// <summary>スレッドID</summary>
        [XmlAttribute]
        public int thread;

        /// <summary>コメント番号</summary>
        [XmlAttribute]
        public int no;

        /// <summary>再生位置</summary>
        [XmlAttribute]
        public string vpos;

        /// <summary>投稿時間</summary>
        [XmlAttribute]
        public string date;

        /// <summary>コマンド</summary>
        [XmlAttribute]
        public string mail;

        /// <summary>ユーザーID</summary>
        [XmlAttribute]
        public string user_id;

        /// <summary>暗号ユーザーIDフラグ</summary>
        [XmlAttribute]
        public string anonymity;

        /// <summary>プレミアムフラグ</summary>
        [XmlAttribute]
        public bool premium;

        /// <summary>再生時間（分）</summary>
        [XmlAttribute]
        public int leaf;

        /// <summary>NG共有スコア</summary>
        [XmlAttribute]
        public string scores;

        /// <summary>自分のコメントフラグ</summary>
        [XmlAttribute]
        public bool yourpost;

        /// <summary>コメント本文</summary>
        [XmlText]
        public string body;

    }
}