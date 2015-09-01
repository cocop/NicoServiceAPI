using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetComment
{

    /******************************************/
    /// <summary>動画周りのカウンタ</summary>
    /******************************************/
    public class ViewCounter
    {
        /// <summary>再生数</summary>
        [XmlAttribute]
        public int video;

        /// <summary>動画ID</summary>
        [XmlAttribute]
        public string id;

        /// <summary>マイリスト登録数</summary>
        [XmlAttribute]
        public int mylist;
    }
}