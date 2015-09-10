using System.Runtime.Serialization;

namespace NicoServiceAPI.NicoVideo.Serial.GetMylist
{
    /// <summary>サムネイルスタイル</summary>
    [DataContract]
    public class ThumbnailStyle
    {
        /// <summary>オフセットX</summary>
        [DataMember]
        public int offset_x;

        /// <summary>オフセットY</summary>
        [DataMember]
        public int offset_y;
        
        /// <summary>幅</summary>
        [DataMember]
        public int width;
    }
}
