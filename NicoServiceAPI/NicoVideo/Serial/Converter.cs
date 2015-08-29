using System;

namespace NicoServiceAPI.NicoVideo.Serial
{
    internal static class Converter
    {
        /********************************************/

        public static NicoVideo.Status ConvertStatus(string Serial, Error ErrorCode)
        {
            NicoVideo.Status result;

            switch (Serial)
            {
                case "ok": result = Status.OK; break;
                case "fail":
                    switch (ErrorCode.Code)
                    {
                        case "DELETED": result = Status.Deleted; break;
                        default: result = Status.UnknownError; break;
                    }
                    break;
                default: result = Status.UnknownError; break;
            }

            return result;
        }

        public static NicoVideo.Tag[] ConvertTags(Serial.Tags Serial)
        {
            var result = new NicoVideo.Tag[Serial.List.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new NicoVideo.Tag()
                {
                    Category = Serial.List[i].Category != 0,
                    Lock = Serial.List[i].Lock != 0,
                    Name = Serial.List[i].Name,
                };
            }

            return result;
        }

        public static TimeSpan ConvertTimeSpan(string Serial)
        {
            string[] buf = Serial.Split(':');
            var minute = int.Parse(buf[0]);

            return new TimeSpan((int)(minute / 60), minute % 60, int.Parse(buf[1]));
        }

        public static NicoVideo.VideoInfo ConvertVideoInfo(VideoInfo Serial, Connection.Client Client)
        {
            var result = new NicoVideo.VideoInfo()
            {
                ComentNumber = Serial.ComentNumber,
                //Description = Serial.Description,HTMLタグが削除されるためここで取得はしない
                EconomyVideoSize = Serial.EconomyVideoSize,
                ExternalPlay = int.Parse(Serial.ExternalPlay) != 0,
                ID = Serial.ID,
                Length = ConvertTimeSpan(Serial.Length),
                MylistCounter = Serial.MylistCounter,
                NoLivePlay = Serial.NoLivePlay,
                PostTime = DateTime.Parse(Serial.PostTime),
                ShortDescription = Serial.ShortDescription,
                Tags = ConvertTags(Serial.Tags),
                Thumbnail = new Thumbnail(Serial.ThumbnailUrl, Client),
                Title = Serial.Title,
                VideoSize = Serial.VideoSize,
                VideoType = Serial.VideoType,
                ViewCounter = Serial.ViewCounter,
            };


            return result;
        }

        public static NicoVideo.VideoInfoResponse ConvertVideoInfoResponse(VideoInfoResponse Serial, Connection.Client Client)
        {
            var result = new NicoVideo.VideoInfoResponse();

            result.Status = ConvertStatus(Serial.Status, Serial.ErrorCode);
            result.ErrorMessage = Serial.ErrorMessage;

            if (Serial.VideoInfos != null)
            {
                result.VideoInfos = new NicoVideo.VideoInfo[Serial.VideoInfos.Length];
                for (int i = 0; i < result.VideoInfos.Length; i++)
                    result.VideoInfos[i] = ConvertVideoInfo(Serial.VideoInfos[i], Client);
            }

            return result;
        }

        /********************************************/

        static DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public static NicoVideo.CommentResponse ConvertCommentResponse(NicoVideo.Serial.CommentResponse Serial)
        {
            var result = new NicoVideo.CommentResponse();

            result.Comment = new NicoVideo.Comment[Serial.Comment.Length];
            for (int i = 0; i < result.Comment.Length; i++)
                result.Comment[i] = ConvertComment(Serial.Comment[i]);

            return result;
        }

        public static NicoVideo.Comment ConvertComment(NicoVideo.Serial.Comment Serial)
        {
            return new NicoVideo.Comment()
            {
                Anonymity = Serial.Anonymity,
                Body = Serial.Body,
                Command = Serial.Command,
                Leaf = Serial.Leaf,
                No = Serial.No,
                PlayTime = TimeSpan.FromMilliseconds(double.Parse(Serial.PlayTime+'0')),
                Premium = Serial.Premium,
                UserID = Serial.UserID,
                WriteTime = unixTime.AddSeconds(long.Parse(Serial.WriteTime)).ToLocalTime(),
                YourPost = Serial.YourPost,
            };
        }
    }
}
