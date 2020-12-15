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
        public enum EditMode
        {
            /// <summary>
            /// 設定の追加モード
            /// </summary>
            APPEND
            ,
            /// <summary>
            /// 既存設定の編集モード
            /// </summary>
            UPDATE
        }
        private CancellationTokenSource currentCTokenSource = null;
        private Task<List<string>> searchTask = null;

        private readonly EditMode editMode;
        private readonly string baseDirPath;
        private readonly string defaultPattern;
        private readonly InazumaSearch.Core.Application _app;

        private bool isShown = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IgnoreEditForm(EditMode editMode, string baseDirPath, string defaultPattern, InazumaSearch.Core.Application app)
        {
            InitializeComponent();
            this.editMode = editMode;
            this.baseDirPath = baseDirPath;
            this.defaultPattern = defaultPattern;
            _app = app;
        }

        /// <summary>
        /// ロード時処理
        /// </summary>
        private void IgnoreEditForm_Load(object sender, EventArgs e)
        {
            // モードに応じた初期値を設定
            switch (editMode)
            {
                case EditMode.APPEND:
                    TxtSetting.Text = defaultPattern;
                    break;

                case EditMode.UPDATE:
                    var setting = _app.UserSettings.TargetFolders.First(f => f.Path == baseDirPath);
                    if (setting != null)
                    {
                        TxtSetting.Text = string.Join("\r\n", setting.IgnoreSettingLines.Concat(new[] { defaultPattern }));
                    }
                    break;
            }

            TxtBaseDirPath.Text = baseDirPath;
        }

        /// <summary>
        /// 確定ボタン押下
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // 無視設定を更新
            var setting = _app.UserSettings.TargetFolders.First(f => f.Path == baseDirPath);
            var inputLines = TxtSetting.Text.Replace("\r", "").Split('\n').Where(line => !string.IsNullOrWhiteSpace(line));

            switch (editMode)
            {
                case EditMode.APPEND:
                    setting.IgnoreSettingLines.AddRange(inputLines);
                    break;

                case EditMode.UPDATE:
                    setting.IgnoreSettingLines = inputLines.ToList();
                    break;
            }
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
        /// 無視対象ファイルをリストアップする
        /// </summary>
        protected virtual List<string> SearchIgnoredFiles(IgnoreSetting setting, CancellationToken cToken)
        {
            var paths = new List<string>();

            foreach (var path in Directory.GetFiles(TxtBaseDirPath.Text.ToLower(), "*", System.IO.SearchOption.AllDirectories))
            {
                var fileAttrs = File.GetAttributes(path);
                var isDirectory = fileAttrs.HasFlag(System.IO.FileAttributes.Directory);
                if (setting.IsMatch(path, isDirectory))
                {
                    paths.Add(path);
                }

                if (cToken.IsCancellationRequested) return null;
            }

            return paths;
        }
        private void lnkPatternHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dialog = new IgnorePatternHelpDialog();
            dialog.ShowDialog(this);
        }
    }
}
