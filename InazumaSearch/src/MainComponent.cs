using System;
using System.ComponentModel;
using System.Windows.Forms;
using InazumaSearch.Forms;

namespace InazumaSearch
{
    public partial class MainComponent : Component
    {
        public Application App { get; set; }

        /// <summary>
        /// 前回のタイマーTick処理時に、「常駐クロールの再起動が必要である」と判定されたかどうか
        /// </summary>
        protected bool _requiredAlwaysCrawlRebootOnLastTick = false;

        public MainComponent()
        {
            InitializeComponent();

            System.Windows.Forms.Application.ApplicationExit += Application_ApplicationExit;

        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            NotifyIcon.Dispose();
        }

        public MainComponent(Application app) : this()
        {
            App = app;
        }

        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            StartBrowser();
        }

        public void StartBrowser()
        {
            try
            {
                var form = new BrowserForm(App.HtmlDirPath)
                {
                    App = App
                };
                Application.BootingBrowserForms.Add(form);
                form.Show();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                App.Logger.Error(ex);
                App.Logger.Error($"見つからなかったファイル: {ex.FileName}");
                App.Logger.Error($"FusionLog: {ex.FusionLog}");
                GUIUtil.ShowErrorMessage("必要なファイルが読み込めなかったため、ブラウザコントロールの初期化に失敗しました。");
                Application.Quit();
            }
        }

        protected void MenuItem_Quit_Click(object sender, EventArgs e)
        {
            Application.Quit();
        }


        private void TaskBarContextMenu_Opening(object sender, CancelEventArgs e)
        {

        }

        private void MenuItem_WindowOpen_Click(object sender, EventArgs e)
        {
            StartBrowser();
        }

        private void ProcessMonitoringTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 常駐クロールONの設定で、常駐クロールのプロセスが終了しており、かつ中断フラグがOFFなら再起動が必要
                // 「再起動が必要」な状態が、少なくとも1 tick以上の期間続いていれば再起動する
                var requiredReboot = (
                    App.UserSettings.AlwaysCrawlMode
                    && !App.Crawler.AlwaysCrawlIsRunning
                    && !App.Crawler.AlwaysCrawlAutoRebootDisabled
                );

                if (requiredReboot)
                {
                    if (_requiredAlwaysCrawlRebootOnLastTick)
                    {
                        // 前回tick時も再起動が必要だった
                        this.App.Logger.Warn("常駐クローラが終了しているため再起動します...");
                        App.Crawler.StartAlwaysCrawl();
                    }
                    else
                    {
                        // 前回tick時はまだ再起動が必要ではなかった（何もしない）
                    }
                }

                // 前回実行時フラグの値を再設定
                _requiredAlwaysCrawlRebootOnLastTick = requiredReboot;
            }
            catch (Exception ex)
            {
                this.App.Logger.Warn(ex);
            }
        }
    }
}
