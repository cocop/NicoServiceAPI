using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NicoServiceAPI.NicoVideo.User
{
    /// <summary></summary>
    public class Mylist
    {
        /// <summary>内部生成時、使用される。</summary>
        internal Mylist()
        {
        }

        /// <summary>IDを指定して作成する、とりあえずマイリストを指定する場合は空文字</summary>
        public Mylist(string ID)
        {
            this.ID = ID;
        }

        /******************************************/
        /******************************************/
        
        /// <summary>マイリストユーザー</summary>
        public User User { get; set; }

        /// <summary>マイリストID、空文字である場合はとりあえずマイリスト</summary>
        public string ID { get; set; }

        /// <summary>タイトル</summary>
        public string Title { get; set; }

        /// <summary>説明文</summary>
        public string Description { get; set; }

        /// <summary>お気に入り登録されている数</summary>
        public int BookmarkCount { get; set; }

        /// <summary>お気に入り登録しているか</summary>
        public bool IsBookmark { get; set; }

        /// <summary>公開設定にしているか</summary>
        public bool IsPublic { get; set; }

        /// <summary>作成時間</summary>
        public DateTime CreateTime { get; set; }

        /// <summary>更新時間</summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>マイリスト動画</summary>
        public MylistItem[] MylistItem { get; set; }

        /// <summary>マイリスト操作用トークン</summary>
        internal string token { get; set; }
    }
}
