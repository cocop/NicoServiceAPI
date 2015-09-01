namespace NicoServiceAPI.NicoVideo
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