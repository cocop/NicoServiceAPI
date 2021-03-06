﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace NicoServiceAPI.Connection
{
    /******************************************/
    /// <summary>ストリームリスト</summary>
    /******************************************/
    public class Streams
    {
        /// <summary>現在の処理中のストリームタイプ</summary>
        public StreamType NowStreamType { get; private set; }

        /// <summary>未処理のストリーム数</summary>
        public int UntreatedCount { get { return streamDatas.Length - nowIndex; } }

        internal int nowIndex;
        internal StreamData[] streamDatas;

        internal Streams(StreamData[] StreamDatas)
        {
            nowIndex = 0;
            streamDatas = StreamDatas;
            NowStreamType = StreamDatas[0].StreamType;
        }

        /// <summary>次のストリームを処理する、取得したストリームの処理が完了した後に呼び出す</summary>
        public bool Next()
        {
            if ((++nowIndex) < streamDatas.Length)
                NowStreamType = streamDatas[nowIndex].StreamType;

            return nowIndex < streamDatas.Length;
        }

        /// <summary>ストリームを取得する、その種類はストリームタイプを参照</summary>
        public Task<ConnectionStream> GetStream()
        {
            if (nowIndex > streamDatas.Length)
                return null;

            return streamDatas[nowIndex].GetStream();
        }

        internal StreamData[] GetStreamDatas()
        {
            return streamDatas;
        }
    }

    /******************************************/
    /// <summary>ストリームリスト</summary>
    /******************************************/
    public class Streams<DataType> : Streams
    {
        Func<DataType> getData;

        internal Streams(StreamData[] StreamDatas, Func<DataType> GetData)
            : base(StreamDatas)
        {
            getData = GetData;
        }

        /// <summary>書き込みデータを取得する、nullの場合は自前で用意すること</summary>
        public byte[] GetWriteData()
        {
            if (streamDatas[nowIndex].GetWriteData == null)
                return null;

            return streamDatas[nowIndex].GetWriteData();
        }

        /// <summary>読み込みデータを設定する</summary>
        /// <param name="ReadData">読み込みデータ</param>
        public void SetReadData(byte[] ReadData)
        {
            if (streamDatas[nowIndex].SetReadData == null)
                return;

            streamDatas[nowIndex].SetReadData(ReadData);
        }

        /// <summary>最終的なデータを取得する</summary>
        public DataType GetData()
        {
            if (nowIndex == streamDatas.Length)
                return getData();

            return default(DataType);
        }

        /// <summary>ストリームを処理する</summary>
        /// <param name="RunCount">処理するストリームの数</param>
        public DataType Run(int RunCount)
        {
            if (RunCount < 0 && RunCount > UntreatedCount)
                return default(DataType);

            for (int i = 0; i < RunCount; i++)
            {
                switch (NowStreamType)
                {
                    case StreamType.Read:
                        {
                            using (var destination = new MemoryStream())
                            {
                                var task = GetStream();
                                task.RunSynchronously();

                                using (Stream source = task.Result)
                                    source.CopyTo(destination);

                                SetReadData(destination.ToArray());
                            }
                        } break;

                    case StreamType.Write:
                        {
                            var data = GetWriteData();

                            using (Stream source = new MemoryStream(data))
                            {
                                var task = GetStream();
                                task.RunSynchronously();

                                using (Stream destination = task.Result)
                                    source.CopyTo(destination);
                            }
                        } break;

                    default: throw new Exception("StreamTypeが不正です");
                }
                Next();
            }

            return GetData();
        }
    }
}
