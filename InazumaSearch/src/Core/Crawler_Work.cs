using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;

namespace InazumaSearch.Core
{
    public partial class Crawler
    {
        public interface IWork : IEquatable<IWork>
        {
            string LogCaption { get; }
            void Execute(IProgress<CrawlState> progress, CancellationToken cToken, Result crawlResult);
        }


        public class TargetFile
        {
            public string Path { get; set; }
            public string Key { get; set; }
            public string ThumbnailName { get; set; }

            public static TargetFile Make(string path, SHA1CryptoServiceProvider cryptProvider)
            {
                var tg = new TargetFile
                {
                    Path = System.IO.Path.GetFullPath(path)
                };
                tg.Key = Util.MakeDocumentFileKey(tg.Path);
                tg.ThumbnailName = Util.HexDigest(cryptProvider, tg.Key) + ".png";

                return tg;
            }
        }

        public abstract class WorkBase : IWork
        {
            protected bool _alwaysCrawlMode = false;
            protected Application _app;
            public abstract string LogCaption { get; }

            public NLog.Logger Logger { get { return _app.Crawler.Logger; } }

            public WorkBase(Application app)
            {
                _app = app;
            }

            public abstract void Execute(IProgress<CrawlState> progress, CancellationToken cToken, Result crawlResult);

            public List<string> GetTargetDirectories(string targetFolderType)
            {
                return _app.UserSettings.TargetFolders.Where(f => f.Type == targetFolderType).Select(f => f.Path).OrderBy(f => f).ToList();
            }

