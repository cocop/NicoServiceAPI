using NicoServiceAPI.Connection;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>マイリストページへアクセスする</summary>
    /******************************************/
    public class MylistPage
    {
        Mylist.Mylist target;
        VideoService host;
        Context context;
        Serial.Converter converter;

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Target">ターゲットマイリスト</param>
        /// <param name="Host">ページを取得できるクラス</param>
        /// <param name="Context">コンテキスト</param>
        internal MylistPage(Mylist.Mylist Target, VideoService Host, Context Context)
        {
            target = Target;
            context = Context;
            converter = new Serial.Converter(context);
            host = Host;
        }

        /// <summary>マイリストを取得する</summary>
        /// <param name="IsHtml">マイリスト取得にHTMLを使用するかどうか</param>
        public Mylist.MylistResponse DownloadMylist(bool IsHtml = false)
        {
            var streams = OpenMylistDownloadStream(IsHtml);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストを取得するストリームを取得する</summary>
        /// <param name="IsHtml">マイリスト取得にHTMLを使用するかどうか</param>
        public Streams<Mylist.MylistResponse> OpenMylistDownloadStream(bool IsHtml = false)
        {
            var streamDataList = new List<StreamData>();
            var deflistSerialize = new DataContractJsonSerializer(typeof(Serial.GetDeflist.Contract));
            var mylistSerialize = new DataContractJsonSerializer(typeof(Serial.GetMylist.Contract));
            Mylist.MylistResponse result = null;


            if (target.ID == "")
            {
                #region とりあえずマイリスト
                streamDataList.Add(
                    new StreamData()
                    {
                        StreamType = StreamType.Read,
                        GetStream = (size) => context.Client.OpenDownloadStream(ApiUrls.GetVideoDeflist),
                        SetReadData = (data) => result = converter.ConvertMylistResponse((Serial.GetDeflist.Contract)deflistSerialize.ReadObject(new MemoryStream(data)), null, null),
                    });
                #endregion
            }
            else if (!IsHtml)
            {
                #region API使用
                streamDataList.Add(
                    new StreamData()
                    {
                        StreamType = StreamType.Read,
                        GetStream = (size) => context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoMylist, target.ID)),
                        SetReadData = (data) =>
                        {
                            result = converter.ConvertMylistResponse(
                                (Serial.GetMylist.Contract)mylistSerialize.ReadObject(context.Client.OpenDownloadStream(
                                    string.Format(ApiUrls.GetVideoMylist, target.ID))),
                                    target.ID);
                        },
                    });
                #endregion
            }
            else
            {
                #region HTML使用
                streamDataList.Add(
                    new StreamData()
                    {
                        StreamType = StreamType.Read,
                        GetStream = (size) => context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoMylistHtml, target.ID)),
                        SetReadData = (data) =>
                        {
                            var html = Encoding.UTF8.GetString(data);
                            var mylist = HtmlTextRegex.VideoMylist.Match(html).Groups["value"].Value;
                            string mylistSerial;

                            var mylistInfo = HtmlTextRegex.VideoMylistInfoCutout.Match(
                                HtmlTextRegex.VideoMylistInfo.Match(html).Groups["value"].Value).Groups;

                            var mylistUserInfo = HtmlTextRegex.VideoMylistUserInfoCutout.Match(
                                HtmlTextRegex.VideoMylistUserInfo.Match(html).Groups["value"].Value).Groups;

                            if (mylist != "")
                            {
                                mylist = Common.UnicodeDecode(mylist);
                                mylistSerial = "{" + "\"mylistitem\":" + mylist + ", \"status\" : \"ok\"}";
                            }
                            else
                            {
                                mylistSerial = "{" + "\"mylistitem\":[], \"status\" : \"fail\"}";
                            }

                            result = converter.ConvertMylistResponse(
                                (Serial.GetDeflist.Contract)deflistSerialize.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(mylistSerial))),
                                mylistInfo,
                                mylistUserInfo);
                        },
                    });
                #endregion
            }

            return new Streams<Mylist.MylistResponse>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>マイリストへ動画を追加する</summary>
        /// <param name="AddItem">追加する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Response MylistAddVideo(Mylist.MylistItem AddItem, bool IsGetToken = true)
        {
            var streams = OpenMylistAddVideoStream(AddItem, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストへ動画を追加するストリームを取得する</summary>
        /// <param name="AddItem">追加する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Streams<Response> OpenMylistAddVideoStream(Mylist.MylistItem AddItem, bool IsGetToken = true)
        {
            var streamDataList = new List<StreamData>();
            StreamData[] uploadStreamDatas = null;
            Response result = null;

            if (IsGetToken)
                streamDataList.AddRange(host.GetToken());

            if (target.ID == "")//とりあえずマイリスト
            {
                uploadStreamDatas = context.Client.OpenUploadStream(ApiUrls.DeflistAddVideo, ContentType.Form).GetStreamDatas();
                uploadStreamDatas[0].GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.DeflistAddVideo,
                        AddItem.VideoInfo.ID,
                        AddItem.Description,
                        host.token));
                };
            }
            else
            {
                uploadStreamDatas = context.Client.OpenUploadStream(ApiUrls.MylistAddVideo, ContentType.Form).GetStreamDatas();
                uploadStreamDatas[0].GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.MylistAddVideo,
                        target.ID,
                        AddItem.VideoInfo.ID,
                        AddItem.Description,
                        "",
                        host.token));
                };
            }

            uploadStreamDatas[1].SetReadData = (data) =>
            {
                var serialize = new DataContractJsonSerializer(typeof(Serial.MylistAddVideo.Contract));
                result = converter.ConvertResponse(
                    (Serial.MylistAddVideo.Contract)serialize.ReadObject(new MemoryStream(data)));
            };
            streamDataList.AddRange(uploadStreamDatas);

            return new Streams<Response>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>マイリストから動画を削除する</summary>
        /// <param name="RemoveItem">削除する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Mylist.MylistRemoveVideoResponse MylistRemoveVideo(Video.VideoInfo RemoveItem, bool IsGetToken = true)
        {
            var streams = OpenMylistRemoveVideStream(RemoveItem, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストから動画を削除するストリームを取得する</summary>
        /// <param name="RemoveItem">削除する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Streams<Mylist.MylistRemoveVideoResponse> OpenMylistRemoveVideStream(Video.VideoInfo RemoveItem, bool IsGetToken = true)
        {
            var streamDataList = new List<StreamData>();
            StreamData[] uploadStreamDatas = null;
            Mylist.MylistRemoveVideoResponse result = null;
            string threadID = "";
            var getThreadID = host.GetVideoPage(RemoveItem).OpenThreadIDStreamData((data) => threadID = data);

            if (IsGetToken)
                streamDataList.AddRange(host.GetToken());

            if (getThreadID != null)
                streamDataList.Add(getThreadID);

            if (target.ID == "")//とりあえずマイリスト
            {
                uploadStreamDatas = context.Client.OpenUploadStream(ApiUrls.DeflistRemoveVideo, ContentType.Form).GetStreamDatas();
                uploadStreamDatas[0].GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.DeflistRemoveVideo,
                        string.Format(PostTexts.ArrayMylistItem, threadID),
                        host.token));
                };
            }
            else
            {
                uploadStreamDatas = context.Client.OpenUploadStream(ApiUrls.MylistRemoveVideo, ContentType.Form).GetStreamDatas();
                uploadStreamDatas[0].GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.MylistRemoveVideo,
                        target.ID,
                        string.Format(PostTexts.ArrayMylistItem, threadID),
                        host.token));
                };
            }

            uploadStreamDatas[1].SetReadData = (data) =>
            {
                var serialize = new DataContractJsonSerializer(typeof(Serial.MylistRemoveVideo.Contract));
                result = converter.ConvertMylistRemoveVideoResponse(
                    (Serial.MylistRemoveVideo.Contract)serialize.ReadObject(new MemoryStream(data)));
            };
            streamDataList.AddRange(uploadStreamDatas);

            return new Streams<Mylist.MylistRemoveVideoResponse>(
                streamDataList.ToArray(),
                () => result);
        }
    }
}
