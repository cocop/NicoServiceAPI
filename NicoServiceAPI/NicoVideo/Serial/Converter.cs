using NicoServiceAPI.Connection;
using System;
using System.Text.RegularExpressions;

namespace NicoServiceAPI.NicoVideo.Serial
{
    internal class Converter
    {
        static DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        Context context;

        /// <summary>インスタンスコンテナ、普通に使う分には名前が長過ぎるので省略</summary>
        InstanceContainer ic { get { return context.InstanceContainer; } }
        Client client { get { return context.Client; } }

        public Converter(Context Context)
        {
            context = Context;
        }



        /********************************************/

        public Video.CommentResponse ConvertCommentResponse(GetComment.Packet Serial)
        {
            return new Video.CommentResponse()
            {
                Comment = ConvertComment(Serial.chat),
                Status = Status.OK,
            };
        }

        public Video.VideoInfoResponse ConvertVideoInfoResponse(GetInfo.NicovideoThumbResponse Serial)
        {
            var result = new Video.VideoInfoResponse();

            result.ErrorMessage = (Serial.error == null) ? null : Serial.error.description;
            result.Status = ConvertStatus(Serial.status, Serial.error);

            if (Serial.thumb != null)
            {
                var info = (ic == null)
                    ? new Video.VideoInfo(Serial.thumb.video_id)
                    : ic.GetVideoInfo(Serial.thumb.video_id);

                info.ComentCounter = Serial.thumb.comment_num;
                info.Description = Serial.thumb.description;
                info.EconomyVideoSize = Serial.thumb.size_low;
                info.IsExternalPlay = Serial.thumb.embeddable;
                info.Length = ConvertTimeSpan(Serial.thumb.length);
                info.MylistCounter = Serial.thumb.mylist_counter;
                info.IsLivePlay = !Serial.thumb.no_live_play;
                info.PostTime = DateTime.Parse(Serial.thumb.first_retrieve);
                info.Tags = ConvertTags(Serial.thumb.tags);
                info.Title = Serial.thumb.title;
                info.VideoSize = Serial.thumb.size_high;
                info.VideoType = Serial.thumb.movie_type;
                info.ViewCounter = Serial.thumb.view_counter;
                info.Thumbnail = NewPicture(info.Thumbnail, Serial.thumb.thumbnail_url);

                info.User = (ic == null)
                    ? new User.User(Serial.thumb.user_id)
                    : ic.GetUser(Serial.thumb.user_id);

                info.User.Name = Serial.thumb.user_nickname;
                info.User.Icon = NewPicture(info.User.Icon, Serial.thumb.user_icon_url);

                result.VideoInfos = new Video.VideoInfo[] { info };
            }

            return result;
        }

        public Video.VideoInfoResponse ConvertVideoInfoResponse(Search.Contract Serial)
        {
            return new Video.VideoInfoResponse()
            {
                ErrorMessage = (Serial.error == null) ? null : Serial.error.description,
                Status = ConvertStatus(Serial.status, Serial.error),
                VideoInfos = ConvertVideoInfo(Serial.list),
            };
        }

        public Video.TagResponse ConvertTagResponse(EditTag.Contract Serial)
        {
            var result = new Video.TagResponse();

            result.ErrorMessage = (Serial.error == null) ? null : Serial.error.description;
            result.Status = ConvertStatus(Serial.status, Serial.error);

            result.Tags = new Video.Tag[Serial.tags.Length];
            for (int i = 0; i < result.Tags.Length; i++)
            {
                result.Tags[i] = new Video.Tag();
                result.Tags[i].IsCategory = Serial.tags[i].can_cat;
                result.Tags[i].IsNicopedia = Serial.tags[i].dic;
                result.Tags[i].IsLock = Serial.tags[i].owner_lock == 1;
                result.Tags[i].Name = Serial.tags[i].tag;
            }

            return result;
        }

        public Mylist.MylistResponse ConvertMylistResponse(GetDeflist.Contract Serial, GroupCollection MylistInfoData, GroupCollection MylistUserInfoData)
        {
            return new Mylist.MylistResponse()
            {
                ErrorMessage = (Serial.error == null) ? null : Serial.error.description,
                Mylist = ConvertMylist(Serial.mylistitem, MylistInfoData, MylistUserInfoData),
                Status = ConvertStatus(Serial.status, Serial.error),
            };
        }

        public Mylist.MylistResponse ConvertMylistResponse(GetMylist.Contract Serial, string MylistID)
        {
            return new Mylist.MylistResponse()
            {
                ErrorMessage = (Serial.error == null) ? null : Serial.error.description,
                Mylist = ConvertMylist(Serial, MylistID),
                Status = ConvertStatus(Serial.status, Serial.error),
            };
        }

