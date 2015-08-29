using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>ニコニコ動画サービスAPIへアクセス</summary>
    /******************************************/
    public class VideoService
    {
        Connection.Client client;

        /******************************************/
        /******************************************/

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Client">Cookie持ちクライアント</param>
        internal VideoService(Connection.Client Client)
        {
            client = Client;
        }

        /// <summary>動画を検索する</summary>
        /// <param name="Keyword">検索キーワード</param>
        /// <param name="SearchPage">検索ページの指定、1～nの間の数値を指定する</param>
        /// <param name="SearchType">検索方法を指定する</param>
        /// <param name="SearchOption">検索オプションを指定する</param>
        public VideoInfoResponse Search(
            string          Keyword,
            int             SearchPage,
            SearchType      SearchType,
            SearchOption    SearchOption)
        {
            var serialize = new DataContractJsonSerializer(typeof(Serial.VideoInfoResponse));
            var serial = (Serial.VideoInfoResponse)serialize.ReadObject(
                client.OpenDownloadStream(
                    String.Format(
                        ApiUrls.VideoSearch,
                        SearchType.ToKey(),
                        Keyword,
                        SearchPage,
                        SearchOption.ToKey())));

            return Serial.Converter.ConvertVideoInfoResponse(serial, client);
        }

        /// <summary>動画をダウンロードする</summary>
        /// <param name="VideoInfo">ダウンロードする動画の指定</param>
        public byte[] DownloadVideo(VideoInfo VideoInfo)
        {
            if (!AccessVideo(VideoInfo)) return null;

            string videoUrl = VideoInfo.cache[3];

            try
            {
                return client.Download(videoUrl);
            }
            catch (WebException e)
            {
                throw new WebException("動画が保存されているサーバーにアクセス出来ませんでした", e);
            }
        }

        /// <summary>動画をダウンロードするストリームを取得する</summary>
        /// <param name="VideoInfo">ダウンロードする動画の指定</param>
        public Stream GetVideoDownloadStream(VideoInfo VideoInfo)
        {
            if (!AccessVideo(VideoInfo)) return null;

            string videoUrl = VideoInfo.cache["url"];

            try
            {
                return client.OpenDownloadStream(videoUrl);
            }
            catch (WebException e)
            {
                throw new WebException("動画が保存されているサーバーにアクセス出来ませんでした", e);
            }
        }

        /// <summary>コメントをダウンロードする</summary>
        /// <param name="VideoInfo">ダウンロードするコメントの動画IDを指定</param>
        public CommentResponse DownloadComment(VideoInfo VideoInfo)
        {
            if (!AccessVideo(VideoInfo)) return null;

            try
            {
                var post =  Encoding.UTF8.GetBytes(
                    String.Format(
                        PostTexts.GetVideoComment,
                        VideoInfo.cache["thread_id"],
                        "100"));

                var streams = client.OpenUploadStream(
                    VideoInfo.cache["ms"]);

                var postStream = streams.GetStream(post.Length);
                postStream.Write(post, 0, post.Length);
                postStream.Close();
                streams.Next();

                var serialize = new XmlSerializer(typeof(Serial.CommentResponse));
                return Serial.Converter.ConvertCommentResponse((Serial.CommentResponse)serialize.Deserialize(streams.GetStream()));
            }
            catch (WebException e)
            {
                throw new WebException("コメントサーバーにアクセス出来ませんでした", e);
            }
        }

        /// <summary>コメントをダウンロードするストリームを取得する</summary>
        /// <param name="VideoInfo">ダウンロードするコメントの動画IDを指定</param>
        public Connection.Streams<CommentResponse> GetCommentDownloadStream(VideoInfo VideoInfo)
        {
            var streamDataList = new List<Connection.StreamData>();
            Connection.Streams streams = null;
            MemoryStream stream = null;

            streamDataList.AddRange(GetVideoAccessStream(VideoInfo));
            streamDataList.Add(new Connection.StreamData()
            {
                StreamType = Connection.StreamType.Write,
                GetStream = (size) => 
                {
                    streams = client.OpenUploadStream(VideoInfo.cache["ms"]);
                    return streams.GetStream(size);
                },
                GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(
                        String.Format(
                            PostTexts.GetVideoComment,
                            VideoInfo.cache["thread_id"],
                            "100"));
                }
            });
            streamDataList.Add(new Connection.StreamData()
            {
                StreamType = Connection.StreamType.Read,
                GetStream = (size) =>
                {
                    streams.Next();
                    return streams.GetStream();
                },
                SetReadData = (data) => stream = new MemoryStream(data),
            });

            return new Connection.Streams<CommentResponse>(
                streamDataList.ToArray(),
                () =>
                {
                    var serialize = new XmlSerializer(typeof(Serial.CommentResponse));
                    return Serial.Converter.ConvertCommentResponse((Serial.CommentResponse)serialize.Deserialize(stream));
                });
        }

        /// <summary>>動画の詳細情報を取得する、情報は0番目の配列に格納される</summary>
        /// <param name="VideoInfo">情報取得する動画を指定する</param>
        /// <param name="IsHtml">合わせてHtmlから情報を取得するか、現在動画説明文のみ</param>
        public VideoInfoResponse GetVideoInfo(VideoInfo VideoInfo, bool IsHtml = true)
        {
            try
            {
                var serialize = new XmlSerializer(typeof(Serial.VideoInfoResponse));
                var serial = (Serial.VideoInfoResponse)serialize.Deserialize(
                    client.OpenDownloadStream(
                        String.Format(ApiUrls.GetVideoInfo, VideoInfo.ID)));
                var result = Serial.Converter.ConvertVideoInfoResponse(serial, client);

                if (IsHtml)
                {
                    var html = Encoding.UTF8.GetString(client.Download(ApiUrls.Host + "watch/" + VideoInfo.ID));
                    var htmls = html.Split(
                        SplitHtmlText.VideoDescription,
                        StringSplitOptions.RemoveEmptyEntries);

                    if (htmls.Length == 3)//HTMLから取得する
                        result.VideoInfos[0].Description = htmls[1];
                }

                return result;
            }
            catch (WebException _VideoInfoAPIAccessError)
            {
                throw new WebException("動画情報取得APIにアクセス出来ませんでした", _VideoInfoAPIAccessError);
            }
        }


        private bool AccessVideo(VideoInfo VideoInfo)
        {
            if (VideoInfo.cache != null) return true;

            try
            {
                client.Download(ApiUrls.Host + "watch/" + VideoInfo.ID);//動画ページにアクセス

                var cacheString = Encoding.UTF8.GetString(client.Download(String.Format(ApiUrls.GetVideo, VideoInfo.ID)));

                VideoInfo.cache = HttpUtility.ParseQueryString(Uri.UnescapeDataString(cacheString));
            }
            catch (WebException e)
            {
                throw new WebException("動画サイトにアクセス出来ませんでした", e);
            }

            return VideoInfo.cache != null;
        }

        private Connection.StreamData[] GetVideoAccessStream(VideoInfo VideoInfo)
        {
            if (VideoInfo.cache != null) return new Connection.StreamData[0];

            return new Connection.StreamData[]
            {
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) => client.OpenDownloadStream(ApiUrls.Host + "watch/" + VideoInfo.ID),
                },
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) => client.OpenDownloadStream(String.Format(ApiUrls.GetVideo, VideoInfo.ID)),
                    SetReadData = (data) =>
                    {
                        VideoInfo.cache = HttpUtility.ParseQueryString(
                            Uri.UnescapeDataString(
                                Encoding.UTF8.GetString(data)));
                    },
                },
            };
        }

    }
}
