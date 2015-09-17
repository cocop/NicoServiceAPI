namespace NicoServiceAPI.NicoVideo.Mylist
{
    /******************************************/
    /// <summary>マイリストからの動画削除レスポンス</summary>
    /******************************************/
    public class MylistRemoveVideoResponse : Response
    {
        /// <summary>削除した動画数</summary>
        public int RemoveCount { get; set; }
    }
}