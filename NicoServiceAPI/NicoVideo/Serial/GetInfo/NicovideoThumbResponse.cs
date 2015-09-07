using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetInfo
{
    /******************************************/
    /// <summary>動画情報レスポンス</summary>
    /******************************************/
    [XmlRoot("nicovideo_thumb_response")]
    public class NicovideoThumbResponse
    {
        /// <summary>動画情報</summary>
        [XmlElement]
        public Thumb thumb;

        /// <summary>エラーコード</summary>
        [XmlElement]
        public Error error;

        /// <summary>成功か失敗か</summary>
        [XmlAttribute]
        public string status;
    }
}