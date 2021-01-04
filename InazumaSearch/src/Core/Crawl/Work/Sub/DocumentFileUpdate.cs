using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core.Crawl;
using Microsoft.WindowsAPICodePack.Shell;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// 文書ファイルの情報をDBに登録（更新）する
    /// </summary>
    public class DocumentFileUpdate : WorkBase
    {
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
            var success = UpdateMain(crawlResult, progress);

            // 結果をカウント
            if (success)
            {
                crawlResult.Updated++;

                // 登録件数が200件を超えるごとにGroongaを再起動
                // (Groongaプロセスを立ち上げてパイプを接続したまま大量データを登録すると、システムエラーが発生する場合があるため)
                if (crawlResult.Updated % 200 == 0)
                {
                    Logger.Debug("Groonga Reboot");
                    _app.GM.Reboot();
                }
            }
            else
            {
                crawlResult.Skipped++;
            }

            // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低0.05秒)
            InsertCoolTime(50);
        }

        /// <summary>
        /// 登録メイン処理
        /// </summary>
        protected virtual bool UpdateMain(
            Result crawlResult,
            IProgress<ProgressState> progress = null
        )
        {
            // 進捗を報告
            ReportProgressLimitedFrequency(
                progress,
                new ProgressState() { CurrentStep = ProgressState.Step.RecordUpdateCheckBegin, CurrentValue = crawlResult.Updated + crawlResult.Skipped, TotalValue = crawlResult.TotalTargetCount, Path = FilePath },
                crawlResult
            );

            // 展開対象の拡張子一覧を取得
            var textExtNames = _app.GetTextExtNames();
            var pluginExtNames = _app.GetPluginExtNames();

            // 実行時、指定ファイルがすでに削除(もしくは移動)されている場合は、処理をスキップ
            if (!File.Exists(FilePath))
            {
                Logger.Debug($"Target file not found - {FilePath}");
                return false;
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
                var recSize = (long)rec[Column.Documents.SIZE];
                var recUpdated = (double)rec[Column.Documents.FILE_UPDATED_AT];

                // 日付はGroonga側とFileInfo側で精度の違いがある場合があるため、秒部分のみ比較
                if (recSize == fileSize && Math.Floor(recUpdated) == Math.Floor(Groonga.Util.ToUnixTime(fileUpdated)))
                {
                    Logger.Debug("Skip - {0}", FilePath);

                    return false;
                }
            }

            // 進捗を報告
            progress?.Report(new ProgressState() { CurrentStep = ProgressState.Step.RecordUpdateBegin, CurrentValue = crawlResult.Updated + crawlResult.Skipped, TotalValue = crawlResult.TotalTargetCount, Path = FilePath });

            // データの登録
            // 拡張子に応じてテキストを抽出する
            string body;
            try
            {
                body = _app.ExtractFileText(FilePath, textExtNames, pluginExtNames);
                Logger.Debug($"Extract OK - {FilePath} (length: {body.Length})");
            }
            catch (OperationCanceledException ex)
            {
                // キャンセル操作の場合は外に投げる
                throw (ex);
            }
            catch (Exception ex)
            {
                // 例外発生時はスキップ
                Logger.Warn("Crawl Extract Error - {0}", FilePath);
                Logger.Warn(ex.ToString());
                return false;
            }

            Thread.Sleep(0); // 他のスレッドに処理を渡す

            // 付与するべきフォルダラベル一覧を取得
            var folderLabels = _app.UserSettings.FindTargetFoldersFromDocumentKey(key).Select((f) => f.Label)
                                                                                      .Where((lbl) => !string.IsNullOrWhiteSpace(lbl))
                                                                                      .ToArray();

            // Groongaへデータを登録
            var obj = new Dictionary<string, object>
                            {
                                { Column.Documents.KEY, key },
                                { Column.Documents.BODY, body },
                                { Column.Documents.FILE_NAME, Path.GetFileName(FilePath) },
                                { Column.Documents.FILE_PATH, FilePath },
                                { Column.Documents.FILE_UPDATED_AT, Groonga.Util.ToUnixTime(fileUpdated) },
                                { Column.Documents.FILE_UPDATED_YEAR, fileUpdated.Year },
                                { Column.Documents.SIZE, fileSize },
                                { Column.Documents.EXT, Path.GetExtension(FilePath).TrimStart('.').ToLower() },
                                { Column.Documents.UPDATED_AT, Groonga.Util.ToUnixTime(DateTime.Now) },
                                { Column.Documents.FOLDER_LABELS, folderLabels}
                            };

            Logger.Trace($"Store to groonga DB");
            _app.GM.Load(new[] { obj }, Table.Documents);

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
            return true;
        }
    }
}
