using System;

namespace InazumaSearch.Core
{
    /// <summary>
    /// イベントログ。文書登録の失敗などにより発行される
    /// </summary>
    public class EventLog
    {
        #region staticメソッド

        /// <summary>
        /// 種別の値を表示文字列に変換
        /// </summary>
        public static string LogTypeToCaption(LogType type)
        {
            switch (type)
            {
                case LogType.DOCUMENT_UPDATE_FAILED:
                    return "文書登録失敗";
                default:
                    return "";
            }
        }

        #endregion

        #region 列挙体

        public enum LogType
        {
            /// <summary>
            /// 文書登録失敗
            /// </summary>
            DOCUMENT_UPDATE_FAILED = 401,
        }

        #endregion

        #region メンバ

        /// <summary>
        /// 種別
        /// </summary>
        public LogType Type;

        /// <summary>
        /// 時刻
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message;

        /// <summary>
        /// 対象ファイルのパス
        /// </summary>
        public string TargetPath;

        /// <summary>
        /// 対象ファイルのファイルサイズ
        /// </summary>
        public long? TargetFileSize;

        /// <summary>
        /// 例外オブジェクト
        /// </summary>
        public Exception InnerException;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type">種別</param>
        public EventLog(LogType type, string message, DateTime timestamp)
        {
            this.Type = type;
            this.Message = message;
            this.Timestamp = timestamp;
        }

        #endregion
    }
}
