﻿using NicoServiceAPI.Connection;
using NicoServiceAPI.NicoVideo.Video;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>動画ページへアクセスする</summary>
    /******************************************/
    public class VideoPage
    {
        VideoInfo target;
        VideoService host;
        Context context;
        Serial.Converter converter;

        string watchAuthKey;

        NameValueCollection videoCache;
        string htmlCache;

        /******************************************/
        /******************************************/

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Target">ターゲット動画</param>
        /// <param name="Host">生成元</param>
        /// <param name="Context">コンテキスト</param>
        internal VideoPage(VideoInfo Target, VideoService Host, Context Context)
        {
            target = Target;
            host = Host;
            context = Context;
            converter = new Serial.Converter(context);
        }

        /// <summary>生成に使用した動画情報を取得する</summary>
        public VideoInfo GetVideoInfo()
        {
            return target;
        }

        /// <summary>動画をダウンロードする</summary>
        public byte[] DownloadVideo()
        {
            var streams = OpenVideoDownloadStreams();
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>動画をダウンロードするストリームを取得する</summary>
        public Streams<byte[]> OpenVideoDownloadStreams()
        {
            var videoAccessStreamData = OpenVideoAccessStreamData();
            var videoPageStreamData = OpenVideoPageStreamData();
            var streamDataList = new List<StreamData>();
            byte[] result = null;

            if (videoAccessStreamData != null)
                streamDataList.Add(videoAccessStreamData);

            if (videoPageStreamData != null)
                streamDataList.Add(videoPageStreamData);

            streamDataList.Add(
                new StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = () =>
                    {
                        try
                        {
                            return context.Client.OpenDownloadStream(videoCache["url"]);
                        }
                        catch (WebException e)
                        {
                            throw new WebException("動画にアクセス出来ませんでした", e);
                        }
                    },
                    SetReadData = (data) => result = data,
                });

            return new Streams<byte[]>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>コメントをアップロードする</summary>
        /// <param name="Comment">投稿するコメント</param>
        public Response UploadComment(Comment Comment)
        {
            var streams = OpenCommentUploadStreams(Comment);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>コメントをアップロードするストリームを取得する</summary>
        /// <param name="Comment">投稿するコメント</param>
        public Streams<Response> OpenCommentUploadStreams(Comment Comment)
        {
            var videoAccessStreamData = OpenVideoAccessStreamData();
            var streamDataList = new List<StreamData>();
            var uploadStreamData = new StreamData[2];
            string postkey = null;
            Response result = null;

            for (int i = 0; i < 2; i++)
                uploadStreamData[i] = new StreamData();

            if (videoAccessStreamData != null)
                streamDataList.Add(videoAccessStreamData);

            if (videoCache == null || videoCache.Count < 15)
                streamDataList.AddRange(OpenCommentDownloadStreams().GetStreamDatas());

            streamDataList.Add(
                new StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = () =>
                    {
                        return context.Client.OpenDownloadStream(
                            string.Format(ApiUrls.PostVideoComment,
                                videoCache["block_no"],
                                videoCache["thread_id"]));
                    },
                    SetReadData = (data) =>
                    {
                        postkey = Uri.UnescapeDataString(Encoding.UTF8.GetString(data)).Replace("postkey=", "");
                    },
                });

            //コメント投稿ストリームデータ生成
            var super = streamDataList[0].SetReadData;
            streamDataList[0].SetReadData = (data) =>
            {
                super(data);
                var streamData = context.Client.OpenUploadStreams(videoCache["ms"], ContentType.XML).GetStreamDatas();
                for (int i = 0; i < 2; i++)
                {
                    uploadStreamData[i].StreamType = streamData[i].StreamType;
                    uploadStreamData[i].GetStream = streamData[i].GetStream;
                }
            };

            uploadStreamData[0].GetWriteData = () =>
            {
                return Encoding.UTF8.GetBytes(
                    string.Format(PostTexts.PostVideoComment,
                        videoCache["thread_id"],
                        ((int)(Comment.PlayTime.TotalMilliseconds / 10)).ToString(),
                        Comment.Command,
                        videoCache["ticket"],
                        videoCache["user_id"],
                        postkey,
                        Comment.Body));
            };

            uploadStreamData[1].SetReadData = (data) =>
            {
                var serialize = new XmlSerializer(typeof(Serial.PostComment.Packet));
                var serial = (Serial.PostComment.Packet)serialize.Deserialize(new MemoryStream(data));
                result = converter.ConvertResponse(serial);
                videoCache["block_no"] = ((int)((serial.chat_result.no + 1) / 100)).ToString();//更新
            };
            streamDataList.AddRange(uploadStreamData);

            return new Streams<Response>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>コメントをダウンロードする</summary>
        public CommentResponse DownloadComment()
        {
            var streams = OpenCommentDownloadStreams();
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>コメントをダウンロードするストリームを取得する</summary>
        public Streams<CommentResponse> OpenCommentDownloadStreams()
        {
            var videoAccessStreamData = OpenVideoAccessStreamData();
            var streamDataList = new List<StreamData>();
            CommentResponse result = null;

            if (videoAccessStreamData != null)
                streamDataList.Add(videoAccessStreamData);

            streamDataList.Add(new StreamData()
            {
                StreamType = StreamType.Read,
                GetStream = () =>
                {
                    try
                    {
                        return context.Client.OpenDownloadStream(
                            videoCache["ms"] + string.Format(
                                PostTexts.GetVideoComment,
                                videoCache["thread_id"]));
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

                    videoCache.Add("ticket", serial.thread[0].ticket);
                    videoCache.Add("block_no", ((int)((serial.thread[0].last_res + 1) / 100)).ToString());

                    result = converter.ConvertCommentResponse(serial);
                }
            });

            return new Streams<CommentResponse>(
                streamDataList.ToArray(),
                () => result );
        }

        /// <summary>>動画の詳細情報を取得する、情報は0番目の配列に格納される</summary>
        /// <param name="IsHtml">合わせてHtmlから情報を取得するか、現在動画説明文のみ</param>
        public VideoInfoResponse DownloadVideoInfo(bool IsHtml = true)
        {
            var streams = OpenVideoInfoDownloadStreams(IsHtml);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>>動画の詳細情報を取得するストリームを取得する、情報は0番目の配列に格納される</summary>
        /// <param name="IsHtml">合わせてHtmlから情報を取得するか、現在動画説明文のみ</param>
        public Streams<VideoInfoResponse> OpenVideoInfoDownloadStreams(bool IsHtml = true)
        {
            var streamDataList = new List<StreamData>();
            VideoInfoResponse result = null;

            #region APIアクセス
            streamDataList.Add(
                new StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = () =>
                    {
                        try
                        {
                            return context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoInfo, target.ID));
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
                var videoPageStreamData = OpenVideoPageStreamData();
                if (videoPageStreamData != null)
                {
                    var super = videoPageStreamData.SetReadData;

                    videoPageStreamData.SetReadData = (data) =>
                    {
                        if (result.Status != Status.OK) return;

                        super(data);
                        result.VideoInfos[0].Description = HtmlTextRegex.VideoDescription.Match(htmlCache).Groups["value"].Value;
                    };

                    streamDataList.Add(videoPageStreamData);
                }
            }
            #endregion

            return new Streams<VideoInfoResponse>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>タグを取得する</summary>
        public TagResponse DownloadTags()
        {
            var streams = OpenTagsDownloadStreams();
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>タグを取得するストリームを取得する</summary>
        public Streams<TagResponse> OpenTagsDownloadStreams()
        {
            var streamDataList = new List<StreamData>();
            TagResponse result = null;

            streamDataList.Add(
                new StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = () =>
                    {
                        try
                        {
                            return context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoTag, target.ID));
                        }
                        catch (WebException e)
                        {
                            throw new WebException("タグ取得APIにアクセス出来ませんでした", e);
                        }
                    },
                    SetReadData = (data) =>
                    {
                        var serialize = new DataContractJsonSerializer(typeof(Serial.EditTag.Contract));
                        var serial = (Serial.EditTag.Contract)serialize.ReadObject(new MemoryStream(data));

                        result = converter.ConvertTagResponse(serial);
                    }
                });

            return new Streams<TagResponse>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>タグを追加する</summary>
        /// <param name="AddItem">追加するタグ</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public TagResponse AddTag(Tag AddItem, bool IsGetToken = true)
        {
            var streams = OpenEditTagStreams(AddItem, PostTexts.AddVideoTag, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>タグを追加するストリームを取得する</summary>
        /// <param name="AddItem">追加するタグ</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Streams<TagResponse> OpenAddTagStreams(Tag AddItem, bool IsGetToken = true)
        {
            return OpenEditTagStreams(AddItem, PostTexts.AddVideoTag, IsGetToken);
        }

        /// <summary>タグを削除する</summary>
        /// <param name="RemoveItem">削除するタグ</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public TagResponse RemoveTag(Tag RemoveItem, bool IsGetToken = true)
        {
            var streams = OpenEditTagStreams(RemoveItem, PostTexts.RemoveVideoTag, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>タグを削除するストリームを取得する</summary>
        /// <param name="RemoveItem">削除するタグ</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Streams<TagResponse> OpenRemoveTagStreams(Tag RemoveItem, bool IsGetToken = true)
        {
            return OpenEditTagStreams(RemoveItem, PostTexts.RemoveVideoTag, IsGetToken);
        }

        
        internal StreamData OpenThreadIDStreamData(Action<string> GetThread)
        {
            var videoAccessStreamData = OpenVideoAccessStreamData();

            if (videoAccessStreamData == null)
            {
                GetThread(videoCache["thread_id"]);
                return null;
            }
            else
            {
                var super = videoAccessStreamData.SetReadData;
                videoAccessStreamData.SetReadData = (data) =>
                {
                    super(data);
                    GetThread(videoCache["thread_id"]);
                };

                return videoAccessStreamData;
            }
        }


        private Streams<TagResponse> OpenEditTagStreams(Tag Tag, string PostText, bool IsGetToken)
        {
            var videoPageStreamData = OpenVideoPageStreamData();
            var streamDataList = new List<StreamData>();
            TagResponse result = null;

            if (videoPageStreamData != null)
                streamDataList.Add(videoPageStreamData);

            var uploadStreamData = context.Client.OpenUploadStreams(
                string.Format(ApiUrls.EditVideoTag, target.ID),
                ContentType.Form).GetStreamDatas();

            uploadStreamData[0].GetWriteData = () =>
            {
                if (IsGetToken)
                    host.token = HtmlTextRegex.VideoTagToken.Match(htmlCache).Groups["value"].Value;

                if (watchAuthKey == null)
                    watchAuthKey = HtmlTextRegex.WatchAuthKey.Match(htmlCache).Groups["value"].Value;

                return Encoding.UTF8.GetBytes(
                    string.Format(
                        PostText,
                        Tag.Name,
                        host.token,
                        watchAuthKey,
                        (Tag.IsLock == true) ? "1" : "0"));
            };
            uploadStreamData[1].SetReadData = (data) =>
            {
                var serialize = new DataContractJsonSerializer(typeof(Serial.EditTag.Contract));
                result = converter.ConvertTagResponse(
                    (Serial.EditTag.Contract)serialize.ReadObject(new MemoryStream(data)));
                target.Tags = result.Tags;
            };
            streamDataList.AddRange(uploadStreamData);

            return new Streams<TagResponse>(
                streamDataList.ToArray(),
                () => result);
        }

        private StreamData OpenVideoAccessStreamData()
        {
            if (videoCache != null) return null;

            return new StreamData()
            {
                StreamType = StreamType.Read,
                GetStream = () =>
                {
                    try
                    {
                        return context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideo, target.ID));
                    }
                    catch (WebException e)
                    {
                        throw new WebException("動画にアクセス出来ませんでした", e);
                    }
                },
                SetReadData = (data) =>
                {
                    var cacheString = Uri.UnescapeDataString(Encoding.UTF8.GetString(data));
                    var cacheStrings = cacheString.Split('&');
                    videoCache = new NameValueCollection();

                    for (int i = 0; i < cacheStrings.Length; i++)
                    {
                        var index = cacheStrings[i].IndexOf('=');
                        videoCache.Add(
                            cacheStrings[i].Substring(0, index),
                            cacheStrings[i].Substring(index + 1, cacheStrings[i].Length - 1 - index));
                    }
                },
            };
        }

        private StreamData OpenVideoPageStreamData()
        {
            if (htmlCache != null) return null;

            return new StreamData()
            {
                StreamType = StreamType.Read,
                GetStream = () =>
                {
                    try
                    {
                        return context.Client.OpenDownloadStream(string.Format(ApiUrls.VideoWatchPage, target.ID));
                    }
                    catch (WebException e)
                    {
                        throw new WebException("動画ページにアクセス出来ませんでした", e);
                    }
                },
                SetReadData = (data) =>
                {
                    htmlCache = Encoding.UTF8.GetString(data);
                }
            };
        }
    }
}