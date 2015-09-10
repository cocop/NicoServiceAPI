using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//res.nimg.jp/js/my3.js参照
//res.nimg.jp/js/nicoapi.js
//jQuery.ajaxで検索

namespace NicoServiceAPI.NicoVideo.User
{
    /// <summary>ユーザー情報</summary>
    public class User
    {
        /// <summary>IDを指定して初期化</summary>
        /// <param name="ID">ユーザーID、空文字の場合自分のアカウント</param>
        public User(string ID)
        {
            this.ID = ID;
        }

        /// <summary>ユーザーID</summary>
        public string ID { get; set; }

        /// <summary>ユーザー名</summary>
        public string Name { get; set; }

        /// <summary>性別</summary>
        public string Sex { get; set; }

        /// <summary>生年月日</summary>
        public string Birthday { get; set; }
        
        /// <summary>お住まいの地域</summary>
        public string Area { get; set; }

        /// <summary>お気に入り登録された数</summary>
        public int BookmarkCount { get; set; }

        /// <summary>スタンプ経験値</summary>
        public int Experience { get; set; }

        /// <summary>アイコン</summary>
        public Picture Icon { get; set; }
    }
}
