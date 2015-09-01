namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>レスポンス</summary>
    /******************************************/
    public abstract class Response
    {
        /// <summary>レスポンスの可否</summary>
        public Status Status { set; get; }

        /// <summary>失敗した場合のメッセージ</summary>
        public string ErrorMessage { set; get; }
    }
}