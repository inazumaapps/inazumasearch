﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Hnx8.ReadJEnc;
using InazumaSearchLib.Core.Crawl;
using MimeKit;
using Semver;

namespace InazumaSearchLib.Core
{
    /// <summary>
    /// Inazuma Searchのメイン処理を担当するクラス。アプリケーションごとに継承して使用する
    /// </summary>
    public abstract class ApplicationBase
    {
        /// <summary>
        /// デバッグモード
        /// </summary>
        public bool DebugMode { get; set; } = false;

        /// <summary>
        /// ログ出力用オブジェクト
        /// </summary>
        public NLog.Logger Logger { get; protected set; }

        /// <summary>
        /// ユーザー設定
        /// </summary>
        public UserSetting.Store UserSettings { get; protected set; }

        /// <summary>
        /// Groonga制御用オブジェクト
        /// </summary>
        public Groonga.Manager GM { get; protected set; }

        /// <summary>
        /// プラグイン管理用オブジェクト
        /// </summary>
        public Plugin.Manager PluginManager { get; protected set; }

        /// <summary>
        /// 対応しているすべてのフォーマットリスト
        /// </summary>
        public IList<Format> Formats { get; protected set; } = new List<Format>();

        /// <summary>
        /// クローラ
        /// </summary>
        public Core.Crawl.Crawler Crawler { get; protected set; }

        /// <summary>
        /// ハッシュ生成を行うオブジェクト。サムネイル画像パスの生成に使用
        /// </summary>
        public HashAlgorithm HashProvider { get; set; } = new SHA1CryptoServiceProvider();

        /// <summary>
        /// イベントログ（新しいログほど後ろに追加されている）
        /// </summary>
        public List<EventLog> EventLogs { get; protected set; } = new List<EventLog>();

        #region パスプロパティ

        /// <summary>
        /// dataディレクトリパス
        /// </summary>
        public abstract string DataDirPath { get; }

        /// <summary>
        /// groonga.exeのパス
        /// </summary>
        public abstract string GroongaExePath { get; }

        /// <summary>
        /// xdoc2txt.exeのパス
        /// </summary>
        public abstract string XDoc2TxtExePath { get; }

        #endregion

        #region 特殊パスの取得

        /// <summary>
        /// 文書DBディレクトリの初期パス
        /// </summary>
        public virtual string DefaultDBDirPath
        {
            get
            {
                return Path.Combine(DataDirPath, "db");
            }
        }

        /// <summary>
        /// 文書DBディレクトリのパス
        /// </summary>
        public virtual string DBDirPath
        {
            get
            {
                if (UserSettings.DocumentDBDirPath != null)
                {
                    return UserSettings.DocumentDBDirPath;
                }
                else
                {
                    return DefaultDBDirPath;
                }
            }
        }

        /// <summary>
        /// サムネイルディレクトリのパス
        /// </summary>
        public virtual string ThumbnailDirPath
        {
            get
            {
                return Path.Combine(DataDirPath, "thumbnail");
            }
        }
        /// <summary>
        /// ログディレクトリのパス
        /// </summary>
        public virtual string LogDirPath
        {
            get
            {
                return Path.Combine(DataDirPath, "log");
            }
        }

        /// <summary>
        /// ユーザー設定ファイルディレクトリのパス
        /// </summary>
        public virtual string UserSettingDirPath
        {
            get
            {
                return DataDirPath;
            }
        }

        /// <summary>
        /// ユーザー設定ファイルのパス
        /// </summary>
        public virtual string UserSettingPath { get { return Path.Combine(UserSettingDirPath, @"UserSettings.json"); } }

        #endregion

        #region メッセージ表示・ユーザー確認

        /// <summary>
        /// 情報メッセージ表示
        /// </summary>
        /// <param name="message">メッセージ内容</param>
        /// <param name="title">ウインドウタイトル</param>
        public abstract void ShowInformationMessage(string message, string title = "情報");

        /// <summary>
        /// エラーメッセージ表示
        /// </summary>
        /// <param name="message">メッセージ内容</param>
        public abstract void ShowErrorMessage(string message);

