using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>ユーザー情報レスポンス</summary>
    /******************************************/
    public class UserResponse : Response
    {
        /// <summary>ユーザー情報</summary>
        public User.User User;
    }
}
