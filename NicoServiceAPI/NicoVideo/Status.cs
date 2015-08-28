﻿namespace NicoServiceAPI.NicoVideo
{
    /******************************************/
    /// <summary>レスポンスステータス</summary>
    /******************************************/
    public enum Status
    {
        /// <summary>正常に情報を取得出来ました。</summary>
        OK,

        /// <summary>その動画は削除済みです。</summary>
        Deleted,

        /// <summary>何らかの理由で情報を取得出来ませんでした、エラーメッセージが参照できる場合があります。</summary>
        UnknownError,
    }
}