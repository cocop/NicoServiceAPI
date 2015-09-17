using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NicoServiceAPI.NicoVideo.Video
{
    /******************************************/
    /// <summary>タグ編集レスポンス</summary>
    /******************************************/
    public class TagResponse : Response
    {
        /// <summary>タグ</summary>
        public Tag[] Tags { get; set; }
    }
}
