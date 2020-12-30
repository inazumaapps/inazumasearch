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
    /// 対象ファイルを計測する (通常の処理とは別スレッドで実行される)
    /// </summary>
    public class TargetFileCount : WorkBase
    {
        /// <summary>
        /// クロール対象のフォルダパスリスト（nullの場合はすべての検索対象フォルダをクロールする）
        /// </summary>
        public virtual IEnumerable<string> TargetDirPaths { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TargetFileCount(Application app, bool isBackgroundCrawl, IEnumerable<string> targetDirPaths = null) : base(app, isBackgroundCrawl)
        {
            TargetDirPaths = targetDirPaths;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        public override bool Equals(IWork other)
        {
            return (other is TargetFileCount);
        }

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
            // 無視設定一覧の取得
            var ignoreSettings = _app.GetIgnoreSettings();

            // 展開対象の拡張子一覧を取得
            var extractableExtNames = _app.GetExtractableExtNames();

            // 対象ディレクトリを探索し、ファイル数をカウント
            var targetFilePaths = new HashSet<string>();
            foreach (var dirPath in TargetDirPaths)
            {
                // 検索対象ディレクトリ内の全ファイルを探索
                ApplyAllFiles(dirPath, (filePath) =>
                {
                    // ファイルの拡張子を "txt" 形式で取得
                    var ext = Path.GetExtension(filePath).TrimStart('.').ToLower();

                    // 登録対象の拡張子である場合のみ処理
                    if (extractableExtNames.Contains(ext))
                    {
                        // 無視設定に合致しない場合のみ処理
                        if (!ignoreSettings.Any(s => s.IsMatch(filePath, false)))
                        {
                            targetFilePaths.Add(filePath);
                        }
                    }

                    Thread.Sleep(0); // 他のスレッドに処理を渡す
                    cToken.ThrowIfCancellationRequested(); // キャンセル受け付け
                });
            }

            // カウントが完了したらセット
            crawlResult.TotalTargetCount = targetFilePaths.Count;
        }
    }
}
