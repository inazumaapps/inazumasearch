using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// 常駐クロールでのフォルダ更新時クロール処理
    /// </summary>
    public class BackgroundDirectoryCrawl : WorkBase
    {
        /// <summary>
        /// クロール対象のフォルダパス
        /// </summary>
        public virtual string DirPath { get; protected set; }

        /// <summary>
        /// 無視設定リスト
        /// </summary>
        public virtual IEnumerable<IgnoreSetting> IgnoreSettings { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BackgroundDirectoryCrawl(
            Application app,
            string dirPath,
            IEnumerable<IgnoreSetting> ignoreSettings
        ) : base(app, true)
        {
            DirPath = dirPath;
            IgnoreSettings = ignoreSettings;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override bool Equals(IWork other)
        {
            var work = other as BackgroundDirectoryCrawl;
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
            IProgress<ProgressState> progress = null
        )
        {

            // 無視設定にマッチしない場合のみ処理
            if (IgnoreSettings.Any(s => s.IsMatch(DirPath, true)))
            {
                Logger.Trace("BackgroundDirectoryCrawl/Ignore directory - {0}", DirPath);
                return;
            }
            else
            {
                // DB内に存在する、対象フォルダ以下の文書データを取得 (スキップ・削除の判定を行うため)
                var dbRecordMap = DBDocumentRecordListUp(new[] { DirPath });

                // DBの不要な文書削除処理をワークスタックに追加（下記処理が終わった後に呼び出される）
                workStack.Push(new DBPurge(_app, IsBackgroundCrawl, dbRecordMap));

                // 検索対象フォルダの更新処理をワークスタックに追加
                workStack.Push(new DirectoryCrawl(_app, IsBackgroundCrawl, DirPath, IgnoreSettings, dbRecordMap));
            }
        }
    }
}
