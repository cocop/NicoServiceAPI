using NicoServiceAPI.Connection;
using NicoServiceAPI.NicoVideo.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>ユーザーページへアクセスする</summary>
    /******************************************/
    public class UserPage
    {
        Context context;
        Serial.Converter converter;

        string token = "";

        /******************************************/
        /******************************************/

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Context">コンテキスト</param>
        internal UserPage(Context Context)
        {
            context = Context;
            converter = new Serial.Converter(context);
        }

        /// <summary>指定したユーザー情報を取得する</summary>
        /// <param name="User">ユーザーの指定</param>
        /// <param name="IsHtml">ユーザー情報取得にHTMLを使用するかどうか、現在使用不可</param>
        public UserResponse DownloadUser(User.User User, bool IsHtml = true)
        {
            var streams = OpenUserDownloadStream(User, IsHtml);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>指定したユーザー情報を取得するストリームを取得する</summary>
        /// <param name="User">ユーザーの指定</param>
        /// <param name="IsHtml">ユーザー情報取得にHTMLを使用するかどうか、現在使用不可</param>
        public Connection.Streams<UserResponse> OpenUserDownloadStream(User.User User, bool IsHtml = true)
        {
            var streamDataList = new List<Connection.StreamData>();
            UserResponse result = null;

            streamDataList.Add(
                new Connection.StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = (size) => context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoUserHtml, User.ID)),
                    SetReadData = (data) =>
                    {
                        var html = Encoding.UTF8.GetString(data);

                        result = converter.ConvertUserResponse(
                            new GroupCollection[]
                            {
                                HtmlTextRegex.VideoUserCutouts[0].Match(html).Groups,
                                HtmlTextRegex.VideoUserCutouts[1].Match(html).Groups,
                                HtmlTextRegex.VideoUserCutouts[2].Match(html).Groups,
                                HtmlTextRegex.VideoUserCutouts[3].Match(html).Groups
                            });
                    },
                });

            return new Streams<UserResponse>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>マイリストグループを取得する、現在は自分のマイリストグループのみ、ユーザー指定は無視される</summary>
        /// <param name="User">ユーザーの指定</param>
        public MylistGroupResponse DownloadMylistGroup(User.User User)
        {
            var streams = OpenMylistGroupDownloadStream(User);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストグループを取得するストリームを取得する、現在は自分のマイリストグループのみ、ユーザー指定は無視される</summary>
        /// <param name="User">ユーザーの指定</param>
        public Connection.Streams<MylistGroupResponse> OpenMylistGroupDownloadStream(User.User User)
        {
            var streamDataList = new List<Connection.StreamData>();
            MylistGroupResponse result = null;

            streamDataList.Add(
                new Connection.StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = (size) => context.Client.OpenDownloadStream(ApiUrls.GetMylistGroup),
                    SetReadData = (data) =>
                    {
                        var serialize = new DataContractJsonSerializer(typeof(Serial.GetMylistGroup.Contract));
                        result = converter.ConvertMylistGroupResponse((Serial.GetMylistGroup.Contract)serialize.ReadObject(new MemoryStream(data)));
                    },
                });

            return new Streams<MylistGroupResponse>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>マイリストを取得する</summary>
        /// <param name="Mylist">IDが空文字である場合、とりあえずマイリストを取得する</param>
        /// <param name="IsHtml">マイリスト取得にHTMLを使用するかどうか</param>
        public MylistResponse DownloadMylist(Mylist Mylist, bool IsHtml = false)
        {
            var streams = OpenMylistDownloadStream(Mylist, IsHtml);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストを取得するストリームを取得する</summary>
        /// <param name="Mylist">IDが空文字である場合、とりあえずマイリストを取得する</param>
        /// <param name="IsHtml">マイリスト取得にHTMLを使用するかどうか</param>
        public Connection.Streams<MylistResponse> OpenMylistDownloadStream(Mylist Mylist, bool IsHtml = false)
        {
            var streamDataList = new List<Connection.StreamData>();
            var deflistSerialize = new DataContractJsonSerializer(typeof(Serial.GetDeflist.Contract));
            var mylistSerialize = new DataContractJsonSerializer(typeof(Serial.GetMylist.Contract));
            MylistResponse result = null;


                if (Mylist.ID == "")
                {
                    #region とりあえずマイリスト
                    streamDataList.Add(
                        new Connection.StreamData()
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
                        new Connection.StreamData()
                        {
                            StreamType = StreamType.Read,
                            GetStream = (size) => context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoMylist, Mylist.ID)),
                            SetReadData = (data) =>
                            {
                                result = converter.ConvertMylistResponse(
                                    (Serial.GetMylist.Contract)mylistSerialize.ReadObject(context.Client.OpenDownloadStream(
                                        string.Format(ApiUrls.GetVideoMylist, Mylist.ID))),
                                        Mylist.ID);
                            },
                        });
                    #endregion
                }
                else
                {
                    #region HTML使用
                    streamDataList.Add(
                        new Connection.StreamData()
                        {
                            StreamType = StreamType.Read,
                            GetStream = (size) => context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoMylistHtml, Mylist.ID)),
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

                return new Streams<MylistResponse>(
                    streamDataList.ToArray(),
                    () => result);
        }

        /// <summary>指定したマイリストへ動画を追加する</summary>
        /// <param name="Target">指定するマイリスト</param>
        /// <param name="Add">追加する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Response MylistAddVideo(Mylist Target, MylistItem AddItem, bool IsGetToken = true)
        {
            var streams = OpenMylistAddVideoStream(Target, AddItem, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>指定したマイリストへ動画を追加するストリームを取得する</summary>
        /// <param name="Target">指定するマイリスト</param>
        /// <param name="Add">追加する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Connection.Streams<Response> OpenMylistAddVideoStream(Mylist Target, MylistItem AddItem, bool IsGetToken = true)
        {
            var streamDataList = new List<Connection.StreamData>();
            Connection.StreamData[] uploadStreamDatas;
            Response result = null;
            
            if (IsGetToken)
                streamDataList.AddRange(GetToken());

            if (Target.ID == "")//とりあえずマイリスト
            {
                uploadStreamDatas = context.Client.OpenUploadStream(ApiUrls.DeflistAddVideo).GetStreamDatas();
                uploadStreamDatas[0].GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.DeflistAddVideo,
                        AddItem.VideoInfo.ID,
                        AddItem.Description,
                        token));
                };
            }
            else
            {
                uploadStreamDatas = context.Client.OpenUploadStream(ApiUrls.MylistAddVideo).GetStreamDatas();
                uploadStreamDatas[0].GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.MylistAddVideo,
                        Target.ID,
                        AddItem.VideoInfo.ID,
                        AddItem.Description,
                        "",
                        token));
                };
            }

            uploadStreamDatas[1].SetReadData = (data) =>
            {
                var serialize = new DataContractJsonSerializer(typeof(Serial.MylistAddVideo.Contract));
                result =  converter.ConvertResponse(
                    (Serial.MylistAddVideo.Contract)serialize.ReadObject(new MemoryStream(data)));
            };
            streamDataList.AddRange(uploadStreamDatas);

            return new Streams<Response>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>指定したマイリストから動画を削除する</summary>
        /// <param name="Target">指定するマイリスト</param>
        /// <param name="Add">削除する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public MylistRemoveVideoResponse MylistRemoveVideo(Mylist Target, Video.VideoInfo RemoveItem, bool IsGetToken = true)
        {
            var streams = OpenMylistRemoveVideStream(Target, RemoveItem, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>指定したマイリストから動画を削除するストリームを取得する</summary>
        /// <param name="Target">指定するマイリスト</param>
        /// <param name="Add">削除する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Connection.Streams<MylistRemoveVideoResponse> OpenMylistRemoveVideStream(Mylist Target, Video.VideoInfo RemoveItem, bool IsGetToken = true)
        {
            var streamDataList = new List<Connection.StreamData>();
            Connection.StreamData[] uploadStreamDatas;
            MylistRemoveVideoResponse result = null;

            if (IsGetToken)
                streamDataList.AddRange(GetToken());

            streamDataList.AddRange(VideoPage.OpenVideoAccessStream(RemoveItem, context.Client));

            if (Target.ID == "")//とりあえずマイリスト
            {
                uploadStreamDatas = context.Client.OpenUploadStream(ApiUrls.DeflistRemoveVideo).GetStreamDatas();
                uploadStreamDatas[0].GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.DeflistRemoveVideo,
                        string.Format(PostTexts.ArrayMylistItem, RemoveItem.cache["thread_id"]),
                        token));
                };
            }
            else
            {
                uploadStreamDatas = context.Client.OpenUploadStream(ApiUrls.MylistRemoveVideo).GetStreamDatas();
                uploadStreamDatas[0].GetWriteData = () =>
                {
                    return Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.MylistRemoveVideo,
                        Target.ID,
                        string.Format(PostTexts.ArrayMylistItem, RemoveItem.cache["thread_id"]),
                        token));
                };
            }

            uploadStreamDatas[1].SetReadData = (data) =>
            {
                var serialize = new DataContractJsonSerializer(typeof(Serial.MylistRemoveVideo.Contract));
                result = converter.ConvertMylistRemoveVideoResponse(
                    (Serial.MylistRemoveVideo.Contract)serialize.ReadObject(new MemoryStream(data)));
            };
            streamDataList.AddRange(uploadStreamDatas);

            return new Streams<MylistRemoveVideoResponse>(
                streamDataList.ToArray(),
                () => result);
        }


        Connection.StreamData[] GetToken()
        {
            return new Connection.StreamData[]
            {
                new Connection.StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = (size) => context.Client.OpenDownloadStream(ApiUrls.Host + "my/mylist"),
                    SetReadData = (data) =>
                    {
                        token = HtmlTextRegex.
                            VideoMylistToken.
                            Match(Encoding.UTF8.GetString(data)).
                            Groups["value"].Value;
                    },
                },
            };
        }
    }
}