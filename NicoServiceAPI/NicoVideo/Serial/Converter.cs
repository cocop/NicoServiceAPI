using System;
using System.Text.RegularExpressions;

namespace NicoServiceAPI.NicoVideo.Serial
{
    internal static class Converter
    {
        static DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /********************************************/

        public static CommentResponse ConvertCommentResponse(GetComment.Packet Serial)
        {
            return new CommentResponse()
            {
                Comment = ConvertComment(Serial.chat),
            };
        }

        public static VideoInfoResponse ConvertVideoInfoResponse(GetInfo.NicovideoThumbResponse Serial, Connection.Client Client)
        {
            var result = new VideoInfoResponse();

            result.Status = ConvertStatus(Serial.status, (Serial.error == null) ? null : Serial.error.code);
            result.ErrorMessage = Serial.message;

            if (Serial.thumb != null)
            {
                result.VideoInfos = new NicoVideo.VideoInfo[]
                {
                    new VideoInfo()
                    {
                        ComentCounter = Serial.thumb.comment_num,
                        Description = Serial.thumb.description,
                        EconomyVideoSize = Serial.thumb.size_low,
                        IsExternalPlay = Serial.thumb.embeddable,
                        ID = Serial.thumb.video_id,
                        Length = ConvertTimeSpan(Serial.thumb.length),
                        MylistCounter = Serial.thumb.mylist_counter,
                        IsLivePlay = !Serial.thumb.no_live_play,
                        PostTime = DateTime.Parse(Serial.thumb.first_retrieve),
                        Tags = ConvertTags(Serial.thumb.tags),
                        Thumbnail = new Thumbnail(Serial.thumb.thumbnail_url, Client),
                        Title = Serial.thumb.title,
                        VideoSize = Serial.thumb.size_high,
                        VideoType = Serial.thumb.movie_type,
                        ViewCounter = Serial.thumb.view_counter,
                    }
                };
            }

            return result;
        }

        public static VideoInfoResponse ConvertVideoInfoResponse(Search.Contract Serial, Connection.Client client)
        {
            return new VideoInfoResponse()
            {
                ErrorMessage = Serial.message,
                Status = ConvertStatus(Serial.status, null),
                VideoInfos = ConvertVideoInfo(Serial.list, client),
            };
        }

        public static MylistResponse ConvertMylistResponse(GetMylist.Contract Serial, GroupCollection MylistInfoData, GroupCollection MylistUserInfoData, Connection.Client Client)
        {
            return new MylistResponse()
            {
                Mylist = ConvertMylist(Serial.mylistitem, MylistInfoData, MylistUserInfoData, Client),
                Status = ConvertStatus(Serial.status, ""),
            };
        }

        /********************************************/

        private static Comment[] ConvertComment(GetComment.Chat[] Serial)
        {
            var result = new Comment[Serial.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Comment()
                {
                    IsAnonymity = Serial[i].anonymity,
                    Body = Serial[i].body,
                    Command = Serial[i].mail,
                    Leaf = Serial[i].leaf,
                    No = Serial[i].no,
                    PlayTime = TimeSpan.FromMilliseconds(double.Parse(Serial[i].vpos + '0')),
                    IsPremium = Serial[i].premium,
                    UserID = Serial[i].user_id,
                    WriteTime = unixTime.AddSeconds(double.Parse(Serial[i].date)).ToLocalTime(),
                    IsYourPost = Serial[i].yourpost,
                };
            }

            return result;
        }

        private static Status ConvertStatus(string Serial, string ErrorCode)
        {
            Status result;

            switch (Serial)
            {
                case "ok": result = Status.OK; break;
                case "fail":
                    switch (ErrorCode)
                    {
                        case "DELETED": result = Status.Deleted; break;
                        default: result = Status.UnknownError; break;
                    }
                    break;
                default: result = Status.UnknownError; break;
            }

            return result;
        }

