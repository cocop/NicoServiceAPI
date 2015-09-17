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
        Context context;
        Serial.Converter converter;

        /******************************************/
        /******************************************/

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Target">ターゲットユーザー</param>
        /// <param name="Context">コンテキスト</param>
        internal UserPage(User.User Target, Context Context)
        {
            context = Context;
            converter = new Serial.Converter(context);

            target = Target;
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