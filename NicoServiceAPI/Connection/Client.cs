using System.IO;
using System.Net;

namespace NicoServiceAPI.Connection
{
    /******************************************/
    /// <summary>クライアント処理まとめ</summary>
    /******************************************/
    internal class Client
    {
        const int BufferLength = 1024;

        public CookieContainer CookieContainer { get; set; }

        /******************************************/
        /******************************************/

        public Client()
        {
            CookieContainer = new CookieContainer();
        }

        /// <summary>データのアップロード</summary>
        /// <param name="Url">アップロードURL</param>
        /// <param name="Data">アップロードデータ</param>
        public byte[] Upload(string Url, byte[] Data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Data.Length;
            request.CookieContainer = CookieContainer;

            //アップロード
            Stream requestStream = request.GetRequestStream();
            
            requestStream.Write(Data, 0, Data.Length);
            requestStream.Close();

            //レスポンス取得
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            MemoryStream memoryStream = new MemoryStream();
            byte[] buffer = new byte[BufferLength];
            int readLength = -1;

            while (readLength != 0)
            {
                readLength = responseStream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, readLength);
            }

            responseStream.Close();
            memoryStream.Close();

            return memoryStream.ToArray();
        }

        /// <summary>データのダウンロード</summary>
        /// <param name="Url">ダウンロードURL</param>
        public byte[] Download(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.CookieContainer = CookieContainer;
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            MemoryStream memoryStream = new MemoryStream();
            byte[] buffer = new byte[BufferLength];
            int readLength = -1;

            while (readLength != 0)
            {
                readLength = responseStream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, readLength);
            }

            responseStream.Close();
            memoryStream.Close();

            return memoryStream.ToArray();
        }

        /// <summary>アップロードストリームを開く</summary>
        /// <param name="Url">アップロードURL</param>
        public Streams OpenUploadStream(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = CookieContainer;

            return new Streams(
                new StreamData[]
                {
                    new StreamData()
                    {
                        StreamType = StreamType.Write,
                        GetStream = (size) => 
                        {
                            request.ContentLength = size;
                            return request.GetRequestStream();
                        },
                    },
                    new StreamData()
                    {
                        StreamType = StreamType.Read,
                        GetStream = (size) => request.GetResponse().GetResponseStream(),
                    },
                });
        }

        /// <summary>ダウンロードストリームを開く</summary>
        /// <param name="Url">ダウンロードURL</param>
        public Stream OpenDownloadStream(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.CookieContainer = CookieContainer;

            return request.GetResponse().GetResponseStream();
        }

    }
}