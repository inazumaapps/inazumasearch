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
            /// <summary>手動クロールを開始した</summary>
            ManualCrawlStart = 2001
            ,
            /// <summary>手動クロールを完了した</summary>
            ManualCrawlFinish = 2002
            ,
            /// <summary>手動クロールをキャンセルした</summary>
            ManualCrawlCancel = 2003
            ,
            /// <summary>文書データを更新した</summary>
            DocumentUpdate = 2201
            ,
            /// <summary>DBの不要な文書データを削除した</summary>
            DocumentsPurge = 2281
            ,
            /// <summary>文書データの更新時、テキストの抽出に失敗した</summary>
            DocumentBodyExtractFailed = 2401
            ,
            /// <summary>文書データの更新時、文書ファイルが移動/削除されていて見つからなかったしていた</summary>
            DocumentFileNotFoundOnCrawling = 2402

        }

        /// <summary>
        /// ログタイプに応じたメッセージを取得する
        /// </summary>
        public static string GetMessage(LogType type)
        {
            if (type == LogType.Boot) return "Inazuma Searchが起動しました";
            if (type == LogType.ShutDown) return "Inazuma Searchが終了しました";
            if (type == LogType.ManualCrawlStart) return "手動クロールを開始しました";
            if (type == LogType.ManualCrawlFinish) return "手動クロールを完了しました";
            if (type == LogType.ManualCrawlCancel) return "手動クロールを中断しました";
            if (type == LogType.DocumentUpdate) return "文書を登録しました";
            if (type == LogType.DocumentsPurge) return "ファイルの移動・削除により不要になった文書データを削除しました";
            if (type == LogType.DocumentBodyExtractFailed) return "文書ファイルからのテキスト抽出処理に失敗しました";
            if (type == LogType.DocumentFileNotFoundOnCrawling) return "クロール対象の文書ファイルが移動または削除されています";
            return "";
        }

        /// <summary>
        /// 操作ログを追加
        /// </summary>
        public static void Add(LogType type, string additionalMessage = null, string filePath = null, TimeSpan? processTime = null)
        {
            string processTimeValue = (processTime.HasValue ? ((decimal)processTime.Value.TotalMilliseconds / 1000m).ToString("N3") : null);
            Logger.WithProperty("LogType", (int)type)
                .WithProperty("AdditionalMessage", additionalMessage)
                .WithProperty("FilePath", filePath)
                .WithProperty("SessionId", Application.SessionId)
                .WithProperty("ProcessTime", processTimeValue)
                .Info(GetMessage(type));
        }
    }
}
