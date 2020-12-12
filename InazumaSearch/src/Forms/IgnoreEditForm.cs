using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using Microsoft.WindowsAPICodePack.Shell;

namespace InazumaSearch.Forms
{
    public partial class IgnoreEditForm : Form
    {
        private CancellationTokenSource currentCTokenSource = null;
        private Task<List<string>> searchTask = null;

        private readonly string baseDirPath;
        private readonly string defaultPattern;
        private readonly HashSet<string> extractableExtNames;
        private readonly InazumaSearch.Core.Application _app;

        private bool isShown = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IgnoreEditForm(string baseDirPath, string defaultPattern, InazumaSearch.Core.Application app)
        {
            InitializeComponent();
            this.baseDirPath = baseDirPath;
            this.defaultPattern = defaultPattern;
            extractableExtNames = new HashSet<string>(app.GetExtractableExtNames());
            _app = app;
        }

        /// <summary>
        /// 確定ボタン押下
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // 無視設定に対象のファイルを追加
            var setting = _app.UserSettings.TargetFolders.OrderByDescending(f => f.Path.Length)
                                                         .FirstOrDefault(f => f.Path.ToLower().StartsWith(baseDirPath.ToLower()));
            List<string> origLines = null;
            if (setting.IgnoreSettingLines != null)
            {
                origLines = setting.IgnoreSettingLines;
            }
            var inputLines = TxtSetting.Text.Replace("\r", "").Split('\n');
            setting.IgnoreSettingLines = (origLines != null ? origLines.Concat(inputLines) : inputLines).ToList();
            _app.UserSettings.Save();

            // DB内から無視パターンに一致するレコードを削除
            var ignoreSetting = IgnoreSetting.Load(baseDirPath, setting.IgnoreSettingLines);
            var t = Task.Run(() =>
            {
                _app.DeleteIgnoredDocumentRecords(ignoreSetting);
            });

            var pf = new ProgressForm(t, "無視対象となる文書データを削除中...");
            pf.ShowDialog(this);

            Close();
        }

        /// <summary>
        /// キャンセルボタン押下
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// ロード時処理
        /// </summary>
        private void IgnoreEditForm_Load(object sender, EventArgs e)
        {
            var setting = _app.UserSettings.TargetFolders.OrderByDescending(f => f.Path.Length)
                                                         .FirstOrDefault(f => f.Path.ToLower().StartsWith(baseDirPath.ToLower()));
            if (setting != null)
            {
                TxtSetting.Text = string.Join("\r\n", setting.IgnoreSettingLines.Concat(new[] { defaultPattern }));
            }
            else
            {
                TxtSetting.Text = defaultPattern;
            }

            TxtBaseDirPath.Text = baseDirPath;
        }

        /// <summary>
        /// 表示後処理
        /// </summary>
        private void IgnoreEditForm_Shown(object sender, EventArgs e)
        {
            isShown = true;

            // スクロールを最下部に移動 (参考: <https://dobon.net/vb/dotnet/control/tbscrolltolast.html>)
            TxtSetting.SelectionStart = TxtSetting.Text.Length;
            TxtSetting.Focus();
            TxtSetting.ScrollToCaret();

            var t = Task.Factory.StartNew(() =>
            {
                // UIスレッド側で処理を実行
                Invoke(new Action(() =>
                {
                    RefreshList();
                }));
            });
        }

        /// <summary>
        /// 設定内容変更時処理
        /// </summary>
        private void TxtSetting_TextChanged(object sender, EventArgs e)
        {
            // 非表示時は処理しない
            if (!isShown) return;

            // テキスト変更後、0.5秒経ったら画面をRefresh
            if (currentCTokenSource != null)
            {
                currentCTokenSource.Cancel();
            }

            var cSource = new CancellationTokenSource();
            currentCTokenSource = cSource;

            var t = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);

                if (!cSource.Token.IsCancellationRequested)
                {
                    // UIスレッド側で処理を実行
                    Invoke(new Action(() =>
                    {
                        RefreshList();
                    }));
                }

            }, cSource.Token);
        }

        /// <summary>
        /// プレビュー表示の更新
        /// </summary>
        protected virtual async void RefreshList()
        {
            LstPreview.Items.Clear();
            LblSearching.Show();

            if (currentCTokenSource != null)
            {
                currentCTokenSource.Cancel();
            }
            if (searchTask != null)
            {
                searchTask.Wait();
            }

            var baseDirPath = TxtBaseDirPath.Text;

            // 設定オブジェクトを作成
            var setting = new IgnoreSetting(baseDirPath);
            var lines = TxtSetting.Text.Replace("\r", "").Split('\n');
            foreach (var line in lines)
            {
                setting.AddPattern(line);
            }

            currentCTokenSource = new CancellationTokenSource();
            searchTask = Task.Run<List<string>>(async () =>
            {
                return SearchIgnoredFiles(setting, currentCTokenSource.Token);
            });

            var paths = await searchTask;

            // キャンセルされていなければ、画面上にパスを設定
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    LstPreview.Items.Add(path);
                }
            }

            // 進捗表示を隠す
            LblSearching.Hide();
        }

        /// <summary>
        /// 無視対象ファイルをリストアップして、プレビュー表示を更新
        /// </summary>
        protected virtual List<string> SearchIgnoredFiles(IgnoreSetting setting, CancellationToken cToken)
        {
            return _app.GetIgnoredDocumentRecords(setting).Select(rec => rec.FilePath).ToList();
        }

        private void lnkPatternHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dialog = new IgnorePatternHelpDialog();
            dialog.ShowDialog(this);
        }
    }
}