        /// <summary>
        /// ユーザー確認
        /// </summary>
        /// <param name="message">メッセージ内容</param>
        /// <param name="defaultNo">初期選択「はい」...true / 初期選択「いいえ」...false</param>
        /// <returns>ユーザーが「はい」を選択...true / ユーザーが「いいえ」を選択...false</returns>
        public abstract bool Confirm(string message, bool defaultNo = false);

        #endregion

        #region 進捗表示

        /// <summary>
        /// 指定した処理を進捗表示ありで実行
        /// </summary>
        /// <param name="task">実行する処理</param>
        /// <param name="message">表示メッセージ</param>
        public abstract void InvokeWithProgressDisplay(Task task, string message);

        #endregion

        /// <summary>
        /// イベントログを追加
        /// </summary>
        public virtual void AddEventLog(EventLog log)
        {
            // イベントログ表示側と同時に触らないようにロック
            lock (EventLogs)
            {
                EventLogs.Add(log);

                // 1,000件を超えた場合は古いものを切り捨てる
                if (EventLogs.Count > 1000)
                {
                    EventLogs.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 対応している拡張子のリストを取得 ("txt" 形式で取得する)
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetExtractableExtNames()
        {
            var ret = new List<string>();
            foreach (var format in Formats)
            {
                ret.AddRange(format.Extensions);
            }
            return ret;
        }

        /// <summary>
        /// 対応フォーマット一覧を更新
        /// </summary>
        public void RefreshFormats()
        {
            // フォーマット関連の変数を初期化
            // (1)プラグインが対応している形式
            // (2)テキストファイルとして設定されている形式
            // (3)デフォルト形式
            // の順で全フォーマットを登録
            var formats = new List<Format>();
            foreach (var pair in PluginManager.GetTextExtNameToLabelMap())
            {
                var extName = pair.Key;
                var label = pair.Value;
                formats.Add(new Format("plugindoc", label, new[] { extName }));
            }
            formats.Add(new Format("text", "テキストファイル", new[] { "txt" }));
            foreach (var ext in UserSettings.TextExtensions)
            {
                formats.Add(new Format("text", ext.Label, new[] { ext.ExtName }));
            }
            formats.AddRange(Format.ALL_DEFAULT_FORMATS);
            Formats = formats;
        }

        /// <summary>
        /// アプリケーションの起動 (進捗ダイアログを表示する場合があるため、UIスレッドで立ち上げる必要がある)
        /// </summary>
        /// <returns>正常に起動したかどうか。falseを返した場合はアプリケーションを終了</returns>
        public virtual bool Boot()
        {
            // ロガー初期化
            Logger = NLog.LogManager.GetLogger("Application");

            // ログ出力
            Logger.Info("アプリケーション起動中...");

            // ユーザー設定の読み込み
            UserSettings = UserSetting.Store.Setup(UserSettingPath);

            // Groongaの起動 (Groongaが存在しなければ終了)
            GM = new InazumaSearchLib.Groonga.Manager(GroongaExePath, DBDirPath, LogDirPath, DebugMode);
            if (!File.Exists(GM.GroongaExePath))
            {
                ShowErrorMessage("groonga.exeが見つかりませんでした。");
                OnQuit();
                return false;
            }
            GM.Boot();

            // プラグインの読み込み
            PluginManager = new Plugin.Manager();
            var loadedPluginVersionNumbers = PluginManager.LoadPlugins();

            // 対応フォーマット一覧の更新
            RefreshFormats();

            // クローラの初期化
            Crawler = new Crawler(this);

            // 現在DBのスキーマバージョンを取得 (DBが存在しない場合は0)
            var schemaVer = GM.GetSchemaVersion();
            var firstBoot = schemaVer == 0;

            // 現在DBのスキーマバージョンが、アプリケーションが要求するスキーマバージョンより高い場合はエラーとして終了
            if (schemaVer > Groonga.Manager.AppSchemaVersion)
            {
                ShowErrorMessage(string.Format("このバージョンのInazuma Searchが最新バージョンではないため、データベースを読み込むことができません。\n最新バージョンのInazuma Searchを使用してください。"));
                OnQuit();
                return false;
            }

            // 現在DBのスキーマバージョンが、アプリケーションが要求するスキーマバージョンより低い場合は
            // DBの新規作成orアップグレード処理を実行


            if (schemaVer < Groonga.Manager.AppSchemaVersion)
            {
                var reCrawlRequired = false;
                var t = Task.Run(() =>
                {
                    GM.SetupSchema(schemaVer, out reCrawlRequired);
                });
                var msg = (firstBoot ? "データベースを作成しています。しばらくお待ちください..." : "データベースを更新しています。しばらくお待ちください...");
                InvokeWithProgressDisplay(t, msg);

                // 再度のクロールが必要ならメッセージを表示
                if (reCrawlRequired)
                {
                    ShowInformationMessage(string.Format("Inazuma Searchのアップグレードにより、再度のクロール実行が必要となります。\nお手数をおかけしますが、起動後にクロールを実行してください。"), "確認");
                }

                // ユーザー設定を再読み込み (スキーマ更新で影響が及ぶ場合があるため)
                UserSettings.Load();
            }

            // プラグイン構成に変更があれば、確認ダイアログを削除してクロール結果を削除
            foreach (var pair in loadedPluginVersionNumbers)
            {
                var pluginFullName = pair.Key;
                var versionNumber = pair.Value;

                if (!UserSettings.LastLoadedPluginVersionNumbers.ContainsKey(pluginFullName)
                    || versionNumber != UserSettings.LastLoadedPluginVersionNumbers[pluginFullName])
                {
                    if (Confirm("プラグインの構成が変更されています。\nこの変更により、クロール結果に差異が出る可能性があります。\n\nInazuma Searchが保持している、クロール済みの文書情報を削除してもよろしいですか？\n（文書ファイルは削除されません）"))
                    {
                        var t = Task.Run(() =>
                        {
                            GM.Truncate(Table.Documents);
                            GM.Truncate(Table.DocumentsIndex);
                            DeleteAllThumbnailFiles();
                        });
                        InvokeWithProgressDisplay(t, "文書データをクリアしています...");
                    };
                    break;
                }
            }

            // DBがすでに存在しており、かつ前回の最終起動バージョンが0.21.0以前であれば、再クロールを促す
            if (!firstBoot && (UserSettings.LastBootVersion == null || UserSettings.LastBootVersion != null && UserSettings.LastBootVersion <= new SemVersion(0, 21, 0)))
            {
                if (Confirm("【バージョンアップに伴うお知らせ】\nver 0.21.0以前のInazuma Searchをお使いいただいていた場合は、不具合により、Inazuma Searchが保持するクロール済み文書情報の一部が破損していた可能性があります。\nこれまでのバージョンで、クロール時や検索時にシステムエラーが多発していた方は、ご迷惑をおかけして申し訳ありませんが、クロール済み文書情報を一度削除していただいた上で、再度のクロール実行をお願いいたします。\n\nInazuma Searchが保持している、クロール済み文書情報を削除してもよろしいですか？\n（文書ファイルや設定情報は削除されません）"))
                {
                    var t = Task.Run(() =>
                    {
                        GM.Truncate(Table.Documents);
                        GM.Truncate(Table.DocumentsIndex);
                        DeleteAllThumbnailFiles();
                    });
                    InvokeWithProgressDisplay(t, "クロール済み文書情報をクリアしています...");

                    ShowInformationMessage("クロール済み文書情報を削除しました。\nお手数ですが、Inazuma Searchの起動後に、もう一度クロールを実行してください。");
                };
            }

            // サムネイルフォルダが存在しなければ作成
            Directory.CreateDirectory(ThumbnailDirPath);

            // 起動完了時の更新処理（最終起動バージョンの更新、プラグイン構成の保存）
            UserSettings.SaveOnAfterBoot(new Dictionary<string, int>(loadedPluginVersionNumbers));

            // ログ出力
            Logger.Info("アプリケーションを起動しました");

            // 常駐クロールモードがONであれば、クロールを開始する
            if (UserSettings.AlwaysCrawlMode)
            {
                Crawler.StartAlwaysCrawl();
            }

            // 正常起動
            return true;
        }

        /// <summary>
        /// 実際にファイルを検索する対象フォルダパスを決定する。
        /// 引数でリストが指定されていればそのパスリストを使用、指定されていなければユーザー設定から取得した全ての検索対象フォルダを返す。
        /// </summary>
        public virtual List<string> GetCrawlTargetDirPaths(IEnumerable<string> specifiedTargetDirPaths)
        {
            var usingTargetDirPaths = specifiedTargetDirPaths;
            if (usingTargetDirPaths == null)
            {
                usingTargetDirPaths = UserSettings.TargetFolders.Where(f => f.Type == UserSetting.TargetFolderType.DocumentFile)
                                                                .Select(f => f.Path).OrderBy(f => f).ToList();
            }

            return usingTargetDirPaths.ToList();
        }

        /// <summary>
        /// テキストファイルとして扱うファイル拡張子の一覧を取得。結果にドット記号は含まない (例: "txt")
        /// </summary>
        public virtual List<string> GetTextExtNames()
        {
            var textExtNames = UserSettings.TextExtensions.Select(x => x.ExtName).ToList();
            textExtNames.Add("txt");
            return textExtNames;
        }

        /// <summary>
        /// プラグインで扱うファイル拡張子の一覧を取得。結果にドット記号は含まない (例: "xdw")
        /// </summary>
        public virtual List<string> GetPluginExtNames()
        {
            return PluginManager.GetTextExtNameToLabelMap().Keys.ToList();
        }

        /// <summary>
        /// <see cref="ExtractFile(string, IEnumerable{string}, IEnumerable{string})" />で抽出した結果を表すクラスです。
        /// </summary>
        public abstract class ExtractFileResult
        {
        }

        /// <summary>
        /// <see cref="ExtractFile(string, IEnumerable{string}, IEnumerable{string})" />で抽出した結果（成功）を表すクラスです。
        /// </summary>
        public class ExtractFileSuccess : ExtractFileResult
        {
            public string Title { get; set; }
            public string Body { get; set; }
        }

        /// <summary>
        /// <see cref="ExtractFile(string, IEnumerable{string}, IEnumerable{string})" />で抽出した結果（失敗）を表すクラスです。
        /// </summary>
        public class ExtractFileFailed : ExtractFileResult
        {
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// 拡張子に応じて、指定した文書ファイルの情報（件名、本文テキスト）を抽出
        /// </summary>
        /// <param name="path">文書ファイルパス</param>
        /// <returns>登録結果</returns>
        public virtual ExtractFileResult ExtractFile(
            string path
            , IEnumerable<string> textExtNames
            , IEnumerable<string> pluginExtNames
        )
        {
            var ext = Path.GetExtension(path).TrimStart('.').ToLower();

            if (pluginExtNames.Contains(ext))
            {
                // プラグインが対応している場合は、プラグインを使用してテキスト抽出
                Logger.Trace($"Extract by plugin - {path}");
                return new ExtractFileSuccess() { Body = PluginManager.ExtractText(path) };
            }
            else if (ext == "eml")
            {
                // メールの場合は、MIMEKitでパース
                using (var stream = File.OpenRead(path))
                {
                    // パーサを生成
                    var parser = new MimeParser(stream, MimeFormat.Entity);
                    while (!parser.IsEndOfStream)
                    {
                        var message = parser.ParseMessage();
                        return new ExtractFileSuccess() { Title = message.Subject, Body = message.TextBody ?? message.HtmlBody ?? "" };
                    }

                    // 1件もメールがなければ空文書とする
                    return new ExtractFileSuccess() { Body = "" };
                }
            }
            else if (textExtNames.Contains(ext))
            {
                // テキストの拡張子として登録されている場合は、テキストファイルとして読み込み
                Logger.Trace($"Extract as text file - {path}");
                var body = "";

                // ファイルサイズを取得
                var fileSize = File.GetSize(path);

                // 最大サイズを超える場合は登録不可
                if (fileSize > UserSettings.TextFileMaxSizeByMB * 1024 * 1024)
                {
                    return new ExtractFileFailed
                    {
                        ErrorMessage = $"テキストファイルのサイズが、登録可能な最大サイズ（{UserSettings.TextFileMaxSizeByMB:#,0}MB）を超えています。　※最大サイズは詳細設定より変更可能です",
                    };
                }

                // 10MBを超えるかどうかで処理を分岐
                var bufferSize = 1024 * 1024 * 10;
                if (fileSize == 0)
                {
                    // 空テキストの場合の特殊処理
                    return new ExtractFileSuccess() { Body = string.Empty };
                }
                else if (fileSize > bufferSize)
                {
                    // 10MBを超える場合は、まず最初の10MB分からエンコーディングを判定
                    CharCode charCode;
                    using (var sourceStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var byteBuffer = new byte[bufferSize];
                        int bytesRead;
                        bytesRead = sourceStream.Read(byteBuffer, 0, byteBuffer.Length);
                        {
                            string _dummy;
                            charCode = ReadJEnc.JP.GetEncoding(byteBuffer, bytesRead, out _dummy);
                        }
                    }

                    // その後に3,000,000文字ずつ読み込む
                    var readCharCount = 3000000; // 一度に読み込む文字数
                    var builder = new StringBuilder();
                    var enc = (charCode != null ? charCode.GetEncoding() : Encoding.UTF8); // エンコーディング。判別に失敗した場合はUTF-8と仮定する
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(path, enc))
                    {
                        var charBuffer = new char[readCharCount];
                        int charRead;

                        while ((charRead = reader.Read(charBuffer, 0, charBuffer.Length)) > 0)
                        {
                            builder.Append(charBuffer);
                        }
                    }
                    return new ExtractFileSuccess() { Body = builder.ToString() };
                }
                else
                {
                    // 10MB未満の場合は、すべてまとめて読み込む
                    var bytes = File.ReadAllBytes(path);
                    var charCode = ReadJEnc.JP.GetEncoding(bytes, bytes.Length, out body);

                    if (body == null)
                    {
                        // 判別できなかった場合
                        return new ExtractFileFailed
                        {
                            ErrorMessage = $"テキストファイルの読み込みに失敗しました。Inazuma Searchが取り扱えないエンコーディングのテキストか、もしくはテキストファイルではない可能性があります。",
                        };
                    }
                    else
                    {
                        return new ExtractFileSuccess() { Body = body };
                    }


                }
            }
            else
            {
                // 上記以外の場合はXDoc2Txtを使用
                Logger.Trace($"Extract by xdoc2txt - {path}");
                var body = XDoc2TxtApi.Extract(XDoc2TxtExePath, path, UserSettings.DocumentExtractTimeoutSecond);
                return new ExtractFileSuccess() { Body = body };
            }
        }

        /// <summary>
        /// ユーザー設定から無視設定一覧を取得
        /// </summary>
        public virtual List<IgnoreSetting> GetIgnoreSettings()
        {
            var settings = new List<IgnoreSetting>();
            foreach (var folderSetting in UserSettings.TargetFolders)
            {
                if (folderSetting.IgnoreSettingLines != null && folderSetting.IgnoreSettingLines.Count >= 1)
                {
                    var newSetting = IgnoreSetting.Load(folderSetting.Path, folderSetting.IgnoreSettingLines);
                    settings.Add(newSetting);
                }
            }

            return settings;
        }

        /// <summary>
        /// 進捗フォームを表示した状態で、指定処理を実行。
        /// 現在実行中の常駐クロール処理がある場合、その常駐クロールを停止したうえで指定処理を実行し、完了後に必要に応じて常駐クロールを再開する
        /// （DB全体に影響を与える更新操作などの実行時に使用）
        /// </summary>
        public virtual void InvokeWithProgressFormWithoutAlwaysCrawl(string caption, Action mainProc)
        {
            // 常駐クロールの自動再起動を無効化
            Crawler.DisableAlwaysCrawlAutoReboot(() =>
        {
            // メイン処理
            var mainTask = Task.Run(async () =>
            {
                // 常駐クロール実行中の場合、停止
                await Crawler.StopAlwaysCrawlIfRunningAsync();

                // メイン処理を実行
                mainProc.Invoke();
            });

            // 進捗状況ダイアログを開き、メイン処理を実行
            InvokeWithProgressDisplay(mainTask, caption);

            // ユーザー設定で常駐クロールがONの場合、メイン処理完了後に常駐クロールを再開
            if (UserSettings.AlwaysCrawlMode)
            {
                Crawler.StartAlwaysCrawl();
            }
        });
        }

        /// <summary>
        /// 文書データのラベル情報を一括更新
        /// </summary>
        public virtual void UpdateDocumentFolderLabels()
        {
            var selectRes = GM.Select(
                  Table.Documents
                , outputColumns: new[] { Column.Documents.KEY, Column.Documents.FILE_PATH }
                , limit: -1
                , sortKeys: new[] { Column.Documents.KEY }
            );

            var documentKeys = selectRes.SearchResult.Records.Select((r) => (string)r.Key);
            var values = new List<IDictionary<string, object>>();
            foreach (var key in documentKeys)
            {
                var valueDict = new Dictionary<string, object>
                {
                    [Column.Documents.KEY] = key
                };

                var labels = UserSettings.FindTargetFoldersFromDocumentKey(key)
                                .Where((f2) => !string.IsNullOrEmpty(f2.Label))
                                .Select((f2) => f2.Label)
                                .ToList();

                valueDict[Column.Documents.FOLDER_LABELS] = labels;

                values.Add(valueDict);
            }

            GM.Load(table: Table.Documents, values: values);
        }


        /// <summary>
        /// 無視対象のファイル情報 (GetIgnoredDocumentRecordsで取得)
        /// </summary>
        public class IgnoredDocumentRecord
        {
            public string Key { get; set; }
            public string FilePath { get; set; }
        }

        /// <summary>
        /// 無視対象のファイル情報を、クロール済みの文書データの中から一括取得
        /// </summary>
        public virtual List<IgnoredDocumentRecord> GetIgnoredDocumentRecords(IgnoreSetting ignoreSetting)
        {
            var ret = new List<IgnoredDocumentRecord>();

            // 無視対象となりうる文書データを全取得
            var selectRes = GM.Select(
                  Table.Documents
                , outputColumns: new[] { Column.Documents.KEY, Column.Documents.FILE_PATH }
                , limit: -1
                , sortKeys: new[] { Column.Documents.KEY }
                , filter: $"{Column.Documents.KEY} @^ {Groonga.Util.EscapeForScript(Util.MakeDocumentDirKeyPrefix(ignoreSetting.DirPathLower))}" // ベースフォルダパスから始まるキーを検索
            );

            // 無視対象の文書データをリストに格納して返す
            var ignoredPaths = new List<string>();
            foreach (var rec in selectRes.SearchResult.Records)
            {
                var key = (string)rec.Key;
                var path = (string)rec[Column.Documents.FILE_PATH];

                if (ignoreSetting.IsMatch(path, isDirectory: false))
                {
                    ret.Add(new IgnoredDocumentRecord { Key = key, FilePath = path });
                }
            };
            return ret;
        }

        /// <summary>
        /// 無視対象のファイルを一括削除
        /// </summary>
        public virtual void DeleteIgnoredDocumentRecords(IgnoreSetting ignoreSetting)
        {
            // 無視対象のファイル情報を取得
            var recs = GetIgnoredDocumentRecords(ignoreSetting);

            // ログ出力
            foreach (var rec in recs)
            {
                Logger.Debug($"Purge - {rec.FilePath}");
            }

            // 20件ずつまとめて削除
            var chunkSize = 20;
            var chunks = recs.Select((r, i) => Tuple.Create(r, i))
                             .GroupBy(t => t.Item2 / chunkSize)
                             .Select(g => g.Select(t => t.Item1));

            foreach (var recsInChunk in chunks)
            {
                var subExprs = recsInChunk.Select(r => $"{Column.Documents.KEY} == {Groonga.Util.EscapeForScript(r.Key)}");
                GM.Delete(Table.Documents, filter: string.Join(" || ", subExprs));
            };
        }

        /// <summary>
        /// サムネイル画像をすべて削除
        /// </summary>
        public virtual void DeleteAllThumbnailFiles()
        {
            if (!Directory.Exists(ThumbnailDirPath)) return;
            foreach (var path in Directory.GetFiles(ThumbnailDirPath, "*.png"))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 文書DB保存先フォルダを変更
        /// </summary>
        /// <remarks>
        /// 変更処理に成功した場合はtrue、失敗した場合はfalse
        /// </remarks>
        public virtual bool ChangeDocumentDBDir(string newDirPath, out string errorMessage, IProgress<ProgressGuageState> progress = null)
        {
            // 出力変数初期化
            errorMessage = null;


            // 進捗を表すオブジェクトを作成
            var progressState = new ProgressGuageState() { Indeterminate = true };
            if (progress != null) progress.Report(progressState);

            // 書き込み可能なフォルダかどうかをチェック (テキストファイルを書き込む)
            try
            {
                var testFilePath = Path.Combine(newDirPath, "__testfile.txt");
                File.WriteAllText(testFilePath, "test");

                // 書き込みに成功した場合は、書き込んだファイルを削除
                File.Delete(testFilePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                errorMessage = "移動先へのファイルの書き込みに失敗しました。";
                return false;
            }

            // Groongaを停止
            GM.Shutdown();

            // 移動対象のファイルリストを取得
            var fromFiles = GM.GetDBFiles();

            // 元フォルダ内の各ファイルについて、ファイルサイズ合計値を取得
            var fromFileSizeMap = fromFiles.ToDictionary(p => p, p => File.GetSize(p));
            var fromFileTotalSize = fromFileSizeMap.Values.Sum();

            // 進捗状態を設定
            progressState.Value = 0;
            progressState.Maximum = 1000;
            progressState.Indeterminate = false;
            if (progress != null) progress.Report(progressState);

            var copyWeight = 800;
            var deleteWeight = progressState.Maximum - copyWeight;

            // コピー処理
            var copiedSize = (long)0;
            var copiedDestPaths = new List<string>();
            try
            {
                // 元フォルダのファイルを全てコピー（ファイルが存在する場合は上書き）
                foreach (var fromPath in fromFiles)
                {
                    var toPath = Path.Combine(newDirPath, Path.GetFileName(fromPath));
                    File.Copy(fromPath, toPath, overwrite: true);

                    // コピーを実施したパスを記録
                    copiedDestPaths.Add(toPath);

                    // コピー済ファイルサイズを加算
                    copiedSize += fromFileSizeMap[fromPath];

                    // 進捗状態を設定
                    progressState.Value = (int)Math.Round(((decimal)copiedSize / (decimal)fromFileTotalSize) * copyWeight);
                    if (progress != null) progress.Report(progressState);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                // 途中で例外が発生した場合は、（可能であれば）コピー先のファイルを削除してから中断
                foreach (var destPath in copiedDestPaths)
                {
                    try
                    {
                        File.Delete(destPath);
                    }
                    catch (Exception ex2)
                    {
                        Logger.Error(ex2);
                    }
                }

                errorMessage = "移動先へのファイルの書き込みに失敗しました。";
                return false;
            }

            // コピーが終了したなら、元フォルダのファイルを削除
            var deletedSize = (long)0;
            foreach (var fromPath in fromFiles)
            {
                // 削除
                File.Delete(fromPath);

                // 削除済ファイルサイズを加算
                deletedSize += fromFileSizeMap[fromPath];

                // 進捗状態を設定
                progressState.Value = copyWeight + (int)Math.Round(((decimal)deletedSize / (decimal)fromFileTotalSize) * deleteWeight);
                if (progress != null) progress.Report(progressState);
            }

            // 新しい文書DBフォルダパスを記録
            this.UserSettings.SaveDocumentDBDirPath(newDirPath);

            // Groonga ManagerのDBパスを置き換える
            GM.DBDirPath = newDirPath;

            // Groongaを起動
            GM.Boot();

            return true;
        }

        /// <summary>
        /// ファイルサイズを取得。失敗した場合はnullを返す
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>成功...ファイルサイズ / 失敗...null</returns>
        public long? TryGetFileSize(string path)
        {
            try
            {
                return File.GetSize(path);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// 終了時に必要な処理を行う
        /// </summary>
        protected virtual void OnQuit()
        {
            // 標準では何もしない
        }
    }
}
