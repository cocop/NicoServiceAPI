using System.IO;

namespace NicoServiceAPI.Connection
{
    /// <summary>ストリーム</summary>
    public class ConnectionStream
    {
        public static implicit operator Stream (ConnectionStream This)
        {
            return This.Stream;
        }

        /// <summary>処理ストリーム</summary>
        public Stream Stream { get; set; }

        /// <summary>読み込むデータのサイズ、書き込みストリームの場合は常に0</summary>
        public long Size { get; set; }
    }
}
