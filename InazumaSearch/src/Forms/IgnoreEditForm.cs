using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using InazumaSearch.Core;
using Microsoft.WindowsAPICodePack.Shell;

namespace InazumaSearch.Forms
{
    public partial class IgnoreEditForm : Form
    {
        private CancellationTokenSource currentCTokenSource = null;
        private Task searchTask = null;

        public IgnoreEditForm()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void IgnoreEditForm_Load(object sender, EventArgs e)
        {
            TxtBaseDirPath.Text = @"d:\document";
        }

        private void TxtSetting_TextChanged(object sender, EventArgs e)
        {
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

        private void BtnRefreshPreview_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        protected virtual void RefreshList()
        {
            LstPreview.Items.Clear();
            ProgPreviewing.Show();

            if (currentCTokenSource != null)
            {
                currentCTokenSource.Cancel();
            }
            if (searchTask != null)
            {
                searchTask.Wait();
            }

            currentCTokenSource = new CancellationTokenSource();
            searchTask = SearchIgnoredFiles(TxtBaseDirPath.Text, currentCTokenSource.Token);
        }

        protected virtual async Task SearchIgnoredFiles(string baseDirPath, CancellationToken cToken)
        {
            var paths = new List<string>();

            var setting = new IgnoreSetting(baseDirPath);
            var lines = TxtSetting.Text.Replace("\r", "").Split('\n');
            foreach (var line in lines)
            {
                setting.AddPattern(line);
            }

            foreach (var path in Directory.GetFiles(TxtBaseDirPath.Text.ToLower(), "*", SearchOption.AllDirectories))
            {
                var fileAttrs = File.GetAttributes(path);
                var isDirectory = fileAttrs.HasFlag(FileAttributes.Directory);
                if (setting.IsMatch(path, isDirectory))
                {
                    paths.Add(path);
                }

                if (cToken.IsCancellationRequested) return;
            }

            // UIを更新
            Invoke(new Action(() =>
            {
                foreach (var path in paths)
                {
                    LstPreview.Items.Add(path);

                    if (cToken.IsCancellationRequested) return;
                }

                ProgPreviewing.Hide();
            }));
        }

    }
}
