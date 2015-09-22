namespace NicoServiceAPI.NicoVideo.User
{
    /******************************************/
    /// <summary>マイリスト追加レスポンス</summary>
    /******************************************/
    public class AddMylistResponse : Response
    {
        /// <summary>追加したマイリスト</summary>
        public Mylist.Mylist AddedMylist { get; set; }
    }
}