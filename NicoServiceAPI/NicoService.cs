using NicoServiceAPI.Connection;
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
        Context context = new Context();
        VideoService videoService;

        /******************************************/
        /******************************************/

        /// <summary>初期化</summary>
        public NicoService()
        {
            context.Client = new Client();
        }


        /// <summary>ログインする</summary>
        /// <param name="Mail">メールアドレス</param>
        /// <param name="Password">パスワード</param>
        public bool Login(string Mail, string Password)
        {
            var tst = context.Client.Upload(
                ApiUrls.Login,
                Encoding.UTF8.GetBytes(
                    String.Format(PostTexts.Login, Mail, Password)),
                ContentType.Form);

            var str = Encoding.UTF8.GetString(tst);

            foreach (Cookie cookie in context.Client.CookieContainer.GetCookies(new Uri(ApiUrls.Host)))
                if (cookie.Name == "user_session")
                    return true;
            return false;
        }

        /// <summary>ニコニコ動画アクセスAPIを取得する</summary>
        public VideoService GetVideoService()
        {
            if (videoService == null)
                videoService = new VideoService(context);

            return videoService;
        }

        /// <summary>内部生成で使用するインスタンスコンテナを設定します</summary>
        /// <param name="InstanceContainer">設定するインスタンスコンテナ</param>
        public void SetInstanceContainer(InstanceContainer InstanceContainer)
        {
            context.InstanceContainer = InstanceContainer;
        }

        /// <summary>内部生成で使用するインスタンスコンテナを取得します</summary>
        public InstanceContainer GetInstanceContainer()
        {
            return context.InstanceContainer;
        }
    }
}
