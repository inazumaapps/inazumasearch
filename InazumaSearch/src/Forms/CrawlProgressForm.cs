using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using InazumaSearch.Core;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace InazumaSearch.Forms
{
    public partial class CrawlProgressForm : Form
    {
        protected DateTime StartingTime { get; set; }
        public Core.Application App { get; set; }
        protected Action StoppedCallback { get; set; }

        /// <summary>
        /// クロール対象のフォルダパスリスト（nullの場合はすべての検索対象フォルダをクロールする）
        /// </summary>
        public virtual IEnumerable<string> TargetDirPaths { get; set; }

        public CrawlProgressForm()
        {
            InitializeComponent();
        }

        public CrawlProgressForm(Core.Application app, Action stoppedCallback = null) : this()
        {
            App = app;
            StoppedCallback = stoppedCallback;
        }


        private void Form_Load(object sender, EventArgs e)
        {
        }


        private async void Form_Shown(object sender, EventArgs e)
        {
            ProgressBar.Enabled = true;
            ProgressBar.Style = ProgressBarStyle.Marquee;

            StartingTime = DateTime.Now;
            timeCounter.Start();

            try
            {
                // 最終クロール日時を終了
                App.UserSettings.SaveLastCrawlTime(DateTime.Now);


                // 常駐クロールモードであれば、いったん常駐クロールを停止
                if (App.UserSettings.AlwaysCrawlMode)
                {
                    App.Crawler.StopIfRunning();
                }

                // 全体クロール開始
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);

                var res = await App.Crawler.RunFullCrawlAsync(TargetDirPaths, (progSender, state) =>
                {
                    if (IsDisposed) return;

                    switch (state.CurrentStep)
                    {
                        case CrawlState.Step.DBRecordListUpBegin:
                            statusText.Text = string.Format("登録済み文書の一覧を取得しています...");
                            break;

                        case CrawlState.Step.TargetListUp:
                            statusText.Text = string.Format("検索対象ファイルを探索しています... ({0})", state.CurrentValue);
                            break;

                        case CrawlState.Step.RecordAddBegin:
                            ProgressBar.Style = ProgressBarStyle.Continuous;
                            ProgressBar.Maximum = state.TotalValue;
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                            TaskbarManager.Instance.SetProgressValue(ProgressBar.Value, ProgressBar.Maximum, Handle);
                            statusText.Text = string.Format("インデックス登録中...");
                            statusText.GetCurrentParent().Refresh();
                            statusText.GetCurrentParent().Update();
                            break;

                        case CrawlState.Step.RecordAdding:
                            // プログレスバーが徐々に伸びていくアニメーションを無効化し、バーに即時反映されるようにするため
                            // まず 現在値+1 の値を設定してから、現在値まで減算する
                            // 参考: https://dobon.net/vb/dotnet/control/pbdisableanimation.html
                            //if (state.CurrentValue + 1 <= ProgressBar.Maximum) {
                            //    ProgressBar.Value = state.CurrentValue + 1;
                            //}
                            ProgressBar.Value = state.CurrentValue;
                            TaskbarManager.Instance.SetProgressValue(ProgressBar.Value, ProgressBar.Maximum, Handle);
                            statusText.Text = string.Format("インデックス登録中... ({0})", state.UpdatedDocumentCount);
                            break;

                        case CrawlState.Step.PurgeBegin:
                            ProgressBar.Style = ProgressBarStyle.Marquee;
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                            statusText.Text = string.Format("存在しない文書データを削除中...");

                            break;

                        case CrawlState.Step.Finish:
                            ProgressBar.Style = ProgressBarStyle.Continuous;

                            // バーに即時反映されるようにするため
                            // Maximumの値を+1→現在値をMaximumと同じ値に設定→現在値を-1減らす→Maximumを-1減らす の順で処理
                            ProgressBar.Maximum++;
                            ProgressBar.Value = ProgressBar.Maximum;
                            ProgressBar.Value--;
                            ProgressBar.Maximum--;

                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);

                            break;


                    }
                });

                if (res.Finished)
                {
                    BtnCancel.Text = "閉じる";
                    if (res.Deleted > 0)
                    {
                        statusText.Text = string.Format("完了 (更新: {0}, スキップ: {1}, 削除: {2})", res.Updated, res.Skipped, res.Deleted);
                    }
                    else
                    {
                        statusText.Text = string.Format("完了 (更新: {0}, スキップ: {1})", res.Updated, res.Skipped);

                    }
                    statusTimeCount.ForeColor = SystemColors.ControlText;
                    Text = "クロール完了";
                }
                timeCounter.Stop();
                if (StoppedCallback != null) StoppedCallback.Invoke();
            }
            catch (OperationCanceledException)
            {
            }

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timeCounter_Tick(object sender, EventArgs e)
        {
            var span = DateTime.Now - StartingTime;
            statusTimeCount.Text = string.Format("処理時間: {0}", span.ToString(@"mm\:ss"));
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 起動中であれば終了
            App.Crawler.StopIfRunning();

            // 常駐クロールモードであれば、常駐クロールを再開
            if (App.UserSettings.AlwaysCrawlMode)
            {
                var t = App.Crawler.RunAlwaysModeAsync();
            }
        }
    }
}