        private static TimeSpan ConvertTimeSpan(string Serial)
        {
            string[] buf = Serial.Split(':');
            var minute = int.Parse(buf[0]);

            return new TimeSpan((int)(minute / 60), minute % 60, int.Parse(buf[1]));
        }

        private static Tag[] ConvertTags(GetInfo.Tags Serial)
        {
            var result = new Tag[Serial.tag.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Tag()
                {
                    Category = Serial.tag[i].category != 0,
                    Lock = Serial.tag[i]._lock != 0,
                    Name = Serial.tag[i].tag,
                };
            }

            return result;
        }

        private static VideoInfo[] ConvertVideoInfo(Search.List[] Serial, Connection.Client Client)
        {
            var result = new VideoInfo[Serial.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new NicoVideo.VideoInfo()
                {
                    ComentCounter = Serial[i].num_res,
                    ID = Serial[i].id,
                    Length = ConvertTimeSpan(Serial[i].length),
                    MylistCounter = Serial[i].mylist_counter,
                    PostTime = DateTime.Parse(Serial[i].first_retrieve),
                    ShortDescription = Serial[i].description_short,
                    Thumbnail = new Thumbnail(Serial[i].thumbnail_url, Client),
                    Title = Serial[i].title,
                    ViewCounter = Serial[i].view_counter,
                };
            }

            return result;
        }

        private static Mylist ConvertMylist(GetMylist.Mylistitem[] MylistitemSerial, GroupCollection MylistInfoData, GroupCollection MylistUserInfoData, Connection.Client Client)
        {
            return new Mylist()
            {
                BookmarkCounter = (MylistUserInfoData == null) ? 0 : ConvertValue<int>(MylistUserInfoData["watched_count"].Value),
                IsBookmark = (MylistUserInfoData == null) ? true : ConvertValue<bool>(MylistUserInfoData["is_watching_this_mylist"].Value),
                Title = (MylistInfoData == null) ? "とりあえずマイリスト" : MylistInfoData["name"].Value,
                Description = (MylistInfoData == null) ? "" : MylistInfoData["description"].Value,
                IsPublic = (MylistInfoData== null ) ? false : ConvertValue<int>(MylistInfoData["public"].Value) != 0,
                MylistItem = ConvertMylistItem(MylistitemSerial, Client),
            };
        }

        private static MylistItem[] ConvertMylistItem(GetMylist.Mylistitem[] Serial, Connection.Client Client)
        {
            var result = new MylistItem[Serial.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new MylistItem()
                {
                    Description = Serial[i].description,
                    RegisterTime = unixTime.AddSeconds(double.Parse(Serial[i].item_data.first_retrieve)).ToLocalTime(),
                    VideoInfo = new VideoInfo()
                    {
                        ComentCounter = int.Parse(Serial[i].item_data.num_res),
                        ID = Serial[i].item_data.video_id,
                        Length = new TimeSpan(0, 0, int.Parse(Serial[i].item_data.length_seconds)),
                        MylistCounter = int.Parse(Serial[i].item_data.mylist_counter),
                        PostTime = unixTime.AddSeconds(double.Parse(Serial[i].item_data.first_retrieve)).ToLocalTime(),
                        Thumbnail = new Thumbnail(Serial[i].item_data.thumbnail_url, Client),
                        Title = Serial[i].item_data.title,
                        ViewCounter = int.Parse(Serial[i].item_data.view_counter),
                    },
                };
            }

            return result;
        }

        //GroupCollectionの値をParseした時、取得できなかった時に例外を出さないためにこの関数を通すこと
        private static ValueTyoe ConvertValue<ValueTyoe>(string Value)
        {
            if (Value.Length != 0)
                return (ValueTyoe)typeof(ValueTyoe)
                    .GetMethod("Parse", new Type[] { typeof(string) })
                    .Invoke(null, new object[] { Value });

            return default(ValueTyoe);
        }

        /********************************************/
    }
}
