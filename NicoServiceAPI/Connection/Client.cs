using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NicoServiceAPI.Connection
{
    /******************************************/
    /// <summary>クライアント処理まとめ</summary>
    /******************************************/
    internal class Client
    {
        public CookieContainer CookieContainer { get; set; }

        /******************************************/
        //HttpWebRequestからTask挟んでストリームを取得する時そのままResultにアクセスしても取れてる
        //自分で作ったTaskだと取れないので多分あんまりよろしくない、そのうち直す
        /******************************************/

        public Client()
        {
            CookieContainer = new CookieContainer();
        }

        /// <summary>データのアップロード</summary>
        /// <param name="Url">アップロードURL</param>
        /// <param name="Data">アップロードデータ</param>
        /// <param name="ContentType">ポストするコンテンツタイプ</param>
        public byte[] Upload(string Url, byte[] Data, ContentType ContentType = ContentType.None)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "POST";
            request.ContentType = ContentType.ToKey();
            request.CookieContainer = CookieContainer;

            //アップロード
            using (var requestStream = request.GetRequestStreamAsync().Result)
                requestStream.Write(Data, 0, Data.Length);

            //レスポンス取得
            using (var response = request.GetResponseAsync().Result.GetResponseStream())
            using (var memoryStream = new MemoryStream())
            {
                response.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>データのダウンロード</summary>
        /// <param name="Url">ダウンロードURL</param>
        public byte[] Download(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "GET";
            request.CookieContainer = CookieContainer;

            using (var response = request.GetResponseAsync().Result.GetResponseStream())
            using (var memoryStream = new MemoryStream())
            {
                response.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>アップロードストリームを開く</summary>
        /// <param name="Url">アップロードURL</param>
        /// <param name="ContentType">ポストするコンテンツタイプ</param>
        public Streams OpenUploadStreams(string Url, ContentType ContentType = ContentType.None)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "POST";
            request.ContentType = ContentType.ToKey();
            request.CookieContainer = CookieContainer;

            return new Streams(
                new StreamData[]
                {
                    new StreamData()
                    {
                        StreamType = StreamType.Write,
                        GetStream = () => request.GetRequestStreamAsync(),
                    },
                    new StreamData()
                    {
                        StreamType = StreamType.Read,
                        GetStream = () => new Task<Stream>(() => request.GetResponseAsync().Result.GetResponseStream()),
                    },
                });
        }

        /// <summary>ダウンロードストリームを開く</summary>
        /// <param name="Url">ダウンロードURL</param>
        public Task<Stream> OpenDownloadStream(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "GET";
            request.CookieContainer = CookieContainer;

            return new Task<Stream>(() => request.GetResponseAsync().Result.GetResponseStream());
        }

    }
}