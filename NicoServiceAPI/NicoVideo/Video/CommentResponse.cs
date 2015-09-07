namespace NicoServiceAPI.NicoVideo.Video
{
    /******************************************/
    /// <summary>コメントレスポンス</summary>
    /******************************************/
    public class CommentResponse : Response
    {
        /// <summary>コメント</summary>
        public Comment[] Comment { get; set; }
    }
}