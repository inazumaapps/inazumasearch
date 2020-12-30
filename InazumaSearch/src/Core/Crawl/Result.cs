using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Core.Crawl
{
    /// <summary>
    /// クロール結果を格納するクラス
    /// </summary>
    public class Result
    {

        #region プロパティ

        /// <summary>
        /// 完了フラグ（最後まで実行した場合はtrue、途中で中断した場合はfalse）
        /// </summary>
        public bool Finished { get; set; } = false;

        /// <summary>
        /// 更新した文書件数
        /// </summary>
        public int Updated { get; set; } = 0;

        /// <summary>
        /// スキップした文書件数
        /// </summary>
        public int Skipped { get; set; } = 0;

        /// <summary>
        /// 削除した文書件数
        /// </summary>
        public int Deleted { get; set; } = 0;

        /// <summary>
        /// 登録対象ファイルの総数（別スレッドでカウントを行い、完了と同時にセットされる）
        /// </summary>
        public int? TotalTargetCount { get; set; } = null;

        /// <summary>
        /// フォルダ内のクロールにより見つかった、登録対象のファイルパスセット（スキップしたファイルも含む）
        /// </summary>
        public virtual ISet<string> FoundTargetFilePathSet { get; protected set; } = new HashSet<string>();

        /// <summary>
        /// 最後に進捗を報告した時間
        /// </summary>
        public virtual DateTime? LastProgressReported { get; set; }

        #endregion
    }
}