        public Mylist.MylistRemoveVideoResponse ConvertMylistRemoveVideoResponse(MylistRemoveVideo.Contract Serial)
        {
            return new Mylist.MylistRemoveVideoResponse()
            {
                ErrorMessage = (Serial.error == null) ? null : Serial.error.description,
                RemoveCount = int.Parse((Serial.delete_count == null) ? "0" : Serial.delete_count),
                Status = ConvertStatus(Serial.status, Serial.error),
            };
        }

        public User.MylistGroupResponse ConvertMylistGroupResponse(GetMylistGroup.Contract Serial)
        {
            return new User.MylistGroupResponse()
            {
                ErrorMessage = (Serial.error == null) ? null : Serial.error.description,
                MylistGroup = ConvertMylistGroup(Serial.mylistgroup),
                Status = ConvertStatus(Serial.status, Serial.error),
            };
        }

        public User.AddMylistResponse ConvertAddMylist(Serial.AddMylist.Contract Serial)
        {
            var result = new User.AddMylistResponse();

            result.AddedMylist = (ic == null)
                    ? new Mylist.Mylist(Serial.id)
                    : ic.GetMylist(Serial.id);

            result.ErrorMessage = (Serial.error == null) ? null : Serial.error.description;
            result.Status = ConvertStatus(Serial.status, Serial.error);

            return result;
        }

        public User.UserResponse ConvertUserResponse(GroupCollection[] Serial)
        {
            var result = new User.UserResponse();

            if (Serial[1]["id"].Value == "")
            {
                result.Status = Status.UnknownError;
                return result;
            }

            result.User = (ic == null)
                ? new User.User(Serial[1]["id"].Value)
                : ic.GetUser(Serial[1]["id"].Value);

            result.Status = Status.OK;
            result.User.Icon = NewPicture(result.User.Icon, Serial[0]["icon"].Value);
            result.User.Name = Serial[0]["name"].Value;
            result.User.IsPremium = Serial[1]["category"].Value == "プレミアム会員";
            result.User.Sex = Serial[1]["sex"].Value;
            result.User.Birthday = Serial[1]["birthday"].Value;
            result.User.Area = Serial[1]["area"].Value;
            result.User.BookmarkCount = ConvertValue<int>(Serial[2]["bookmark"].Value);
            result.User.Experience = ConvertValue<int>(Serial[2]["exp"].Value);
            result.User.Description = Serial[3]["description"].Value;

            return result;
        }

        public User.ViewHistoryResponse ConvertViewHistoryResponse(GetVideoViewHistory.Contract Serial)
        {
            var result = new User.ViewHistoryResponse();

            if (result.History != null) return null;


            result.Status = ConvertStatus(Serial.status, Serial.error);
            result.ErrorMessage = (Serial.error == null) ? null : Serial.error.description;

            result.History = new Video.VideoInfo[Serial.history.Length];
            for (int i = 0; i < result.History.Length; i++)
            {

                result.History[i] = new Video.VideoInfo();

                result.History[i].ID = Serial.history[i].item_id;
                result.History[i].Length = ConvertTimeSpan(Serial.history[i].length);
                result.History[i].Thumbnail = NewPicture(result.History[i].Thumbnail, Serial.history[i].thumbnail_url);
                result.History[i].Title = Serial.history[i].title;
            }

            return result;
        }

        public Response ConvertResponse(MylistAddVideo.Contract Serial)
        {
            return new Response()
            {
                ErrorMessage = (Serial.error == null) ? null : Serial.error.description,
                Status = ConvertStatus(Serial.status, Serial.error),
            };
        }

        public Response ConvertResponse(UpdateMylist.Contract Serial)
        {
            return new Response()
            {
                ErrorMessage = (Serial.error == null) ? null : Serial.error.description,
                Status = ConvertStatus(Serial.status, Serial.error),
            };
        }

        public Response ConvertResponse(RemoveMylist.Contract Serial)
        {
            return new Response()
            {
                ErrorMessage = (Serial.error == null) ? null : Serial.error.description,
                Status = ConvertStatus(Serial.status, Serial.error),
            };
        }

        public Response ConvertResponse(PostComment.Packet packet)
        {
            var result = new Response();

            if (packet.chat_result.status == "0")
                result.Status = Status.OK;
            else
            {
                switch (packet.chat_result.status)
                {
                    case "1": result.ErrorMessage = "同じコメントを投稿しようとしました"; break;
                    default: break;
                }
                result.Status = Status.UnknownError;
            }

            return result;
        }


