using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>スレッド情報</summary>
    /******************************************/
    public class Thread
    {
        /// <summary>スレッドのレスポンス</summary>
        [XmlAttribute("resultcode")]
        public int ResultCode;

        //0 = FOUND(スレッドを見つけた)
        //1 = NOT_FOUND(スレッドが見つからない)
        //2 = INVALID(スレッドIDがおかしい)
        //3 = VERSION(パケットバージョンがおかしい)
        //4 = INVALID_WAYBACKKEY(ウェイバックキーが無い、一致しない)
        //5 = TOO_ODD_WAYBACKKEY(ウェイバックキーの形式がおかしい)
        //6 = INVALID_ADMINKEY(管理者キーが無い、一致しない)
        //7 = TOO_ODD_ADMINKEY(管理者キーの形式がおかしい)
        //8 = INVALID_THREADKEY(スレッドキーが無い、一致しない)
        //9 = TOO_ODD_THREADKEY(スレッドキーの形式がおかしい)
        //10 = NOT_IMPLEMENTED(実装されていない？)
        //11 = LEAF_NOT_ACTIVATE(リーフ管理されていない)
        //12 = LEAF_NOT_ACTIVATE(リーフ管理されていない)
        //13 = LANGUAGE_NOT_FOUND(言語が間違っている)


        /// <summary>スレッドID</summary>
        [XmlAttribute("thread")]
        public int Value;

        /// <summary>最後に投稿されたコメントのコメントNo、通常コメント数と同値</summary>
        [XmlAttribute("last_res")]
        public int LastRes;

        /// <summary>投稿チケット</summary>
        [XmlAttribute("ticket")]
        public string Ticket;

        /// <summary>恐らくキャッシュの改訂版号</summary>
        [XmlAttribute("revision")]
        public int Revision;

        /// <summary>サーバー時間</summary>
        [XmlAttribute("server_time")]
        public int ServerTime;
    }
}