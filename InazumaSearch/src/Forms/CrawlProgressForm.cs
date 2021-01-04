using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InazumaSearch.Core;
using InazumaSearch.Core.Crawl;
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

            // 最終クロール情報を保存
            App.UserSettings.SaveOnCrawl(DateTime.Now, TargetDirPaths);

            // 常駐クロール中であれば、いったん常駐クロールを停止
            if (App.Crawler.AlwaysCrawlIsRunning)
            {
                await App.Crawler.StopAlwaysCrawlIfRunningAsync(); // 停止完了まで待機
            }

            // 全体クロール開始
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);

            var res = await App.Crawler.RunFullCrawlAsync(TargetDirPaths, (progSender, state) =>
            {
                if (IsDisposed) return;

                switch (state.CurrentStep)
                {
                    case ProgressState.Step.DBRecordListUpBegin:
                        statusText.Text = string.Format("登録済み文書の一覧を取得しています...");
                        break;

                    case ProgressState.Step.RecordUpdateProcessBegin:
                        ProgressBar.Style = ProgressBarStyle.Marquee;
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                        statusText.Text = string.Format("文書データ登録中...");
                        break;

                    case ProgressState.Step.RecordUpdateCheckBegin:
                        // 計測が完了している場合、最大値を設定
                        if (state.TotalValue != null)
                        {
                            if (state.CurrentValue > state.TotalValue.Value)
                            {
                                // 計測が完了していても、現在値が最大値を超えている場合は、残り時間不定
                                // （クロール実施中にファイルが新しく追加されると発生する場合がある）
                                ProgressBar.Style = ProgressBarStyle.Marquee;
                                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                            }
                            else
                            {
                                ProgressBar.Style = ProgressBarStyle.Continuous;
                                ProgressBar.Maximum = state.TotalValue.Value;
                                ProgressBar.Value = state.CurrentValue;
                                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                                TaskbarManager.Instance.SetProgressValue(ProgressBar.Value, ProgressBar.Maximum, Handle);
                            }
                        }
                        if (state.TotalValue != null)
                        {
                            statusText.Text = $"文書データ登録中... ({state.CurrentValue} / {state.TotalValue})";
                        }
                        else
                        {
                            statusText.Text = $"文書データ登録中... ({state.CurrentValue})";
                        }
                        break;

                    case ProgressState.Step.PurgeProcessBegin:
                        ProgressBar.Style = ProgressBarStyle.Marquee;
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                        statusText.Text = string.Format("存在しない文書データを削除中...");

                        break;

                    case ProgressState.Step.Finish:
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

            // 正常終了時、表示を更新
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

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timeCounter_Tick(object sender, EventArgs e)
        {
            var span = DateTime.Now - StartingTime;
            statusTimeCount.Text = string.Format("処理時間: {0}", span.ToString(@"mm\:ss"));
        }

        private async void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 実行中の手動クロール処理があればキャンセル
            if (App.Crawler.ManualCrawlIsRunning)
            {
                await App.Crawler.StopManualCrawlIfRunningAsync();
            }

            // 常駐クロールモードであれば、常駐クロールを再開
            if (App.UserSettings.AlwaysCrawlMode)
            {
                App.Crawler.StartAlwaysCrawl();
            }
        }
    }
}
