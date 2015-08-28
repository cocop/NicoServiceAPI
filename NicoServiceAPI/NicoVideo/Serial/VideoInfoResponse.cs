using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>動画情報レスポンス</summary>
    /******************************************/
    [XmlRoot("nicovideo_thumb_response")]
    [DataContract]
    public class VideoInfoResponse
    {
        /// <summary>動画情報のリスト</summary>
        [XmlElement("thumb")]
        [DataMember(Name = "list")]
        public VideoInfo[] VideoInfos { set; get; }

        /// <summary>エラーコード</summary>
        [XmlElement("error")]
        public Error ErrorCode { set; get; }

        /// <summary>成功か失敗か</summary>
        [XmlAttribute("status")]
        [DataMember(Name = "status")]
        public string Status { set; get; }

        /// <summary>失敗した場合のメッセージ</summary>
        [XmlAttribute("message")]
        [DataMember(Name = "message")]
        public string ErrorMessage { set; get; }
    }
}