        /********************************************/

        private Video.Comment[] ConvertComment(GetComment.Chat[] Serial)
        {
            var result = new Video.Comment[Serial.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Video.Comment()
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
                    Scores = int.Parse((Serial[i].scores == null) ? "0" : Serial[i].scores),
                    IsYourPost = Serial[i].yourpost,
                };
            }

            return result;
        }

        private Status ConvertStatus(string Serial, Error ErrorCode)
        {
            switch (Serial)
            {
                case "ok": return Status.OK;
                case "fail":
                    if (ErrorCode == null) break;
                    
                    switch (ErrorCode.code)
                    {
                        case "DELETED": return Status.Deleted;
                    }
                    break;
            }

            return Status.UnknownError;
        }

        private TimeSpan ConvertTimeSpan(string Serial)
        {
            string[] buf = Serial.Split(':');
            var minute = int.Parse(buf[0]);

            return new TimeSpan((int)(minute / 60), minute % 60, int.Parse(buf[1]));
        }

        private Picture NewPicture(Picture Target, string PictureURL)
        {
            if (Target == null)
                return new Picture(PictureURL, client);
            return Target;
        }

        private Video.Tag[] ConvertTags(GetInfo.Tags Serial)
        {
            var result = new Video.Tag[Serial.tag.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Video.Tag()
                {
                    IsCategory = Serial.tag[i].category != 0,
                    IsLock = Serial.tag[i]._lock != 0,
                    Name = Serial.tag[i].tag,
                };
            }

            return result;
        }

        private Video.VideoInfo[] ConvertVideoInfo(Search.List[] Serial)
        {
            if (Serial == null) return null;

            var result = new Video.VideoInfo[Serial.Length];

            for (int i = 0; i < result.Length; i++)
            {
                var info = (ic == null)
                    ? new Video.VideoInfo(Serial[i].id)
                    : ic.GetVideoInfo(Serial[i].id);

               info.ComentCounter = Serial[i].num_res;
               info.Length = ConvertTimeSpan(Serial[i].length);
               info.MylistCounter = Serial[i].mylist_counter;
               info.PostTime = DateTime.Parse(Serial[i].first_retrieve);
               info.ShortDescription = Serial[i].description_short;
               info.Title = Serial[i].title;
               info.ViewCounter = Serial[i].view_counter;
               info.Thumbnail = NewPicture(info.Thumbnail, Serial[i].thumbnail_url);
               result[i] = info;
            }

            return result;
        }

        private Mylist.Mylist ConvertMylist(GetDeflist.Mylistitem[] MylistitemSerial, GroupCollection MylistInfoData, GroupCollection MylistUserInfoData)
        {
            if (MylistitemSerial == null) return null;

            var result = (ic == null)
                ? (MylistInfoData == null)
                ? new Mylist.Mylist("")
                : new Mylist.Mylist(MylistInfoData["id"].Value)
                : (MylistInfoData == null)
                ? ic.GetMylist("")
                : ic.GetMylist(MylistInfoData["id"].Value);

            result.BookmarkCount = (MylistUserInfoData == null) ? 0 : ConvertValue<int>(MylistUserInfoData["watched_count"].Value);
            result.CreateTime = unixTime.AddSeconds((MylistUserInfoData == null) ? 0 : ConvertValue<double>(MylistInfoData["create_time"].Value)).ToLocalTime();
            result.IsBookmark = (MylistUserInfoData == null) ? false : ConvertValue<bool>(MylistUserInfoData["is_watching_this_mylist"].Value);
            result.Title = (MylistInfoData == null) ? "とりあえずマイリスト" : MylistInfoData["name"].Value;
            result.UpdateTime = unixTime.AddSeconds((MylistUserInfoData == null) ? 0 : ConvertValue<double>(MylistInfoData["update_time"].Value)).ToLocalTime();
            result.Description = (MylistInfoData == null) ? "" : MylistInfoData["description"].Value;
            result.IsPublic = (MylistInfoData == null) ? false : ConvertValue<int>(MylistInfoData["public"].Value) != 0;
            result.MylistItem = ConvertMylistItem(MylistitemSerial);

            if (MylistInfoData != null)
            {
                result.User = (ic == null)
                    ? new User.User(MylistUserInfoData["user_id"].Value)
                    : ic.GetUser(MylistUserInfoData["user_id"].Value);

                result.User.Name = MylistUserInfoData["nickname"].Value;
            }

            return result;
        }

