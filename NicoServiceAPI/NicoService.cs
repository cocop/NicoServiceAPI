using NicoServiceAPI.NicoVideo;
using System;
using System.Net;
using System.Text;

namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>ドワンゴサービスAPIへアクセス</summary>
    /******************************************/
    public class NicoService
    {
        Connection.Client client = new Connection.Client();
        VideoService videoService;

        /******************************************/
        /******************************************/

        /// <summary>ログインする</summary>
        /// <param name="Mail">メールアドレス</param>
        /// <param name="Password">パスワード</param>
        public bool Login(string Mail, string Password)
        {
            client.Upload(
                ApiUrls.Login,
                Encoding.UTF8.GetBytes(
                    String.Format(PostTexts.Login, Mail, Password)));

            foreach (Cookie cookie in client.CookieContainer.GetCookies(new Uri(ApiUrls.Host)))
                if (cookie.Name == "user_session")
                    return true;
            return false;
        }

        /// <summary>ニコニコ動画アクセスAPIを取得する</summary>
        public VideoService GetVideoService()
        {
            if (videoService == null)
                videoService = new VideoService(client);

            return videoService;
        }
    }
}
