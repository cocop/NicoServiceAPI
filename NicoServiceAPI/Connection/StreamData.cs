using System;
using System.IO;

namespace NicoServiceAPI.Connection
{
    /******************************************/
    /// <summary>ストリームデータ</summary>
    /******************************************/
    internal class StreamData
    {
        /// <summary>処理ストリーム</summary>
        public Func<int, Stream> GetStream { get; set; }

        /// <summary>ストリームタイプ</summary>
        public StreamType StreamType { get; set; }

        /// <summary>ストリームへ書き込むデータの取得</summary>
        public Func<byte[]> GetWriteData { get; set; }

        /// <summary>ストリームから読み込んだデータの設定</summary>
        public Action<byte[]> SetReadData { get; set; }
    }
}