using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
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
            var serialize = new DataContractJsonSerializer(typeof(Serial.Search.Contract));
            var serial = (Serial.Search.Contract)serialize.ReadObject(
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
            var streams = GetVideoDownloadStream(VideoInfo);
            return streams.Run(streams.UntreatedStreamsCount);
        }

        /// <summary>動画をダウンロードするストリームを取得する</summary>
        /// <param name="VideoInfo">ダウンロードする動画の指定</param>
        public Connection.Streams<byte[]> GetVideoDownloadStream(VideoInfo VideoInfo)
        {
            var streamDataList = new List<Connection.StreamData>();
            byte[] result = null;

            streamDataList.AddRange(GetVideoAccessStream(VideoInfo));
            streamDataList.Add(
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) =>
                    {
                        try
                        {
                            return client.OpenDownloadStream(VideoInfo.cache["url"]);
                        }
                        catch (WebException e)
                        {
                            throw new WebException("動画にアクセス出来ませんでした", e);
                        }
                    },
                    SetReadData = (data) => result = data,
                });

            return new Connection.Streams<byte[]>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>コメントをダウンロードする</summary>
        /// <param name="VideoInfo">ダウンロードするコメントの動画IDを指定</param>
        public CommentResponse DownloadComment(VideoInfo VideoInfo)
        {
            var streams = GetCommentDownloadStream(VideoInfo);
            return streams.Run(streams.UntreatedStreamsCount);
        }

        /// <summary>コメントをダウンロードするストリームを取得する</summary>
        /// <param name="VideoInfo">ダウンロードするコメントの動画IDを指定</param>
        public Connection.Streams<CommentResponse> GetCommentDownloadStream(VideoInfo VideoInfo)
        {
            var streamDataList = new List<Connection.StreamData>();
            Connection.Streams uploadStreams = null;
            MemoryStream stream = null;

            streamDataList.AddRange(GetVideoAccessStream(VideoInfo));
            streamDataList.Add(new Connection.StreamData()
            {   //リクエスト
                StreamType = Connection.StreamType.Write,
                GetStream = (size) =>
                {
                    Stream result = null;
                    try
                    {
                        uploadStreams = client.OpenUploadStream(VideoInfo.cache["ms"]);
                        result = uploadStreams.GetStream(size);
                        uploadStreams.Next();
                    }
                    catch(WebException e)
                    {
                        throw new WebException("コメントサーバーにアクセスできません", e);
                    }
                    return result;
                },
                GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(
                        String.Format(
                            PostTexts.GetVideoComment,
                            VideoInfo.cache["thread_id"]));
                }
            });
            streamDataList.Add(new Connection.StreamData()
            {   //レスポンス
                StreamType = Connection.StreamType.Read,
                GetStream = (size) =>
                {
                    try
                    {
                        return uploadStreams.GetStream();
                    }
                    catch(WebException e)
                    {
                        throw new WebException("コメントサーバーにアクセスできません", e);
                    }
                },
                SetReadData = (data) => stream = new MemoryStream(data),
            });

            return new Connection.Streams<CommentResponse>(
                streamDataList.ToArray(),
                () =>
                {
                    var serialize = new XmlSerializer(typeof(Serial.GetComment.Packet));
                    return Serial.Converter.ConvertCommentResponse((Serial.GetComment.Packet)serialize.Deserialize(stream));
                });
        }

        /// <summary>マイリストを取得する</summary>
        /// <param name="Mylist">IDがnullである場合、とりあえずマイリストを取得する、nullでない場合、指定したマイリストをHTML解析して取得する</param>
        /// <param name="IsHtml">現在は無視される</param>
        public MylistResponse DownloadMylist(Mylist Mylist, bool IsHtml = true)
        {
            var serialize = new DataContractJsonSerializer(typeof(Serial.GetMylist.Contract));
            string mylistSerial;
            GroupCollection mylistInfo = null;
            GroupCollection mylistUserInfo = null;

            if (Mylist.ID == null)
            {
                mylistSerial = Encoding.UTF8.GetString(client.Download(
                    ApiUrls.GetDefaultVideoMylist));
            }
            else
            {//HTML内にJSON文があるので抜き出してシリアライズ、JavaScriptオブジェクトもあるので一緒に抜き出す
                var html = Encoding.UTF8.GetString(client.Download(string.Format(ApiUrls.GetVideoMylist, Mylist.ID)));
                var mylist = HtmlTextRegex.VideoMylist.Match(html).Groups["value"].Value;

                mylistInfo = HtmlTextRegex.VideoMylistInfoCutout.Match(
                    HtmlTextRegex.VideoMylistInfo.Match(html).Groups["value"].Value).Groups;

                mylistUserInfo = HtmlTextRegex.VideoMylistUserInfoCutout.Match(
                    HtmlTextRegex.VideoMylistUserInfo.Match(html).Groups["value"].Value).Groups;

                if (mylist != "")
                {
                    mylist = Regex.Replace(mylist, "\\\\u(?<value>[0-9a-fA-F]{4})", (match) =>//Unicodeエスケープシーケンスをデコード
                        ((char)Convert.ToInt32(match.Groups["value"].Value, 16)).ToString());

                    mylistSerial = "{" + "\"mylistitem\":" + mylist + ", \"status\" : \"ok\"}";
                }
                else
                {
                    mylistSerial = "{" + "\"mylistitem\":[], \"status\" : \"fail\"}";
                }
            }

            return Serial.Converter.ConvertMylistResponse(
                (Serial.GetMylist.Contract)serialize.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(mylistSerial))),
                mylistInfo,
                mylistUserInfo,
                client);

            ////マイリストグループ
            //var mylistGroup = Encoding.UTF8.GetString(client.Download(
            //    ApiUrls.GetMylistGroup));

            //mylistGroup = HttpUtility.HtmlDecode(mylistGroup);
        }

        /// <summary>>動画の詳細情報を取得する、情報は0番目の配列に格納される</summary>
        /// <param name="VideoInfo">情報取得する動画を指定する</param>
        /// <param name="IsHtml">合わせてHtmlから情報を取得するか、現在動画説明文のみ</param>
        public VideoInfoResponse DownloadVideoInfo(VideoInfo VideoInfo, bool IsHtml = true)
        {
            var streams = GetVideoInfoDownloadStream(VideoInfo, IsHtml);
            return streams.Run(streams.UntreatedStreamsCount);
        }

        /// <summary>>動画の詳細情報を取得するストリームを取得する、情報は0番目の配列に格納される</summary>
        /// <param name="VideoInfo">情報取得する動画を指定する</param>
        /// <param name="IsHtml">合わせてHtmlから情報を取得するか、現在動画説明文のみ</param>
        public Connection.Streams<VideoInfoResponse> GetVideoInfoDownloadStream(VideoInfo VideoInfo, bool IsHtml = true)
        {
            var streamDataList = new List<Connection.StreamData>();
            VideoInfoResponse lastData = null;

            #region APIアクセス
            streamDataList.Add(
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) =>
                    {
                        try
                        {
                            return client.OpenDownloadStream(String.Format(ApiUrls.GetVideoInfo, VideoInfo.ID));
                        }
                        catch (WebException e)
                        {
                            throw new WebException("動画情報取得APIにアクセス出来ませんでした", e);
                        }
                    },
                    SetReadData = (data) =>
                    {

                        var serialize = new XmlSerializer(typeof(Serial.GetInfo.NicovideoThumbResponse));
                        var serial = (Serial.GetInfo.NicovideoThumbResponse)serialize.Deserialize(
                            new MemoryStream(data));

                        lastData = Serial.Converter.ConvertVideoInfoResponse(serial, client);
                    }
                });
            #endregion

            #region HTMLアクセス
            if (IsHtml)//HTMLから取得する
            {
                streamDataList.Add(
                    new Connection.StreamData()
                    {
                        StreamType = Connection.StreamType.Read,
                        GetStream = (size) => client.OpenDownloadStream(ApiUrls.Host + "watch/" + VideoInfo.ID),
                        SetReadData = (data) =>
                        {
                            var html = Encoding.UTF8.GetString(data);
                            lastData.VideoInfos[0].Description = HtmlTextRegex.VideoDescription.Match(html).Groups["value"].Value;
                        },
                    });
            }
            #endregion

            return new Connection.Streams<VideoInfoResponse>(
                streamDataList.ToArray(),
                () => lastData);
        }


        private Connection.StreamData[] GetVideoAccessStream(VideoInfo VideoInfo)
        {
            if (VideoInfo.cache != null) return new Connection.StreamData[0];

            return new Connection.StreamData[]
            {
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) =>
                    {
                        try
                        {
                            return client.OpenDownloadStream(ApiUrls.Host + "watch/" + VideoInfo.ID);
                        }
                        catch (WebException e)
                        {
                            throw new WebException("動画サイトにアクセス出来ませんでした", e);
                        }
                    }
                },
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) =>
                    {
                        try
                        {
                            return client.OpenDownloadStream(String.Format(ApiUrls.GetVideo, VideoInfo.ID));
                        }
                        catch(WebException e)
                        {
                            throw new WebException("動画にアクセス出来ませんでした", e);
                        }
                    },
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