        private Mylist.Mylist ConvertMylist(GetMylist.Contract Serial, string MylistID)
        {
            if (Serial == null) return null;

            var result = (ic == null)
                ? new Mylist.Mylist(MylistID)
                : ic.GetMylist(MylistID);

            result.Description = Serial.description;
            result.Title = Serial.name;
            result.IsBookmark = Serial.is_watching_this_mylist;
            result.MylistItem = ConvertMylistItem(Serial.list);
            result.User = (ic == null)
                ? new User.User(Serial.user_id)
                : ic.GetUser(Serial.user_id);

            result.User.Name = Serial.user_nickname;

            return result;
        }

        private Mylist.MylistItem[] ConvertMylistItem(GetDeflist.Mylistitem[] Serial)
        {
            var result = new Mylist.MylistItem[Serial.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Mylist.MylistItem()
                {
                    Description = Serial[i].description,
                    RegisterTime = unixTime.AddSeconds(double.Parse(Serial[i].create_time)).ToLocalTime(),
                    UpdateTime = unixTime.AddSeconds(double.Parse(Serial[i].update_time)).ToLocalTime(),
                    VideoInfo = (ic == null)
                        ? new Video.VideoInfo(Serial[i].item_data.video_id)
                        : ic.GetVideoInfo(Serial[i].item_data.video_id),
                };

                result[i].VideoInfo.ComentCounter = int.Parse(Serial[i].item_data.num_res);
                result[i].VideoInfo.Length = new TimeSpan(0, 0, int.Parse(Serial[i].item_data.length_seconds));
                result[i].VideoInfo.MylistCounter = int.Parse(Serial[i].item_data.mylist_counter);
                result[i].VideoInfo.PostTime = unixTime.AddSeconds(double.Parse(Serial[i].item_data.first_retrieve)).ToLocalTime();
                result[i].VideoInfo.Title = Serial[i].item_data.title;
                result[i].VideoInfo.ViewCounter = int.Parse(Serial[i].item_data.view_counter);
                result[i].VideoInfo.Thumbnail = NewPicture(result[i].VideoInfo.Thumbnail, Serial[i].item_data.thumbnail_url);

            }

            return result;
        }

        private Mylist.MylistItem[] ConvertMylistItem(GetMylist.List[] Serial)
        {
            var result = new Mylist.MylistItem[Serial.Length];

            for (int i = 0; i < result.Length; i++)
			{
                result[i] = new Mylist.MylistItem();
                result[i].Description = Serial[i].mylist_comment;
                result[i].RegisterTime = unixTime.AddSeconds(Serial[i].create_time).ToLocalTime();
                result[i].UpdateTime = DateTime.Parse(Serial[i].thread_update_time);
                result[i].VideoInfo = (ic == null)
                    ? new Video.VideoInfo(Serial[i].id)
                    : ic.GetVideoInfo(Serial[i].id);

                result[i].VideoInfo.ComentCounter = Serial[i].num_res;
                result[i].VideoInfo.ID = Serial[i].id;
                result[i].VideoInfo.Length = new TimeSpan(0, 0, Serial[i].length_seconds);
                result[i].VideoInfo.MylistCounter = Serial[i].mylist_counter;
                result[i].VideoInfo.PostTime = DateTime.Parse(Serial[i].first_retrieve);
                result[i].VideoInfo.ShortDescription = Serial[i].description_short;
                result[i].VideoInfo.Thumbnail = NewPicture(result[i].VideoInfo.Thumbnail, Serial[i].thumbnail_url);
                result[i].VideoInfo.Title = Serial[i].title;
                result[i].VideoInfo.ViewCounter = Serial[i].view_counter;
            }

            return result;
        }

        private Mylist.Mylist[] ConvertMylistGroup(GetMylistGroup.MylistGroup[] Serial)
        {
            if (Serial == null) return null;

            var result = new Mylist.Mylist[Serial.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (ic == null)
                    ? new Mylist.Mylist(Serial[i].id)
                    : ic.GetMylist(Serial[i].id);

                result[i].CreateTime = unixTime.AddSeconds(Serial[i].create_time).ToLocalTime();
                result[i].Description = Serial[i].description;
                result[i].IsPublic = Serial[i]._public;
                result[i].Title = Serial[i].name;
                result[i].UpdateTime = unixTime.AddSeconds(Serial[i].update_time).ToLocalTime();
            }

            return result;
        }

        //GroupCollectionの値をParseする時、取得できなかった場合に例外を出さないためにこの関数を通すこと
        private ValueTyoe ConvertValue<ValueTyoe>(string Value)
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
