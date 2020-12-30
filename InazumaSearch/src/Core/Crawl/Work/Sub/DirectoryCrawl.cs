using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core.Crawl;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// フォルダの内容を検索し、その中の文書ファイルを登録・削除。また、無視対象でないサブフォルダがあればその中も再帰的にクロールする
    /// </summary>
    public class DirectoryCrawl : WorkBase
    {
        /// <summary>
        /// クロール対象のフォルダパス
        /// </summary>
        public virtual string DirPath { get; protected set; }

        /// <summary>
        /// DBに登録されている文書レコード一覧
        /// </summary>
        public virtual IDictionary<string, Groonga.RecordSet.Record> AlreadyDBRecordMap { get; protected set; }

        /// <summary>
        /// 無視設定リスト
        /// </summary>
        public virtual IEnumerable<IgnoreSetting> IgnoreSettings { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DirectoryCrawl(
            Application app,
            bool isBackgroundCrawl,
            string dirPath,
            IEnumerable<IgnoreSetting> ignoreSettings,
            IDictionary<string, Groonga.RecordSet.Record> alreadyDBRecordMap
        ) : base(app, isBackgroundCrawl)
        {
            DirPath = dirPath;
            IgnoreSettings = ignoreSettings;
            AlreadyDBRecordMap = alreadyDBRecordMap;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override bool Equals(IWork other)
        {
            var work = other as DirectoryCrawl;
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

        /// <summary>
        /// トレースログ出力時の表記
        /// </summary>
        public override string TraceLogCaption { get { return $"{GetType().Name}({DirPath})"; } }

        /// <summary>
        /// 処理実行
        /// </summary>
        public override void ExecuteMain(
            Stack<IWork> workStack,
            Result crawlResult,
            CancellationToken cToken,
            IProgress<CrawlState> progress = null
        )
        {
            // 展開対象の拡張子一覧を取得
            var extractableExtNames = _app.GetExtractableExtNames();

            // 実行時、指定ディレクトリがすでに削除(もしくは移動)されている場合は、処理をスキップ
            if (!Directory.Exists(DirPath))
            {
                Logger.Debug($"Target directory not found - {DirPath}");
                return;
            }

            Logger.Debug($"フォルダ内クロール - {DirPath}");

            // 直下のサブフォルダを検索し、クロール対象のサブフォルダを取得
            var targetSubDirs = new List<string>();
            ApplySubDirectories(DirPath, (subDirPath) =>
            {
                // 無視設定に合致するフォルダはスキップ
                if (IgnoreSettings.Any(s => s.IsMatch(subDirPath, true)))
                {
                    Logger.Trace("DirectoryCrawl/Ignore directory - {0}", subDirPath);
                    return;
                }

                // サブフォルダを検索対象リストに追加
                targetSubDirs.Add(subDirPath);

                Thread.Sleep(0); // 他のスレッドに処理を渡す
                cToken.ThrowIfCancellationRequested(); // 取り消し判定
            });

            // サブフォルダに対するクロール処理をワークスタックに追加
            foreach (var subDirPath in targetSubDirs.Reverse<string>()) // フォルダパスが若い順から処理されるようにするため、逆順にする
            {
                workStack.Push(new DirectoryCrawl(_app, IsBackgroundCrawl, subDirPath, IgnoreSettings, AlreadyDBRecordMap));
            }

            // 直下のファイルを検索し、登録対象の文書ファイルを取得
            var targetFilePaths = new List<string>();
            ApplyFiles(DirPath, (filePath) =>
            {
                // ファイルの拡張子を "txt" 形式で取得
                var ext = Path.GetExtension(filePath).TrimStart('.').ToLower();

                // 登録対象の拡張子である場合のみ処理
                if (extractableExtNames.Contains(ext))
                {
                    // 無視設定に合致しない場合のみ処理
                    if (IgnoreSettings.Any(s => s.IsMatch(filePath, false)))
                    {
                        Logger.Trace("DirectoryCrawl/Ignore file - {0}", filePath);
                        return;

                    }
                    else
                    {
                        targetFilePaths.Add(filePath);
                    }
                }

                Thread.Sleep(0); // 他のスレッドに処理を渡す
                cToken.ThrowIfCancellationRequested(); // 取り消し判定
            });

            // ファイルに対するクロール処理をワークスタックに追加
            foreach (var filePath in targetFilePaths.Reverse<string>()) // フォルダパスが若い順から処理されるようにするため、逆順にする
            {
                workStack.Push(new DocumentFileUpdate(_app, IsBackgroundCrawl, filePath, AlreadyDBRecordMap));
            }
        }
    }
}
