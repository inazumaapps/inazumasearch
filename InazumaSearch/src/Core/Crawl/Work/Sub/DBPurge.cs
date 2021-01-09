using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alphaleonis.Win32.Filesystem;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// DB内に存在する処理対象外の文書データを削除
    /// </summary>
    public class DBPurge : WorkBase
    {
        /// <summary>
        /// DBに登録されている文書レコード一覧
        /// </summary>
        public virtual IDictionary<string, Groonga.RecordSet.Record> AlreadyDBRecordMap { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DBPurge(
            Application app,
            bool isBackgroundCrawl,
            IDictionary<string, Groonga.RecordSet.Record> alreadyDBRecordMap
        ) : base(app, isBackgroundCrawl)
        {
            AlreadyDBRecordMap = alreadyDBRecordMap;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override bool Equals(IWork other)
        {
            return Object.ReferenceEquals(this, other); // 同一インスタンスである場合のみ等価
        }

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
            Logger.Info("DB内の不要な文書データを一括削除");
            var lastCoolTimeEnd = DateTime.Now;

            // 進捗を報告
            progress?.Report(new ProgressState() { CurrentStep = ProgressState.Step.PurgeProcessBegin });

            // 見つかった登録対象ファイルパスのセットを、Groonga用キーとファイルパスを格納したDictionaryに変換
            var targetKeyToPathMap = new Dictionary<string, string>();
            foreach (var path in crawlResult.FoundTargetFilePathSet)
            {
                targetKeyToPathMap[Util.MakeDocumentFileKey(path)] = path;
            }

            // ユーザー設定で登録されている検索対象フォルダを、キー文字列プレフィックスのリストに変換
            var targetFolderKeyPrefixes = _app.UserSettings.TargetFolders.Select(folder => Util.MakeDocumentDirKeyPrefix(folder.Path))
                                                                         .ToList();

            // DB内の文書データ1件ごとに処理
            foreach (var key in AlreadyDBRecordMap.Keys.Where(k => k.StartsWith("f:")))
            {
                // DBに存在しているが、登録対象ファイルパスのセットに含まれていない場合、削除対象とする
                if (!targetKeyToPathMap.ContainsKey(key))
                {
                    var rec = AlreadyDBRecordMap[key];
                    var path = (string)rec[Column.Documents.FILE_PATH];

                    // 削除直前に、ファイルが存在しないことを再確認 (対象ファイルパスの特定から削除の実行までに時間差があるため)
                    // ただし、ユーザー設定で登録されているどの検索対象フォルダとも一致しないパスの場合は
                    // 「設定から削除された検索対象フォルダ内に含まれているファイル」とみなし、ファイルが存在していても削除
                    if (!File.Exists(path)
                        || !targetFolderKeyPrefixes.Any(keyPrefix => key.StartsWith(keyPrefix)))
                    {
                        // 進捗を報告
                        ReportProgressLimitedFrequency(
                            progress
                            , new ProgressState() { CurrentStep = ProgressState.Step.PurgeBegin, Path = path }
                            , crawlResult
                        );

                        // DBから対象の文書データを削除
                        _app.GM.Delete(Table.Documents, key: key);
                        crawlResult.Deleted++;
                        Logger.Trace("Purge - {0}", key);

                        // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低0.05秒)
                        InsertCoolTime(50);
                    }

                    Thread.Sleep(0); // 他のスレッドに処理を渡す
                    cToken.ThrowIfCancellationRequested(); // キャンセル受付
                }
            }

            // 進捗を報告
            progress?.Report(new ProgressState() { CurrentStep = ProgressState.Step.PurgeProcessEnd });
        }
    }
}
