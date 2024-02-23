using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Groonga.Exceptions;
using Microsoft.WindowsAPICodePack.Shell;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// 文書ファイルの情報をDBに登録（更新）する
    /// </summary>
    public class DocumentFileUpdate : WorkBase
    {
        /// <summary>
        /// 登録結果
        /// </summary>
        public class UpdateResult
        {
            /// <summary>
            /// 結果タイプを表す列挙体
            /// </summary>
            public enum ResultType
            {
                /// <summary>
                /// 成功
                /// </summary>
                Success,

                /// <summary>
                /// 失敗
                /// </summary>
                Failed,

                /// <summary>
                /// スキップ
                /// </summary>
                Skipped
            }

            /// <summary>
            /// 結果タイプ
            /// </summary>
            public ResultType Type;

            /// <summary>
            /// エラー発生時の時刻
            /// </summary>
            public DateTime ErrorRaisedAt;

            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public string ErrorMessage;

            /// <summary>
            /// エラーになったファイルのパス
            /// </summary>
            public string ErrorFilePath;

            /// <summary>
            /// エラーになったファイルのファイルサイズ
            /// </summary>
            public long? ErrorFileSize;

            /// <summary>
            /// 例外オブジェクト
            /// </summary>
            public Exception InnerException;

            public UpdateResult(ResultType type)
            {
                this.Type = type;
                if (type == ResultType.Failed)
                {
                    ErrorRaisedAt = DateTime.Now;
                }
            }

            /// <summary>
            /// 「成功」を表す定数
            /// </summary>
            public static UpdateResult SUCCESS = new UpdateResult(ResultType.Success);

            /// <summary>
            /// 「スキップ」を表す定数
            /// </summary>
            public static UpdateResult SKIPPED = new UpdateResult(ResultType.Skipped);
        }


        /// <summary>
        /// 登録対象のファイルパス
        /// </summary>
        public virtual string FilePath { get; protected set; }

        /// <summary>
        /// DBに登録されている文書レコード一覧
        /// </summary>
        public virtual IDictionary<string, Groonga.RecordSet.Record> AlreadyDBRecordMap { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DocumentFileUpdate(
            Application app,
            bool isBackgroundCrawl,
            string filePath,
            IDictionary<string, Groonga.RecordSet.Record> alreadyDBRecordMap
        ) : base(app, isBackgroundCrawl)
        {
            FilePath = filePath;
            AlreadyDBRecordMap = alreadyDBRecordMap;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override bool Equals(IWork other)
        {
            var work = other as DocumentFileUpdate;
            return (work != null && work.FilePath == FilePath);
        }

        /// <summary>
        /// ハッシュ値算出
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ FilePath.GetHashCode();
        }

        /// <summary>
        /// トレースログ出力時の表記
        /// </summary>
        public override string TraceLogCaption { get { return $"{GetType().Name}({FilePath})"; } }

        /// <summary>
        /// 処理実行
        /// </summary>
        public override void ExecuteMain(
            Stack<IWork> workStack,
            Result crawlResult,
            CancellationToken cToken,
            IProgress<ProgressState> progress = null
        )
        {
            // 登録メイン処理を実行
            var result = UpdateMain(crawlResult, progress);

            // 結果をカウント
            if (result.Type == UpdateResult.ResultType.Success)
            {
                // 成功時
                crawlResult.Updated++;

                // 進捗を報告
                progress?.Report(new ProgressState() { CurrentStep = ProgressState.Step.RecordUpdateEnd, CurrentValue = crawlResult.UpdateProcessed, TotalValue = crawlResult.TotalTargetCount, Path = FilePath });

                // 登録件数が200件を超えるごとにGroongaを再起動
                // (Groongaプロセスを立ち上げてパイプを接続したまま大量データを登録すると、システムエラーが発生する場合があるため)
                if (crawlResult.Updated % 200 == 0)
                {
                    Logger.Debug("Groonga Reboot");
                    _app.GM.Reboot();
                }
            }
            else if (result.Type == UpdateResult.ResultType.Skipped)
            {
                // スキップ時
                crawlResult.Skipped++;
            }
            else
            {
                // 失敗時
                crawlResult.UpdateFailed++;

                // イベントログ追加
                var log = new EventLog(EventLog.LogType.DOCUMENT_UPDATE_FAILED, result.ErrorMessage, result.ErrorRaisedAt) { TargetPath = result.ErrorFilePath, TargetFileSize = result.ErrorFileSize, InnerException = result.InnerException };
                _app.AddEventLog(log);
            }

            // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低0.05秒)
            InsertCoolTime(50);
        }

        /// <summary>
        /// 登録メイン処理
        /// </summary>
        protected virtual UpdateResult UpdateMain(
            Result crawlResult,
            IProgress<ProgressState> progress = null
        )
        {
            // 進捗を報告
            ReportProgressLimitedFrequency(
                progress,
                new ProgressState() { CurrentStep = ProgressState.Step.RecordUpdateCheckBegin, CurrentValue = crawlResult.UpdateProcessed, TotalValue = crawlResult.TotalTargetCount, Path = FilePath },
                crawlResult
            );

            // 展開対象の拡張子一覧を取得
            var textExtNames = _app.GetTextExtNames();
            var pluginExtNames = _app.GetPluginExtNames();

            // 実行時、指定ファイルがすでに削除(もしくは移動)されている場合は、処理をスキップ
            if (!File.Exists(FilePath))
            {
                Logger.Debug($"Target file not found - {FilePath}");
                return UpdateResult.SKIPPED;
            }

            // ファイルが削除されていない場合、見つかった対象ファイルとして登録（DBからの文書削除時に使用）
            crawlResult.FoundTargetFilePathSet.Add(FilePath);

            // ファイル情報取得
            var fileInfo = new FileInfo(FilePath);
            var fileUpdated = fileInfo.LastWriteTime;
            var fileSize = fileInfo.Length;
            var key = Util.MakeDocumentFileKey(FilePath);

            // ファイルがすでに登録されていて、日付もサイズも変わっていなければスキップ
            if (AlreadyDBRecordMap.ContainsKey(key))
            {
                var rec = AlreadyDBRecordMap[key];
                var recSize = rec.GetIntValue(Column.Documents.SIZE).Value;
                var recUpdated = (double)rec[Column.Documents.FILE_UPDATED_AT];
                var recMajorVerOnUpdated = rec.GetIntValue(Column.Documents.APPLICATION_MAJOR_VERSION_ON_UPDATED).Value;
                var recMinorVerOnUpdated = rec.GetIntValue(Column.Documents.APPLICATION_MINOR_VERSION_ON_UPDATED).Value;

                // 日付はGroonga側とFileInfo側で精度の違いがある場合があるため、秒部分のみ比較
                var recUpdatedSec = Math.Floor(recUpdated);
                var fileUpdatedSec = Math.Floor(Groonga.Util.ToUnixTime(fileUpdated));
                if (recSize == fileSize && recUpdatedSec == fileUpdatedSec)
                {
                    // ver 0.25.0より前かつeml形式のファイルであればスキップしない（ver 0.25.0でパース方法に変更が入ったため）
                    var ext = Path.GetExtension(FilePath).TrimStart('.').ToLower();
                    if (recMajorVerOnUpdated == 0 && recMinorVerOnUpdated < 25 && ext == "eml")
                    {
                    }
                    else
                    {
                        Logger.Debug("Skip - {0}", FilePath);

                        return UpdateResult.SKIPPED;
                    }
                }
            }

            // 進捗を報告
            progress?.Report(new ProgressState() { CurrentStep = ProgressState.Step.RecordUpdateBegin, CurrentValue = crawlResult.UpdateProcessed, TotalValue = crawlResult.TotalTargetCount, Path = FilePath });

            // データの登録処理
            Application.ExtractFileSuccess extResSuccess;

            // 拡張子に応じてテキストを抽出する
            try
            {
                var extRes = _app.ExtractFile(FilePath, textExtNames, pluginExtNames);
                if (extRes is Application.ExtractFileSuccess)
                {
                    extResSuccess = (Application.ExtractFileSuccess)extRes;
                    Logger.Debug($"Extract OK - {FilePath} (title: {extResSuccess.Title}, body length: {extResSuccess.Body.Length})");
                }
                else
                {
                    // テキスト抽出失敗
                    var extResFailed = (Application.ExtractFileFailed)extRes;
                    Logger.Debug($"Extract Failed - {FilePath} (message: {extResFailed.ErrorMessage}, size: {fileSize})");

                    return new UpdateResult(UpdateResult.ResultType.Failed)
                    {
                        ErrorMessage = extResFailed.ErrorMessage,
                        ErrorFilePath = FilePath,
                        ErrorFileSize = fileSize
                    };
                }
            }
            catch (TimeoutException ex)
            {
                // タイムアウト発生時はエラー扱い
                Logger.Warn(ex.ToString());
                Logger.Debug($"Extract Timeout - {FilePath}");
                return new UpdateResult(UpdateResult.ResultType.Failed)
                {
                    ErrorMessage = $"ファイルの解析処理で、処理時間が{_app.UserSettings.DocumentExtractTimeoutSecond}秒を超えたためにタイムアウトしました。",
                    ErrorFilePath = FilePath,
                    ErrorFileSize = _app.TryGetFileSize(FilePath)
                };
            }
            catch (OperationCanceledException ex)
            {
                // キャンセル操作の場合は外に投げる
                throw (ex);
            }
            catch (Exception ex)
            {
                // 上記以外の例外発生時
                Logger.Warn($"Crawl Extract Error - {FilePath}");
                Logger.Warn(ex.ToString());

                string errorMessage;
                if (ex is OutOfMemoryException)
                {
                    errorMessage = $"ファイルの解析処理でメモリ不足エラーが発生しました。対象のファイルサイズが大きすぎる可能性があります。";
                }
                else
                {
                    errorMessage = $"ファイルの解析処理でエラーが発生しました。";
                }
                return new UpdateResult(UpdateResult.ResultType.Failed)
                {
                    ErrorMessage = errorMessage,
                    ErrorFilePath = FilePath,
                    ErrorFileSize = _app.TryGetFileSize(FilePath),
                    InnerException = ex
                };
            }

            Thread.Sleep(0); // 他のスレッドに処理を渡す

            // 付与するべきフォルダラベル一覧を取得
            var folderLabels = _app.UserSettings.FindTargetFoldersFromDocumentKey(key).Select((f) => f.Label)
                                                                                      .Where((lbl) => !string.IsNullOrWhiteSpace(lbl))
                                                                                      .ToArray();

            // Groongaへデータを登録
            var appVer = ApplicationEnvironment.GetVersion();
            var obj = new Dictionary<string, object>
                            {
                                { Column.Documents.KEY, key },
                                { Column.Documents.TITLE, extResSuccess.Title },
                                { Column.Documents.BODY, extResSuccess.Body },
                                { Column.Documents.FILE_NAME, Path.GetFileName(FilePath) },
                                { Column.Documents.FOLDER_PATH, Path.GetDirectoryName(FilePath) },
                                { Column.Documents.FILE_PATH, FilePath },
                                { Column.Documents.FILE_UPDATED_AT, Groonga.Util.ToUnixTime(fileUpdated) },
                                { Column.Documents.FILE_UPDATED_YEAR, fileUpdated.Year },
                                { Column.Documents.SIZE, fileSize },
                                { Column.Documents.EXT, Path.GetExtension(FilePath).TrimStart('.').ToLower() },
                                { Column.Documents.UPDATED_AT, Groonga.Util.ToUnixTime(DateTime.Now) },
                                { Column.Documents.FOLDER_LABELS, folderLabels},
                                { Column.Documents.APPLICATION_MAJOR_VERSION_ON_UPDATED, appVer.Major },
                                { Column.Documents.APPLICATION_MINOR_VERSION_ON_UPDATED, appVer.Minor },
                                { Column.Documents.APPLICATION_PATCH_VERSION_ON_UPDATED, appVer.Patch },
                            };

            Logger.Trace($"Store to groonga DB");
            try
            {
                try
                {
                    _app.GM.Load(new[] { obj }, Table.Documents);
                }
                catch (GroongaCommandError ex)
                {

                    if (ex.ReturnCode == Groonga.CommandReturnCode.GRN_NO_MEMORY_AVAILABLE)
                    {
                        // メモリ不足の場合、Groongaを再起動してもう一度取込を試みる
                        // それでも同じエラーが発生した場合は改めてスロー
                        Logger.Warn(ex);
                        Logger.Debug("Groonga Reboot");
                        _app.GM.Reboot();
            _app.GM.Load(new[] { obj }, Table.Documents);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);

                // 例外が発生した場合は、その文書の削除を試みる
                _app.GM.Delete(Table.Documents, key);

                // 登録失敗
                return new UpdateResult(UpdateResult.ResultType.Failed)
                {
                    ErrorMessage = $"ファイルの文書DB登録処理でエラーが発生しました。",
                    ErrorFilePath = FilePath,
                    ErrorFileSize = _app.TryGetFileSize(FilePath),
                    InnerException = ex
                };

            }

            Thread.Sleep(0); // 他のスレッドに処理を渡す

            // 同時に、可能であればサムネイルも保存
            Logger.Trace("Get thumbnail - {0}", FilePath);
            try
            {
                var sh = ShellObject.FromParsingName(FilePath);
                sh.Thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;

                var bmp = sh.Thumbnail.Bitmap;
                var thumbnailName = Util.HexDigest(_app.HashProvider, key) + ".png";
                bmp.Save(Path.Combine(_app.ThumbnailDirPath, thumbnailName));
            }
            catch (OperationCanceledException ex)
            {
                // キャンセル操作の場合は外に投げる
                throw (ex);
            }
            catch (Exception ex)
            {
                Logger.Warn("Thumbnail Error - {0}", FilePath);
                Logger.Debug(ex.ToString());
            }

            Logger.Debug("Update OK - {0}", FilePath);

            // 登録成功
            return UpdateResult.SUCCESS;
        }
    }
}
