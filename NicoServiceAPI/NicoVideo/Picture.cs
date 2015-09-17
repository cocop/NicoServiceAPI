using NicoServiceAPI.Connection;
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

        /// <summary>画像データのURL</summary>
        public string Url { private set; get; }

        /// <summary>画像データ</summary>
        public byte[] Data { set; get; }


        Client client;

        /******************************************/
        /******************************************/

        /// <summary>自動生成される画像データ</summary>
        /// <param name="Url">画像データのURL</param>
        /// <param name="Client">クッキーを持っているClient</param>
        internal Picture(string Url, Client Client)
        {
            IsDownloaded = false;
            this.Url = Url;
            client = Client;
        }

        /// <summary>画像ダウンロード用ストリームの取得</summary>
        public Stream GetStream()
        {
            return client.OpenDownloadStream(Url);
        }

        /// <summary>画像のダウンロード</summary>
        public byte[] Download()
        {
            Data = client.Download(Url);
            return Data;
        }
        
    }
}