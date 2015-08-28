using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial
{

    /******************************************/
    /// <summary>動画周りのカウンタ</summary>
    /******************************************/
    public class ViewCounter
    {
        /// <summary>再生数</summary>
        [XmlAttribute("video")]
        public int View;

        /// <summary>動画ID</summary>
        [XmlAttribute("id")]
        public string ID;

        /// <summary>マイリスト登録数</summary>
        [XmlAttribute("mylist")]
        public int MyList;
    }
}