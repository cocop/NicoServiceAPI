using NicoServiceAPI.NicoVideo.Video;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>動画へアクセスする</summary>
    /******************************************/
    public class VideoPage
    {
        Context context;
        Serial.Converter converter;

        /******************************************/
        /******************************************/

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Context">コンテキスト</param>
        internal VideoPage(Context Context)
        {
            context = Context;
            converter = new Serial.Converter(context);
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

            streamDataList.AddRange(GetVideoAccessStream(VideoInfo, context.Client));
            streamDataList.Add(
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) =>
                    {
                        try
                        {
                            return context.Client.OpenDownloadStream(VideoInfo.cache["url"]);
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

            streamDataList.AddRange(GetVideoAccessStream(VideoInfo, context.Client));
            streamDataList.Add(new Connection.StreamData()
            {   //リクエスト
                StreamType = Connection.StreamType.Write,
                GetStream = (size) =>
                {
                    Stream result = null;
                    try
                    {
                        uploadStreams = context.Client.OpenUploadStream(VideoInfo.cache["ms"]);
                        result = uploadStreams.GetStream(size);
                        uploadStreams.Next();
                    }
                    catch (WebException e)
                    {
                        throw new WebException("コメントサーバーにアクセスできません", e);
                    }
                    return result;
                },
                GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(
                        string.Format(
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
                    catch (WebException e)
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
                    return converter.ConvertCommentResponse((Serial.GetComment.Packet)serialize.Deserialize(stream));
                });
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
                            return context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoInfo, VideoInfo.ID));
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

                        lastData = converter.ConvertVideoInfoResponse(serial);
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
                        GetStream = (size) => context.Client.OpenDownloadStream(ApiUrls.Host + "watch/" + VideoInfo.ID),
                        SetReadData = (data) =>
                        {
                            if (lastData.Status != Status.OK) return;

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


        internal static Connection.StreamData[] GetVideoAccessStream(VideoInfo VideoInfo, Connection.Client Client)
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
                            return Client.OpenDownloadStream(string.Format(ApiUrls.GetVideo, VideoInfo.ID));
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