namespace NicoServiceAPI.NicoVideo.User
{
    /******************************************/
    /// <summary>動画視聴履歴レスポンス</summary>
    /******************************************/
    public class ViewHistoryResponse : Response
    {
        /// <summary>動画視聴履歴</summary>
        public Video.VideoInfo[] History { get; set; }
    }
}