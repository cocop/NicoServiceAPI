﻿using NicoServiceAPI.Connection;

namespace NicoServiceAPI
{
    /******************************************/
    /// <summary>受け渡しオブジェクト</summary>
    /******************************************/
    internal class Context
    {
        /// <summary>クライアント通信処理</summary>
        public Client Client;

        /// <summary>インスタンスの管理</summary>
        public InstanceContainer InstanceContainer;
    }
}