using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// 常駐クロールでのファイル更新時クロール処理
    /// </summary>
    public class BackgroundFileCrawl : WorkBase
    {
        /// <summary>
        /// クロール対象のファイルパス
        /// </summary>
        public virtual string FilePath { get; protected set; }

        /// <summary>
        /// 無視設定リスト
        /// </summary>
        public virtual IEnumerable<IgnoreSetting> IgnoreSettings { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BackgroundFileCrawl(
            Application app,
            string dirPath,
            IEnumerable<IgnoreSetting> ignoreSettings
        ) : base(app, true)
        {
            FilePath = dirPath;
            IgnoreSettings = ignoreSettings;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override bool Equals(IWork other)
        {
            var work = other as BackgroundFileCrawl;
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
            Logger.Debug("ファイル変更時クロール - {0}", FilePath);

            // 展開対象の拡張子一覧を取得
            var extractableExtNames = _app.GetExtractableExtNames();

            // ファイルの拡張子を "txt" 形式で取得
            var ext = Path.GetExtension(FilePath).TrimStart('.').ToLower();

            // 登録対象の拡張子である場合のみ処理
            if (extractableExtNames.Contains(ext))
            {
                // 無視設定に合致しない場合のみ処理
                if (IgnoreSettings.Any(s => s.IsMatch(FilePath, false)))
                {
                    Logger.Trace("BackgroundFileCrawl/Ignore file - {0}", FilePath);
                    return;
                }
                else
                {
                    // DB内に存在する親フォルダ内の文書データを取得 (スキップ・削除の判定を行うため)
                    var dbRecordMap = DBDocumentRecordListUp(new[] { Path.GetDirectoryName(FilePath) });

                    // ファイルの登録処理をワークスタックに追加
                    workStack.Push(new Work.DocumentFileUpdate(_app, IsBackgroundCrawl, FilePath, dbRecordMap));
                }
            }
        }
    }
}
