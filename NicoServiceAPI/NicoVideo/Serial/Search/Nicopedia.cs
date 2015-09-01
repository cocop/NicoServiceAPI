using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.Search
{
    /// <summary>ニコニコ大百科</summary>
    public class Nicopedia
    {
        /// <summary>タグ説明、一番最初に指定したタグのニコニコ大百科から参照される</summary>
        [DataMember]
        public string html;
    }
}
