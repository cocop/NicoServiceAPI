namespace NicoServiceAPI.NicoVideo.Video
{
    /******************************************/
    /// <summary>動画情報レスポンス</summary>
    /******************************************/
    public class VideoInfoResponse : Response
    {
        /// <summary>動画情報のリスト</summary>
        public VideoInfo[] VideoInfos { set; get; }
    }
}