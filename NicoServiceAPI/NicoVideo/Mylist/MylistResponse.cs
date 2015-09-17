namespace NicoServiceAPI.NicoVideo.Mylist
{
    /******************************************/
    /// <summary>マイリストレスポンス</summary>
    /******************************************/
    public class MylistResponse : Response
    {
        /// <summary>マイリスト</summary>
        public Mylist Mylist { get; set; }
    }
}