            /// <summary>
            /// 等価比較 (Objectメソッドのオーバーライド)
            /// </summary>
            public override bool Equals(object obj)
            {
                if (obj is IWork)
                {
                    return Equals((IWork)obj);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// 等価比較
            /// </summary>
            public abstract bool Equals(IWork other);

            /// <summary>
            /// ハッシュ値算出
            /// </summary>
            /// <remarks>重複チェックのために必要です。</remarks>
            public abstract override int GetHashCode();

            /// <summary>
            /// クールタイムを挟む
            /// </summary>
            protected void InsertCoolTime(ref DateTime lastCoolTimeEnd, int coolTimeMin)
            {
                // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低でもcoolTimeMin分のクールタイムを取る)
                if (_alwaysCrawlMode)
                {
                    var coolTime = (DateTime.Now - lastCoolTimeEnd).Milliseconds * 3;
                    if (coolTime < 10) coolTime = 10;
                    Thread.Sleep(coolTime);
                    lastCoolTimeEnd = DateTime.Now;
                }

            }

            /// <summary>
            /// Groonga内に登録された対象文書のキー, 更新日付, サイズを取得し、それをDictionaryに格納して返す
            /// </summary>
            /// <returns>対象ファイルの一覧</returns>
            protected virtual IDictionary<string, Groonga.RecordSet.Record> DBDocumentRecordListUp(
                  IProgress<CrawlState> progress
                , CancellationToken cToken
                , string targetKey = null
                , string targetKeyStartsWith = null
            )
            {
                progress.Report(new CrawlState() { CurrentStep = CrawlState.Step.DBRecordListUpBegin, CurrentValue = 0 });

                string query = null;
                if (targetKeyStartsWith != null)
                {
                    query = string.Format("{0}:^{1}", Column.Documents.KEY, Groonga.Util.EscapeForQuery(targetKeyStartsWith));
                }
                if (targetKey != null)
                {
                    query = string.Format("{0}:{1}", Column.Documents.KEY, Groonga.Util.EscapeForQuery(targetKey));
                }

                var alreadyRecords = _app.GM.Select(
                      Table.Documents
                    , outputColumns: new[] {
                          Column.Documents.KEY
                        , Column.Documents.FILE_UPDATED_AT
                        , Column.Documents.SIZE
                    }
                    , limit: -1 // 全件
                    , query: query
                );
                var alreadyRecordMap = new Dictionary<string, Groonga.RecordSet.Record>();
                foreach (var rec in alreadyRecords.SearchResult.Records)
                {
                    alreadyRecordMap.Add((string)rec[Column.Documents.KEY], rec);
                }
                cToken.ThrowIfCancellationRequested();

                return alreadyRecordMap;
            }

            /// <summary>
            /// 対象ディレクトリ一覧の取得
            /// </summary>
            /// <returns>対象ディレクトリの一覧</returns>
            protected virtual List<string> DirectoryListUpTask(
                  IProgress<CrawlState> progress
                , CancellationToken cToken
                , List<string> dirPaths
                , out List<IgnoreSetting> ignoreSettings
            )
            {
                // 対象ディレクトリの探索
                var targets = new List<string>();
                var lastReported = DateTime.Now;
                var lastCoolTimeEnd = DateTime.Now;

                var curSearch = 0;
                var aborting = false;

                var ignoreSettingsLocal = new List<IgnoreSetting>();

                foreach (var dirPath in dirPaths)
                {
                    Logger.Trace("FileListUp/Directory - {0}", dirPath);

                    // 検索対象ディレクトリを登録
                    targets.Add(dirPath);

                    // 無視設定ファイルが存在すれば読み込む
                    {
                        var ignorePath = Path.Combine(dirPath, ".inazumaignore");
                        if (File.Exists(ignorePath))
                        {
                            ignoreSettingsLocal.Add(IgnoreSetting.Load(ignorePath));
                        }
                    }

                    // 検索対象ディレクトリ内のサブディレクトリを探索
                    ApplyAllDirectories(dirPath, (targetDir) =>
                    {

                        Logger.Trace("FileListUp/Target Dir Found - {0}", dirPath);

                        curSearch++;

                        // 無視設定ファイルが存在すれば読み込む
                        var ignorePath = Path.Combine(targetDir, ".inazumaignore");
                        if (File.Exists(ignorePath))
                        {
                            ignoreSettingsLocal.Add(IgnoreSetting.Load(ignorePath));
                        }

                        targets.Add(targetDir);
                        Thread.Sleep(0); // 他のスレッドに処理を渡す

                        // 0.05秒経過するまでは進捗を報告しない
                        if ((DateTime.Now - lastReported).Milliseconds >= 50)
                        {
                            Thread.Sleep(0); // 他のスレッドに処理を渡す
                            progress.Report(new CrawlState() { CurrentStep = CrawlState.Step.TargetListUp, CurrentValue = curSearch });
                            lastReported = DateTime.Now;
                            cToken.ThrowIfCancellationRequested();
                        }

                        // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低10ms)
                        InsertCoolTime(ref lastCoolTimeEnd, 10);

                    }, ref aborting);
                }

                // 取得した無視設定一覧を返す
                ignoreSettings = ignoreSettingsLocal;

                return targets;
            }


            /// <summary>
            /// DBへ文書を登録
            /// </summary>
            protected virtual void UpdateDocumentFileRecords(
                  IProgress<CrawlState> progress
                , CancellationToken cToken
                , Result crawlResult
                , List<string> targetSubDirs
                , IDictionary<string, Groonga.RecordSet.Record> alreadyRecordMap
                , List<IgnoreSetting> ignoreSettings
                , out List<TargetFile> targets
            )
            {
                // ローカル定数の設定
                const int SubDirWeight = 100;

                // 出力変数の初期化
                targets = new List<TargetFile>();

                // 各種変数の初期化
                progress.Report(new CrawlState() { CurrentStep = CrawlState.Step.RecordAddBegin, TotalValue = targetSubDirs.Count * SubDirWeight });
                var lastReported = DateTime.Now;
                var lastCoolTimeEnd = DateTime.Now;
                var thumbnailDirPath = _app.ThumbnailDirPath;
                var externalDirPath = Directory.CreateDirectory(thumbnailDirPath);
                var pluginExtNames = _app.PluginManager.GetTextExtNameToLabelMap().Keys.ToList();
                var textExtNames = _app.UserSettings.TextExtensions.Select(x => x.ExtName).ToList();
                textExtNames.Add("txt");

                var extractableExtNames = new List<string>();
                foreach (var format in _app.Formats)
                {
                    extractableExtNames.AddRange(format.Extensions);
                }
                var cryptProvider = new SHA1CryptoServiceProvider();

                var cur = 0;
                var updatedDocumentCount = 0;
                foreach (var targetSubDir in targetSubDirs)
                {
                    var currentSubDirTargets = new List<TargetFile>();

                    // 指定ディレクトリがすでに削除(もしくは移動)されている場合はスキップ
                    if (!Directory.Exists(targetSubDir))
                    {
                        cur += SubDirWeight;
                        continue;
                    }

                    // 無視設定に合致するフォルダはスキップ
                    if (ignoreSettings.Any(s => s.IsMatch(targetSubDir, true)))
                    {
                        Logger.Trace("Ignore directory - {0}", targetSubDir);
                        cur += SubDirWeight;
                        continue;
                    }

                    // 対象となる無視設定のみを抽出
                    var ignoreSettingsLocal = ignoreSettings.Where(s => targetSubDir.ToLower().StartsWith(s.DirPathLower));

                    // ディレクトリ内にある対象ファイルを検索
                    try
                    {
                        foreach (var filePath in Directory.GetFiles(targetSubDir))
                        {
                            // ファイルの拡張子を "txt" 形式で取得
                            var ext = Path.GetExtension(filePath).TrimStart('.').ToLower();

                            // 登録対象の拡張子であれば処理
                            if (extractableExtNames.Contains(ext))
                            {
                                Logger.Trace("FileListUp/Target File Found - {0}", filePath);

                                var tg = TargetFile.Make(filePath, cryptProvider);
                                currentSubDirTargets.Add(tg);
                                Thread.Sleep(0); // 他のスレッドに処理を渡す
                            }
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // 権限の問題でアクセスできなかった場合はスキップ
                        Debug.WriteLine(ex.ToString());
                    }

                    // 無視設定に合致するファイルはスキップ
                    foreach (var ignore in ignoreSettingsLocal)
                    {
                        currentSubDirTargets = currentSubDirTargets.Where(target =>
                        {
                            if (ignore.IsMatch(target.Path, false))
                            {
                                Logger.Trace("Ignore file - {0}", target.Path);
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }).ToList();
                    }

                    // 指定ディレクトリ内に対象ファイルがなければスキップ
                    if (currentSubDirTargets.Count == 0)
                    {
                        cur += SubDirWeight;
                        continue;
                    }

                    // 対象ファイル一覧を追加
                    targets.AddRange(currentSubDirTargets);

                    // 1ファイルごとに登録処理
                    var curFileInSubDir = 0;
                    var startCurInSubDir = cur;
                    foreach (var target in currentSubDirTargets)
                    {
                        // ファイル情報取得
                        var fileInfo = new FileInfo(target.Path);
                        var fileUpdated = fileInfo.LastWriteTime;
                        var fileSize = fileInfo.Length;

                        // ファイルがすでに登録されていて、日付もサイズも変わっていなければスキップ
                        var skipping = false;
                        if (alreadyRecordMap.ContainsKey(target.Key))
                        {
                            var rec = alreadyRecordMap[target.Key];
                            var recSize = (long)rec[Column.Documents.SIZE];
                            var recUpdated = (double)rec[Column.Documents.FILE_UPDATED_AT];

                            // 日付はGroonga側とFileInfo側で精度の違いがある場合があるため、秒部分のみ比較
                            if (recSize == fileSize && Math.Floor(recUpdated) == Math.Floor(Groonga.Util.ToUnixTime(fileUpdated)))
                            {
                                skipping = true;
                                crawlResult.Skipped++;
                                Logger.Trace("Skip - {0}", target.Path);
                                Thread.Sleep(1);
                            }
                        }

                        if (!skipping)
                        {
                            // データの登録
                            // 拡張子に応じてテキストを抽出する
                            var ext = Path.GetExtension(target.Path).TrimStart('.').ToLower();
                            string body;
                            if (pluginExtNames.Contains(ext))
                            {
                                // プラグインが対応している場合は、プラグインを使用してテキスト抽出
                                body = _app.PluginManager.ExtractText(target.Path);

                            }
                            else if (textExtNames.Contains(ext))
                            {
                                // テキストの拡張子として登録されている場合は、テキストファイルとして読み込み
                                body = "";
                                var fi = new FileInfo(target.Path);
                                using (var input = new Hnx8.ReadJEnc.FileReader(fi))
                                {
                                    var charCode = input.Read(fi);
                                    body = input.Text;
                                }
                            }
                            else
                            {

                                // 上記以外の場合はXDoc2Txtを使用
                                try
                                {
                                    body = XDoc2TxtApi.Extract(target.Path);
                                }
                                catch (Exception ex)
                                {
                                    crawlResult.Skipped++;
                                    Logger.Warn("Crawl Extract Error - {0}", target.Path);
                                    Logger.Warn(ex.ToString());
                                    Thread.Sleep(1);
                                    continue;
                                }
                            }

                            var folderLabels = _app.UserSettings.FindTargetFoldersFromDocumentKey(target.Key).Select((f) => f.Label)
                                                                                                      .Where((lbl) => !string.IsNullOrWhiteSpace(lbl))
                                                                                                      .ToArray();
                            var obj = new Dictionary<string, object>
                            {
                                { Column.Documents.KEY, target.Key },
                                { Column.Documents.BODY, body },
                                { Column.Documents.FILE_NAME, Path.GetFileName(target.Path) },
                                { Column.Documents.FILE_PATH, target.Path },
                                { Column.Documents.FILE_UPDATED_AT, Groonga.Util.ToUnixTime(fileUpdated) },
                                { Column.Documents.FILE_UPDATED_YEAR, fileUpdated.Year },
                                { Column.Documents.SIZE, fileSize },
                                { Column.Documents.EXT, ext },
                                { Column.Documents.UPDATED_AT, Groonga.Util.ToUnixTime(DateTime.Now) },
                                { Column.Documents.FOLDER_LABELS, folderLabels}
                            };

                            _app.GM.Load(new[] { obj }, Table.Documents);

                            // 同時に、可能であればサムネイルも保存
                            try
                            {
                                var sh = ShellObject.FromParsingName(target.Path);
                                sh.Thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;

                                var bmp = sh.Thumbnail.Bitmap;
                                bmp.Save(Path.Combine(thumbnailDirPath, target.ThumbnailName));
                            }
                            catch (Exception ex)
                            {
                                Logger.Debug(ex.ToString());
                                Debug.Print(ex.ToString());
                            }


                            crawlResult.Updated++;
                            Logger.Trace("Update - {0}", target.Path);

                            // 登録件数が200件を超えるごとにGroongaを再起動
                            // (Groongaプロセスを立ち上げてパイプを接続したまま大量データを登録すると、システムエラーが発生する場合があるため)
                            if (crawlResult.Updated % 200 == 0)
                            {
                                Logger.Debug("Groonga Reboot");
                                _app.GM.Reboot();
                            }
                        }

                        cur = startCurInSubDir + (curFileInSubDir * SubDirWeight / currentSubDirTargets.Count);
                        curFileInSubDir++;
                        updatedDocumentCount++;

                        // 0.05秒経過するまでは進捗を報告しない
                        if ((DateTime.Now - lastReported).Milliseconds >= 50)
                        {
                            Thread.Sleep(0); // 他のスレッドに処理を渡す
                            lastReported = DateTime.Now;
                            progress.Report(new CrawlState() { CurrentStep = CrawlState.Step.RecordAdding, CurrentValue = cur, UpdatedDocumentCount = updatedDocumentCount });
                            cToken.ThrowIfCancellationRequested();
                        }

                        // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低50ms)
                        InsertCoolTime(ref lastCoolTimeEnd, 50);
                    }

                    // サブディレクトリ処理終了
                    cur++;

                }
            }

            /// <summary>
            /// 実ファイルが存在しない／無視対象となっている文書データの削除
            /// </summary>
            protected virtual void PurgeDocumentFileRecords(
                  IProgress<CrawlState> progress
                , CancellationToken cToken
                , Result crawlResult
                , IDictionary<string, Groonga.RecordSet.Record> alreadyRecordMap
                , IList<IgnoreSetting> IgnoreSettings
                , IList<TargetFile> targets
            )
            {
                progress.Report(new CrawlState() { CurrentStep = CrawlState.Step.PurgeBegin, TotalValue = targets.Count });
                var lastCoolTimeEnd = DateTime.Now;

                // 存在するファイルのパスをすべてキー形式に変換
                var targetKeys = targets.Select(t => t.Key).ToList();

                foreach (var key in alreadyRecordMap.Keys.Where(k => k.StartsWith("f:")))
                {
                    if (!targetKeys.Contains(key))
                    {
                        _app.GM.Delete(Table.Documents, key: key);
                        crawlResult.Deleted++;
                        Logger.Trace("Purge - {0}", key);
                        cToken.ThrowIfCancellationRequested();
                        Thread.Sleep(0); // 他のスレッドに処理を渡す
                    }

                    // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低50ms)
                    InsertCoolTime(ref lastCoolTimeEnd, 50);
                }
            }

            /// <summary>
            /// 指定した条件の文書データを削除する
            /// </summary>
            protected virtual void DeleteDocumentFileRecords(
                  IProgress<CrawlState> progress
                , CancellationToken cToken
                , Result crawlResult
                , string targetKey = null
                , string targetExpr = null
            )
            {
                var lastCoolTimeEnd = DateTime.Now;
                // 削除
                _app.GM.Delete(Table.Documents, key: targetKey, filter: targetExpr);
                crawlResult.Deleted++;
                Logger.Trace("Delete - {0}", (targetKey != null ? targetKey : targetExpr));
                cToken.ThrowIfCancellationRequested();
                Thread.Sleep(0); // 他のスレッドに処理を渡す

                // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低50ms)
                InsertCoolTime(ref lastCoolTimeEnd, 50);
            }

            /// <summary>
            /// 子ディレクトリすべてに対して処理 (例外が発生したファイルは無視する)
            /// </summary>
            /// <remarks>https://stackoverflow.com/questions/172544/ignore-folders-files-when-directory-getfiles-is-denied-access</remarks>
            protected virtual void ApplyAllDirectories(string folder, Action<string> dirAction, ref bool aborting)
            {
                foreach (var dir in Directory.GetDirectories(folder))
                {
                    try
                    {
                        dirAction(dir);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    catch (IOException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
                foreach (var subDir in Directory.GetDirectories(folder))
                {
                    try
                    {
                        ApplyAllDirectories(subDir, dirAction, ref aborting);
                        if (aborting) return;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    catch (IOException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }


            /// <summary>
            /// 子ファイルすべてに対して処理 (例外が発生したファイルは無視する)
            /// </summary>
            /// <remarks>https://stackoverflow.com/questions/172544/ignore-folders-files-when-directory-getfiles-is-denied-access</remarks>
            protected virtual void ApplyAllFiles(string folder, Action<string> fileAction, ref bool aborting)
            {
                foreach (var file in Directory.GetFiles(folder))
                {
                    try
                    {
                        fileAction(file);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    catch (IOException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
                foreach (var subDir in Directory.GetDirectories(folder))
                {
                    try
                    {
                        ApplyAllFiles(subDir, fileAction, ref aborting);
                        if (aborting) return;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    catch (IOException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public class Work
        {
            /// <summary>
            /// 全体クロール
            /// </summary>
            public class FullCrawl : WorkBase
            {
                public FullCrawl(Application app, bool alwaysCrawlMode) : base(app)
                {
                    _alwaysCrawlMode = alwaysCrawlMode;
                }

                /// <summary>
                /// 等価比較
                /// </summary>
                public override bool Equals(IWork other)
                {
                    return (other is FullCrawl);
                }

                /// <summary>
                /// ハッシュ値算出
                /// </summary>
                /// <remarks>重複チェックのために必要です。</remarks>
                public override int GetHashCode()
                {
                    return GetType().GetHashCode();
                }

                public override string LogCaption { get { return "フルクロール"; } }


                public override void Execute(IProgress<CrawlState> progress, CancellationToken cToken, Result crawlResult)
                {
                    Logger.Info("フルクロール開始");


                    // ファイルのクロール処理を実行
                    var dbRecordMap = DBDocumentRecordListUp(progress, cToken); // DB内の全レコード一覧を取得

                    var dirPaths = GetTargetDirectories(UserSetting.TargetFolderType.DocumentFile); // 対象ディレクトリ一覧を取得
                    List<IgnoreSetting> ignoreSettings;
                    var targetSubDirs = DirectoryListUpTask(progress, cToken, dirPaths, out ignoreSettings); // 対象ディレクトリ一覧取得
                    List<TargetFile> targets = null;
                    UpdateDocumentFileRecords(progress, cToken, crawlResult, targetSubDirs, dbRecordMap, ignoreSettings, out targets); // 文書データ登録
                    PurgeDocumentFileRecords(progress, cToken, crawlResult, dbRecordMap, ignoreSettings, targets); // 不要な文書データ削除

                    //var targetMailBoxes = MailListUpTask(progress, ctSource.Token); // 対象mbox一覧取得
                    //UpdateMailRecords(progress, ctSource.Token, res, targetMailBoxes, dbRecordMap); // メールデータ登録
                    //PurgeMailRecords(progress, ctSource.Token, res, targetMailBoxes, dbRecordMap); // 不要なメールデータ削除

                    progress.Report(new CrawlState() { CurrentStep = CrawlState.Step.Finish });

                    crawlResult.Finished = true;

                    Logger.Info("フルクロール完了 ({0})", JsonConvert.SerializeObject(crawlResult));
                }
            }

            /// <summary>
            /// 文書フォルダの削除
            /// </summary>
            public class DocumentDirectoryDelete : WorkBase
            {
                public virtual string DirPath { get; protected set; }

                public DocumentDirectoryDelete(Application app, string dirPath) : base(app)
                {
                    DirPath = dirPath;
                }

                public override string LogCaption { get { return string.Format("文書フォルダ削除({0})", DirPath); } }

                /// <summary>
                /// 等価比較
                /// </summary>
                /// <remarks>重複チェックのために必要です。</remarks>
                public override bool Equals(IWork other)
                {
                    var work = other as DocumentDirectoryDelete;
                    return (work != null && work.DirPath == DirPath);
                }

                /// <summary>
                /// ハッシュ値算出
                /// </summary>
                /// <remarks>重複チェックのために必要です。</remarks>
                public override int GetHashCode()
                {
                    return GetType().GetHashCode() ^ DirPath.GetHashCode();
                }

                public override void Execute(IProgress<CrawlState> progress, CancellationToken cToken, Result crawlResult)
                {
                    // ディレクトリ配下のファイルをすべて削除する
                    var expr = string.Format("{0} @^ {1}", Column.Documents.KEY, Groonga.Util.EscapeForQuery(Util.MakeDocumentFileKey(DirPath)));
                    DeleteDocumentFileRecords(progress, cToken, crawlResult, targetExpr: expr);

                    crawlResult.Finished = true;
                }
            }

            /// <summary>
            /// 文書ディレクトリのクロール (削除含む)
            /// </summary>
            public class DocumentDirectoryCrawl : WorkBase
            {
                public virtual string DirPath { get; protected set; }

                public DocumentDirectoryCrawl(Application app, string dirPath) : base(app)
                {
                    DirPath = dirPath;
                }

                public override string LogCaption { get { return string.Format("文書ディレクトリクロール({0})", DirPath); } }

                /// <summary>
                /// 等価比較
                /// </summary>
                /// <remarks>重複チェックのために必要です。</remarks>
                public override bool Equals(IWork other)
                {
                    var work = other as DocumentDirectoryCrawl;
                    return (work != null && work.DirPath == DirPath);
                }

                /// <summary>
                /// ハッシュ値算出
                /// </summary>
                /// <remarks>重複チェックのために必要です。</remarks>
                public override int GetHashCode()
                {
                    return GetType().GetHashCode() ^ DirPath.GetHashCode();
                }

                public override void Execute(IProgress<CrawlState> progress, CancellationToken cToken, Result crawlResult)
                {
                    // 登録対象のディレクトリパスを、キー形式に変換
                    var keyPrefix = Util.MakeDocumentFileKey(DirPath);

                    // 上記ディレクトリ以下にある登録済み文書のデータを取得
                    var dbRecordMap = DBDocumentRecordListUp(progress, cToken, targetKeyStartsWith: keyPrefix);

                    // 上記ディレクトリ以下にある対象サブディレクトリ一覧取得
                    List<IgnoreSetting> ignoreSettings;
                    var targetDirs = new string[] { DirPath };
                    var targetDirPaths = DirectoryListUpTask(progress, cToken, targetDirs.ToList(), out ignoreSettings);

                    // 対象ファイルを登録/削除する
                    List<TargetFile> targets;
                    UpdateDocumentFileRecords(progress, cToken, crawlResult, targetDirPaths, dbRecordMap, ignoreSettings, out targets); // 文書データ登録
                    PurgeDocumentFileRecords(progress, cToken, crawlResult, dbRecordMap, ignoreSettings, targets); // 不要な文書データ削除

                    crawlResult.Finished = true;
                }

            }

            /// <summary>
            /// 文書ファイル1つを削除
            /// </summary>
            public class DocumentFileDelete : WorkBase
            {
                public virtual string FilePath { get; protected set; }

                public DocumentFileDelete(Application app, string filePath) : base(app)
                {
                    FilePath = filePath;
                }

                public override string LogCaption { get { return string.Format("文書ファイル削除({0})", FilePath); } }

                /// <summary>
                /// 等価比較
                /// </summary>
                /// <remarks>重複チェックのために必要です。</remarks>
                public override bool Equals(IWork other)
                {
                    var work = other as DocumentFileDelete;
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

                public override void Execute(IProgress<CrawlState> progress, CancellationToken cToken, Result crawlResult)
                {
                    // 指定ファイルをすべて削除する
                    DeleteDocumentFileRecords(progress, cToken, crawlResult, targetKey: Util.MakeDocumentFileKey(FilePath));

                    crawlResult.Finished = true;
                }
            }

        }
    }


}
