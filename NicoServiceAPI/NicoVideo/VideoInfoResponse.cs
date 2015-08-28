namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>動画情報レスポンス</summary>
    /******************************************/
    public class VideoInfoResponse
    {
        /// <summary>動画情報のリスト</summary>
        public VideoInfo[] VideoInfos { set; get; }

        /// <summary>レスポンスの可否</summary>
        public Status Status { set; get; }

        /// <summary>失敗した場合のメッセージ</summary>
        public string ErrorMessage { set; get; }
    }
}