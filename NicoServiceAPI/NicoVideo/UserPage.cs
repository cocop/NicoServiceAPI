using NicoServiceAPI.Connection;
using NicoServiceAPI.NicoVideo.User;
using System;
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

        /******************************************/
        /******************************************/

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Context">コンテキスト</param>
        internal UserPage(Context Context)
        {
            context = Context;
            converter = new Serial.Converter(context);
        }

        /// <summary>マイリストグループを取得する、現在は自分のマイリストグループのみ、ユーザー指定は無視される</summary>
        /// <param name="User">ユーザーの指定</param>
        public MylistGroupResponse DownloadMylistGroup(User.User User)
        {
            var serialize = new DataContractJsonSerializer(typeof(Serial.GetMylistGroup.Contract));
            var mylistGroup = Encoding.UTF8.GetString(context.Client.Download(
                ApiUrls.GetMylistGroup));

            return converter.ConvertMylistGroupResponse((Serial.GetMylistGroup.Contract)serialize.ReadObject(new MemoryStream(
                Encoding.UTF8.GetBytes(Common.UnicodeDecode(mylistGroup)))));
        }

        /// <summary>マイリストを取得する</summary>
        /// <param name="Mylist">IDが空文字である場合、とりあえずマイリストを取得する</param>
        /// <param name="IsHtml">マイリスト取得にHTMLを取得するかどうか</param>
        public MylistResponse DownloadMylist(Mylist Mylist, bool IsHtml = false)
        {
            var deflistSerialize = new DataContractJsonSerializer(typeof(Serial.GetDeflist.Contract));
            var mylistSerialize = new DataContractJsonSerializer(typeof(Serial.GetMylist.Contract));
            string mylistSerial;
            GroupCollection mylistInfo = null;
            GroupCollection mylistUserInfo = null;

            if (Mylist.ID == "")
            {
                mylistSerial = Encoding.UTF8.GetString(context.Client.Download(
                    ApiUrls.GetVideoDeflist));
            }
            else if (IsHtml)
            {
                //HTML内にJSON文があるので抜き出してシリアライズ、JavaScriptオブジェクトもあるので一緒に抜き出す
                var html = Encoding.UTF8.GetString(context.Client.Download(string.Format(ApiUrls.GetVideoMylistHtml, Mylist.ID)));
                var mylist = HtmlTextRegex.VideoMylist.Match(html).Groups["value"].Value;

                mylistInfo = HtmlTextRegex.VideoMylistInfoCutout.Match(
                    HtmlTextRegex.VideoMylistInfo.Match(html).Groups["value"].Value).Groups;

                mylistUserInfo = HtmlTextRegex.VideoMylistUserInfoCutout.Match(
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
            }
            else//APIを使用してマイリストを取得する、この場合帰ってくるJSONが他と違うので注意
            {
                return converter.ConvertMylistResponse(
                    (Serial.GetMylist.Contract)mylistSerialize.ReadObject(context.Client.OpenDownloadStream(
                        string.Format(ApiUrls.GetVideoMylist, Mylist.ID))),
                        Mylist.ID);
            }

            return converter.ConvertMylistResponse(
                (Serial.GetDeflist.Contract)deflistSerialize.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(mylistSerial))),
                mylistInfo,
                mylistUserInfo);
        }

        /// <summary>指定したマイリストへ動画を追加する</summary>
        /// <param name="Target">指定するマイリスト</param>
        /// <param name="Add">追加する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public Response MylistAddVideo(Mylist Target, MylistItem AddItem, bool IsGetToken = true)
        {
            var serialize = new DataContractJsonSerializer(typeof(Serial.MylistAddVideo.Contract));
            string responseSerial;

            if (IsGetToken)
                Target.token = HtmlTextRegex.VideoMylistToken.Match(Encoding.UTF8.GetString(context.Client.Download(
                    ApiUrls.Host + "my/mylist"))).Groups["value"].Value;

            if (Target.ID == "")
            {
                responseSerial = Common.UnicodeDecode(Encoding.UTF8.GetString(context.Client.Upload(
                    ApiUrls.DeflistAddVideo,
                    Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.DeflistAddVideo,
                        AddItem.VideoInfo.ID,
                        AddItem.Description,
                        Target.token)))));
            }
            else
            {
                responseSerial = Common.UnicodeDecode(Encoding.UTF8.GetString(context.Client.Upload(
                    ApiUrls.MylistAddVideo,
                    Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.MylistAddVideo,
                        Target.ID,
                        AddItem.VideoInfo.ID,
                        AddItem.Description,
                        "",
                        Target.token)))));
            }

            return converter.ConvertMylistAddVideoResponse(
                (Serial.MylistAddVideo.Contract)serialize.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseSerial))));
        }

        /// <summary>指定したマイリストから動画を削除する</summary>
        /// <param name="Target">指定するマイリスト</param>
        /// <param name="Add">削除する動画</param>
        /// <param name="IsGetToken">トークンを取得するかどうか</param>
        public MylistRemoveVideoResponse MylistRemoveVideo(Mylist Target, Video.VideoInfo RemoveItem, bool IsGetToken = true)
        {
            var serialize = new DataContractJsonSerializer(typeof(Serial.MylistRemoveVideo.Contract));
            string responseSerial;

            if (IsGetToken)
                Target.token = HtmlTextRegex.VideoMylistToken.Match(Encoding.UTF8.GetString(context.Client.Download(
                    ApiUrls.Host + "my/mylist"))).Groups["value"].Value;

            var streams = new Streams<byte>(VideoPage.OpenVideoAccessStream(RemoveItem, context.Client), () => 0);
            streams.Run(streams.UntreatedCount);

            if (Target.ID == "")
            {
                responseSerial = Common.UnicodeDecode(Encoding.UTF8.GetString(context.Client.Upload(
                    ApiUrls.DeflistRemoveVideo,
                    Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.DeflistRemoveVideo,
                        string.Format(PostTexts.ArrayMylistItem, RemoveItem.cache["thread_id"]),
                        Target.token)))));
            }
            else
            {
                responseSerial = Common.UnicodeDecode(Encoding.UTF8.GetString(context.Client.Upload(
                    ApiUrls.MylistRemoveVideo,
                    Encoding.UTF8.GetBytes(string.Format(
                        PostTexts.MylistRemoveVideo,
                        Target.ID,
                        string.Format(PostTexts.ArrayMylistItem, RemoveItem.cache["thread_id"]),
                        Target.token)))));
            }

            return converter.ConvertMylistRemoveVideoResponse(
                (Serial.MylistRemoveVideo.Contract)serialize.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseSerial))));
        }
    }
}