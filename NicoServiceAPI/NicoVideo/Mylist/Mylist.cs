using System;

namespace NicoServiceAPI.NicoVideo.Mylist
{
    /******************************************/
    /// <summary>マイリスト</summary>
    /******************************************/
    public class Mylist
    {
        /// <summary>IDを指定せず作成する</summary>
        public Mylist()
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
        public User.User User { get; set; }

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

        /// <summary>マイリストページ</summary>
        internal MylistPage mylistPage { get; set; }
    }
}
