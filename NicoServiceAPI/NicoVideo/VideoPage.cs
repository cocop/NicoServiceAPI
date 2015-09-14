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
    /// <summary>動画ページへアクセスする</summary>
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
            var streams = OpenVideoDownloadStream(VideoInfo);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>動画をダウンロードするストリームを取得する</summary>
        /// <param name="VideoInfo">ダウンロードする動画の指定</param>
        public Connection.Streams<byte[]> OpenVideoDownloadStream(VideoInfo VideoInfo)
        {
            var streamDataList = new List<Connection.StreamData>();
            byte[] result = null;

            streamDataList.AddRange(OpenVideoAccessStream(VideoInfo, context.Client));
            streamDataList.Add(
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) =>
                    {
                        try
                        {
                            return context.Client.OpenDownloadStream(ApiUrls.Host + "watch/" + VideoInfo.ID);
                        }
                        catch (WebException e)
                        {
                            throw new WebException("動画ページにアクセス出来ませんでした", e);
                        }
                    },
                    SetReadData = (data) => { },
                });
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

        /// <summary>コメントをアップロードする</summary>
        /// <param name="VideoInfo">アップロードするコメントの動画ID</param>
        /// <param name="Comment">投稿するコメント</param>
        public Response UploadComment(VideoInfo VideoInfo, Comment Comment)
        {
            var streams = OpenCommentUploadStream(VideoInfo, Comment);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>コメントをアップロードするストリームを取得する</summary>
        /// <param name="VideoInfo">アップロードするコメントの動画ID</param>
        /// <param name="Comment">投稿するコメント</param>
        public Connection.Streams<Response> OpenCommentUploadStream(VideoInfo VideoInfo, Comment Comment)
        {
            var streamDataList = new List<Connection.StreamData>();
            string postkey = null;
            Response result = null;

            streamDataList.AddRange(OpenVideoAccessStream(VideoInfo, context.Client));

            if (VideoInfo.cache == null || VideoInfo.cache.Count < 15)
                streamDataList.AddRange(OpenCommentDownloadStream(VideoInfo).GetStreamDatas());

            streamDataList.Add(
                new Connection.StreamData()
                {
                    StreamType = Connection.StreamType.Read,
                    GetStream = (size) =>
                    {
                        return context.Client.OpenDownloadStream(
                            string.Format(ApiUrls.PostVideoComment,
                                VideoInfo.cache["block_no"],
                                VideoInfo.cache["thread_id"]));
                    },
                    SetReadData = (data) =>
                    {
                        postkey = Uri.UnescapeDataString(Encoding.UTF8.GetString(data)).Replace("postkey=", "");
                    },
                });

            var uploadStreamData = context.Client.OpenUploadStream(VideoInfo.cache["ms"]).GetStreamDatas();
            uploadStreamData[0].GetWriteData = () =>
            {
                return Encoding.UTF8.GetBytes(
                    string.Format(PostTexts.PostVideoComment,
                        VideoInfo.cache["thread_id"],
                        ((int)(Comment.PlayTime.TotalMilliseconds / 10)).ToString(),
                        Comment.Command,
                        VideoInfo.cache["ticket"],
                        VideoInfo.cache["user_id"],
                        postkey,
                        Comment.Body));
            };

            uploadStreamData[1].SetReadData = (data) =>
            {
                var serialize = new XmlSerializer(typeof(Serial.PostComment.Packet));
                var serial = (Serial.PostComment.Packet)serialize.Deserialize(new MemoryStream(data));
                result = converter.ConvertResponse(serial);
                VideoInfo.cache["block_no"] = ((int)((serial.chat_result.no + 1) / 100)).ToString();//更新
            };
            streamDataList.AddRange(uploadStreamData);

            return new Connection.Streams<Response>(
            
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>コメントをダウンロードする</summary>
        /// <param name="VideoInfo">ダウンロードするコメントの動画ID</param>
        public CommentResponse DownloadComment(VideoInfo VideoInfo)
        {
            var streams = OpenCommentDownloadStream(VideoInfo);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>コメントをダウンロードするストリームを取得する</summary>
        /// <param name="VideoInfo">ダウンロードするコメントの動画IDを指定</param>
        public Connection.Streams<CommentResponse> OpenCommentDownloadStream(VideoInfo VideoInfo)
        {
            var streamDataList = new List<Connection.StreamData>();
            Connection.Streams uploadStreams = null;
            CommentResponse result = null;

            streamDataList.AddRange(OpenVideoAccessStream(VideoInfo, context.Client));
            streamDataList.Add(new Connection.StreamData()
            {   //リクエスト
                StreamType = Connection.StreamType.Write,
                GetStream = (size) =>
                {
                    try
                    {
                        uploadStreams = context.Client.OpenUploadStream(VideoInfo.cache["ms"]);
                        Stream stream = uploadStreams.GetStream(size);
                        uploadStreams.Next();
                        return stream;
                    }
                    catch (WebException e)
                    {
                        throw new WebException("コメントサーバーにアクセスできません", e);
                    }
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
                SetReadData = (data) =>
                {
                    var serialize = new XmlSerializer(typeof(Serial.GetComment.Packet));
                    var serial = (Serial.GetComment.Packet)serialize.Deserialize(new MemoryStream(data));

                    VideoInfo.cache.Add("ticket", serial.thread[0].ticket);
                    VideoInfo.cache.Add("block_no", ((int)((serial.thread[0].last_res + 1) / 100)).ToString());

                    result = converter.ConvertCommentResponse(serial);
                }
            });

            return new Connection.Streams<CommentResponse>(
                streamDataList.ToArray(),
                () => result );
        }

        /// <summary>>動画の詳細情報を取得する、情報は0番目の配列に格納される</summary>
        /// <param name="VideoInfo">情報取得する動画を指定する</param>
        /// <param name="IsHtml">合わせてHtmlから情報を取得するか、現在動画説明文のみ</param>
        public VideoInfoResponse DownloadVideoInfo(VideoInfo VideoInfo, bool IsHtml = true)
        {
            var streams = OpenVideoInfoDownloadStream(VideoInfo, IsHtml);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>>動画の詳細情報を取得するストリームを取得する、情報は0番目の配列に格納される</summary>
        /// <param name="VideoInfo">情報取得する動画を指定する</param>
        /// <param name="IsHtml">合わせてHtmlから情報を取得するか、現在動画説明文のみ</param>
        public Connection.Streams<VideoInfoResponse> OpenVideoInfoDownloadStream(VideoInfo VideoInfo, bool IsHtml = true)
        {
            var streamDataList = new List<Connection.StreamData>();
            VideoInfoResponse result = null;

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

                        result = converter.ConvertVideoInfoResponse(serial);
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
                            if (result.Status != Status.OK) return;

                            var html = Encoding.UTF8.GetString(data);
                            result.VideoInfos[0].Description = HtmlTextRegex.VideoDescription.Match(html).Groups["value"].Value;
                        },
                    });
            }
            #endregion

            return new Connection.Streams<VideoInfoResponse>(
                streamDataList.ToArray(),
                () => result);
        }


        internal static Connection.StreamData[] OpenVideoAccessStream(VideoInfo VideoInfo, Connection.Client Client)
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