using System;
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
        VideoPage videoPage;
        UserPage userPage;

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
        public VideoPage GetVideoPage()
        {
            if (videoPage == null)
                videoPage = new VideoPage(context);
            return videoPage;
        }

        /// <summary>ユーザーへアクセスするページを取得する</summary>
        public UserPage GetUserPage()
        {
            if (userPage == null)
                userPage = new UserPage(context);
            return userPage;
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
            var serialize = new DataContractJsonSerializer(typeof(Serial.Search.Contract));
            var serial = (Serial.Search.Contract)serialize.ReadObject(
                context.Client.OpenDownloadStream(
                    String.Format(
                        ApiUrls.VideoSearch,
                        SearchType.ToKey(),
                        Keyword,
                        SearchPage,
                        SearchOption.ToKey())));

            return converter.ConvertVideoInfoResponse(serial);
        }

    }
}
