using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace NicoServiceAPI.NicoVideo.Serial
{
    /******************************************/
    /// <summary>動画情報</summary>
    /******************************************/
    [DataContract()]
    public class VideoInfo
    {
        /// <summary>動画ID</summary>
        [XmlElement("video_id")]
        [DataMember(Name = "id")]
        public string ID { set; get; }

        /// <summary>タイトル</summary>
        [XmlElement("title")]
        [DataMember(Name = "title")]
        public string Title { set; get; }

        /// <summary>短い動画説明文</summary>
        [DataMember(Name = "description_short")]
        public string ShortDescription { set; get; }

        /// <summary>サムネイル画像URL</summary>
        [XmlElement("thumbnail_url")]
        [DataMember(Name = "thumbnail_url")]
        public string ThumbnailUrl { set; get; }

        /// <summary>投稿日時</summary>
        [XmlElement("first_retrieve")]
        [DataMember(Name = "first_retrieve")]
        public string PostTime { set; get; }

        /// <summary>再生時間</summary>
        [XmlElement("length")]
        [DataMember(Name = "length")]
        public string Length { set; get; }

        /// <summary>再生数</summary>
        [XmlElement("view_counter")]
        [DataMember(Name = "view_counter")]
        public int ViewCounter { set; get; }

        /// <summary>コメント数</summary>
        [XmlElement("comment_num")]
        [DataMember(Name = "num_res")]
        public int ComentNumber { set; get; }

        /// <summary>マイリスト数</summary>
        [XmlElement("mylist_counter")]
        [DataMember(Name = "mylist_counter")]
        public int MylistCounter { set; get; }

        /// <summary>動画説明文</summary>
        [XmlElement(ElementName = "description")]
        public string Description { set; get; }

        /// <summary>タグ</summary>
        [XmlElement("tags")]
        public Tags Tags { set; get; }

        /// <summary>動画の形式</summary>
        [XmlElement(ElementName = "movie_type")]
        public string VideoType { set; get; }

        /// <summary>動画サイズ</summary>
        [XmlElement(ElementName = "size_high")]
        public int VideoSize { set; get; }

        /// <summary>エコノミー時の動画サイズ、0である場合はVideoSizeと同じ</summary>
        [XmlElement(ElementName = "size_low")]
        public int EconomyVideoSize { set; get; }

        /// <summary>ニコ生で再生できるかどうか</summary>
        [XmlElement(ElementName = "no_live_play")]
        public bool NoLivePlay { set; get; }

        /// <summary>外部再生の可否</summary>
        [XmlElement(ElementName = "embeddable")]
        public string ExternalPlay { set; get; }
    }
}