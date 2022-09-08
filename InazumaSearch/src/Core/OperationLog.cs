using System;

namespace InazumaSearch.Core
{
    /// <summary>
    /// 動作ログ
    /// </summary>
    public class OperationLog
    {
        /// <summary>
        /// ロガー（アプリケーション全体で1つのみ存在）
        /// </summary>
        protected static NLog.Logger Logger = NLog.LogManager.GetLogger("OperationLog");

        /// <summary>
        /// 動作ログのタイプ
        /// </summary>
        public enum LogType
        {
            /// <summary>アプリケーションが起動した</summary>
            Boot = 1001
            ,
            /// <summary>アプリケーションが終了した</summary>
            ShutDown = 1002
            ,
            /// <summary>文書データを更新した</summary>
            DocumentUpdate = 2001
        }

        /// <summary>
        /// ログタイプに応じたメッセージを取得する
        /// </summary>
        public static string GetMessage(LogType type)
        {
            if (type == LogType.Boot) return "Inazuma Searchが起動しました";
            if (type == LogType.ShutDown) return "Inazuma Searchが終了しました";
            if (type == LogType.DocumentUpdate) return "文書を登録しました";
            return "";
        }

        /// <summary>
        /// 操作ログを追加
        /// </summary>
        public static void Add(LogType type, string filePath = null, TimeSpan? processTime = null)
        {
            string processTimeValue = (processTime.HasValue ? ((decimal)processTime.Value.TotalMilliseconds / 1000m).ToString("N3") : null);
            Logger.WithProperty("LogType", (int)type).WithProperty("FilePath", filePath).WithProperty("SessionId", Application.SessionId).WithProperty("ProcessTime", processTimeValue).Info(GetMessage(type));
        }
    }
}
