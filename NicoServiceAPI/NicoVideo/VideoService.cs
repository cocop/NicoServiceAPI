using NicoServiceAPI.Connection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>ニコニコ動画サービスへアクセス</summary>
    /******************************************/
    public class VideoService
    {
        Context context;
        Serial.Converter converter;

        /******************************************/
        /******************************************/

        /// <summary>内部生成時、使用される</summary>
        /// <param name="Context">コンテキスト</param>
        internal VideoService(Context Context)
        {
            context = Context;
            converter = new Serial.Converter(context);
        }

        /// <summary>動画へアクセスするページを取得する</summary>
        /// <param name="Target">ターゲット動画</param>
        public VideoPage GetVideoPage(Video.VideoInfo Target)
        {
            if (Target.videoPage != null)
                return Target.videoPage;
            else
                return Target.videoPage = new VideoPage(Target, context);
        }

        /// <summary>ユーザーへアクセスするページを取得する</summary>
        /// <param name="Target">ターゲットユーザー</param>
        public UserPage GetUserPage(User.User Target)
        {
            if (Target.userPage != null)
                return Target.userPage;
            else
                return Target.userPage = new UserPage(Target, context);
        }

        /// <summary>ユーザーへアクセスするページを取得する</summary>
        /// <param name="Target">ターゲットユーザー</param>
        public MylistPage GetMylistPage(Mylist.Mylist Target)
        {
            if (Target.mylistPage != null)
                return Target.mylistPage;
            else
                return Target.mylistPage = new MylistPage(Target, this, context);
        }


        /// <summary>動画を検索する</summary>
        /// <param name="Keyword">検索キーワード</param>
        /// <param name="SearchPage">検索ページの指定、1～nの間の数値を指定する</param>
        /// <param name="SearchType">検索方法を指定する</param>
        /// <param name="SearchOption">検索オプションを指定する</param>
        public Video.VideoInfoResponse Search(
            string          Keyword,
            int             SearchPage,
            SearchType      SearchType,
            SearchOption    SearchOption)
        {
            var streams = OpenSearchStream(Keyword, SearchPage, SearchType, SearchOption);
            return streams.Run(streams.UntreatedCount);
        }

        /// <summary>動画を検索するストリームを取得する</summary>
        /// <param name="Keyword">検索キーワード</param>
        /// <param name="SearchPage">検索ページの指定、1～nの間の数値を指定する</param>
        /// <param name="SearchType">検索方法を指定する</param>
        /// <param name="SearchOption">検索オプションを指定する</param>
        public Streams<Video.VideoInfoResponse> OpenSearchStream(
            string          Keyword,
            int             SearchPage,
            SearchType      SearchType,
            SearchOption    SearchOption)
        {
            var serialize = new DataContractJsonSerializer(typeof(Serial.Search.Contract));
            var streamDataList = new List<StreamData>();
            Video.VideoInfoResponse lastData = null;

            streamDataList.Add(
                new StreamData()
                {
                    StreamType = StreamType.Read,
                    GetStream = (size) =>
                    {
                        return context.Client.OpenDownloadStream(
                            String.Format(
                                ApiUrls.VideoSearch,
                                SearchType.ToKey(),
                                Keyword,
                                SearchPage,
                                SearchOption.ToKey()));
                    },
                    SetReadData = (data) =>
                    {
                        lastData = converter.ConvertVideoInfoResponse(
                            (Serial.Search.Contract)serialize.ReadObject(new MemoryStream(data)));
                    }
                });

            return new Streams<Video.VideoInfoResponse>(
                streamDataList.ToArray(),
                () => lastData);
        }
    }
}
