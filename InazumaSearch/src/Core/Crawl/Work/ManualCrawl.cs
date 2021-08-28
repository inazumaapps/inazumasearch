using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// 手動クロール
    /// </summary>
    public class ManualCrawl : WorkBase
    {
        /// <summary>
        /// クロール対象のフォルダパスリスト（nullの場合はすべての検索対象フォルダをクロールする）
        /// </summary>
        public virtual IEnumerable<string> TargetDirPaths { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ManualCrawl(Application app, bool isBackgroundCrawl, IEnumerable<string> targetDirPaths = null) : base(app, isBackgroundCrawl)
        {
            TargetDirPaths = targetDirPaths;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        public override bool Equals(IWork other)
        {
            return (other is ManualCrawl);
        }

        /// <summary>
        /// ハッシュ値算出
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
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
            // 無視設定一覧の取得
            var ignoreSettings = _app.GetIgnoreSettings();

            // DB内に存在する全文書ファイルのデータを取得 (スキップ・削除の判定を行うため)
            var dbRecordMap = DBDocumentRecordListUp(TargetDirPaths);

            // 実際にファイルを検索する対象フォルダパスを決定
            // プロパティで指定されていればそのパスリストを使用、指定されていなければユーザー設定から取得した全ての検索対象フォルダ
            var usingTargetDirPaths = _app.GetCrawlTargetDirPaths(TargetDirPaths);

            // クロール開始を報告
            progress?.Report(new ProgressState() { CurrentStep = ProgressState.Step.RecordUpdateProcessBegin });

            // DBの不要な文書削除処理をワークスタックに追加（下記処理が全て終わった後に呼び出される）
            workStack.Push(new DBPurge(_app, IsBackgroundCrawl, dbRecordMap));

            // 検索対象フォルダの更新処理をワークスタックに追加
            usingTargetDirPaths.Reverse();
            foreach (var targetDirPath in usingTargetDirPaths) // フォルダパスが若い順から処理されるようにするため、逆順にする
            {
                workStack.Push(new DirectoryCrawl(_app, IsBackgroundCrawl, targetDirPath, ignoreSettings, dbRecordMap));
            }
        }
    }
}
