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
        /// ログレベル
        /// </summary>
        public enum LogLevel
        {
            /// <summary>エラー</summary>
            Error
                ,
            /// <summary>警告</summary>
            Warn
                ,
            /// <summary>情報</summary>
            Info
        }

        /// <summary>
        /// 動作ログのタイプ
        /// </summary>
        public class LogType
        {
            /// <summary>
            /// タイプと対応するコード
            /// </summary>
            public int Code { get; protected set; }

            /// <summary>
            /// メッセージ
            /// </summary>
            public string Message { get; protected set; }

            /// <summary>
            /// ログレベル
            /// </summary>
            public LogLevel Level { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="code">ログタイプと対応するコード</param>
            /// <param name="message">メッセージ</param>
            /// <param name="level">ログレベル</param>
            protected LogType(int code, string message, LogLevel level = LogLevel.Info)
            {
                this.Code = code;
                this.Message = message;
                this.Level = level;
            }

            /// <summary>アプリケーションが起動した</summary>
            public static LogType Boot = new LogType(1001, "Inazuma Searchが起動しました");

            /// <summary>アプリケーションが正常終了した</summary>
            public static LogType ShutDown = new LogType(1002, "Inazuma Searchが終了しました");

            /// <summary>手動クロールを開始した</summary>
            public static LogType ManualCrawlStart = new LogType(2001, "手動クロールを開始しました");

            /// <summary>手動クロールを完了した</summary>
            public static LogType ManualCrawlFinish = new LogType(2002, "手動クロールを完了しました");

            /// <summary>手動クロールをキャンセルした</summary>
            public static LogType ManualCrawlCancel = new LogType(2003, "手動クロールを中断しました");

            /// <summary>常駐クロールを開始した</summary>
            public static LogType AlwaysCrawlStart = new LogType(2021, "常駐クロールを開始しました");

            /// <summary>常駐クロールを終了した</summary>
            public static LogType AlwaysCrawlEnd = new LogType(2022, "常駐クロールを終了しました");

            /// <summary>常駐クロールがエラーにより強制終了した</summary>
            public static LogType AlwaysCrawlAbort = new LogType(2039, "常駐クロールがシステムエラーにより強制終了した", level: LogLevel.Error);

            /// <summary>文書データ1件を更新した</summary>
            public static LogType DocumentUpdate = new LogType(2201, "文書を登録しました");

            /// <summary>DBの不要な文書データを削除した</summary>
            public static LogType DocumentsPurge = new LogType(2281, "ファイルの移動・削除により不要になった文書データを削除しました");

            /// <summary>文書データの更新時、テキストの抽出に失敗した</summary>
            public static LogType DocumentBodyExtractFailed = new LogType(2401, "文書ファイルからのテキスト抽出処理に失敗しました", level: LogLevel.Error);

            /// <summary>文書データの更新時、文書ファイルが移動/削除されていて見つからなかった</summary>
            public static LogType DocumentFileNotFoundOnCrawling = new LogType(2402, "クロール対象の文書ファイルが移動または削除されています");

            /// <summary>検索実行（正常終了）</summary>
            public static LogType Search = new LogType(6001, "検索しました");

            /// <summary>検索を実行したが、検索語を正しく解析できずエラー</summary>
            public static LogType InvalidSearchQuery
                = new LogType(6401, "検索語の解析時にエラーが発生しました");

            /// <summary>システムエラー発生</summary>
            public static LogType SystemErrorAbort = new LogType(9001, "システムエラーによりInazuma Searchを強制終了しました", level: LogLevel.Error);
        }

        /// <summary>
        /// 操作ログを追加
        /// </summary>
        public static void Add(LogType type, string additionalMessage = null, string filePath = null, TimeSpan? processTime = null)
        {
            string processTimeValue = (processTime.HasValue ? ((decimal)processTime.Value.TotalMilliseconds / 1000m).ToString("N3") : null);
            Logger
                .WithProperty("Level", type.Level)
                .WithProperty("LogType", type.Code)
                .WithProperty("AdditionalMessage", additionalMessage)
                .WithProperty("FilePath", filePath)
                .WithProperty("SessionId", Application.SessionId)
                .WithProperty("ProcessTime", processTimeValue)
                .Info(type.Message);
        }
    }
}
