using System.IO;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>画像データ</summary>
    /******************************************/
    public class Picture
    {
        /// <summary>ダウンロード済みかどうか</summary>
        public bool IsDownloaded { set; get; }

        /// <summary>サムネイルデータのURL</summary>
        public string Url { private set; get; }

        /// <summary>サムネイルデータ</summary>
        public byte[] Data { set; get; }


        Connection.Client client;

        /******************************************/
        /******************************************/

        /// <summary>自動生成される動画サムネイル</summary>
        /// <param name="Url">サムネイルデータのURL</param>
        /// <param name="Client">クッキーを持っているClient</param>
        internal Picture(string Url, Connection.Client Client)
        {
            IsDownloaded = false;
            this.Url = Url;
            client = Client;
        }

        /// <summary>サムネイルダウンロード用ストリームの取得</summary>
        public Stream GetStream()
        {
            return client.OpenDownloadStream(Url);
        }

        /// <summary>サムネイルのダウンロード</summary>
        public byte[] Download()
        {
            Data = client.Download(Url);
            return Data;
        }
        
    }
}