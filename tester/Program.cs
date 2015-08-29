using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NicoServiceAPI;
using NicoServiceAPI.NicoVideo;
using NicoServiceAPI.Connection;

namespace tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var ns = new NicoService();
            var li = ns.Login("cypsf18611@yahoo.co.jp", "asdfghjkl");
            var vs = ns.GetVideoService();
            var vi = new VideoInfo("sm26983122");
            var vi2 = vs.DownloadVideoInfo(vi);
        }
    }
}
