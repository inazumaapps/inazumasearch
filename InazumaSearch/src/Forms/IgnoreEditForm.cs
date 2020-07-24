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

        private bool isShown = false;

        public IgnoreEditForm(string baseDirPath, string defaultPattern, List<string> extractableExtNames)
        {
            InitializeComponent();
            this.baseDirPath = baseDirPath;
            this.defaultPattern = defaultPattern;
            this.extractableExtNames = new HashSet<string>(extractableExtNames);
        }

        /// <summary>
        /// 確定ボタン押下
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            var ignorePath = Path.Combine(TxtBaseDirPath.Text, ".inazumaignore");
            List<string> origLines = null;
            if (File.Exists(ignorePath))
            {
                origLines = File.ReadAllLines(ignorePath).ToList();
            }
            var inputLines = TxtSetting.Text.Replace("\r", "").Split('\n');
            File.WriteAllLines(ignorePath, (origLines != null ? origLines.Concat(inputLines) : inputLines));
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
            TxtBaseDirPath.Text = baseDirPath;
            TxtSetting.Text = defaultPattern;
        }

        /// <summary>
        /// 表示後処理
        /// </summary>
        private void IgnoreEditForm_Shown(object sender, EventArgs e)
        {
            isShown = true;

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
               return SearchIgnoredFiles(baseDirPath, setting, currentCTokenSource.Token);
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
        protected virtual List<string> SearchIgnoredFiles(string baseDirPath, IgnoreSetting setting, CancellationToken cToken)
        {
            var paths = new List<string>();

            foreach (var path in Directory.GetFiles(baseDirPath, "*", System.IO.SearchOption.AllDirectories))
            {
                var fileAttrs = File.GetAttributes(path);
                var isDirectory = fileAttrs.HasFlag(System.IO.FileAttributes.Directory);

                // ファイルの場合は、拡張子が検索対象のものかどうかをチェック (ファイルでなければ無視)
                var extNameOk = true;
                if (!isDirectory)
                {
                    var extName = Path.GetExtension(path).TrimStart('.').ToLower();
                    extNameOk = extractableExtNames.Contains(extName);
                }

                if (extNameOk && setting.IsMatch(path, isDirectory))
                {
                    paths.Add(path);
                }

                if (cToken.IsCancellationRequested) return null;
            }

            return paths;
        }
    }
}
