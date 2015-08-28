using System;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>検索オプション</summary>
    /******************************************/
    public partial class SearchOption
    {
        /// <summary>ソート順</summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>ソートに使用する値</summary>
        public SortTarget SortTarget { get; set; }

        /// <summary>投稿時間</summary>
        public PostTime PostTime { get; set; }

        /// <summary>動画時間</summary>
        public VideoTime VideoTime { get; set; }

        /******************************************/
        /******************************************/

        /// <summary>オプション指定せず作成</summary>
        public SearchOption()
        {
        }

        /// <summary>オプション指定して作成</summary>
        public SearchOption(
            SortOrder   SortOrder,
            SortTarget  SortTarget,
            PostTime    PostTime,
            VideoTime   VideoTime)
        {
            
            this.SortOrder = SortOrder;
            this.SortTarget = SortTarget;
            this.PostTime = PostTime;
            this.VideoTime = VideoTime;
        }

        internal string ToKey()
        {
            string Key = "order={0}&sort={1}&f_range={2}&l_range={3}";
            string[] Keys = new string[4];

            const int sortOrder = 0;
            const int sortTarget = 1;
            const int postTime = 2;
            const int videoTime = 3;

            #region 昇順か降順か
            switch (SortOrder)
            {
                case SortOrder.None:
                    Keys[sortOrder] = "d";
                    break;
                case SortOrder.Up:
                    Keys[sortOrder] = "d";
                    break;
                case SortOrder.Down:
                    Keys[sortOrder] = "a";
                    break;
                default:
                    throw new Exception("設定したサーチオプションが不正です");
            }
            #endregion
            
            #region ソート
            switch (SortTarget)
            {
                case SortTarget.None:
                    Keys[sortTarget] = "n";
                    break;
                case SortTarget.Comment:
                    Keys[sortTarget] = "n";
                    break;
                case SortTarget.ViewCount:
                    Keys[sortTarget] = "v";
                    break;
                case SortTarget.CommentCount:
                    Keys[sortTarget] = "r";
                    break;
                case SortTarget.MylistCount:
                    Keys[sortTarget] = "m";
                    break;
                case SortTarget.PostTime:
                    Keys[sortTarget] = "f";
                    break;
                case SortTarget.VideoTime:
                    Keys[sortTarget] = "l";
                    break;
                default:
                    throw new Exception("設定したサーチオプションが不正です");
            }
            #endregion

            #region 投稿時間の指定
            switch (PostTime)
            {
                case PostTime.None:
                    Keys[postTime] = "";
                    break;
                case PostTime.WithinDay:
                    Keys[postTime] = "1";
                    break;
                case PostTime.WithinWeek:
                    Keys[postTime] = "2";
                    break;
                case PostTime.WithinMonth:
                    Keys[postTime] = "3";
                    break;
                default:
                    throw new Exception("設定したサーチオプションが不正です");
            }
            #endregion

            #region 再生時間の指定
            switch (VideoTime)
            {
                case VideoTime.None:
                    Keys[videoTime] = "";
                    break;
                case VideoTime.Short:
                    Keys[videoTime] = "1";
                    break;
                case VideoTime.Long:
                    Keys[videoTime] = "2";
                    break;
                default:
                    throw new Exception("設定したサーチオプションが不正です");
            }
            #endregion

            return String.Format(Key, Keys);
        }
    }
}
