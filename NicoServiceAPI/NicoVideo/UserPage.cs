using NicoServiceAPI.Connection;
using NicoServiceAPI.NicoVideo.User;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>ユーザーページへアクセスする</summary>
    /******************************************/
    public class UserPage
    {
        User.User target;
        VideoService host;
        Context context;
        Serial.Converter converter;

        /******************************************/
        /******************************************/

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Target">ターゲットユーザー</param>
        /// <param name="Host">生成元</param>
        /// <param name="Context">コンテキスト</param>
        internal UserPage(User.User Target, VideoService Host, Context Context)
        {
            target = Target;
            host = Host;
            context = Context;
            converter = new Serial.Converter(context);
        }

        /// <summary>ユーザー情報を取得する</summary>
        /// <param name="IsHtml">ユーザー情報取得にHTMLを使用するかどうか、現在使用不可</param>
        public UserResponse DownloadUser(bool IsHtml = true)
        {
            var streams = OpenUserDownloadStream(IsHtml);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>ユーザー情報を取得するストリームを取得する</summary>
        /// <param name="IsHtml">ユーザー情報取得にHTMLを使用するかどうか、現在使用不可</param>
        public Streams<UserResponse> OpenUserDownloadStream(bool IsHtml = true)
        {
            var streamDataList = new List<StreamData>();
            UserResponse result = null;

            streamDataList.Add(
                new StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = (size) => context.Client.OpenDownloadStream(string.Format(ApiUrls.GetVideoUserHtml, target.ID)),
                    SetReadData = (data) =>
                    {
                        var html = Encoding.UTF8.GetString(data);

                        result = converter.ConvertUserResponse(
                            new GroupCollection[]
                            {
                                HtmlTextRegex.VideoUsers[0].Match(html).Groups,
                                HtmlTextRegex.VideoUsers[1].Match(html).Groups,
                                HtmlTextRegex.VideoUsers[2].Match(html).Groups,
                                HtmlTextRegex.VideoUsers[3].Match(html).Groups
                            });
                    },
                });

            return new Streams<UserResponse>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>マイリストグループを取得する、現在は自分のマイリストグループのみ、ユーザー指定は無視される</summary>
        public MylistGroupResponse DownloadMylistGroup()
        {
            var streams = OpenMylistGroupDownloadStream();
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストグループを取得するストリームを取得する、現在は自分のマイリストグループのみ、ユーザー指定は無視される</summary>
        public Streams<MylistGroupResponse> OpenMylistGroupDownloadStream()
        {
            var streamDataList = new List<StreamData>();
            MylistGroupResponse result = null;

            streamDataList.Add(
                new StreamData()
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

        /// <summary>マイリストを追加する</summary>
        /// <param name="AddItem">追加するマイリスト</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public AddMylistResponse AddMylist(Mylist.Mylist AddItem, bool IsGetToken = true)
        {
            var streams = OpenMylistAddStream(AddItem, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストを追加するストリームを取得する</summary>
        /// <param name="AddItem">追加するマイリスト</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Streams<AddMylistResponse> OpenMylistAddStream(Mylist.Mylist AddItem, bool IsGetToken = true)
        {
            var streamDataList = new List<StreamData>();
            AddMylistResponse result = null;

            if (IsGetToken)
                streamDataList.AddRange(host.GetToken());

            var updateStreamDatas = context.Client.OpenUploadStream(ApiUrls.AddMylist, ContentType.Form).GetStreamDatas();
            updateStreamDatas[0].GetWriteData = () =>
                Encoding.UTF8.GetBytes(string.Format(
                            PostTexts.AddMylist,
                            AddItem.Title,
                            AddItem.Description,
                            (AddItem.IsPublic) ? "1" : "0",
                            host.token));

            updateStreamDatas[1].SetReadData = (data) =>
            {
                var serialize = new DataContractJsonSerializer(typeof(Serial.AddMylist.Contract));
                result = converter.ConvertAddMylist((Serial.AddMylist.Contract)serialize.ReadObject(new MemoryStream(data)));
                result.AddedMylist.Title = AddItem.Title;
                result.AddedMylist.Description = AddItem.Description;
                result.AddedMylist.IsPublic = AddItem.IsPublic;
            };
            streamDataList.AddRange(updateStreamDatas);

            return new Streams<AddMylistResponse>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>マイリストを更新する</summary>
        /// <param name="UpdateItem">更新するマイリスト</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Response UpdateMylist(Mylist.Mylist UpdateItem, bool IsGetToken = true)
        {
            var streams = OpenMylistUpdateStream(UpdateItem, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストを更新するストリームを取得する</summary>
        /// <param name="UpdateItem">更新するマイリスト</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Streams<Response> OpenMylistUpdateStream(Mylist.Mylist UpdateItem, bool IsGetToken = true)
        {
            var streamDataList = new List<StreamData>();
            Response result = null;

            if (IsGetToken)
                streamDataList.AddRange(host.GetToken());

            var updateStreamDatas = context.Client.OpenUploadStream(ApiUrls.UpdateMylist, ContentType.Form).GetStreamDatas();
            updateStreamDatas[0].GetWriteData = () =>
                Encoding.UTF8.GetBytes(string.Format(
                            PostTexts.UpdateMylist,
                            UpdateItem.ID,
                            UpdateItem.Title,
                            UpdateItem.Description,
                            (UpdateItem.IsPublic) ? "1" : "0",
                            host.token));

            updateStreamDatas[1].SetReadData = (data) =>
            {
                var serialize = new DataContractJsonSerializer(typeof(Serial.UpdateMylist.Contract));
                result = converter.ConvertResponse((Serial.UpdateMylist.Contract)serialize.ReadObject(new MemoryStream(data)));
            };
            streamDataList.AddRange(updateStreamDatas);

            return new Streams<Response>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>マイリストを削除する</summary>
        /// <param name="RemoveItem">削除するマイリスト</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Response RemoveMylist(Mylist.Mylist RemoveItem, bool IsGetToken = true)
        {
            var streams = OpenMylistRemoveStream(RemoveItem, IsGetToken);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>マイリストを削除するストリームを取得する</summary>
        /// <param name="RemoveItem">削除するマイリスト</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Streams<Response> OpenMylistRemoveStream(Mylist.Mylist RemoveItem, bool IsGetToken = true)
        {
            var streamDataList = new List<StreamData>();
            Response result = null;

            if (IsGetToken)
                streamDataList.AddRange(host.GetToken());

            var updateStreamDatas = context.Client.OpenUploadStream(ApiUrls.RemoveMylist, ContentType.Form).GetStreamDatas();
            updateStreamDatas[0].GetWriteData = () =>
                Encoding.UTF8.GetBytes(string.Format(
                            PostTexts.RemoveMylist,
                            RemoveItem.ID,
                            host.token));

            updateStreamDatas[1].SetReadData = (data) =>
            {
                var str = Encoding.UTF8.GetString(data);

                var serialize = new DataContractJsonSerializer(typeof(Serial.RemoveMylist.Contract));
                result = converter.ConvertResponse((Serial.RemoveMylist.Contract)serialize.ReadObject(new MemoryStream(data)));
            };
            streamDataList.AddRange(updateStreamDatas);

            return new Streams<Response>(
                streamDataList.ToArray(),
                () => result);
        }

        /// <summary>視聴履歴をダウンロードする、ユーザー指定は無視される</summary>
        public ViewHistoryResponse DownloadViewHistory()
        {
            var streams = OpenViewHistoryDownloadStream();
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>視聴履歴をダウンロードするストリームを取得する、ユーザー指定は無視される</summary>
        public Streams<ViewHistoryResponse> OpenViewHistoryDownloadStream()
        {
            var serialize = new DataContractJsonSerializer(typeof(Serial.GetVideoViewHistory.Contract));
            var streamDataList = new List<StreamData>();
            ViewHistoryResponse result = null;

            streamDataList.Add(new StreamData()
            {
                StreamType = StreamType.Read,
                GetStream = (size) => context.Client.OpenDownloadStream(ApiUrls.GetVideoViewHistory),
                SetReadData = (data) =>
                {
                    var serial = (Serial.GetVideoViewHistory.Contract)serialize.ReadObject(new MemoryStream(data));
                    result = converter.ConvertViewHistoryResponse(serial);
                },
            });

            return new Streams<ViewHistoryResponse>(
                streamDataList.ToArray(),
                () => result);
        }
    }
}