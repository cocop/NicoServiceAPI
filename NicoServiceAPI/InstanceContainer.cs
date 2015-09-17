using NicoServiceAPI.NicoVideo.Mylist;
using NicoServiceAPI.NicoVideo.User;
using NicoServiceAPI.NicoVideo.Video;
using System;
using System.Collections.Generic;

namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>このクラスからインスタンスを取得する事で一つのIDに付き一つのインスタンスが保証される</summary>
    /******************************************/
    public class InstanceContainer
    {
        Dictionary<string, VideoInfo> videoInfoTable = new Dictionary<string,VideoInfo>();
        Dictionary<string, Mylist> mylistTable = new Dictionary<string, Mylist>();
        Dictionary<string, User> userTable = new Dictionary<string, User>();

        public VideoInfo GetVideoInfo(string ID)
        {
            return GetInstance(ID, videoInfoTable, (id) => new VideoInfo(id));
        }

        public Mylist GetMylist(string ID)
        {
            return GetInstance(ID, mylistTable, (id) => new Mylist(id));
        }

        public User GetUser(string ID)
        {
            return GetInstance(ID, userTable, (id) => new User(id));
        }

        ManageType GetInstance<ManageType>(string ID, Dictionary<string, ManageType> Table, Func<string, ManageType> New)
        {
            ManageType result;

            if (!Table.TryGetValue(ID, out result))
            {
                result = New(ID);
                Table.Add(ID, result);
            }
            return result;
        }
    }
}
