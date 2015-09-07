using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NicoServiceAPI.NicoVideo.User
{
    /******************************************/
    /// <summary>マイリストグループレスポンス</summary>
    /******************************************/
    public class MylistGroupResponse : Response
    {
        public Mylist[] MylistGroup { get; set; }
    }
}
